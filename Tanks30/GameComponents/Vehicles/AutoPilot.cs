using Microsoft.Xna.Framework;
using Physics;
using GameComponents.Scenery;

namespace GameComponents.Vehicles
{
    /// <summary>
    /// Piloto autom�tico
    /// </summary>
    public class AutoPilot
    {
        /// <summary>
        /// Indica si el piloto autom�tico est� activado
        /// </summary>
        private bool m_Enabled = false;
        /// <summary>
        /// Posici�n objetivo del piloto autom�tico
        /// </summary>
        private Vector3 m_AutoTarget;
        /// <summary>
        /// Velocidad m�xima del piloto autom�tico
        /// </summary>
        private float m_AutoVelocity;
        /// <summary>
        /// Veh�culo a seguir
        /// </summary>
        private IVehicleController m_VehicleToFollow;
        /// <summary>
        /// Indica si el veh�culo est�n en rango
        /// </summary>
        private bool m_OnRange = false;
        /// <summary>
        /// Distancia al objetivo
        /// </summary>
        private float m_DistanceToTarget = 0f;

        /// <summary>
        /// Obtiene o establece si el piloto autom�tico est� activado
        /// </summary>
        public bool Enabled
        {
            get
            {
                return m_Enabled;
            }
            set
            {
                m_Enabled = value;

                if (!m_Enabled)
                {
                    m_VehicleToFollow = null;
                }
            }
        }
        /// <summary>
        /// Obtiene la posici�n destino
        /// </summary>
        public Vector3 Target
        {
            get
            {
                if (this.m_VehicleToFollow != null)
                {
                    return this.m_VehicleToFollow.Position;
                }
                else
                {
                    return this.m_AutoTarget;
                }
            }
        }
        /// <summary>
        /// Obtiene la velocidad m�xima
        /// </summary>
        public float MaximumVelocity
        {
            get
            {
                return this.m_AutoVelocity;
            }
        }
        /// <summary>
        /// Obtiene si se est� siguiendo a algun objetivo
        /// </summary>
        public bool Following
        {
            get
            {
                return (this.m_VehicleToFollow != null);
            }
        }
        /// <summary>
        /// Indica si el veh�culo est�n en rango
        /// </summary>
        public bool OnRange
        {
            get
            {
                return this.m_OnRange;
            }
        }
        /// <summary>
        /// Distancia al objetivo
        /// </summary>
        public float DistanceToTarget
        {
            get
            {
                return this.m_DistanceToTarget;
            }
        }

        /// <summary>
        /// Establece el objetivo y la velocidad del piloto autom�tico y lo activa
        /// </summary>
        /// <param name="target">Objetivo</param>
        /// <param name="velocity">Velocidad m�xima</param>
        public void GoTo(Vector3 target, float velocity)
        {
            this.m_AutoTarget = target;

            this.m_AutoVelocity = velocity;

            this.m_Enabled = true;
        }
        /// <summary>
        /// Establece el veh�culo a seguir
        /// </summary>
        /// <param name="vehicle">Veh�culo</param>
        public void Follow(IVehicleController vehicle, float velocity)
        {
            this.m_VehicleToFollow = vehicle;

            this.m_AutoVelocity = velocity;

            this.m_Enabled = true;
        }

        /// <summary>
        /// Actualiza el piloto autom�tico
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        /// <param name="vehicle">Veh�culo</param>
        public void UpdateAutoPilot(GameTime gameTime, IVehicleController vehicle)
        {
            if (this.m_Enabled)
            {
                // Obtener las componentes sin altura
                Vector3 currentPosition = new Vector3(vehicle.Position.X, 0f, vehicle.Position.Z);
                Vector3 targetPosition = new Vector3(this.Target.X, 0f, this.Target.Z);

                this.m_DistanceToTarget = Vector3.Distance(currentPosition, targetPosition);
                if (this.m_DistanceToTarget < vehicle.Velocity * 2f)
                {
                    this.m_OnRange = true;

                    // Frenar ...
                    if (this.m_VehicleToFollow != null)
                    {
                        if (this.m_VehicleToFollow.Velocity > vehicle.Velocity)
                        {
                            // Frenar
                            //vehicle.Brake();
                        }
                    }
                    else
                    {
                        // Frenar
                        //vehicle.Brake();

                        // Detener el piloto autom�tico si se ha alcanzado el destino
                        this.m_Enabled = (this.m_DistanceToTarget > 100f);
                    }                    
                }
                else
                {
                    this.m_OnRange = false;

                    // Obtener la rotaci�n que se quiere alcanzar girando con un Billboard
                    Matrix rotationMatrix = Matrix.CreateBillboard(currentPosition, targetPosition, Vector3.Up, null);
                    Quaternion rotationQuaternion = Quaternion.CreateFromRotationMatrix(rotationMatrix);

                    // Obtener el �ngulo de la rotaci�n
                    Vector3 targetDirection = Vector3.Normalize(targetPosition - currentPosition);
                    Vector3 currentDirection = Vector3.Normalize(vehicle.Direction);
                    float angle = currentDirection.Angle(targetDirection);

                    // Aplicar la nueva rotaci�n directamente a la rotaci�n actual
                    if (angle >= 0.01f)
                    {
                        vehicle.Orientation = Quaternion.Slerp(vehicle.Orientation, rotationQuaternion, MathHelper.ToRadians(1f));
                    }

                    // Si el �ngulo es menor de 180� se acelera
                    if (angle < MathHelper.PiOver2)
                    {
                        if (this.m_VehicleToFollow != null)
                        {
                            if (this.m_DistanceToTarget >= vehicle.Velocity * 3f)
                            {
                                // Acelerar
                                if (this.m_AutoVelocity > vehicle.Velocity)
                                {
                                    // La velocidad actual es menor que la m�xima que se puede usar
                                    vehicle.Accelerate(gameTime);
                                }
                            }
                        }
                        else
                        {
                            // Acelerar
                            if (this.m_AutoVelocity > vehicle.Velocity)
                            {
                                // Si la velocidad a alcanzar no es suficiente se acelera
                                vehicle.Accelerate(gameTime);
                            }
                        }
                    }
                }
            }
        }
    }
}
