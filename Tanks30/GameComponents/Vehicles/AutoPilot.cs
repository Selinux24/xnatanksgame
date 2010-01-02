using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Physics;

namespace GameComponents.Vehicles
{
    /// <summary>
    /// Piloto autom�tico
    /// </summary>
    public class AutoPilot
    {
        // Indica si el piloto autom�tico est� activado
        private bool m_Enabled = false;
        // Posici�n objetivo del piloto autom�tico
        private Vector3 m_AutoTarget;
        // Velocidad m�xima del piloto autom�tico
        private float m_AutoVelocity;
        // Veh�culo a seguir
        private IVehicleController m_VehicleToFollow;

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
        /// Establece el objetivo y la velocidad del piloto autom�tico y lo activa
        /// </summary>
        /// <param name="target">Objetivo</param>
        /// <param name="velocity">Velocidad m�xima</param>
        public void GoTo(Vector3 target, float velocity)
        {
            this.AccelerateTo(velocity);

            this.LookTo(target);

            m_Enabled = true;
        }
        /// <summary>
        /// Establece el veh�culo a seguir
        /// </summary>
        /// <param name="vehicle">Veh�culo</param>
        public void Follow(IVehicleController vehicle)
        {
            m_VehicleToFollow = vehicle;

            m_Enabled = true;
        }

        /// <summary>
        /// Establece la velocidad m�xima del piloto autom�tico
        /// </summary>
        /// <param name="velocity">Velocidad m�xima</param>
        private void AccelerateTo(float velocity)
        {
            m_AutoVelocity = velocity;
        }
        /// <summary>
        /// Establece el objetivo del piloto autom�tico
        /// </summary>
        /// <param name="target">Objetivo</param>
        private void LookTo(Vector3 target)
        {
            m_AutoTarget = target;
        }
        /// <summary>
        /// Actualiza el piloto autom�tico
        /// </summary>
        /// <param name="vehicle">Veh�culo</param>
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

                    // Aplicar la nueva rotaci�n directamente a la rotaci�n actual
                    vehicle.Rotation = Quaternion.Slerp(vehicle.Rotation, quat, MathHelper.ToRadians(1f));

                    // Obtener el �ngulo despu�s de la rotaci�n
                    Vector3 targetDirection = Vector3.Normalize(tg - pos);
                    Vector3 currentDirection = Vector3.Normalize(vehicle.Direction);
                    float angle = PhysicsMathHelper.Angle(currentDirection, targetDirection);

                    // Si el �ngulo es menor de 180� se acelera
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
