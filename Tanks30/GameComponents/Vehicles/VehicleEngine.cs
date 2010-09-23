using Microsoft.Xna.Framework;
using Physics;

namespace GameComponents.Vehicles
{
    /// <summary>
    /// Motor
    /// </summary>
    public class VehicleEngine
    {
        /// <summary>
        /// Indica si el motor está activo
        /// </summary>
        public bool Active { get; protected set; }
        /// <summary>
        /// Indica que el vehículo se mueve
        /// </summary>
        public bool Moving { get; protected set; }
        /// <summary>
        /// Dirección de movimiento del tanque
        /// </summary>
        public MovingDirections MovingDirection { get; protected set; }
        /// <summary>
        /// Indica si se puede cambiar la dirección del vehículo
        /// </summary>
        public bool CanChangeDirection
        {
            get
            {
                return (this.AcceleratingPower.IsZero());
            }
        }
        /// <summary>
        /// Fuerza de avance
        /// </summary>
        public float AcceleratingPower
        {
            get
            {
                if (this.Active)
                {
                    if (this.MovingDirection == MovingDirections.Forward)
                    {
                        return this.m_Power * this.m_Status * this.MaxForwardVelocity;
                    }
                    else
                    {
                        return this.m_Power * this.m_Status * this.MaxBackwardVelocity;
                    }
                }

                return 0f;
            }
        }
        /// <summary>
        /// Fuerza de elevación
        /// </summary>
        public float GravityPower
        {
            get
            {
                if (this.Skimmer)
                {
                    if (this.Active)
                    {
                        return 1f - ((1f - this.m_GravityPower) * this.m_Status);
                    }

                    //Sin acción de gravedad
                    return 0f;
                }
                else
                {
                    //Toda la acción de la gravedad
                    return 1f;
                }
            }
        }

        /// <summary>
        /// Estado del motor
        /// </summary>
        private float m_Status = 1.0f;
        /// <summary>
        /// Avance
        /// </summary>
        private float m_Power = 0.0f;
        /// <summary>
        /// Modificador de la fuerza de gravedad
        /// </summary>
        private float m_GravityPower = 0.0f;
        /// <summary>
        /// Modificación de ángulo de giro en Y
        /// </summary>
        private float m_YawAngleDelta = 0f;
        /// <summary>
        /// Modificación de ángulo de giro en X
        /// </summary>
        private float m_PitchAngleDelta = 0f;
        /// <summary>
        /// Modificación de ángulo de giro en Z
        /// </summary>
        private float m_RollAngleDelta = 0f;

        /// <summary>
        /// Velocidad máxima que puede alcanzar el tanque hacia delante
        /// </summary> 
        public float MaxForwardVelocity = 0f;
        /// <summary>
        /// Velocidad máxima que puede alcanzar el tanque marcha atrás
        /// </summary> 
        public float MaxBackwardVelocity = 0f;
        /// <summary>
        /// Modificador de aceleración
        /// </summary>
        public float AccelerationModifier = 0f;
        /// <summary>
        /// Modificador de frenado
        /// </summary>
        public float BrakeModifier = 0f;
        /// <summary>
        /// Velocidad angular
        /// </summary>
        public float AngularVelocityModifier = 0f;

        /// <summary>
        /// Vehículo volador
        /// </summary>
        public bool Skimmer = false;
        /// <summary>
        /// Altura mínima base
        /// </summary>
        public float InitialMinFlightHeight = 0f;
        /// <summary>
        /// Altura máxima base
        /// </summary>
        public float InitialMaxFlightHeight = 0f;
        /// <summary>
        /// Altura de vuelo máxima
        /// </summary>
        public float MaxFlightHeight = 0f;
        /// <summary>
        /// Altura de vuelo mínima
        /// </summary>
        public float MinFlightHeight = 0f;
        /// <summary>
        /// Angulo de inclinación del morro en el ascenso
        /// </summary>
        public float AscendingAngle = 0f;
        /// <summary>
        /// Angulo de inclinación del morro en el descenso
        /// </summary>
        public float DescendingAngle = 0f;
        /// <summary>
        /// Modificador a la fuerza de la gravedad
        /// </summary>
        public float GravityPowerModifier = 0f;

        /// <summary>
        /// Constructor
        /// </summary>
        public VehicleEngine()
        {
            this.Active = false;
            this.Moving = false;
            this.MovingDirection = MovingDirections.Forward;
        }

        /// <summary>
        /// Enceder el motor
        /// </summary>
        public void Start()
        {
            this.Active = true;
        }
        /// <summary>
        /// Apagar el motor
        /// </summary>
        public void Stop()
        {
            this.Active = false;

            this.m_Power = 0.0f;
            this.m_GravityPower = 0.0f;
        }
        /// <summary>
        /// Modifica la dirección
        /// </summary>
        /// <param name="movingDirection">Dirección modificada</param>
        public void ChangeDirection(MovingDirections movingDirection)
        {
            if (this.CanChangeDirection)
            {
                this.MovingDirection = movingDirection;
            }
        }

        /// <summary>
        /// Calcula el efecto de los daños sobre el motor
        /// </summary>
        /// <param name="modifier">Modificador de 0 a 1</param>
        public void TakeDamage(float modifier)
        {
            this.m_Status -= modifier;

            if (this.m_Status < 0f)
            {
                this.m_Status = 0f;
            }
        }

        /// <summary>
        /// Acelerar
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public void Accelerate(GameTime gameTime)
        {
            if (this.Active)
            {
                this.m_Power += this.AccelerationModifier * (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (this.m_Power > 1f)
                {
                    this.m_Power = 1f;
                }
            }
        }
        /// <summary>
        /// Decelerar
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public void Decelerate(GameTime gameTime)
        {
            if (this.Active)
            {
                this.m_Power -= this.BrakeModifier * (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (this.m_Power < 0f)
                {
                    this.m_Power = 0f;
                }
            }
        }
        /// <summary>
        /// Girar a la izquierda
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public void TurnLeft(GameTime gameTime)
        {
            if (this.Active)
            {
                this.m_YawAngleDelta = this.m_Status * this.AngularVelocityModifier * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }
        /// <summary>
        /// Girar a la derecha
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public void TurnRight(GameTime gameTime)
        {
            if (this.Active)
            {
                this.m_YawAngleDelta = -this.m_Status * this.AngularVelocityModifier * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }
        /// <summary>
        /// Girar hacia arriba
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public void TurnUp(GameTime gameTime)
        {
            if (this.Active)
            {
                this.m_PitchAngleDelta = this.m_Status * this.AngularVelocityModifier * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }
        /// <summary>
        /// Girar hacia abajo
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public void TurnDown(GameTime gameTime)
        {
            if (this.Active)
            {
                this.m_PitchAngleDelta = -this.m_Status * this.AngularVelocityModifier * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
        }
        /// <summary>
        /// Elevar
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public void Elevate(GameTime gameTime)
        {
            if (this.Active)
            {
                this.m_GravityPower -= this.GravityPowerModifier * (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (this.m_GravityPower < -0.5f)
                {
                    this.m_GravityPower = -0.5f;
                }
            }
        }
        /// <summary>
        /// Descender
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public void Descend(GameTime gameTime)
        {
            if (this.Active)
            {
                this.m_GravityPower += this.GravityPowerModifier * (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (this.m_GravityPower > 0.5f)
                {
                    this.m_GravityPower = 0.5f;
                }
            }
        }
        /// <summary>
        /// Flotar
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public void Float(GameTime gameTime)
        {
            if (this.Active)
            {
                if (this.m_GravityPower > 0f)
                {
                    this.m_GravityPower -= this.GravityPowerModifier * (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (this.m_GravityPower < 0f)
                    {
                        this.m_GravityPower = 0f;
                    }
                }
                else if (this.m_GravityPower < 0f)
                {
                    this.m_GravityPower += this.GravityPowerModifier * (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (this.m_GravityPower > 0f)
                    {
                        this.m_GravityPower = 0f;
                    }
                }

            }
        }

        /// <summary>
        /// Actualiza el vehículo
        /// </summary>
        /// <param name="vehicle">Vehículo</param>
        public void UpdateVehicle(Vehicle vehicle)
        {
            this.Moving = false;

            //Velocidad
            float acceleratingPower = this.AcceleratingPower;
            if (acceleratingPower != 0f)
            {
                Vector3 velocity = vehicle.Direction * this.AcceleratingPower;

                //Actualizar la posición
                vehicle.Position += velocity;

                this.Moving = true;
            }

            //Actualizar la fuerza de gravedad
            if (this.Skimmer)
            {
                //Obtener el factor modificador de la gravedad
                float gravityPower = this.GravityPower;

                vehicle.Primitive.SetAcceleration(Constants.GravityForce * gravityPower);

                if (this.m_YawAngleDelta != 0f || this.m_PitchAngleDelta != 0f || this.m_RollAngleDelta != 0f)
                {
                    float yaw;
                    float pitch;
                    float roll;
                    vehicle.Orientation.Decompose(out yaw, out pitch, out roll);

                    yaw += this.m_YawAngleDelta;
                    pitch += this.m_PitchAngleDelta;
                    roll += this.m_RollAngleDelta;

                    vehicle.Orientation = Quaternion.CreateFromYawPitchRoll(yaw, pitch, roll);

                    this.m_YawAngleDelta = 0f;
                    this.m_PitchAngleDelta = 0f;
                    this.m_RollAngleDelta = 0f;
                }

                this.Moving = true;
            }
            else
            {
                //Actualizar la rotación
                if (this.m_YawAngleDelta != 0f)
                {
                    Quaternion turn = Quaternion.CreateFromAxisAngle(Vector3.Up, this.m_YawAngleDelta);

                    vehicle.Orientation *= turn;

                    this.m_YawAngleDelta = 0f;

                    this.Moving = true;
                }
            }
        }
    }
}
