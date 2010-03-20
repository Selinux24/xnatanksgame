using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Common.Helpers
{
    /// <summary>
    /// Captura de entrada del usuario
    /// </summary>
    public abstract class InputHelper
    {
        /// <summary>
        /// Dispositivo gr�fico
        /// </summary>
        private static GraphicsDevice GraphicsDevice;

        /// <summary>
        /// �ltimo estado del teclado
        /// </summary>
        private static KeyboardState g_LastKeyboardState;
        /// <summary>
        /// Estado del teclado
        /// </summary>
        private static KeyboardState g_CurrentKeyboardState;
        /// <summary>
        /// �ltimo estado del rat�n
        /// </summary>
        private static MouseState g_LastMouseState;
        /// <summary>
        /// Estado del rat�n
        /// </summary>
        private static MouseState g_CurrentMouseState;

        /// <summary>
        /// Rotaci�n actual en el eje X
        /// </summary>
        public static float Pitch { get; protected set; }
        /// <summary>
        /// Rotaci�n actual en el eje Y
        /// </summary>
        public static float Yaw { get; protected set; }
        /// <summary>
        /// Variaci�n de la rotaci�n actual en el eje X
        /// </summary>
        public static float PitchDelta { get; protected set; }
        /// <summary>
        /// Variaci�n de la rotaci�n actual en el eje Y
        /// </summary>
        public static float YawDelta { get; protected set; }
        /// <summary>
        /// Indica si la rotaci�n en X del rat�n es invertida
        /// </summary>
        public static bool InvertMouse = false;

        /// <summary>
        /// Obtiene si el bot�n izquierdo del rat�n est� siendo presionado
        /// </summary>
        /// <returns>Devuelve verdadero si el bot�n izquierdo del rat�n est� siendo presionado</returns>
        public static bool LeftMouseButtonPressed()
        {
            return (g_CurrentMouseState.LeftButton == ButtonState.Pressed);
        }
        /// <summary>
        /// Obtiene si el bot�n izquierdo del rat�n acaba de soltarse
        /// </summary>
        /// <returns>Devuelve verdadero si el bot�n izquierdo del rat�n acaba de soltarse</returns>
        public static bool LeftMouseButtonEvent()
        {
            return (g_CurrentMouseState.LeftButton == ButtonState.Released && g_LastMouseState.LeftButton == ButtonState.Pressed);
        }

        /// <summary>
        /// Inicializa el componente
        /// </summary>
        /// <param name="device">Dispositivo gr�fico</param>
        public static void Initialize(GraphicsDevice device)
        {
            GraphicsDevice = device;
        }
        /// <summary>
        /// Comenzar la lectura del rat�n y del teclado
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public static void Begin(GameTime gameTime)
        {
            float amountOfMovement = (float)gameTime.ElapsedGameTime.Milliseconds / 30.0f;

            g_CurrentKeyboardState = Keyboard.GetState();
            g_CurrentMouseState = Mouse.GetState();

            int centerX = GraphicsDevice.Viewport.Width / 2;
            int centerY = GraphicsDevice.Viewport.Height / 2;

            Mouse.SetPosition(centerX, centerY);

            float pitch = MathHelper.ToRadians((g_CurrentMouseState.Y - centerY) * 90f * 0.005f);
            float yaw = MathHelper.ToRadians((g_CurrentMouseState.X - centerX) * 90f * 0.005f);

            Pitch -= pitch;
            Yaw -= yaw;
            PitchDelta = (InvertMouse) ? -pitch : pitch;
            YawDelta = -yaw;
        }
        /// <summary>
        /// Finalizar la lectura del rat�n y del teclado
        /// </summary>
        public static void End()
        {
            g_LastKeyboardState = g_CurrentKeyboardState;
            g_LastMouseState = g_CurrentMouseState;
        }

        /// <summary>
        /// Obtiene si la tecla especificada est� siendo presionada
        /// </summary>
        /// <param name="key">Tecla</param>
        /// <returns>Devuelve verdadero si la tecla especificada est� siendo presionada</returns>
        public static bool IsKeyDown(Keys key)
        {
            return g_CurrentKeyboardState.IsKeyDown(key);
        }
        /// <summary>
        /// Obtiene si la tecla especificada no est� siendo presionada
        /// </summary>
        /// <param name="key">Tecla</param>
        /// <returns>Devuelve verdadero si la tecla especificada no est� siendo presionada</returns>
        public static bool IsKeyUp(Keys key)
        {
            return g_CurrentKeyboardState.IsKeyUp(key);
        }
        /// <summary>
        /// Obtiene si la tecla especificada acaba de soltarse
        /// </summary>
        /// <param name="key">Tecla</param>
        /// <returns>Devuelve verdadero si la tecla acaba de soltarse</returns>
        public static bool KeyUpEvent(Keys key)
        {
            return (g_CurrentKeyboardState.IsKeyUp(key) && g_LastKeyboardState.IsKeyDown(key));
        }
        /// <summary>
        /// Obtiene si la tecla especificada acaba de pulsarse
        /// </summary>
        /// <param name="key">Tecla</param>
        /// <returns>Devuelve verdadero si la tecla acaba de pulsarse</returns>
        public static bool KeyDownEvent(Keys key)
        {
            return (g_CurrentKeyboardState.IsKeyDown(key) && g_LastKeyboardState.IsKeyUp(key));
        }
    }
}
