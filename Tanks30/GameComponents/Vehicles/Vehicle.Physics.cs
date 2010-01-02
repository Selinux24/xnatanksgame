using System;
using Microsoft.Xna.Framework;
using Physics;

namespace GameComponents.Vehicles
{
    using GameComponents.Physics;

    /// <summary>
    /// Control de físicas de un vehículo
    /// </summary>
    public partial class Vehicle : IVehicleController
    {
        // Colección de triángulos del modelo
        private TriangleInfo m_TriangleInfo = null;

        // Velocidad de traslación
        private float m_Velocity = 0f;
        // Velocidad de rotación a aplicar
        private float m_AngularVelocity = 0f;
        // Vector velocidad
        private Vector3 m_Direction = Vector3.Forward;
        // Inclinación
        private Quaternion m_Inclination = Quaternion.Identity;

        /// <summary>
        /// Velocidad máxima que puede alcanzar el tanque hacia delante
        /// </summary> 
        protected float MaxForwardVelocity = 0f;
        /// <summary>
        /// Velocidad máxima que puede alcanzar el tanque marcha atrás
        /// </summary> 
        protected float MaxBackwardVelocity = 0f;
        /// <summary>
        /// Modificador de aceleración
        /// </summary>
        protected float AccelerationModifier = 0f;
        /// <summary>
        /// Modificador de frenado
        /// </summary>
        protected float BrakeModifier = 0f;
        /// <summary>
        /// Velocidad angular
        /// </summary>
        protected float AngularVelocityModifier = 0f;
        /// <summary>
        /// Vehículo volador
        /// </summary>
        protected bool Skimmer = false;
        /// <summary>
        /// Altura de vuelo máxima
        /// </summary>
        protected float MaxFlightHeight = 0f;
        /// <summary>
        /// Altura de vuelo mínima
        /// </summary>
        protected float MinFlightHeight = 0f;
        /// <summary>
        /// Angulo de inclinación del morro en el ascenso
        /// </summary>
        protected float AscendingAngle = 0f;
        /// <summary>
        /// Angulo de inclinación del morro en el descenso
        /// </summary>
        protected float DescendingAngle = 0f;

        // test - Número de frames desde la última actualización con el terreno
        private int m_FramesSinceLastUpdate = 0;
        // test - Máximo número de frames para actualizar con el terreno
        private int m_MaxFrames = 12;

        // Piloto automático
        private AutoPilot m_Autopilot = new AutoPilot();
        // Dirección de movimiento del tanque
        private MovingDirections m_MovingDirection = MovingDirections.Forward;

        /// <summary>
        /// Obtiene la información de primitivas del modelo
        /// </summary>
        public TriangleInfo TriangleInfo
        {
            get
            {
                if (this.m_TriangleInfo == null)
                {
                    this.m_TriangleInfo = m_Model.Tag as TriangleInfo;
                }

                return this.m_TriangleInfo;
            }
        }

        /// <summary>
        /// Obtiene la cantidad de velocidad
        /// </summary>
        public virtual float Velocity
        {
            get
            {
                return m_Velocity;
            }
        }
        /// <summary>
        /// Obtiene el vector de dirección actual del vehículo
        /// </summary>
        public virtual Vector3 Direction
        {
            get
            {
                return m_Direction;
            }
        }
        /// <summary>
        /// Obtiene la dirección relativa del vehículo
        /// </summary>
        public virtual MovingDirections MovingDirection
        {
            get
            {
                return m_MovingDirection;
            }
        }
        /// <summary>
        /// Obtiene si el objeto está avanzando
        /// </summary>
        public virtual bool IsAdvancing
        {
            get
            {
                return (m_MovingDirection == MovingDirections.Forward);
            }
        }
        /// <summary>
        /// Indica si el vehículo está detenido
        /// </summary>
        public virtual bool IsStatic
        {
            get
            {
                return (m_Velocity == 0.0f);
            }
        }

        /// <summary>
        /// Obtiene la esfera circundante del modelo con la transformación aplicada
        /// </summary>
        public virtual BoundingSphere TransformedBSph
        {
            get
            {
                BoundingSphere sphere = this.TriangleInfo.BSph;

                Vector3 center = Vector3.Transform(sphere.Center, GetTransform());
                float radius = sphere.Radius;

                return new BoundingSphere(center, radius);
            }
        }
        /// <summary>
        /// Obtiene la caja orientada con la transformación aplicada
        /// </summary>
        public virtual CollisionBox TransformedOBB
        {
            get
            {
                CollisionBox obb = this.TriangleInfo.OBB;

                obb.Offset = GetTransform();

                return obb;
            }
        }

        /// <summary>
        /// Obtiene el piloto automático
        /// </summary>
        public AutoPilot AutoPilot
        {
            get
            {
                return m_Autopilot;
            }
        }

        /// <summary>
        /// Establece la velocidad de traslación del modelo
        /// </summary>
        /// <param name="velocity">Velocidad</param>
        protected virtual void SetVelocity(float velocity)
        {
            m_Velocity = velocity;

            //UpdatePosition();
        }
        /// <summary>
        /// Añade la cantidad especificada a la velocidad de traslación del modelo
        /// </summary>
        /// <param name="velocity">Velocidad</param>
        protected virtual void AddToVelocity(float velocity)
        {
            if (this.IsAdvancing)
            {
                m_Velocity = MathHelper.Clamp(m_Velocity + velocity, 0.0f, MaxForwardVelocity);
            }
            else
            {
                m_Velocity = MathHelper.Clamp(m_Velocity + velocity, 0.0f, MaxBackwardVelocity);
            }

            //UpdatePosition();
        }
        /// <summary>
        /// Actualiza la posición con respecto a la velocidad y el vector dirección
        /// </summary>
        protected virtual void UpdatePosition(GameTime gameTime)
        {
            this.m_Position = CalcPositionValue(gameTime, this.m_Position, this.m_Direction, this.m_Velocity);
        }
        /// <summary>
        /// Calcula la posición con respecto a la dirección y la velocidad
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        /// <param name="currentPosition">Posición actual</param>
        /// <param name="direction">Dirección</param>
        /// <param name="velocity">Velocidad</param>
        /// <returns>Devuelve la nueva posición</returns>
        private Vector3 CalcPositionValue(GameTime gameTime, Vector3 currentPosition, Vector3 direction, float velocity)
        {
            Vector3 newPosition = currentPosition;

            if (velocity != 0f)
            {
                // Calcular la velocidad a aplicar este paso
                float timedVelocity = velocity / (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (!float.IsInfinity(timedVelocity))
                {
                    // Trasladar la posición
                    newPosition += Vector3.Multiply(direction, timedVelocity);

                    // La posición ha sido modificada
                    this.PositionHasChanged = true;
                }
            }

            return newPosition;
        }

        /// <summary>
        /// Actualizar la rotación
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        protected virtual void UpdateRotation(GameTime gameTime)
        {
            if (m_AngularVelocity != 0f)
            {
                // Obtener el ángulo a aplicar
                float timedAngularVelocity = m_AngularVelocity / (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (!float.IsInfinity(timedAngularVelocity))
                {
                    // Calcular el Quaternion de la rotación parcial
                    Quaternion partialRotation = Quaternion.CreateFromAxisAngle(Vector3.Up, timedAngularVelocity);

                    // Modificar la rotación
                    m_Rotation = Quaternion.Multiply(m_Rotation, partialRotation);

                    // Modificar la dirección
                    m_Direction = Vector3.Transform(m_Direction, partialRotation);

                    // Indicar que la rotación ha sido modificada
                    RotationHasChanged = true;
                }

                // Inicializar la velocidad angular
                m_AngularVelocity = 0f;
            }
        }
        /// <summary>
        /// Actualiza el vehículo con el escenario
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        protected virtual void UpdateWithScenery(GameTime gameTime)
        {
            if (!this.Initialized)
            {
                if (!this.Skimmer)
                {
                    this.UpdateDefaultWithScenery(gameTime);
                }
                else
                {
                    this.UpdateSkimmerWithScenery(gameTime);
                }

                this.Initialized = true;
            }
            else if (!this.Destroyed)
            {
                if (this.PositionHasChanged || this.RotationHasChanged || this.ScaleHasChanged)
                {
                    if (!this.Skimmer)
                    {
                        if (this.Visible)
                        {
                            this.UpdateDefaultWithScenery(gameTime);
                        }
                        else
                        {
                            this.m_FramesSinceLastUpdate++;

                            if (this.m_FramesSinceLastUpdate >= this.m_MaxFrames)
                            {
                                this.UpdateDefaultWithScenery(gameTime);

                                this.m_FramesSinceLastUpdate = 0;
                            }
                        }
                    }
                    else
                    {
                        this.UpdateSkimmerWithScenery(gameTime);
                    }
                }
            }
        }
        /// <summary>
        /// Actualiza el componente con el escenario
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        protected virtual void UpdateDefaultWithScenery(GameTime gameTime)
        {
            Triangle? tri = null;
            Vector3? point = null;
            float? distance = null;

            if (this.Scenery.Intersects(this.m_Position.X, this.m_Position.Z, out tri, out point, out distance))
            {
                // Obtener la normal actual
                Vector3 currentNormal = Matrix.CreateFromQuaternion(m_Rotation).Up;

                // Obtener la normal del triángulo
                Vector3 newNormal = tri.Value.Normal;

                // Calcular inclinación a aplicar entre las dos normales
                Quaternion newInclination = this.CalcInclinationQuaternion(currentNormal, newNormal);

                if (!this.Initialized)
                {
                    this.m_Position = point.Value;

                    this.m_Inclination = newInclination;
                }
                else
                {
                    // Establecer la posición definitiva
                    this.m_Position = point.Value;

                    // Obtener factor de interpolación dependiendo de la velocidad
                    float slerpFactor = this.CalcInclinationSlerpFactor(25f);

                    // Establecer la interpolación entre la nueva inclinación y la posición vertical
                    this.m_Inclination = Quaternion.Slerp(this.m_Inclination, newInclination, slerpFactor);
                }
            }
        }
        /// <summary>
        /// Actualiza el componente volador con el escenario
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        protected virtual void UpdateSkimmerWithScenery(GameTime gameTime)
        {
            Triangle? currentTri = null;
            Vector3? currentPoint = null;
            float? currentDistance = null;
            if (this.Scenery.Intersects(this.m_Position.X, this.m_Position.Z, out currentTri, out currentPoint, out currentDistance))
            {
                if (!this.Initialized)
                {
                    this.m_Position = currentPoint.Value + (Vector3.Up * this.MinFlightHeight);

                    this.m_Inclination = Quaternion.Identity;
                }
                else
                {
                    // Obtener la posición futura
                    Vector3 futurePosition = this.CalcPositionValue(gameTime, this.m_Position, this.m_Direction, this.m_Velocity);

                    // Obtener la intersección con la posición futura y el suelo
                    Triangle? nextTri = null;
                    Vector3? nextPoint = null;
                    float? nextDistance = null;
                    if (this.Scenery.Intersects(futurePosition.X, futurePosition.Z, out nextTri, out nextPoint, out nextDistance))
                    {
                        // Altura actual
                        float currentHeight = (this.m_Position.Y - currentPoint.Value.Y);
                        float nextHeight = (futurePosition.Y - nextPoint.Value.Y);

                        float diff = 0;
                        if (currentHeight < 0)
                        {
                            this.Destroyed = true;
                        }
                        else if (currentHeight < this.MinFlightHeight)
                        {
                            // Hay que elevarse rápidamente
                            diff = (this.MinFlightHeight - currentHeight) * 0.25f;
                        }
                        else if (currentHeight > this.MaxFlightHeight)
                        {
                            // Hay que bajar rápidamente
                            diff = (this.MaxFlightHeight - currentHeight) * 0.005f;
                        }

                        // Agregar la nueva altura a la posición
                        this.m_Position.Y += diff;

                        // Obtener la normal actual
                        Matrix currentRotation = Matrix.CreateFromQuaternion(m_Rotation);
                        Vector3 currentNormal = currentRotation.Up;
                        Vector3 currentAxis = currentRotation.Right;
                        Vector3 newNormal = Vector3.Up;
                        float slerpSeed = 0f;
                        if (Math.Abs(diff) <= 0.1f)
                        {
                            // Sin inclinación
                            newNormal = Vector3.Up;
                            // Interpolación rápida
                            slerpSeed = 25f;
                        }
                        else if (diff > 0f)
                        {
                            // Inclinación ascendente
                            newNormal = Matrix.CreateFromAxisAngle(currentAxis, this.AscendingAngle).Up;
                            // Interpolación rápida
                            slerpSeed = 25f;
                        }
                        else if (diff < 0f)
                        {
                            // Inclinación descendente
                            newNormal = Matrix.CreateFromAxisAngle(currentAxis, -this.DescendingAngle).Up;
                            // Interpolación lenta
                            slerpSeed = 205f;
                        }

                        // Nueva inclinación
                        Quaternion newInclination = this.CalcInclinationQuaternion(currentNormal, newNormal);

                        // Obtener factor de interpolación dependiendo de la velocidad
                        float slerpFactor = this.CalcInclinationSlerpFactor(slerpSeed);

                        // Establecer la interpolación entre la nueva inclinación y la posición vertical
                        this.m_Inclination = Quaternion.Slerp(this.m_Inclination, newInclination, slerpFactor);
                    }
                }
            }
        }
        /// <summary>
        /// Obtiene el factor para aplicar la inclinación según la velocidad
        /// </summary>
        protected virtual float CalcInclinationSlerpFactor(float factor)
        {
            if (this.m_Velocity == 0f)
            {
                return 1f;
            }
            else
            {
                if (this.m_MovingDirection == MovingDirections.Forward)
                {
                    return this.m_Velocity / this.MaxForwardVelocity / factor;
                }
                else if (this.m_MovingDirection == MovingDirections.Backward)
                {
                    return this.m_Velocity / this.MaxBackwardVelocity / factor;
                }
                else
                {
                    return 0.05f;
                }
            }
        }
        /// <summary>
        /// Obtiene el Quaterion de inclinación usando las dos normales especificadas
        /// </summary>
        /// <param name="normal1">Normal 1</param>
        /// <param name="normal2">Normal 2</param>
        /// <returns>Devuelve el cuaternion que representa la inclinación entre las dos normales</returns>
        protected virtual Quaternion CalcInclinationQuaternion(Vector3 normal1, Vector3 normal2)
        {
            Quaternion inclination = Quaternion.Identity;

            // Angulo que forman las dos normales
            float angle = (float)Math.Acos(Vector3.Dot(normal1, normal2));
            if (angle != 0.0f)
            {
                // Eje entre las dos normales, la nueva y la actual
                Vector3 axis = Vector3.Normalize(Vector3.Cross(normal1, normal2));

                // Inclinación
                inclination = Quaternion.CreateFromAxisAngle(axis, angle);
            }

            return inclination;
        }

        /// <summary>
        /// Modifica la dirección de movimiento
        /// </summary>
        public virtual void ChangeDirection()
        {
            if (this.IsStatic)
            {
                m_Direction = Vector3.Negate(m_Direction);

                if (m_MovingDirection == MovingDirections.Forward)
                {
                    m_MovingDirection = MovingDirections.Backward;
                }
                else
                {
                    m_MovingDirection = MovingDirections.Forward;
                }
            }
        }

        /// <summary>
        /// Ejecuta la reacción del tanque a colisiones
        /// </summary>
        public virtual void Reaction(IPhysicObject other)
        {
            // TODO: Reacción de colisión. No debe modificarse directamente porque los cuerpos se superponen con la reacción
            IVehicleController vehicle = other as IVehicleController;
            if (vehicle != null)
            {
                // Dirección de la reacción
                Vector3 reactionDirection = Vector3.Normalize(this.m_Position - vehicle.Position);

                if (vehicle.Velocity > 0.0f)
                {
                    // Reacción
                    this.m_Position += Vector3.Multiply(reactionDirection, vehicle.Velocity);
                }
                else
                {
                    this.m_Position += Vector3.Multiply(reactionDirection, 0.01f);
                }

                m_Velocity /= 2.0f;
            }
            else
            {
                m_Velocity = 0.0f;
            }
        }
    }
}
