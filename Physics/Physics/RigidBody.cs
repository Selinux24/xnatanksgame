using System;
using Microsoft.Xna.Framework;

namespace Physics
{
    /// <summary>
    /// A rigid body is the basic simulation object in the physics
    /// core.
    ///
    /// It has position and orientation data, along with first
    /// derivatives. It can be integrated forward through time, and
    /// have forces, torques and impulses (linear or angular) applied
    /// to it. The rigid body manages its state and allows access
    /// through a set of methods.
    ///
    /// A ridid body contains 64 words (the size of which is given
    /// by the precision: sizeof(real)). It contains no virtual
    /// functions, so should take up exactly 64 words in memory. Of
    /// this total 15 words are padding, distributed among the
    /// Vector3 data members.
    /// </summary>
    public class RigidBody
    {
        /// <summary>
        /// Magnitud inversa de masa
        /// </summary>
        private float m_InverseMass;
        /// <summary>
        /// Coheficiente de rebote lineal
        /// </summary>
        private float m_LinearDamping;
        /// <summary>
        /// Coheficiente de rebote angular
        /// </summary>
        private float m_AngularDamping;

        /// <summary>
        /// Vector posición
        /// </summary>
        private Vector3 m_Position = Vector3.Zero;
        /// <summary>
        /// Orientación
        /// </summary>
        private Quaternion m_Orientation = Quaternion.Identity;
        /// <summary>
        /// Vector de velocidad lineal
        /// </summary>
        private Vector3 m_LinearVelocity = Vector3.Zero;
        /// <summary>
        /// Vector de velocidad angular
        /// </summary>
        private Vector3 m_AngularVelocity = Vector3.Zero;
        /// <summary>
        /// Vector de aceleración
        /// </summary>
        private Vector3 m_Acceleration = Vector3.Zero;
        /// <summary>
        /// Vector de aceleración del último frame
        /// </summary>
        private Vector3 m_LastFrameAcceleration = Vector3.Zero;

        /// <summary>
        /// Tensor inverso de inercia
        /// </summary>
        private Matrix3 m_InverseInertiaTensor = Matrix3.Identity;
        /// <summary>
        /// Tensor inverso de inercia en coordenadas del mundo
        /// </summary>
        private Matrix3 m_InverseInertiaTensorWorld = Matrix3.Identity;
        /// <summary>
        /// Matriz de transformación
        /// </summary>
        private Matrix m_TransformMatrix = Matrix.Identity;

        /// <summary>
        /// Indicador de estado del cuerpo
        /// </summary>
        private bool m_IsAwake;
        /// <summary>
        /// Indica si el cuerpo puede desactivarse o no
        /// </summary>
        private bool m_CanSleep;

        /// <summary>
        /// Acumulador de energía cinética
        /// </summary>
        private float m_Motion;
        /// <summary>
        /// Acumulador de fuerzas lineales
        /// </summary>
        private Vector3 m_ForceAccum = Vector3.Zero;
        /// <summary>
        /// Acumulador de fuerzas angulares
        /// </summary>
        private Vector3 m_TorqueAccum = Vector3.Zero;

        /// <summary>
        /// Obtiene o establece la masa
        /// </summary>
        public float Mass
        {
            get
            {
                if (this.m_InverseMass == 0f)
                {
                    return float.PositiveInfinity;
                }
                else
                {
                    return (1.0f) / this.m_InverseMass;
                }
            }
            set
            {
                if (value != 0)
                {
                    this.m_InverseMass = (1.0f) / value;
                }
            }
        }
        /// <summary>
        /// Obtiene la masa inversa
        /// </summary>
        public float InverseMass
        {
            get
            {
                return this.m_InverseMass;
            }
        }

        /// <summary>
        /// Obtiene o establece la posición del cuerpo
        /// </summary>
        public Vector3 Position
        {
            get
            {
                return m_Position;
            }
            set
            {
                m_Position = value;
            }
        }
        /// <summary>
        /// Obtiene o establece la orientación del cuerpo
        /// </summary>
        public Quaternion Orientation
        {
            get
            {
                return this.m_Orientation;
            }
            set
            {
                this.m_Orientation = Quaternion.Normalize(value);
            }
        }
        /// <summary>
        /// Obtiene o establece la velocidad del cuerpo
        /// </summary>
        public Vector3 Velocity
        {
            get
            {
                return this.m_LinearVelocity;
            }
            set
            {
                this.m_LinearVelocity = value;
            }
        }
        /// <summary>
        /// Obtiene o establece la rotación del cuerpo
        /// </summary>
        public Vector3 Rotation
        {
            get
            {
                return this.m_AngularVelocity;
            }
            set
            {
                this.m_AngularVelocity = value;
            }
        }
        /// <summary>
        /// Obtiene o establece la aceleración del cuerpo
        /// </summary>
        public Vector3 Acceleration
        {
            get
            {
                return this.m_Acceleration;
            }
            set
            {
                this.m_Acceleration = value;
            }
        }
        /// <summary>
        /// Obtiene la aceleración en el último frame
        /// </summary>
        public Vector3 LastFrameAcceleration
        {
            get
            {
                return this.m_LastFrameAcceleration;
            }
        }

        /// <summary>
        /// Obtiene o establece el tensor de inercia
        /// </summary>
        public Matrix3 InertiaTensor
        {
            get
            {
                return Matrix3.Invert(this.m_InverseInertiaTensorWorld);
            }
            set
            {
                this.m_InverseInertiaTensor = Matrix3.Invert(value);
            }
        }
        /// <summary>
        /// Obtiene o establece el tensor inverso de inercia
        /// </summary>
        public Matrix3 InverseInertiaTensor
        {
            get
            {
                return this.m_InverseInertiaTensor;
            }
            set
            {
                this.m_InverseInertiaTensor = value;
            }
        }
        /// <summary>
        /// Obtiene el tensor de inercia en coordenadas del mundo
        /// </summary>
        public Matrix3 InertiaTensorWorld
        {
            get
            {
                return Matrix3.Invert(this.m_InverseInertiaTensorWorld);
            }
        }
        /// <summary>
        /// Obtiene el tensor inverso de inercia en coordenadas del mundo
        /// </summary>
        public Matrix3 InverseInertiaTensorWorld
        {
            get
            {
                return this.m_InverseInertiaTensorWorld;
            }
        }

        /// <summary>
        /// Obtiene la matriz de transformación del cuerpo
        /// </summary>
        public Matrix Transform
        {
            get
            {
                return m_TransformMatrix;
            }
        }

        /// <summary>
        /// Activa o desactiva el cuerpo
        /// </summary>
        public bool IsAwake
        {
            get
            {
                return m_IsAwake;
            }
            set
            {
                if (value)
                {
                    m_IsAwake = true;

                    // Añadir un poco de movimiento para evitar que se active inmediatamente
                    m_Motion = Core.SleepEpsilon * 2.0f;
                }
                else
                {
                    m_IsAwake = false;
                    this.m_LinearVelocity = Vector3.Zero;
                    this.m_AngularVelocity = Vector3.Zero;
                }
            }
        }
        /// <summary>
        /// Obtiene o establece si el objeto puede quedarse inactivo
        /// </summary>
        public bool CanSleep
        {
            get
            {
                return m_CanSleep;
            }
            set
            {
                this.m_CanSleep = value;

                if (!this.m_CanSleep && !this.m_IsAwake)
                {
                    this.IsAwake = true;
                }
            }
        }

        /// <summary>
        /// Obtiene si el cuerpo tiene masa finita
        /// </summary>
        /// <returns>Devuelve verdadero si el cuerpo tiene masa finita</returns>
        public bool HasFiniteMass()
        {
            return this.m_InverseMass >= 0.0f;
        }

        /// <summary>
        /// Establece los coheficientes de rebote
        /// </summary>
        /// <param name="linearDamping">Rebote lineal</param>
        /// <param name="angularDamping">Rebote angular</param>
        public void SetDamping(float linearDamping, float angularDamping)
        {
            this.m_LinearDamping = linearDamping;
            this.m_AngularDamping = angularDamping;
        }
        /// <summary>
        /// Calcula el estado interno del cuerpo
        /// </summary>
        public void CalculateDerivedData()
        {
            // Normalizar el quaternion de orientación
            this.m_Orientation.Normalize();

            // Calcular la matriz de transformación con la orientación y la posición
            this.m_TransformMatrix =
                Matrix.CreateFromQuaternion(this.m_Orientation) *
                Matrix.CreateTranslation(this.m_Position);

            // Calcular el tensor de inercia en coordenadas del mundo
            Core.TransformInertiaTensor(
                ref this.m_Orientation,
                ref this.m_InverseInertiaTensor,
                ref this.m_TransformMatrix,
                out this.m_InverseInertiaTensorWorld);
        }
        /// <summary>
        /// Integra el cuerpo en el tiempo
        /// </summary>
        /// <param name="duration">Tiempo</param>
        public void Integrate(float duration)
        {
            if (m_IsAwake)
            {
                // Obtener los coheficientes de rebote para este intervalo de tiempo
                float linearDampingOnTime = Core.Pow(m_LinearDamping, duration);
                float angularDampingOnTime = Core.Pow(m_AngularDamping, duration);

                // Calcular la aceleración lineal desde las fuerzas
                m_LastFrameAcceleration = m_Acceleration;
                m_LastFrameAcceleration += Vector3.Multiply(m_ForceAccum, m_InverseMass);

                // Calcular la aceleración angular desde los pares de torsión
                Vector3 angularAcceleration = Matrix3.Transform(m_InverseInertiaTensorWorld, m_TorqueAccum);

                // Ajustar velocidades
                
                // Actualizar la velocidad lineal usando aceleración lineal
                m_LinearVelocity += Vector3.Multiply(m_LastFrameAcceleration, duration);

                // Actualizar la velocidad angular usando aceleración angular
                m_AngularVelocity += Vector3.Multiply(angularAcceleration, duration);

                // Aplicar los coheficientes de rebote
                m_LinearVelocity *= linearDampingOnTime;
                m_AngularVelocity *= angularDampingOnTime;

                // Ajustar posiciones

                // Actualizar posición lineal
                m_Position += Vector3.Multiply(m_LinearVelocity, duration);

                // Actualizar orientación (posición angular)
                m_Orientation += Core.AddScaledVector(m_AngularVelocity, duration, m_Orientation);

                // Aplicar los coheficientes de rebote
                m_LinearVelocity *= linearDampingOnTime;
                m_AngularVelocity *= angularDampingOnTime;

                // Normalizar la orientación y actualizar las matrices con la nueva posición y orientación
                this.CalculateDerivedData();

                // Limpiar los acumuladores de fuerzas
                this.ClearAccumulators();

                // Actualizar el acumulador de energía cinética
                if (m_CanSleep)
                {
                    // Energía cinética actual
                    float currentMotion = Vector3.Dot(m_LinearVelocity, m_LinearVelocity) + Vector3.Dot(m_AngularVelocity, m_AngularVelocity);
                    float bias = Core.Pow(0.5f, duration);

                    m_Motion = bias * m_Motion + (1f - bias) * currentMotion;

                    if (m_Motion < Core.SleepEpsilon)
                    {
                        // Si no hay energía cinética suficiente se pone el cuerpo a dormir
                        this.IsAwake = false;
                    }
                    else if (m_Motion > 10f * Core.SleepEpsilon)
                    {
                        // Acumular la energía cinética
                        m_Motion = 10f * Core.SleepEpsilon;
                    }
                }
            }
        }
        /// <summary>
        /// Limpia los acumuladores de fuerzas
        /// </summary>
        /// <remarks>Vacía los acumuladores de fuerzas lineales y angulares</remarks>
        public void ClearAccumulators()
        {
            this.m_ForceAccum = Vector3.Zero;
            this.m_TorqueAccum = Vector3.Zero;
        }
        /// <summary>
        /// Suma al acumulador de fuerzas lineales la fuerza especificada
        /// </summary>
        /// <param name="force">Vector de fuerza</param>
        /// <remarks>Cuando se suma la fuerza, se activa el cuerpo</remarks>
        public void AddForce(Vector3 force)
        {
            this.m_ForceAccum += force;

            this.m_IsAwake = true;
        }
        /// <summary>
        /// Suma al acumulador de fuerzas angulares la fuerza especificada
        /// </summary>
        /// <param name="torque">Vector de fuerza angular o par de torsión</param>
        /// <remarks>Cuando se suma la fuerza, se activa el cuerpo</remarks>
        public void AddTorque(Vector3 torque)
        {
            this.m_TorqueAccum += torque;

            this.m_IsAwake = true;
        }
        /// <summary>
        /// Suma la fuerza especificada en el punto en coordenadas del mundo especificado
        /// </summary>
        /// <param name="force">Fuerza</param>
        /// <param name="point">Punto en coordenadas del mundo</param>
        public void AddForceAtPoint(Vector3 force, Vector3 point)
        {
            // Convertir el punto a coordenadas relativas al centro de masas del cuerpo
            Vector3 pt = point - this.m_Position;

            this.m_TorqueAccum += force;
            this.m_TorqueAccum += Vector3.Cross(pt, force);

            this.m_IsAwake = true;
        }
        /// <summary>
        /// Suma la fuerza especificada en el punto en coordenadas locales
        /// </summary>
        /// <param name="force">Fuerza</param>
        /// <param name="point">Punto en coordenadas locales</param>
        public void AddForceAtBodyPoint(Vector3 force, Vector3 point)
        {
            // Convertir el punto en coordenadas relativas al centro de masas del cuerpo
            Vector3 pt = this.GetPointInWorldSpace(point);
            this.AddForceAtPoint(force, pt);

            this.m_IsAwake = true;
        }

        /// <summary>
        /// Obtiene el punto especificado en coordenadas locales
        /// </summary>
        /// <param name="point">Punto</param>
        /// <returns>Devuelve el punto especificado en coordenadas locales</returns>
        public Vector3 GetPointInLocalSpace(Vector3 point)
        {
            Matrix inverse = Matrix.Invert(m_TransformMatrix);

            return Vector3.Transform(point, inverse);
        }
        /// <summary>
        /// Obtiene el punto especificado en coordenadas del mundo
        /// </summary>
        /// <param name="point">Punto</param>
        /// <returns>Devuelve el punto especificado en coordenadas del mundo</returns>
        public Vector3 GetPointInWorldSpace(Vector3 point)
        {
            return Vector3.Transform(point, m_TransformMatrix);
        }
        /// <summary>
        /// Obtiene el vector de dirección especificado en coordenadas locales
        /// </summary>
        /// <param name="direction">Vector de dirección</param>
        /// <returns>Devuelve el vector de dirección especificado en coordenadas locales</returns>
        protected Vector3 GetDirectionInLocalSpace(Vector3 direction)
        {
            Matrix inverse = Matrix.Invert(m_TransformMatrix);

            return Vector3.Transform(direction, inverse);
        }
        /// <summary>
        /// Obtiene el vector de dirección especificado en coordenadas del mundo
        /// </summary>
        /// <param name="direction">Vector de dirección</param>
        /// <returns>Devuelve el vector de dirección especificado en coordenadas del mundo</returns>
        protected Vector3 GetDirectionInWorldSpace(Vector3 direction)
        {
            return Vector3.Transform(direction, m_TransformMatrix);
        }
    }
}