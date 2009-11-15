using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DrawingComponents;

namespace BigBallisticDemo
{
    /// <summary>
    /// Componente que dibuja las balas
    /// </summary>
    class AmmoDrawer : DrawableGameComponent
    {
        /// <summary>
        /// Efecto
        /// </summary>
        private BasicEffect m_BasicEffect = null;
        /// <summary>
        /// Declaraci�n de formato de v�rtices
        /// </summary>
        private VertexDeclaration m_VertexDeclaration = null;
        /// <summary>
        /// V�rtices
        /// </summary>
        private VertexPositionNormalTexture[] m_Vertices = null;
        /// <summary>
        /// �ndices
        /// </summary>
        private short[] m_Indices = null;
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
        public AmmoDrawer(Game game)
            : base(game)
        {

        }

        /// <summary>
        /// Carga de contenidos gr�ficos
        /// </summary>
        protected override void LoadContent()
        {
            PolyGenerator.InitializeSphere(out this.m_Vertices, out this.m_Indices, 1f);

            this.m_VertexDeclaration = new VertexDeclaration(this.GraphicsDevice, VertexPositionNormalTexture.VertexElements);
            this.m_BasicEffect = new BasicEffect(this.GraphicsDevice, null);
            this.m_Texture = this.Game.Content.Load<Texture2D>(@"dharma");

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

            m_BasicEffect.View = GlobalMatrices.View;
            m_BasicEffect.Projection = GlobalMatrices.Projection;

            foreach (AmmoRound round in Rounds)
            {
                if (round.ShotType != ShotType.UnUsed)
                {
                    float radius = round.Radius;

                    m_BasicEffect.World = Matrix.CreateScale(radius) * round.Transform * GlobalMatrices.World;

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
        /// Dibuja la geometr�a usando una lista de v�rtices
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
        /// Dibuja la geometr�a usando una lista de v�rtices e �ndices
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