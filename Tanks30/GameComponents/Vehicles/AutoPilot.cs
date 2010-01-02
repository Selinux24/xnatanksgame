using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Physics;

namespace GameComponents.Vehicles
{
    /// <summary>
    /// Piloto automático
    /// </summary>
    public class AutoPilot
    {
        // Indica si el piloto automático está activado
        private bool m_Enabled = false;
        // Posición objetivo del piloto automático
        private Vector3 m_AutoTarget;
        // Velocidad máxima del piloto automático
        private float m_AutoVelocity;
        // Vehículo a seguir
        private IVehicleController m_VehicleToFollow;

        /// <summary>
        /// Obtiene o establece si el piloto automático está activado
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
        /// Establece el objetivo y la velocidad del piloto automático y lo activa
        /// </summary>
        /// <param name="target">Objetivo</param>
        /// <param name="velocity">Velocidad máxima</param>
        public void GoTo(Vector3 target, float velocity)
        {
            this.AccelerateTo(velocity);

            this.LookTo(target);

            m_Enabled = true;
        }
        /// <summary>
        /// Establece el vehículo a seguir
        /// </summary>
        /// <param name="vehicle">Vehículo</param>
        public void Follow(IVehicleController vehicle)
        {
            m_VehicleToFollow = vehicle;

            m_Enabled = true;
        }

        /// <summary>
        /// Establece la velocidad máxima del piloto automático
        /// </summary>
        /// <param name="velocity">Velocidad máxima</param>
        private void AccelerateTo(float velocity)
        {
            m_AutoVelocity = velocity;
        }
        /// <summary>
        /// Establece el objetivo del piloto automático
        /// </summary>
        /// <param name="target">Objetivo</param>
        private void LookTo(Vector3 target)
        {
            m_AutoTarget = target;
        }
        /// <summary>
        /// Actualiza el piloto automático
        /// </summary>
        /// <param name="vehicle">Vehículo</param>
        public void UpdateAutoPilot(IVehicleController vehicle)
        {
            if (m_Enabled)
            {
                if (m_VehicleToFollow != null)
                {
                    // Actualizar objetivo y velocidad hasta el objetivo
                    m_AutoTarget = m_VehicleToFollow.Position;
                    m_AutoVelocity = m_VehicleToFollow.Velocity;
                }

                // Obtener las componentes sin altura
                Vector3 pos = new Vector3(vehicle.Position.X, 0f, vehicle.Position.Z);
                Vector3 tg = new Vector3(m_AutoTarget.X, 0f, m_AutoTarget.Z);

                float distance = Vector3.Distance(pos, tg);
                if (distance < vehicle.Velocity * 3f || distance < 10f)
                {
                    // Frenar ...
                    vehicle.Brake();

                    // ... hasta detenerse
                    m_Enabled = !vehicle.IsStatic;
                }
                else
                {
                    // Obtener la matriz que se quiere alcanzar girando con un Billboard
                    Matrix mat = Matrix.CreateBillboard(pos, tg, Vector3.Up, null);
                    Quaternion quat = Quaternion.CreateFromRotationMatrix(mat);

                    // Aplicar la nueva rotación directamente a la rotación actual
                    vehicle.Rotation = Quaternion.Slerp(vehicle.Rotation, quat, MathHelper.ToRadians(1f));

                    // Obtener el ángulo después de la rotación
                    Vector3 targetDirection = Vector3.Normalize(tg - pos);
                    Vector3 currentDirection = Vector3.Normalize(vehicle.Direction);
                    float angle = PhysicsMathHelper.Angle(currentDirection, targetDirection);

                    // Si el ángulo es menor de 180º se acelera
                    if (angle < MathHelper.PiOver2)
                    {
                        if (vehicle.Velocity < m_AutoVelocity)
                        {
                            // Si la velocidad a alcanzar no es suficiente se acelera
                            vehicle.Accelerate();
                        }
                    }
                }
            }
        }
    }
}
