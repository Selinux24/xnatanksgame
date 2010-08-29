using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DrawingComponents
{
    using Common;
    using Common.Helpers;

    /// <summary>
    /// Componente que dibuja geometr�a
    /// </summary>
    public class SphereGameComponent : Microsoft.Xna.Framework.DrawableGameComponent
    {
        /// <summary>
        /// Efecto
        /// </summary>
        protected BasicEffect basicEffect;
        /// <summary>
        /// Declaraci�n de formato de v�rtices
        /// </summary>
        protected VertexDeclaration basicEffectVertexDeclaration;
        /// <summary>
        /// V�rtices
        /// </summary>
        protected VertexPositionNormalTexture[] vertices;
        /// <summary>
        /// Tipo de primitivas a dibujar
        /// </summary>
        protected PrimitiveType primitiveType = PrimitiveType.TriangleList;
        /// <summary>
        /// N�mero de primitivas
        /// </summary>
        protected int primitiveCount = -1;
        /// <summary>
        /// Textura
        /// </summary>
        protected Texture2D Texture = null;
        /// <summary>
        /// Indica si es geometr�a indexada
        /// </summary>
        protected bool indexed = false;
        /// <summary>
        /// �ndices
        /// </summary>
        protected short[] indices;
        /// <summary>
        /// M�todo de relleno de la geometr�a
        /// </summary>
        protected FillMode FillMode = FillMode.Solid;

        /// <summary>
        /// Transformaci�n
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
        /// Carga los contenidos gr�ficos
        /// </summary>
        protected override void LoadContent()
        {
            this.basicEffectVertexDeclaration = new VertexDeclaration(this.GraphicsDevice, VertexPositionNormalTexture.VertexElements);
            this.basicEffect = new BasicEffect(this.GraphicsDevice, null);
            this.Texture = this.Game.Content.Load<Texture2D>(@"dharma");

            base.LoadContent();
        }
        /// <summary>
        /// Dibuja la geometr�a
        /// </summary>
        /// <param name="device">Dispositivo gr�fico</param>
        /// <param name="world">Matriz mundo del objeto</param>
        /// <param name="view">Matriz vista</param>
        /// <param name="projection">Matriz proyecci�n</param>
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