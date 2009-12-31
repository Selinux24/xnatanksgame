using Physics;
using Microsoft.Xna.Framework;
using System;

namespace GameComponents.Vehicles
{
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
        /// Altura
        /// </summary>
        protected float FilghtHeight = 0f;

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
        public virtual OrientedBoundingBox TransformedOBB
        {
            get
            {
                OrientedBoundingBox obb = this.TriangleInfo.OBB;

                obb.Transforms = GetTransform();

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
            if (m_Velocity != 0f)
            {
                // Calcular la velocidad a aplicar este paso
                float timedVelocity = m_Velocity / (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (!float.IsInfinity(timedVelocity))
                {
                    // Trasladar la posición
                    m_Position += Vector3.Multiply(m_Direction, timedVelocity);

                    // La posición ha sido modificada
                    PositionHasChanged = true;
                }
            }
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
            Triangle? tri = null;
            Vector3? point = null;
            float? distance = null;

            if (Scenery.Intersects(m_Position.X, m_Position.Z, out tri, out point, out distance))
            {
                float slerpSeed;
                if (Skimmer)
                {
                    // Establecer la posición definitiva
                    m_Position = point.Value + (Vector3.Up * FilghtHeight);

                    slerpSeed = 500f;
                }
                else
                {
                    // Establecer la posición definitiva
                    m_Position = point.Value;

                    slerpSeed = 25f;
                }

                // Obtener la normal actual
                Vector3 currentNormal = Matrix.CreateFromQuaternion(m_Rotation).Up;

                // Obtener la normal del triángulo
                Vector3 newNormal = tri.Value.Normal;

                // Calcular inclinación a aplicar
                Quaternion newInclination = Quaternion.Identity;
                Vector3 axis = Vector3.Normalize(Vector3.Cross(currentNormal, newNormal));
                float angle = (float)Math.Acos(Vector3.Dot(currentNormal, newNormal));
                if (angle != 0.0f)
                {
                    newInclination = Quaternion.CreateFromAxisAngle(axis, angle);
                }

                // Obtener factor de interpolación dependiendo de la velocidad
                float slerpFactor = this.CalcInclinationSlerpFactor(slerpSeed);

                // Establecer la interpolación entre la nueva inclinación y la posición vertical
                m_Inclination = Quaternion.Slerp(m_Inclination, newInclination, slerpFactor);
            }
        }
        /// <summary>
        /// Obtiene el factor para aplicar la inclinación según la velocidad
        /// </summary>
        protected virtual float CalcInclinationSlerpFactor(float factor)
        {
            if (m_Velocity == 0f)
            {
                return 1f;
            }
            else
            {
                if (m_MovingDirection == MovingDirections.Forward)
                {
                    return m_Velocity / MaxForwardVelocity / factor;
                }
                else if (m_MovingDirection == MovingDirections.Backward)
                {
                    return m_Velocity / MaxBackwardVelocity / factor;
                }
                else
                {
                    return 0.05f;
                }
            }
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
                Vector3 reactionDirection = Vector3.Normalize(m_Position - vehicle.Position);

                if (vehicle.Velocity > 0.0f)
                {
                    // Reacción
                    m_Position += Vector3.Multiply(reactionDirection, vehicle.Velocity);
                }
                else
                {
                    m_Position += Vector3.Multiply(reactionDirection, 0.01f);
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
