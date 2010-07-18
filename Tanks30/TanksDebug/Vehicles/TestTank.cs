using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TanksDebug
{
    using Common;
    using Physics;
    using Physics.CollideCoarse;

    class TestTank : Microsoft.Xna.Framework.DrawableGameComponent
    {
        Model m_Model = null;
        CollisionBox m_Box = null;
        Matrix[] m_BoneTransforms = null;
        Matrix m_Transform = Matrix.Identity;
        Matrix m_Offset = Matrix.Identity;

        float m_FrontArmor = 11;
        float m_UpperArmor = 11;
        float m_LateralArmor = 11;
        float m_RearArmor = 10;
        float m_Hull = 100;

        float m_LaserDelay = 10f;
        float m_ArtilleryDelay = 1f;

        float m_LastLaser = 0f;
        float m_LastArtillery = 0f;

        public Matrix Transform
        {
            get { return m_Transform; }
        }

        public TestTank(Game game)
            : base(game)
        {

        }

        protected override void LoadContent()
        {
            base.LoadContent();

            this.m_Model = this.Game.Content.Load<Model>(@"Content/rhino");
            this.m_BoneTransforms = new Matrix[m_Model.Bones.Count];
            this.m_Model.CopyAbsoluteBoneTransformsTo(m_BoneTransforms);

            Vector3 minCorner = new Vector3(-1.41379f, -0.2828076f, -2.537132f);
            Vector3 maxCorner = new Vector3(1.392938f, 1.856467f, 2.462098f);
            Vector3 halfSize = (maxCorner - minCorner) / 2f;

            this.m_Box = new CollisionBox(halfSize, 1000f);
            //this.m_Box.OnPrimitiveContacted += new CollisionBox.PrimitiveInContactDelegate(OnPrimitiveContacted);

            this.m_Offset = Matrix.CreateTranslation(new Vector3(0f, -halfSize.Y, 0f));
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

                    effect.View = GlobalMatrices.gViewMatrix;
                    effect.Projection = GlobalMatrices.gGlobalProjectionMatrix;
                    effect.World = m_BoneTransforms[mesh.ParentBone.Index] * this.m_Transform;
                }

                mesh.Draw();
            }
        }

        public void SetState(Vector3 position, Quaternion orientation)
        {
            m_Box.SetInitialState(position, orientation);
        }
        public void GoForward(float amount)
        {
            if (this.CanMove()) m_Box.SetVelocity(m_Transform.Forward * amount);
        }
        public void GoBackward(float amount)
        {
            if (this.CanMove()) m_Box.SetVelocity(m_Transform.Backward * amount);
        }
        public void TurnLeft(float amount)
        {
            if (this.CanMove()) m_Box.AddToOrientation(Quaternion.CreateFromAxisAngle(m_Transform.Up, amount));
        }
        public void TurnRight(float amount)
        {
            if (this.CanMove()) m_Box.AddToOrientation(Quaternion.CreateFromAxisAngle(m_Transform.Up, -amount));
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

                this.TakeDamage(ammo.Mass, ammo.Velocity, ammo.Position);
            }
        }
        void TakeDamage(float mass, Vector3 velocity, Vector3 point)
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
            int force = rnd.Next(1, 6);
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
    }
}