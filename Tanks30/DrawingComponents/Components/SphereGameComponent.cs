using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DrawingComponents
{
    using Common;
    using Common.Helpers;

    /// <summary>
    /// Componente que dibuja geometría
    /// </summary>
    public class SphereGameComponent : Microsoft.Xna.Framework.DrawableGameComponent
    {
        /// <summary>
        /// Efecto
        /// </summary>
        protected BasicEffect basicEffect;
        /// <summary>
        /// Declaración de formato de vértices
        /// </summary>
        protected VertexDeclaration basicEffectVertexDeclaration;
        /// <summary>
        /// Vértices
        /// </summary>
        protected VertexPositionNormalTexture[] vertices;
        /// <summary>
        /// Tipo de primitivas a dibujar
        /// </summary>
        protected PrimitiveType primitiveType = PrimitiveType.TriangleList;
        /// <summary>
        /// Número de primitivas
        /// </summary>
        protected int primitiveCount = -1;
        /// <summary>
        /// Textura
        /// </summary>
        protected Texture2D Texture = null;
        /// <summary>
        /// Indica si es geometría indexada
        /// </summary>
        protected bool indexed = false;
        /// <summary>
        /// Índices
        /// </summary>
        protected short[] indices;
        /// <summary>
        /// Método de relleno de la geometría
        /// </summary>
        protected FillMode FillMode = FillMode.Solid;

        /// <summary>
        /// Transformación
        /// </summary>
        public Matrix Transform = Matrix.Identity;

        /// <summary>
        /// Constructor
        /// </summary>
        public SphereGameComponent(Game game, float radius)
            : base(game)
        {
            PolyGenerator.InitializeSphere(out this.vertices, out this.indices, radius);

            this.primitiveType = PrimitiveType.TriangleList;
            this.primitiveCount = indices.Length / 3;
            this.indexed = true;
        }

        /// <summary>
        /// Carga los contenidos gráficos
        /// </summary>
        protected override void LoadContent()
        {
            this.basicEffectVertexDeclaration = new VertexDeclaration(this.GraphicsDevice, VertexPositionNormalTexture.VertexElements);
            this.basicEffect = new BasicEffect(this.GraphicsDevice, null);
            this.Texture = this.Game.Content.Load<Texture2D>(@"dharma");

            base.LoadContent();
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

            FillMode prev = this.GraphicsDevice.RenderState.FillMode;
            this.GraphicsDevice.RenderState.FillMode = this.FillMode;

            this.GraphicsDevice.VertexDeclaration = basicEffectVertexDeclaration;

            basicEffect.EnableDefaultLighting();
            basicEffect.Texture = Texture;
            basicEffect.TextureEnabled = (Texture != null);
            basicEffect.VertexColorEnabled = (Texture == null);

            basicEffect.World = this.Transform * GlobalMatrices.gWorldMatrix;
            basicEffect.View = GlobalMatrices.gViewMatrix;
            basicEffect.Projection = GlobalMatrices.gProjectionMatrix;

            basicEffect.Begin();

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Begin();

                if (indexed)
                {
                    this.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(
                        primitiveType,
                        vertices,
                        0,
                        vertices.Length,
                        indices,
                        0,
                        primitiveCount);
                }
                else
                {
                    this.GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>(
                        primitiveType,
                        vertices,
                        0,
                        primitiveCount);
                }

                pass.End();
            }

            basicEffect.End();

            this.GraphicsDevice.VertexDeclaration = null;

            this.GraphicsDevice.RenderState.FillMode = prev;
        }
    }
}