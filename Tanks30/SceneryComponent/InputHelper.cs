using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GameComponents
{
    public abstract class InputHelper
    {
        public static GraphicsDevice GraphicsDevice;
        private static KeyboardState lastKeyboardState;
        private static KeyboardState currentKeyboardState;
        private static MouseState lastMouseState;
        private static MouseState currentMouseState;

        public static float Pitch;
        public static float Yaw;
        public static float PitchDelta;
        public static float YawDelta;

        public static void Begin(GameTime gameTime)
        {
            float amountOfMovement = (float)gameTime.ElapsedGameTime.Milliseconds / 30.0f;

            currentKeyboardState = Keyboard.GetState();
            currentMouseState = Mouse.GetState();

            int centerX = GraphicsDevice.Viewport.Width / 2;
            int centerY = GraphicsDevice.Viewport.Height / 2;

            Mouse.SetPosition(centerX, centerY);

            float pitch = MathHelper.ToRadians((currentMouseState.Y - centerY) * 90f * 0.005f);
            float yaw = MathHelper.ToRadians((currentMouseState.X - centerX) * 90f * 0.005f);

            Pitch -= pitch;
            Yaw -= yaw;
            PitchDelta = -pitch;
            YawDelta = -yaw;
        }

        public static void End()
        {
            lastKeyboardState = currentKeyboardState;
            lastMouseState = currentMouseState;
        }

        public static bool KeyUpEvent(Keys key)
        {
            return (currentKeyboardState.IsKeyUp(key) && lastKeyboardState.IsKeyDown(key));
        }

        public static bool KeyDownEvent(Keys key)
        {
            return (currentKeyboardState.IsKeyDown(key) && lastKeyboardState.IsKeyUp(key));
        }
    }
}
