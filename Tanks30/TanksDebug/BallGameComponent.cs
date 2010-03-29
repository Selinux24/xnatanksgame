using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TanksDebug
{
    using Common;
    using Common.Helpers;
    using Physics;

    public class BallGameComponent : DrawableGameComponent, IPhysicObject
    {
        public readonly CollisionSphere Sphere;

        private BasicEffect m_BasicEffect = null;
        private VertexDeclaration m_VertexDeclaration = null;

        private VertexPositionNormalTexture[] m_SphereVertices = null;
        private Int16[] m_SphereIndices = null;
        private PrimitiveType m_SpherePrimitiveType = PrimitiveType.TriangleList;
        private Texture2D m_SphereTexture = null;

        public BallGameComponent(Game game, float radius, float mass)
            : base(game)
        {
            this.Sphere = new CollisionSphere(radius, mass);

            PolyGenerator.InitializeSphere(out this.m_SphereVertices, out this.m_SphereIndices, radius);
        }

        protected override void LoadContent()
        {
            this.m_BasicEffect = new BasicEffect(this.GraphicsDevice, null);
            this.m_BasicEffect.EnableDefaultLighting();

            this.m_VertexDeclaration = new VertexDeclaration(this.GraphicsDevice, VertexPositionNormalTexture.VertexElements);

            this.m_SphereTexture = this.Game.Content.Load<Texture2D>(@"Content/dharma");

            base.LoadContent();
        }
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            this.GraphicsDevice.VertexDeclaration = this.m_VertexDeclaration;

            this.DrawSphere(this.Sphere);

            this.GraphicsDevice.VertexDeclaration = null;

#if DEBUG
            // Dibujar el AABB
            GameComponents.Debug.DebugDrawer.DrawDebugAABB(this.GraphicsDevice, this.Sphere.AABB);
#endif
        }
        private void DrawSphere(CollisionSphere sphere)
        {
            this.m_BasicEffect.Texture = this.m_SphereTexture;
            this.m_BasicEffect.TextureEnabled = (this.m_SphereTexture != null);
            this.m_BasicEffect.VertexColorEnabled = (this.m_SphereTexture == null);

            this.m_BasicEffect.World = sphere.Transform * GlobalMatrices.gWorldMatrix;
            this.m_BasicEffect.View = GlobalMatrices.gViewMatrix;
            this.m_BasicEffect.Projection = GlobalMatrices.gGlobalProjectionMatrix;

            if (sphere.IsAwake)
            {
                this.m_BasicEffect.DiffuseColor = Color.BurlyWood.ToVector3();
                this.m_BasicEffect.EmissiveColor = Color.Black.ToVector3();
                this.m_BasicEffect.SpecularColor = Color.Black.ToVector3();
                this.m_BasicEffect.SpecularPower = 0f;
            }
            else
            {
                this.m_BasicEffect.DiffuseColor = Color.Gray.ToVector3();
                this.m_BasicEffect.EmissiveColor = Color.Black.ToVector3();
                this.m_BasicEffect.SpecularColor = Color.Black.ToVector3();
                this.m_BasicEffect.SpecularPower = 0f;
            }

            this.m_BasicEffect.PreferPerPixelLighting = true;

            this.m_BasicEffect.Begin();

            foreach (EffectPass pass in this.m_BasicEffect.CurrentTechnique.Passes)
            {
                pass.Begin();

                this.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(
                    this.m_SpherePrimitiveType,
                    m_SphereVertices,
                    0,
                    m_SphereVertices.Length,
                    m_SphereIndices,
                    0,
                    m_SphereIndices.Length / 3);

                pass.End();
            }

            this.m_BasicEffect.End();
        }

        public void SetPosition(Vector3 position)
        {
            this.Sphere.SetInitialState(position, Quaternion.Identity);
        }

        public void Integrate(float duration)
        {
            this.Sphere.Integrate(duration);
        }
        public CollisionPrimitive GetPrimitive()
        {
            return this.Sphere;
        }
        public CollisionPrimitive GetContactedPrimitive(IPhysicObject physicObject)
        {
            return this.Sphere;
        }
        public BoundingBox GetAABB()
        {
            return this.Sphere.AABB;
        }
        public BoundingSphere GetSPH()
        {
            return this.Sphere.SPH;
        }
        public bool IsActive()
        {
            return this.Sphere.IsAwake;
        }
        public event ObjectInContactDelegate OnObjectContacted;
        public void Contacted(IPhysicObject obj)
        {
            if (this.OnObjectContacted != null)
            {
                this.OnObjectContacted(obj);
            }
        }
    }
}
