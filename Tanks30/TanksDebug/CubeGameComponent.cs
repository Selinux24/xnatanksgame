using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TanksDebug
{
    using Common;
    using Common.Drawing;
    using Physics;

    /// <summary>
    /// Componente que dibuja geometría
    /// </summary>
    public class CubeGameComponent : DrawableGameComponent, IPhysicObject
    {
        /// <summary>
        /// Efecto
        /// </summary>
        private BasicEffect m_BasicEffect = null;
        /// <summary>
        /// Declaración de formato de vértices
        /// </summary>
        private VertexDeclaration m_VertexDeclaration = null;
        /// <summary>
        /// Vértices
        /// </summary>
        private VertexPositionNormalTexture[] m_Vertices = null;
        /// <summary>
        /// Tipo de primitivas a dibujar
        /// </summary>
        private PrimitiveType m_PrimitiveType = PrimitiveType.TriangleList;
        /// <summary>
        /// Número de primitivas
        /// </summary>
        private int m_PrimitiveCount = -1;
        /// <summary>
        /// Textura
        /// </summary>
        private Texture2D m_Texture = null;
        /// <summary>
        /// Caja que representa este componente
        /// </summary>
        private CollisionBox m_Box = null;

        /// <summary>
        /// Método de relleno de la geometría
        /// </summary>
        public FillMode FillMode = FillMode.Solid;

        /// <summary>
        /// Constructor
        /// </summary>
        public CubeGameComponent(Game game, Vector3 min, Vector3 max)
            : base(game)
        {
            Vector3 halfSize = (max - min) / 2f;

            this.m_Box = new CollisionBox(halfSize, halfSize.X * halfSize.Y * halfSize.Z * 20f);

            PolyGenerator.InitializeCube(out this.m_Vertices, min, max);

            this.m_PrimitiveCount = this.m_Vertices.Length / 3;
        }

        /// <summary>
        /// Carga el contenido gráfico del componente
        /// </summary>
        protected override void LoadContent()
        {
            this.m_VertexDeclaration = new VertexDeclaration(this.GraphicsDevice, VertexPositionNormalTexture.VertexElements);
            this.m_BasicEffect = new BasicEffect(this.GraphicsDevice, null);
            this.m_BasicEffect.EnableDefaultLighting();
            this.m_Texture = this.Game.Content.Load<Texture2D>(@"Content/crate");

            base.LoadContent();
        }
        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
        /// <summary>
        /// Dibuja la geometría
        /// </summary>
        /// <param name="device">Dispositivo gráfico</param>
        /// <param name="world">Matriz mundo del objeto</param>
        /// <param name="view">Matriz vista</param>
        /// <param name="projection">Matriz proyección</param>
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            this.GraphicsDevice.VertexDeclaration = this.m_VertexDeclaration;

            FillMode prev = this.GraphicsDevice.RenderState.FillMode;
            this.GraphicsDevice.RenderState.FillMode = this.FillMode;

            this.m_BasicEffect.Texture = this.m_Texture;
            this.m_BasicEffect.TextureEnabled = (this.m_Texture != null);
            this.m_BasicEffect.VertexColorEnabled = (this.m_Texture == null);

            this.m_BasicEffect.World = this.m_Box.Transform * GlobalMatrices.gWorldMatrix;
            this.m_BasicEffect.View = GlobalMatrices.gViewMatrix;
            this.m_BasicEffect.Projection = GlobalMatrices.gGlobalProjectionMatrix;

            if (this.m_Box.IsAwake)
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

                this.GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>(
                    this.m_PrimitiveType,
                    this.m_Vertices,
                    0,
                    this.m_PrimitiveCount);

                pass.End();
            }

            this.m_BasicEffect.End();

            this.GraphicsDevice.RenderState.FillMode = prev;

            this.GraphicsDevice.VertexDeclaration = null;

#if DEBUG
            // Dibujar el AABB
            GameComponents.Debug.DebugDrawer.DrawDebugAABB(this.GraphicsDevice, this.GetAABB());
#endif
        }
        /// <summary>
        /// Establece el estado inicial de posición y orientación del componente
        /// </summary>
        /// <param name="position">Posición</param>
        /// <param name="orientation">Orientación</param>
        public void SetState(Vector3 position, Quaternion orientation)
        {
            this.m_Box.SetInitialState(position, orientation);
        }

        public CollisionPrimitive GetPrimitive()
        {
            return this.m_Box;
        }

        public CollisionPrimitive GetContactedPrimitive(IPhysicObject physicObject)
        {
            if (physicObject != null)
            {
                BoundingSphere sph = physicObject.GetSPH();
                BoundingSphere thisSph = this.GetSPH();

                if (sph.Contains(thisSph) != ContainmentType.Disjoint)
                {
                    return this.m_Box;
                }
            }

            return null;
        }

        public BoundingBox GetAABB()
        {
            return this.m_Box.AABB;
        }

        public BoundingSphere GetSPH()
        {
            return this.m_Box.SPH;
        }

        public bool IsActive()
        {
            return this.m_Box.IsAwake;
        }

        public void Integrate(float time)
        {
            if (this.m_Box != null)
            {
                this.m_Box.Integrate(time);
            }
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