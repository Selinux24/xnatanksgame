using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;
using DrawingComponents;
using Physics;

namespace BigBallisticDemo
{
    /// <summary>
    /// Componente que dibuja geometría
    /// </summary>
    public class CubeGameComponent : Microsoft.Xna.Framework.DrawableGameComponent
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
        /// Transformación
        /// </summary>
        private Matrix m_Transform = Matrix.Identity;
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

            PolyGenerator.InitializeCube(out m_Vertices, min, max);

            this.m_PrimitiveCount = m_Vertices.Length / 3;
        }

        /// <summary>
        /// Carga el contenido gráfico del componente
        /// </summary>
        protected override void LoadContent()
        {
            this.m_VertexDeclaration = new VertexDeclaration(this.GraphicsDevice, VertexPositionNormalTexture.VertexElements);
            this.m_BasicEffect = new BasicEffect(this.GraphicsDevice, null);
            this.m_BasicEffect.EnableDefaultLighting();
            this.m_Texture = this.Game.Content.Load<Texture2D>(@"crate");

            base.LoadContent();
        }
        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.m_Transform = this.m_Box.Transform;
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

            this.GraphicsDevice.VertexDeclaration = m_VertexDeclaration;

            FillMode prev = this.GraphicsDevice.RenderState.FillMode;
            this.GraphicsDevice.RenderState.FillMode = this.FillMode;

            m_BasicEffect.Texture = m_Texture;
            m_BasicEffect.TextureEnabled = (m_Texture != null);
            m_BasicEffect.VertexColorEnabled = (m_Texture == null);

            m_BasicEffect.World = this.m_Transform * GlobalMatrices.World;
            m_BasicEffect.View = GlobalMatrices.View;
            m_BasicEffect.Projection = GlobalMatrices.Projection;

            if (m_Box.Body.IsAwake)
            {
                m_BasicEffect.DiffuseColor = Color.BurlyWood.ToVector3();
                m_BasicEffect.EmissiveColor = Color.Black.ToVector3();
                m_BasicEffect.SpecularColor = Color.Black.ToVector3();
                m_BasicEffect.SpecularPower = 0f;
            }
            else
            {
                m_BasicEffect.DiffuseColor = Color.Gray.ToVector3();
                m_BasicEffect.EmissiveColor = Color.Black.ToVector3();
                m_BasicEffect.SpecularColor = Color.Black.ToVector3();
                m_BasicEffect.SpecularPower = 0f;
            }

            m_BasicEffect.PreferPerPixelLighting = true;

            m_BasicEffect.Begin();

            foreach (EffectPass pass in m_BasicEffect.CurrentTechnique.Passes)
            {
                pass.Begin();

                this.GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>(
                    m_PrimitiveType,
                    m_Vertices,
                    0,
                    m_PrimitiveCount);

                pass.End();
            }

            m_BasicEffect.End();

            this.GraphicsDevice.RenderState.FillMode = prev;

            this.GraphicsDevice.VertexDeclaration = null;
        }
        /// <summary>
        /// Registra el componente en el controlador de físicas
        /// </summary>
        /// <param name="controller">Controlador de físicas</param>
        internal void Register(PhysicsController controller)
        {
            controller.RegisterBox(this.m_Box);
        }
        /// <summary>
        /// Establece el estado inicial de posición y orientación del componente
        /// </summary>
        /// <param name="position">Posición</param>
        /// <param name="orientation">Orientación</param>
        public void SetState(Vector3 position, Quaternion orientation)
        {
            this.m_Box.SetState(position, orientation);
        }
    }
}