using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;
using Physics;
using DrawingComponents;

namespace BigBallisticDemo
{
    class Tank : Microsoft.Xna.Framework.DrawableGameComponent
    {
        Model m_Model = null;
        Matrix[] m_BoneTransforms = null;
        Box m_Box = null;
        Matrix m_Offset = Matrix.Identity;
        Matrix m_Transform = Matrix.Identity;

        float m_FrontArmor = 11;
        float m_UpperArmor = 11;
        float m_LateralArmor = 11;
        float m_RearArmor = 10;
        float m_Hull = 100;

        float m_LaserDelay = 10f;
        float m_ArtilleryDelay = 25f;

        float m_LastLaser = 0f;
        float m_LastArtillery = 0f;

        public Matrix Transform
        {
            get { return m_Transform; }
        }

        public Tank(Game game)
            : base(game)
        {
            m_Box = new Box(Vector3.One);
            m_Box.OnPrimitiveContacted += new Box.PrimitiveInContactDelegate(OnPrimitiveContacted);
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            m_Model = this.Game.Content.Load<Model>(@"rhino");
            m_BoneTransforms = new Matrix[m_Model.Bones.Count];
            m_Model.CopyAbsoluteBoneTransformsTo(m_BoneTransforms);

            //Vector3 maxCorner = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            //Vector3 minCorner = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            //foreach (ModelMesh mesh in m_Model.Meshes)
            //{
            //    Matrix transform = m_BoneTransforms[mesh.ParentBone.Index];

            //    int sizeInBytes = mesh.VertexBuffer.SizeInBytes;
            //    int count = sizeInBytes / VertexPositionNormalTexture.SizeInBytes;

            //    VertexPositionNormalTexture[] data = new VertexPositionNormalTexture[count];

            //    mesh.VertexBuffer.GetData<VertexPositionNormalTexture>(data);

            //    foreach (VertexPositionNormalTexture vertex in data)
            //    {
            //        Vector3 position = Vector3.Transform(vertex.Position, transform);

            //        minCorner = Vector3.Min(minCorner, position);
            //        maxCorner = Vector3.Max(maxCorner, position);
            //    }
            //}

            Vector3 minCorner = new Vector3(-1.41379f, -0.2828076f, -2.537132f);
            Vector3 maxCorner = new Vector3(1.392938f, 1.856467f, 2.462098f);

            Vector3 halfSize = (maxCorner - minCorner) / 2f;

            m_Box.HalfSize = halfSize;
            m_Box.Body.Mass = 1000f;
            m_Offset = Matrix.CreateTranslation(new Vector3(0f, -halfSize.Y, 0f));
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.m_Transform = m_Offset * m_Box.Transform;
        }
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            foreach (ModelMesh mesh in m_Model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.PreferPerPixelLighting = true;

                    effect.View = GlobalMatrices.View;
                    effect.Projection = GlobalMatrices.Projection;
                    effect.World = m_BoneTransforms[mesh.ParentBone.Index] * this.m_Transform;
                }

                mesh.Draw();
            }
        }

        public void SetState(Vector3 position, Quaternion orientation)
        {
            m_Box.SetState(position, orientation);
        }
        public void GoForward(float amount)
        {
            if (this.CanMove()) m_Box.Body.Velocity = m_Transform.Forward * amount;

            m_Box.Body.IsAwake = true;
        }
        public void GoBackward(float amount)
        {
            if (this.CanMove()) m_Box.Body.Velocity = m_Transform.Backward * amount;

            m_Box.Body.IsAwake = true;
        }
        public void TurnLeft(float amount)
        {
            if (this.CanMove()) m_Box.Body.Orientation *= Quaternion.CreateFromAxisAngle(m_Transform.Up, amount);

            m_Box.Body.IsAwake = true;
        }
        public void TurnRight(float amount)
        {
            if (this.CanMove()) m_Box.Body.Orientation *= Quaternion.CreateFromAxisAngle(m_Transform.Up, -amount);

            m_Box.Body.IsAwake = true;
        }
        public bool CanMove()
        {
            float dot = Vector3.Dot(Vector3.Up, this.Transform.Up);

            return (dot >= 0.7f && dot <= 1f && m_Hull >= 0f);
        }
        public bool CanFire(GameTime gameTime, ShotType type)
        {
            if (m_Hull <= 0f)
            {
                return false;
            }

            float time = (float)gameTime.TotalGameTime.TotalSeconds;
            if (type == ShotType.Artillery)
            {
                if (time - m_LastArtillery < m_ArtilleryDelay)
                {
                    return false;
                }

                m_LastArtillery = time;
            }
            else if (type == ShotType.Laser)
            {
                if (time - m_LastLaser < m_LaserDelay)
                {
                    return false;
                }

                m_LastLaser = time;
            }

            return true;
        }

        void OnPrimitiveContacted(CollisionPrimitive primitive)
        {
            if (primitive is AmmoRound)
            {
                AmmoRound ammo = (AmmoRound)primitive;

                this.TakeDamage(ammo.ShotType, ammo.Position);
            }
        }
        void TakeDamage(ShotType shotType, Vector3 point)
        {
            Vector3 pointTrn = Vector3.Transform(point, Matrix.Invert(this.Transform));

            pointTrn.X += m_Box.HalfSize.X;
            pointTrn.Z += m_Box.HalfSize.Z;

            //Altura de la caja
            float height = m_Box.HalfSize.Y * 2f;

            //Anchura de la caja
            float width = m_Box.HalfSize.X * 2f;

            //Largura de la caja
            float length = m_Box.HalfSize.Z * 2f;

            float armor = 0f;
            if (Math.Abs(pointTrn.Y) >= (height - (height * 0.1f)))
            {
                armor = this.m_UpperArmor;
            }
            else if (Math.Abs(pointTrn.Z) <= (length - (length * 0.3f)))
            {
                armor = this.m_FrontArmor;
            }
            else if (Math.Abs(pointTrn.Z) >= (length * 0.3f))
            {
                armor = this.m_RearArmor;
            }
            else
            {
                armor = this.m_LateralArmor;
            }

            Random rnd = new Random(DateTime.Now.Millisecond);
            int force = (int)shotType + rnd.Next(1, 6);
            if (force == armor)
            {
                //Impacto superficial
                this.m_Hull -= force;
            }
            else if (force > armor)
            {
                //Impacto interno
                this.m_Hull -= (force * 2);
            }
        }

        internal void Register(PhysicsController physicsController)
        {
            physicsController.RegisterBox(m_Box);
        }
    }
}