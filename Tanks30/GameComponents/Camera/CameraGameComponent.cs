using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameComponents.Camera
{
    using Common;
    using GameComponents.Scenery;
    using GameComponents.Vehicles;
    using Physics;

    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class CameraGameComponent : GameComponent
    {
        /// <summary>
        /// Modos posibles de vista de la c�mara
        /// </summary>
        public enum CameraModes
        {
            /// <summary>
            /// Libre
            /// </summary>
            Free,
            /// <summary>
            /// Primera persona
            /// </summary>
            FirstPerson,
            /// <summary>
            /// Tercera persona
            /// </summary>
            ThirdPerson,
        }

        /// <summary>
        /// Dispositivo gr�fico
        /// </summary>
        protected GraphicsDevice GraphicsDevice;

        /// <summary>
        /// Posici�n
        /// </summary>
        private Vector3 m_Position = Vector3.Zero;
        /// <summary>
        /// Direcci�n
        /// </summary>
        private Vector3 m_Direction = Vector3.Forward;
        /// <summary>
        /// Vertical
        /// </summary>
        private Vector3 m_Vertical = Vector3.Up;
        /// <summary>
        /// Rotaci�n en X
        /// </summary>
        private Quaternion m_Pitch = Quaternion.Identity;
        /// <summary>
        /// Rotaci�n en Y
        /// </summary>
        private Quaternion m_Yaw = Quaternion.Identity;
        /// <summary>
        /// Obtiene la matriz de rotaci�n a partir de quaternion de rotaci�n
        /// </summary>
        private Matrix m_RotationMatrix = Matrix.Identity;
        /// <summary>
        /// Modo de c�mara
        /// </summary>
        private CameraModes m_Mode = CameraModes.Free;

        /// <summary>
        /// Modo de vista de la c�mara
        /// </summary>
        public CameraModes Mode 
        {
            get
            {
                return m_Mode;
            }
            set
            {
                if (m_Mode != value)
                {
                    if (m_Mode != CameraModes.Free)
                    {
                        Vector3 scale;
                        Quaternion rotation;
                        Vector3 position;
                        GlobalMatrices.gViewMatrix.Decompose(out scale, out rotation, out position);

                        float yaw;
                        float pitch;
                        float roll;
                        rotation.Decompose(out yaw, out pitch, out roll);

                        m_Yaw = Quaternion.CreateFromAxisAngle(Vector3.Up, -yaw);
                        m_Pitch = Quaternion.CreateFromAxisAngle(Vector3.Right, -pitch);
                        m_Vertical = Vector3.Up;
                    }
                }

                m_Mode = value;
            }
        }

        /// <summary>
        /// Tecla para avanzar
        /// </summary>
        public Keys Forward = Keys.W;
        /// <summary>
        /// Tecla para retroceder
        /// </summary>
        public Keys Backward = Keys.S;
        /// <summary>
        /// Tecla para moverse a la izquierda
        /// </summary>
        public Keys Left = Keys.A;
        /// <summary>
        /// Tecla para moverse a la derecha
        /// </summary>
        public Keys Right = Keys.D;
        /// <summary>
        /// Tecla para moverse arriba
        /// </summary>
        public Keys Up = Keys.Q;
        /// <summary>
        /// Tecla para moverse abajo
        /// </summary>
        public Keys Down = Keys.E;
        /// <summary>
        /// Indica si se usa el rat�n para girar la c�mara
        /// </summary>
        public bool UseMouse = false;
        /// <summary>
        /// Tecla para girar a la izquierda
        /// </summary>
        public Keys YawLeft = Keys.U;
        /// <summary>
        /// Tecla para girar a la derecha
        /// </summary>
        public Keys YawRight = Keys.I;
        /// <summary>
        /// Tecla para girar hacia arriba
        /// </summary>
        public Keys PitchUp = Keys.Y;
        /// <summary>
        /// Tecla para girar hacia abajo
        /// </summary>
        public Keys PitchDown = Keys.H;

        /// <summary>
        /// Sensibilidad del teclado en porcentaje
        /// </summary>
        public static float KeyBoardSensibility = 100;
        /// <summary>
        /// Sensibilidad del rat�n en el eje vertical en porcentaje
        /// </summary>
        public static float MouseVerticalSensibility = 100;
        /// <summary>
        /// Sensibilidad del rat�n en el eje horizontal en porcentaje
        /// </summary>
        public static float MouseHorizontalSensibility = 100;

        /// <summary>
        /// Establece la posici�n de la c�mara
        /// </summary>
        public virtual Vector3 Position
        {
            get
            {
                return this.m_Position;
            }
        }
        /// <summary>
        /// Obtiene o establece la direcci�n
        /// </summary>
        public virtual Vector3 Direction
        {
            get
            {
                return this.m_Direction;
            }
        }
        /// <summary>
        /// Obtiene o establece la vertical de c�mara
        /// </summary>
        public virtual Vector3 Vertical
        {
            get
            {
                return this.m_Vertical;
            }
        }

        /// <summary>
        /// Posici�n relativa del espectador
        /// </summary>
        public Vector3 ViewerRelativePosition = new Vector3(0f, 10f, 18f);
        /// <summary>
        /// Modelo que sigue la c�mara
        /// </summary>
        public Vehicle ModelToFollow { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game">Juego</param>
        public CameraGameComponent(Game game)
            : base(game)
        {
            GraphicsDeviceManager deviceManager = (GraphicsDeviceManager)game.Services.GetService(typeof(IGraphicsDeviceManager));
            if (deviceManager != null)
            {
                GraphicsDevice = deviceManager.GraphicsDevice;

                // Calcular la matriz de proyecci�n
                GlobalMatrices.gGlobalProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                    MathHelper.PiOver4,
                    (float)GraphicsDevice.Viewport.Width / (float)GraphicsDevice.Viewport.Height,
                    SceneryEnvironment.GlobalNearClip,
                    SceneryEnvironment.GlobalFarClip * 100);

                // Calcular la matriz de proyecci�n
                GlobalMatrices.gLODHighProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                    MathHelper.PiOver4,
                    (float)GraphicsDevice.Viewport.Width / (float)GraphicsDevice.Viewport.Height,
                    SceneryEnvironment.LevelOfDetail.HighNearClip,
                    SceneryEnvironment.LevelOfDetail.HighFarClip);

                // Calcular la matriz de proyecci�n
                GlobalMatrices.gLODMediumProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                    MathHelper.PiOver4,
                    (float)GraphicsDevice.Viewport.Width / (float)GraphicsDevice.Viewport.Height,
                    SceneryEnvironment.LevelOfDetail.MediumNearClip,
                    SceneryEnvironment.LevelOfDetail.MediumFarClip);

                // Calcular la matriz de proyecci�n
                GlobalMatrices.gLODLowProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                    MathHelper.PiOver4,
                    (float)GraphicsDevice.Viewport.Width / (float)GraphicsDevice.Viewport.Height,
                    SceneryEnvironment.LevelOfDetail.LowNearClip,
                    SceneryEnvironment.LevelOfDetail.LowFarClip);
            }
        }

        /// <summary>
        /// Actualizar
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (this.Mode == CameraModes.Free)
            {
                // Actualizar la posici�n
                this.UpdatePosition();

                // Actualizar la orientaci�n
                this.UpdateRotation();

                // Obtener la matriz de rotaci�n
                this.m_RotationMatrix = Matrix.CreateFromQuaternion(this.m_Pitch) * Matrix.CreateFromQuaternion(this.m_Yaw);

                // Actualizar el vector vista
                this.m_Direction = m_RotationMatrix.Forward;

                // Actualizar el vector vertical
                this.m_Vertical = Vector3.Up;
            }
            else if (this.Mode == CameraModes.FirstPerson)
            {
                if (this.ModelToFollow != null)
                {
                    Matrix modelMatrix = this.ModelToFollow.CurrentPlayerControlTransform;

                    this.m_Position = modelMatrix.Translation;

                    this.m_Direction = modelMatrix.Forward;

                    this.m_Vertical = modelMatrix.Up;
                }
            }
            else if (this.Mode == CameraModes.ThirdPerson)
            {
                if (this.ModelToFollow != null)
                {
                    this.m_Position = this.CalcRelativePosition();

                    this.m_Direction = this.CalcRelativeDirection(this.m_Position);

                    this.m_Vertical = Vector3.Up;
                }
            }

            // Calcular la matriz de vista
            GlobalMatrices.gViewMatrix = Matrix.CreateLookAt(
                this.m_Position,
                this.m_Position + this.m_Direction,
                this.m_Vertical);
        }

        /// <summary>
        /// Mueve la c�mara a lo largo de su eje de altura
        /// </summary>
        /// <param name="distance">Distancia</param>
        public void MoveUp(float distance)
        {
            this.MoveByAxis(this.m_RotationMatrix.Up, distance);
        }
        /// <summary>
        /// Mueve la c�mara a lo largo de su eje de altura
        /// </summary>
        /// <param name="distance">Distancia</param>
        public void MoveDown(float distance)
        {
            this.MoveByAxis(this.m_RotationMatrix.Down, distance);
        }
        /// <summary>
        /// Mueve la c�mara a lo largo de su eje derecho
        /// </summary>
        /// <param name="distance">Distancia</param>
        public void MoveRight(float distance)
        {
            this.MoveByAxis(this.m_RotationMatrix.Right, distance);
        }
        /// <summary>
        /// Mueve la c�mara a lo largo de su eje izquierdo
        /// </summary>
        /// <param name="distance">Distancia</param>
        public void MoveLeft(float distance)
        {
            this.MoveByAxis(this.m_RotationMatrix.Left, distance);
        }
        /// <summary>
        /// Mueve la c�mara a lo largo de su eje de vista
        /// </summary>
        /// <param name="distance">Distancia</param>
        public void MoveForward(float distance)
        {
            this.MoveByAxis(this.m_RotationMatrix.Forward, distance);
        }
        /// <summary>
        /// Mueve la c�mara a lo largo de su eje de vista
        /// </summary>
        /// <param name="distance">Distancia</param>
        public void MoveBackward(float distance)
        {
            this.MoveByAxis(this.m_RotationMatrix.Backward, distance);
        }
        /// <summary>
        /// Mueve la c�mara a lo largo del eje especificado
        /// </summary>
        /// <param name="axis">Eje de tralaci�n</param>
        /// <param name="distance">Distancia</param>
        public void MoveByAxis(Vector3 axis, float distance)
        {
            Vector3 axisNorm = Vector3.Normalize(axis);

            this.MoveByVector(Vector3.Multiply(axisNorm, distance));
        }
        /// <summary>
        /// Aplica el vector especificado a la posici�n
        /// </summary>
        /// <param name="direction"></param>
        public void MoveByVector(Vector3 direction)
        {
            this.m_Position += direction;
        }

        /// <summary>
        /// Girar en el eje Y
        /// </summary>
        /// <param name="angle">�ngulo</param>
        public void Yaw(float angle)
        {
            this.m_Yaw *= Quaternion.Slerp(this.m_Yaw, Quaternion.CreateFromAxisAngle(Vector3.Up, angle), 1f);
        }
        /// <summary>
        /// Girar en el eje X
        /// </summary>
        /// <param name="angle">�ngulo</param>
        public void Pitch(float angle)
        {
            this.m_Pitch *= Quaternion.Slerp(this.m_Pitch, Quaternion.CreateFromAxisAngle(Vector3.Right, angle), 1f);
        }

        /// <summary>
        /// Actualizar la posici�n
        /// </summary>
        private void UpdatePosition()
        {
            float sensibilityFactor = KeyBoardSensibility / 100f;

            if (InputHelper.IsKeyDown(this.Forward))
            {
                this.MoveForward(sensibilityFactor);
            }
            if (InputHelper.IsKeyDown(this.Backward))
            {
                this.MoveBackward(sensibilityFactor);
            }
            if (InputHelper.IsKeyDown(this.Left))
            {
                this.MoveLeft(sensibilityFactor);
            }
            if (InputHelper.IsKeyDown(this.Right))
            {
                this.MoveRight(sensibilityFactor);
            }
            if (InputHelper.IsKeyDown(this.Up))
            {
                this.MoveUp(sensibilityFactor);
            }
            if (InputHelper.IsKeyDown(this.Down))
            {
                this.MoveDown(sensibilityFactor);
            }
        }
        /// <summary>
        /// Actualizar la rotaci�n
        /// </summary>
        private void UpdateRotation()
        {
            float yawDelta = 0f;
            float pitchDelta = 0f;
            if (this.UseMouse)
            {
                yawDelta = InputHelper.YawDelta;
                pitchDelta = InputHelper.PitchDelta;
            }
            else
            {
                if (InputHelper.KeyDownEvent(this.YawLeft))
                {
                    yawDelta = KeyBoardSensibility * 0.001f;
                }
                if (InputHelper.KeyDownEvent(this.YawRight))
                {
                    yawDelta = -(KeyBoardSensibility * 0.001f);
                }
                if (InputHelper.KeyDownEvent(this.PitchUp))
                {
                    pitchDelta = KeyBoardSensibility * 0.001f;
                }
                if (InputHelper.KeyDownEvent(this.PitchDown))
                {
                    pitchDelta = -(KeyBoardSensibility * 0.001f);
                }
            }

            this.Yaw(yawDelta);
            this.Pitch(pitchDelta);
        }

        /// <summary>
        /// Calcula la posici�n relativa de la c�mara
        /// </summary>
        /// <returns>Devuelve la posici�n relativa</returns>
        private Vector3 CalcRelativePosition()
        {
            // Obtener la rotaci�n del objetivo desde el quaternion
            Matrix rotation = Matrix.CreateFromQuaternion(this.ModelToFollow.Orientation);

            // Calcular la posici�n relativa de la c�mara
            Vector3 transformedCameraPosition = Vector3.Transform(this.ViewerRelativePosition, rotation);

            // A�adir la posici�n relativa de la c�mara a la posici�n del modelo para obtener la posici�n de la c�mara
            return this.ModelToFollow.Position + transformedCameraPosition;
        }
        /// <summary>
        /// Calcula la direcci�n relativa de la c�mara
        /// </summary>
        /// <param name="currentPosition">Posici�n actual</param>
        /// <returns>Devuelve la direcci�n relativa de la c�mara</returns>
        private Vector3 CalcRelativeDirection(Vector3 currentPosition)
        {
            //La c�mara mira al modelo
            return this.ModelToFollow.Position - currentPosition;
        }
    }
}


