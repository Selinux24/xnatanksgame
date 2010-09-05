using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameComponents.Camera
{
    using Common;
    using Common.Helpers;
    using GameComponents.Scenery;
    using GameComponents.Vehicles;
    using Physics;

    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class CameraGameComponent : GameComponent
    {
        /// <summary>
        /// Modos posibles de vista de la cámara
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
            /// <summary>
            /// Tercera persona con transición
            /// </summary>
            Chase,
        }

        /// <summary>
        /// Dispositivo gráfico
        /// </summary>
        protected GraphicsDevice GraphicsDevice;

        /// <summary>
        /// Posición
        /// </summary>
        private Vector3 m_Position = Vector3.Zero;
        /// <summary>
        /// Dirección
        /// </summary>
        private Vector3 m_Direction = Vector3.Forward;
        /// <summary>
        /// Vertical
        /// </summary>
        private Vector3 m_Vertical = Vector3.Up;
        /// <summary>
        /// Rotación en X
        /// </summary>
        private Quaternion m_Pitch = Quaternion.Identity;
        /// <summary>
        /// Rotación en Y
        /// </summary>
        private Quaternion m_Yaw = Quaternion.Identity;
        /// <summary>
        /// Obtiene la matriz de rotación a partir de quaternion de rotación
        /// </summary>
        private Matrix m_RotationMatrix = Matrix.Identity;
        /// <summary>
        /// Modo de cámara
        /// </summary>
        private CameraModes m_Mode = CameraModes.Free;

        /// <summary>
        /// Modo de vista de la cámara
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
        /// Indica si se usa el ratón para girar la cámara
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
        /// Sensibilidad del ratón en el eje vertical en porcentaje
        /// </summary>
        public static float MouseVerticalSensibility = 100;
        /// <summary>
        /// Sensibilidad del ratón en el eje horizontal en porcentaje
        /// </summary>
        public static float MouseHorizontalSensibility = 100;

        /// <summary>
        /// Establece la posición de la cámara
        /// </summary>
        public virtual Vector3 Position
        {
            get
            {
                return this.m_Position;
            }
        }
        /// <summary>
        /// Obtiene o establece la dirección
        /// </summary>
        public virtual Vector3 Direction
        {
            get
            {
                return this.m_Direction;
            }
        }
        /// <summary>
        /// Obtiene o establece la vertical de cámara
        /// </summary>
        public virtual Vector3 Vertical
        {
            get
            {
                return this.m_Vertical;
            }
        }

        /// <summary>
        /// Posición relativa del espectador
        /// </summary>
        public Vector3 ViewerPositionOffset = new Vector3(0.0f, 4.0f, 6.0f);
        /// <summary>
        /// Vista relativa del espectador
        /// </summary>
        public Vector3 ViewerLookAtOffset = new Vector3(0.0f, 0.5f, -1.0f);
        /// <summary>
        /// Modelo que sigue la cámara
        /// </summary>
        public Vehicle ModelToFollow { get; set; }

        /// <summary>
        /// Velocidad de aproximación de la cámara al objetivo
        /// </summary>
        public Vector3 Velocity { get; set; }
        /// <summary>
        /// Coheficiente de elasticidad entre la cámara y el objetivo. Cuanto más grande menos elástico
        /// </summary>
        public float Stiffness = 36.0f;
        /// <summary>
        /// Coheficiente de atenuación de cálculos de la cámara. Evita que la cámara esté moviéndose eternamente
        /// </summary>
        public float Damping = 12.0f;
        /// <summary>
        /// Masa de la cámara
        /// </summary>
        public float Mass = 1.0f;

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

                // Calcular la matriz de proyección
                GlobalMatrices.gProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                    MathHelper.PiOver4,
                    (float)GraphicsDevice.Viewport.Width / (float)GraphicsDevice.Viewport.Height,
                    SceneryEnvironment.GlobalNearClip,
                    SceneryEnvironment.GlobalFarClip * 100);

                // Calcular la matriz de proyección
                GlobalMatrices.gLODHighProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                    MathHelper.PiOver4,
                    (float)GraphicsDevice.Viewport.Width / (float)GraphicsDevice.Viewport.Height,
                    SceneryEnvironment.LevelOfDetail.HighNearClip,
                    SceneryEnvironment.LevelOfDetail.HighFarClip);

                // Calcular la matriz de proyección
                GlobalMatrices.gLODMediumProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                    MathHelper.PiOver4,
                    (float)GraphicsDevice.Viewport.Width / (float)GraphicsDevice.Viewport.Height,
                    SceneryEnvironment.LevelOfDetail.MediumNearClip,
                    SceneryEnvironment.LevelOfDetail.MediumFarClip);

                // Calcular la matriz de proyección
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
                // Actualizar la posición
                this.UpdatePosition();

                // Actualizar la orientación
                this.UpdateRotation();

                // Obtener la matriz de rotación
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
            else if (this.Mode == CameraModes.Chase)
            {
                if (this.ModelToFollow != null)
                {
                    Matrix orientation = Matrix.CreateFromQuaternion(this.ModelToFollow.Orientation);

                    Vector3 chasePosition = this.ModelToFollow.Position;
                    Vector3 chaseDirection = Vector3.Normalize(orientation.Forward);
                    Vector3 up = Vector3.Up;

                    // Construct a matrix to transform from object space to worldspace
                    Matrix transform = Matrix.Identity;
                    transform.Forward = chaseDirection;
                    transform.Up = up;
                    transform.Right = Vector3.Cross(up, chaseDirection);

                    // Calculate desired camera properties in world space
                    Vector3 desiredPosition = chasePosition + Vector3.TransformNormal(this.ViewerPositionOffset, transform);
                    this.m_Direction = chasePosition + Vector3.TransformNormal(this.ViewerLookAtOffset, transform);

                    float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

                    // Calculate spring force
                    Vector3 stretch = this.m_Position - desiredPosition;
                    Vector3 force = -this.Stiffness * stretch - this.Damping * this.Velocity;

                    // Apply acceleration
                    Vector3 acceleration = force / this.Mass;
                    this.Velocity += acceleration * elapsed;

                    // Apply velocity
                    this.m_Position += this.Velocity * elapsed;

                    GlobalMatrices.gViewMatrix = Matrix.CreateLookAt(
                        this.m_Position,
                        this.m_Direction,
                        this.m_Vertical);

                    return;
                }
            }

            // Calcular la matriz de vista
            GlobalMatrices.gViewMatrix = Matrix.CreateLookAt(
                this.m_Position,
                this.m_Position + this.m_Direction,
                this.m_Vertical);
        }

        /// <summary>
        /// Mueve la cámara a lo largo de su eje de altura
        /// </summary>
        /// <param name="distance">Distancia</param>
        public void MoveUp(float distance)
        {
            this.MoveByAxis(this.m_RotationMatrix.Up, distance);
        }
        /// <summary>
        /// Mueve la cámara a lo largo de su eje de altura
        /// </summary>
        /// <param name="distance">Distancia</param>
        public void MoveDown(float distance)
        {
            this.MoveByAxis(this.m_RotationMatrix.Down, distance);
        }
        /// <summary>
        /// Mueve la cámara a lo largo de su eje derecho
        /// </summary>
        /// <param name="distance">Distancia</param>
        public void MoveRight(float distance)
        {
            this.MoveByAxis(this.m_RotationMatrix.Right, distance);
        }
        /// <summary>
        /// Mueve la cámara a lo largo de su eje izquierdo
        /// </summary>
        /// <param name="distance">Distancia</param>
        public void MoveLeft(float distance)
        {
            this.MoveByAxis(this.m_RotationMatrix.Left, distance);
        }
        /// <summary>
        /// Mueve la cámara a lo largo de su eje de vista
        /// </summary>
        /// <param name="distance">Distancia</param>
        public void MoveForward(float distance)
        {
            this.MoveByAxis(this.m_RotationMatrix.Forward, distance);
        }
        /// <summary>
        /// Mueve la cámara a lo largo de su eje de vista
        /// </summary>
        /// <param name="distance">Distancia</param>
        public void MoveBackward(float distance)
        {
            this.MoveByAxis(this.m_RotationMatrix.Backward, distance);
        }
        /// <summary>
        /// Mueve la cámara a lo largo del eje especificado
        /// </summary>
        /// <param name="axis">Eje de tralación</param>
        /// <param name="distance">Distancia</param>
        public void MoveByAxis(Vector3 axis, float distance)
        {
            Vector3 axisNorm = Vector3.Normalize(axis);

            this.MoveByVector(Vector3.Multiply(axisNorm, distance));
        }
        /// <summary>
        /// Aplica el vector especificado a la posición
        /// </summary>
        /// <param name="direction"></param>
        public void MoveByVector(Vector3 direction)
        {
            this.m_Position += direction;
        }

        /// <summary>
        /// Girar en el eje Y
        /// </summary>
        /// <param name="angle">Ángulo</param>
        public void Yaw(float angle)
        {
            this.m_Yaw *= Quaternion.Slerp(this.m_Yaw, Quaternion.CreateFromAxisAngle(Vector3.Up, angle), 1f);
        }
        /// <summary>
        /// Girar en el eje X
        /// </summary>
        /// <param name="angle">Ángulo</param>
        public void Pitch(float angle)
        {
            this.m_Pitch *= Quaternion.Slerp(this.m_Pitch, Quaternion.CreateFromAxisAngle(Vector3.Right, angle), 1f);

            if (this.m_Pitch.X > 0.65f)
            {
                this.m_Pitch.X = 0.65f;

                this.m_Pitch.Normalize();
            }

            if (this.m_Pitch.X < -0.65f)
            {
                this.m_Pitch.X = -0.65f;

                this.m_Pitch.Normalize();
            }
        }

        /// <summary>
        /// Actualizar la posición
        /// </summary>
        private void UpdatePosition()
        {
            float sensibilityFactor = 0f;
            if (InputHelper.IsKeyDown(Keys.LeftShift))
            {
                sensibilityFactor = KeyBoardSensibility / 1000f;
            }
            else
            {
                sensibilityFactor = KeyBoardSensibility / 100f;
            }

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
        /// Actualizar la rotación
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
        /// Calcula la posición relativa de la cámara
        /// </summary>
        /// <returns>Devuelve la posición relativa</returns>
        private Vector3 CalcRelativePosition()
        {
            // Obtener la rotación del objetivo desde el quaternion
            Matrix rotation = Matrix.CreateFromQuaternion(this.ModelToFollow.Orientation);

            // Calcular la posición relativa de la cámara
            Vector3 transformedCameraPosition = Vector3.Transform(this.ViewerPositionOffset, rotation);

            // Añadir la posición relativa de la cámara a la posición del modelo para obtener la posición de la cámara
            return this.ModelToFollow.Position + transformedCameraPosition;
        }
        /// <summary>
        /// Calcula la dirección relativa de la cámara
        /// </summary>
        /// <param name="currentPosition">Posición actual</param>
        /// <returns>Devuelve la dirección relativa de la cámara</returns>
        private Vector3 CalcRelativeDirection(Vector3 currentPosition)
        {
            //La cámara mira al modelo
            return this.ModelToFollow.Position - currentPosition;
        }

        /// <summary>
        /// Obtiene la información del estado de la cámara
        /// </summary>
        /// <returns>Devuelve una cadena de texto con la información de estado de la cámara</returns>
        public string GetStatus()
        {
            return string.Format("Mode:{0}\nPosition:{1}\nPitch:{2}\nYaw:{3}\n", this.m_Mode, this.m_Position, this.m_Pitch, this.m_Yaw);
        }
    }
}


