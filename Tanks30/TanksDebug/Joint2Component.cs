using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TanksDebug
{
    using Common;
    using Common.Helpers;
    using Physics;

    /// <summary>
    /// Componente barra
    /// </summary>
    public class Joint2Component : DrawableGameComponent
    {
        /// <summary>
        /// Barra
        /// </summary>
        public readonly Joint2 Rod;

        /// <summary>
        /// Efecto de renderizado
        /// </summary>
        private BasicEffect m_BasicEffect = null;
        /// <summary>
        /// Declaración de vértices de la barra
        /// </summary>
        private VertexDeclaration m_VertexDeclaration = null;
        /// <summary>
        /// Vértices
        /// </summary>
        private VertexPositionColor[] m_LineVertices = null;
        /// <summary>
        /// Tipo de primitiva
        /// </summary>
        private PrimitiveType m_LinePrimitiveType = PrimitiveType.LineList;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game">Juego</param>
        /// <param name="worldPoint">Punto fijo al que está conectado el mundo</param>
        /// <param name="obj">Objeto al que está conectado la barra</param>
        /// <param name="relativeContactPoint">Posición relativa al objeto</param>
        /// <param name="size">Longitud de la barra</param>
        public Joint2Component(Game game, Vector3 worldPoint, IPhysicObject obj, Vector3 relativeContactPoint, float size)
            : this(game, null, worldPoint, obj, relativeContactPoint, size)
        {

        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game">Juego</param>
        /// <param name="objOne">Objeto primero al que está conectado la barra</param>
        /// <param name="relativeContactPointOne">Posición relativa al objeto uno</param>
        /// <param name="objTwo">Objeto segundo al que está conectado la barra</param>
        /// <param name="relativeContactPointTwo">Posición relativa al objeto dos</param>
        /// <param name="size">Longitud de la barra</param>
        public Joint2Component(
            Game game,
            IPhysicObject objOne,
            Vector3 relativeContactPointOne,
            IPhysicObject objTwo,
            Vector3 relativeContactPointTwo,
            float size)
            : base(game)
        {
            this.Rod = new Joint2(objOne, relativeContactPointOne, objTwo, relativeContactPointTwo, size);

            PolyGenerator.InitializeLine(out this.m_LineVertices, Vector3.Zero, Vector3.One, Color.Red);
        }

        /// <summary>
        /// Carga del contenido
        /// </summary>
        protected override void LoadContent()
        {
            this.m_BasicEffect = new BasicEffect(this.GraphicsDevice, null);
            this.m_BasicEffect.EnableDefaultLighting();

            this.m_VertexDeclaration = new VertexDeclaration(this.GraphicsDevice, VertexPositionNormalTexture.VertexElements);

            base.LoadContent();
        }
        /// <summary>
        /// Dibujado
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            this.GraphicsDevice.VertexDeclaration = this.m_VertexDeclaration;

            Vector3 trnPositionOne = this.Rod.PointOneWorld;
            Vector3 trnPositionTwo = this.Rod.PointTwoWorld;
            this.DrawLine(trnPositionOne, trnPositionTwo);

            this.GraphicsDevice.VertexDeclaration = null;
        }
        /// <summary>
        /// Dibuja una línea
        /// </summary>
        /// <param name="position1">Posición 1</param>
        /// <param name="position2">Posición 2</param>
        private void DrawLine(Vector3 position1, Vector3 position2)
        {
            this.m_LineVertices[0].Position = position1;
            this.m_LineVertices[1].Position = position2;

            FillMode prev = this.GraphicsDevice.RenderState.FillMode;
            this.GraphicsDevice.RenderState.FillMode = FillMode.Solid;

            this.m_BasicEffect.EnableDefaultLighting();

            this.m_BasicEffect.Texture = null;
            this.m_BasicEffect.TextureEnabled = false;
            this.m_BasicEffect.VertexColorEnabled = true;

            this.m_BasicEffect.World = GlobalMatrices.gWorldMatrix;
            this.m_BasicEffect.View = GlobalMatrices.gViewMatrix;
            this.m_BasicEffect.Projection = GlobalMatrices.gGlobalProjectionMatrix;

            this.m_BasicEffect.Begin();

            foreach (EffectPass pass in this.m_BasicEffect.CurrentTechnique.Passes)
            {
                pass.Begin();

                this.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(
                    this.m_LinePrimitiveType,
                    m_LineVertices,
                    0,
                    m_LineVertices.Length / 2);

                pass.End();
            }

            this.m_BasicEffect.End();

            this.GraphicsDevice.RenderState.FillMode = prev;
        }
    }
}
