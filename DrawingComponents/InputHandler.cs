using System;
using Microsoft.Xna.Framework.Input;

namespace DrawingComponents
{
    /// <summary>
    /// Gestor de entrada de teclado y pulsaciones de ratón
    /// </summary>
    public abstract class InputHandler
    {
        // Estado de teclado anterior
        private static KeyboardState OldKeyBoardState;
        // Estado de teclado actual
        private static KeyboardState CurrentKeyBoardState;
        // Estado de ratón anterior
        private static MouseState OldMouseState;
        // Estado de ratón actual
        private static MouseState CurrentMouseState;

        /// <summary>
        /// Comienza la captura de teclado
        /// </summary>
        public static void Begin()
        {
            CurrentKeyBoardState = Keyboard.GetState();

            CurrentMouseState = Mouse.GetState();
        }
        /// <summary>
        /// Indica si la tecla especificada se está presionando
        /// </summary>
        /// <param name="key">Tecla</param>
        /// <returns>Devuelve verdadero si actualmente se está pulsando la tecla</returns>
        public static bool KeyPressing(Keys key)
        {
            return CurrentKeyBoardState.IsKeyDown(key);
        }
        /// <summary>
        /// Indica si la tecla especificada se acaba de soltar
        /// </summary>
        /// <param name="key">Tecla</param>
        /// <returns>Devuelve verdadero si en el estado anterior se estaba pulsando la tecla, pero en el actual no</returns>
        public static bool KeyPressed(Keys key)
        {
            return (CurrentKeyBoardState.IsKeyDown(key) && !OldKeyBoardState.IsKeyDown(key));
        }

        /// <summary>
        /// Indica si el botón izquierdo del ratón está siendo pulsado
        /// </summary>
        public static bool LeftButtonPressing
        {
            get
            {
                return (CurrentMouseState.LeftButton == ButtonState.Pressed);
            }
        }
        /// <summary>
        /// Indica si el botón izquierdo del ratón acaba de ser soltado
        /// </summary>
        public static bool LeftButtonPressed
        {
            get
            {
                return (OldMouseState.LeftButton == ButtonState.Pressed &&
                    CurrentMouseState.LeftButton == ButtonState.Released);
            }
        }
        /// <summary>
        /// Indica si el botón derecho del ratón está siendo pulsado
        /// </summary>
        public static bool RightButtonPressing
        {
            get
            {
                return (CurrentMouseState.RightButton == ButtonState.Pressed);
            }
        }
        /// <summary>
        /// Indica si el botón derecho del ratón acaba de ser soltado
        /// </summary>
        public static bool RightButtonPressed
        {
            get
            {
                return (OldMouseState.RightButton == ButtonState.Pressed &&
                    CurrentMouseState.RightButton == ButtonState.Released);
            }
        }

        /// <summary>
        /// Finaliza la captura de teclado
        /// </summary>
        public static void End()
        {
            OldKeyBoardState = CurrentKeyBoardState;

            OldMouseState = CurrentMouseState;
        }
    }
}
