using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameComponents.Scenery
{
    using Common;

    /// <summary>
    /// Escenario
    /// </summary>
    public class SceneryGameComponent : DrawableGameComponent
    {
        /// <summary>
        /// Contenidos
        /// </summary>
        protected ContentManager Content = null;

        /// <summary>
        /// Escenografía
        /// </summary>
        public Scenery Scenery;

        /// <summary>
        /// Indica el modo de relleno a usar
        /// </summary>
        private FillMode m_FillMode = FillMode.Solid;
        /// <summary>
        /// Obtiene o establece el modo de relleno a usar
        /// </summary>
        public FillMode FillMode
        {
            get
            {
                return m_FillMode;
            }
            set
            {
                m_FillMode = value;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public SceneryGameComponent(Game game)
            : base(game)
        {
            this.Content = game.Content;
        }

        /// <summary>
        /// Cargar el contenito del componente
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();

            // Cargar el contenido
            this.Scenery = this.Content.Load<Scenery>(@"Content\Terrain\Scenery");
        }
        /// <summary>
        /// Dibujar el contenido gráfico del componente
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public override void Draw(GameTime gameTime)
        {
            FillMode currentMode = this.GraphicsDevice.RenderState.FillMode;
            this.GraphicsDevice.RenderState.FillMode = m_FillMode;

            this.Scenery.SetWorldViewProjection(
                Matrix.Identity,
                GlobalMatrices.gViewMatrix,
                GlobalMatrices.gGlobalProjectionMatrix);

            this.Scenery.SetLights(
                SceneryEnvironment.Ambient.LightingEnabled,
                SceneryEnvironment.Ambient.LightDirection,
                0.2f);

            this.Scenery.Draw(this.GraphicsDevice, gameTime);

            this.GraphicsDevice.RenderState.FillMode = currentMode;
        }
    }
}
