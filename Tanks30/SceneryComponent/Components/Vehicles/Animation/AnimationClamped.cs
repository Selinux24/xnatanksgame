using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameComponents.Vehicles.Animation
{
    /// <summary>
    /// Representa una rotación entre límites
    /// </summary>
    public class AnimationClamped : AnimationBase
    {
        // Ángulo inicial de la rotación en radianes
        private float m_RotationFrom = 0f;
        // Ángulo final de la rotación en radianes
        private float m_RotationTo = 0f;
        // Ángulo actual en radianes
        private float m_CurrentAngle = 0f;
        // Velocidad angular en radianes
        private float m_AngularVelocity = 0f;

        /// <summary>
        /// Indica si la rotación tiene límites establecidos
        /// </summary>
        public bool HasLimits
        {
            get
            {
                return (m_RotationFrom != m_RotationTo);
            }
        }
        /// <summary>
        /// Indica si se ha alcanzado el inicio de la rotación
        /// </summary>
        public bool RotationFromReached
        {
            get
            {
                if (HasLimits)
                {
                    return (m_RotationFrom == m_CurrentAngle);
                }

                return false;
            }
        }
        /// <summary>
        /// Indica si se ha alcanzado el final de la rotación
        /// </summary>
        public bool RotationToReached
        {
            get
            {
                if (HasLimits)
                {
                    return (m_RotationTo == m_CurrentAngle);
                }

                return false;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="bone">Bone que se va a animar</param>
        public AnimationClamped(ModelBone bone)
            : base(bone)
        {

        }

        /// <summary>
        /// Actualiza los datos de animación
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //Animación automática
            if (m_AngularVelocity != 0f)
            {
                this.Rotate(m_AngularVelocity);

                if (RotationFromReached || RotationToReached)
                {
                    m_AngularVelocity = 0f;
                }
            }
        }
        /// <summary>
        /// Inicializa la animación
        /// </summary>
        /// <param name="axis">Eje de rotación</param>
        /// <param name="angleFrom">Ángulo desde</param>
        /// <param name="angleTo">Ángulo hasta</param>
        public virtual void Initialize(Vector3 axis, float angleFrom, float angleTo)
        {
            base.Initialize(axis);

            m_RotationFrom = MathHelper.ToRadians(angleFrom);
            m_RotationTo = MathHelper.ToRadians(angleTo);
        }
        /// <summary>
        /// Reinicia la animación
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            m_CurrentAngle = 0f;
        }
        /// <summary>
        /// Establece el ángulo de rotación entre los límites informados
        /// </summary>
        /// <param name="angle">Ángulo de rotación</param>
        public override void SetRotationAngle(float angle)
        {
            // Cortar el ángulo si traspasa los límites
            m_CurrentAngle = Clamp(angle);

            base.SetRotationAngle(m_CurrentAngle);
        }
        /// <summary>
        /// Añade el ángulo a la rotación entre los límites de rotación
        /// </summary>
        /// <param name="angle">Ángulo de rotación</param>
        public override void Rotate(float angle)
        {
            // Añadir el ángulo
            m_CurrentAngle += angle;
            // Cortar el ángulo si está fuera de los límites de rotación
            m_CurrentAngle = Clamp(m_CurrentAngle);

            base.SetRotationAngle(m_CurrentAngle);
        }
        /// <summary>
        /// Comienza la animación
        /// </summary>
        /// <param name="angularVelocity">Velocidad angular de la animación</param>
        public void Begin(float angularVelocity)
        {
            m_AngularVelocity = angularVelocity;
        }
        /// <summary>
        /// Termina la animación
        /// </summary>
        public void End()
        {
            m_AngularVelocity = 0f;
        }
        /// <summary>
        /// Corta el ángulo especificado entre los límites establecidos
        /// </summary>
        /// <param name="angle">Ángulo a cortar</param>
        /// <returns>Devuelve el ángulo cortado</returns>
        protected virtual float Clamp(float angle)
        {
            float newAngle = 0f;

            if (!HasLimits)
            {
                // Si no hay límites establecemos el ángulo
                newAngle = angle;
            }
            else
            {
                // Si hay límites se corta el ángulo
                newAngle = MathHelper.Clamp(angle, m_RotationFrom, m_RotationTo);
            }

            return newAngle;
        }
    }
}
