using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameComponents.Text
{
    /// <summary>
    /// Componente de escritura de texto
    /// </summary>
    public class TextDrawerComponent : DrawableGameComponent
    {
        /// <summary>
        /// Componente para dibujar el texto
        /// </summary>
        protected SpriteBatch SpriteBatch;
        /// <summary>
        /// Fuente
        /// </summary>
        protected SpriteFont Font;
        /// <summary>
        /// Posición del texto
        /// </summary>
        protected Vector2 OutputPosition = new Vector2(5, 5);
        /// <summary>
        /// Texto
        /// </summary>
        protected string OutputText;
        /// <summary>
        /// Color
        /// </summary>
        protected Color OutputColor = Color.White;
        /// <summary>
        /// Gestor de contenidos
        /// </summary>
        protected ContentManager Content;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game">Juego</param>
        public TextDrawerComponent(Game game)
            : base(game)
        {
            this.Content = game.Content;
        }
        /// <summary>
        /// Inicialización
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            this.SpriteBatch = new SpriteBatch(this.GraphicsDevice);
        }
        /// <summary>
        /// Carga de contenidos
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();

            this.Font = this.Content.Load<SpriteFont>(@"Content/Info");
        }
        /// <summary>
        /// Dibujado
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (!string.IsNullOrEmpty(this.OutputText))
            {
                this.SpriteBatch.Begin();

                this.SpriteBatch.DrawString(
                    this.Font,
                    this.OutputText,
                    this.OutputPosition,
                    this.OutputColor,
                    0,
                    Vector2.Zero,
                    1.0f,
                    SpriteEffects.None,
                    0.5f);

                this.SpriteBatch.End();

                this.GraphicsDevice.RenderState.DepthBufferEnable = true;
                this.GraphicsDevice.RenderState.AlphaBlendEnable = false;
                this.GraphicsDevice.RenderState.AlphaTestEnable = false;
                this.GraphicsDevice.SamplerStates[0].AddressU = TextureAddressMode.Wrap;
                this.GraphicsDevice.SamplerStates[0].AddressV = TextureAddressMode.Wrap;
            }
        }

        /// <summary>
        /// Escribe el texto especificado en la posición especificada
        /// </summary>
        /// <param name="text">Texto</param>
        /// <param name="x">Posición X</param>
        /// <param name="y">Posición Y</param>
        public void WriteText(string text, int x, int y)
        {
            this.OutputText = text;
            this.OutputPosition.X = x;
            this.OutputPosition.Y = y;
        }
        /// <summary>
        /// Escribe el texto especificado en la posición especificada
        /// </summary>
        /// <param name="text">Texto</param>
        /// <param name="x">Posición X</param>
        /// <param name="y">Posición Y</param>
        /// <param name="color">Color</param>
        public void WriteText(string text, int x, int y, Color color)
        {
            this.OutputColor = color;

            this.WriteText(text, x, y);
        }
    }
}
