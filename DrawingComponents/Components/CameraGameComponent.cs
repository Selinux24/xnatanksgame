using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DrawingComponents
{
    /// <summary>
    /// Componente de cámara
    /// </summary>
    public partial class CameraGameComponent : Microsoft.Xna.Framework.GameComponent
    {
        /// <summary>
        /// Vector posición
        /// </summary>
        protected Vector3 position = Vector3.Zero;
        /// <summary>
        /// Quaternion de rotación en X
        /// </summary>
        protected Quaternion yaw = Quaternion.Identity;
        /// <summary>
        /// Quaternion de rotación en Y
        /// </summary>
        protected Quaternion pitch = Quaternion.Identity;

        /// <summary>
        /// Obtiene la matriz
        /// </summary>
        public Matrix RotationMatrix
        {
            get
            {
                return Matrix.CreateFromQuaternion(pitch) * Matrix.CreateFromQuaternion(yaw);
            }
        }
        /// <summary>
        /// Obtiene o establece la posición de la cámara
        /// </summary>
        public Vector3 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }
        /// <summary>
        /// Sensibilidad del teclado en porcentaje
        /// </summary>
        public float KeyBoardSensibility = 100;
        /// <summary>
        /// Sensibilidad del ratón en el eje vertical en porcentaje
        /// </summary>
        public float MouseVerticalSensibility = 100;
        /// <summary>
        /// Sensibilidad del ratón en el eje horizontal en porcentaje
        /// </summary>
        public float MouseHorizontalSensibility = 100;

        /// <summary>
        /// Constructor
        /// </summary>
        public CameraGameComponent(Game game)
            : base(game)
        {

        }

        /// <summary>
        /// Inicializar
        /// </summary>
        public override void Initialize()
        {
            int x = this.Game.GraphicsDevice.Viewport.Width / 2;
            int y = this.Game.GraphicsDevice.Viewport.Height / 2;

            Mouse.SetPosition(x, y);

            base.Initialize();
        }
        /// <summary>
        /// Actualizar
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            UpdatePosition();

            UpdateRotation();

            UpdateFirstPerson();
        }

        /// <summary>
        /// Actualizar la posición
        /// </summary>
        void UpdatePosition()
        {
            KeyboardState keyboardState = Keyboard.GetState();

            float sensibilityFactor = this.KeyBoardSensibility / 100f;

            if (keyboardState.IsKeyDown(Keys.W))
            {
                position += (RotationMatrix.Forward * sensibilityFactor);
            }
            if (keyboardState.IsKeyDown(Keys.S))
            {
                position += (RotationMatrix.Backward * sensibilityFactor);
            }
            if (keyboardState.IsKeyDown(Keys.A))
            {
                position += (RotationMatrix.Left * sensibilityFactor);
            }
            if (keyboardState.IsKeyDown(Keys.D))
            {
                position += (RotationMatrix.Right * sensibilityFactor);
            }
        }
        /// <summary>
        /// Actualizar la rotación
        /// </summary>
        void UpdateRotation()
        {
            int x = this.Game.GraphicsDevice.Viewport.Width / 2;
            int y = this.Game.GraphicsDevice.Viewport.Height / 2;

            int lastMouseX = Mouse.GetState().X - x;
            int lastMouseY = Mouse.GetState().Y - y;

            Mouse.SetPosition(x, y);

            float avatarYaw = -lastMouseX * 0.0015f * (this.MouseHorizontalSensibility / 100f);
            float avatarPitch = lastMouseY * 0.0010f * (this.MouseVerticalSensibility / 100f);

            yaw *= Quaternion.CreateFromAxisAngle(Vector3.Up, avatarYaw);
            pitch *= Quaternion.CreateFromAxisAngle(Vector3.Right, avatarPitch);
        }
        /// <summary>
        /// Actualizar cámara en modo primera persona
        /// </summary>
        void UpdateFirstPerson()
        {
            // Create a vector pointing the direction the camera is facing.
            Vector3 transformedReference = Vector3.Transform(Vector3.Forward, RotationMatrix);

            // Calculate the position the camera is looking at.
            Vector3 cameraLookat = transformedReference + position;

            // Set up view matrix and projection matrix
            GlobalMatrices.View = Matrix.CreateLookAt(position, cameraLookat, Vector3.Up);
        }
    }
}


