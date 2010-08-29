using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameComponents.Weapons
{
    using Common;
    using Common.Helpers;
    using Physics;

    /// <summary>
    /// Componente que dibuja las balas
    /// </summary>
    public class AmmoDrawer : DrawableGameComponent
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
        /// Índices
        /// </summary>
        private short[] m_Indices = null;
        /// <summary>
        /// Nombre de la textura
        /// </summary>
        private string m_TextureAsset = null;
        /// <summary>
        /// Textura
        /// </summary>
        private Texture2D m_Texture = null;

        /// <summary>
        /// Lista de balas a dibujar
        /// </summary>
        public AmmoRound[] Rounds = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game">Juego</param>
        /// <param name="textureAsset">Nombre de la textura</param>
        public AmmoDrawer(Game game, string textureAsset)
            : base(game)
        {
            this.m_TextureAsset = textureAsset;
        }

        /// <summary>
        /// Carga de contenidos gráficos
        /// </summary>
        protected override void LoadContent()
        {
            PolyGenerator.InitializeSphere(out this.m_Vertices, out this.m_Indices, 1f);

            this.m_VertexDeclaration = new VertexDeclaration(this.GraphicsDevice, VertexPositionNormalTexture.VertexElements);
            this.m_BasicEffect = new BasicEffect(this.GraphicsDevice, null);
            this.m_Texture = this.Game.Content.Load<Texture2D>(this.m_TextureAsset);

            base.LoadContent();
        }
        /// <summary>
        /// Dibuja la lista de balas del componente
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public override void Draw(GameTime gameTime)
        {
            this.GraphicsDevice.VertexDeclaration = m_VertexDeclaration;

            m_BasicEffect.EnableDefaultLighting();
            m_BasicEffect.Texture = m_Texture;
            m_BasicEffect.TextureEnabled = (m_Texture != null);
            m_BasicEffect.VertexColorEnabled = (m_Texture == null);

            m_BasicEffect.View = GlobalMatrices.gViewMatrix;
            m_BasicEffect.Projection = GlobalMatrices.gProjectionMatrix;

            foreach (AmmoRound round in Rounds)
            {
                if (round.IsActive())
                {
                    float radius = round.Radius;

                    m_BasicEffect.World = Matrix.CreateScale(radius) * round.Transform * GlobalMatrices.gWorldMatrix;

                    if (m_Indices != null && m_Indices.Length > 0)
                    {
                        DrawIndexBuffer();
                    }
                    else
                    {
                        DrawVertexBuffer();
                    }
                }
            }

            this.GraphicsDevice.VertexDeclaration = null;

            base.Draw(gameTime);
        }

        /// <summary>
        /// Dibuja la geometría usando una lista de vértices
        /// </summary>
        private void DrawVertexBuffer()
        {
            m_BasicEffect.Begin();

            foreach (EffectPass pass in m_BasicEffect.CurrentTechnique.Passes)
            {
                pass.Begin();

                this.GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>(
                    PrimitiveType.TriangleList,
                    m_Vertices,
                    0,
                    m_Vertices.Length / 3);

                pass.End();
            }

            m_BasicEffect.End();
        }
        /// <summary>
        /// Dibuja la geometría usando una lista de vértices e índices
        /// </summary>
        private void DrawIndexBuffer()
        {
            m_BasicEffect.Begin();

            foreach (EffectPass pass in m_BasicEffect.CurrentTechnique.Passes)
            {
                pass.Begin();

                this.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(
                    PrimitiveType.TriangleList,
                    m_Vertices,
                    0,
                    m_Vertices.Length,
                    m_Indices,
                    0,
                    m_Indices.Length / 3);

                pass.End();
            }

            m_BasicEffect.End();
        }
    }
}
