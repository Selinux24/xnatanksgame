using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TanksDebug
{
    using Common;
    using Common.Helpers;
    using GameComponents.Geometry;
    using Physics;

    public class BallGameComponent : DrawableGameComponent, IPhysicObject
    {
        /// <summary>
        /// Radio
        /// </summary>
        public readonly float Radius;
        /// <summary>
        /// Masa
        /// </summary>
        public readonly float Mass;
        /// <summary>
        /// Esfera que representa este componente
        /// </summary>
        private CollisionSphere m_Sphere;
        /// <summary>
        /// Efecto
        /// </summary>
        private BasicEffect m_BasicEffect = null;
        /// <summary>
        /// Información de geometría
        /// </summary>
        private BufferedGeometryInfo m_Geometry = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game">Juego</param>
        /// <param name="radius">Radio</param>
        /// <param name="mass">Masa</param>
        public BallGameComponent(Game game, float radius, float mass)
            : base(game)
        {
            this.Radius = radius;
            this.Mass = mass;
        }

        /// <summary>
        /// Carga el contenido gráfico del componente
        /// </summary>
        protected override void LoadContent()
        {
            this.m_BasicEffect = new BasicEffect(this.GraphicsDevice, null);
            this.m_BasicEffect.EnableDefaultLighting();

            this.m_Sphere = new CollisionSphere(this.Radius, this.Mass);

            VertexPositionNormalTexture[] buffer = null;
            Int16[] indices = null;

            PolyGenerator.InitializeSphere(out buffer, out indices, this.Radius);

            this.m_Geometry = new BufferedGeometryInfo()
            {
                FillMode = FillMode.Solid,
                PrimitiveType = PrimitiveType.TriangleList,
                Indexed = true,
                Vertices = buffer,
                Indices = indices,
                VertexDeclaration = new VertexDeclaration(this.GraphicsDevice, VertexPositionNormalTexture.VertexElements),
                Texture = this.Game.Content.Load<Texture2D>(@"Content/dharma"),
                PrimitiveCount = indices.Length / 3,
            };

            base.LoadContent();
        }
        /// <summary>
        /// Dibuja la geometría
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            this.m_BasicEffect.World = this.m_Sphere.Transform * GlobalMatrices.gWorldMatrix;
            this.m_BasicEffect.View = GlobalMatrices.gViewMatrix;
            this.m_BasicEffect.Projection = GlobalMatrices.gGlobalProjectionMatrix;

            if (this.m_Sphere.IsAwake)
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

            this.m_Geometry.Draw(gameTime, this.GraphicsDevice, this.m_BasicEffect);

#if DEBUG
            // Dibujar el AABB
            GameComponents.Debug.DebugDrawer.DrawDebugAABB(this.GraphicsDevice, this.m_Sphere.AABB);
#endif
        }
        /// <summary>
        /// Establece el estado inicial de posición del componente
        /// </summary>
        /// <param name="position">Posición</param>
        public void SetPosition(Vector3 position)
        {
            this.m_Sphere.SetInitialState(position, Quaternion.Identity);
        }
   
        public CollisionPrimitive GetPrimitive()
        {
            return this.m_Sphere;
        }
        public CollisionPrimitive GetContactedPrimitive(IPhysicObject physicObject)
        {
            return this.m_Sphere;
        }
        public BoundingBox GetAABB()
        {
            return this.m_Sphere.AABB;
        }
        public BoundingSphere GetSPH()
        {
            return this.m_Sphere.SPH;
        }
        public bool IsActive()
        {
            return this.m_Sphere.IsAwake;
        }
        public void Integrate(float duration)
        {
            this.m_Sphere.Integrate(duration);
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
