using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameComponents.Animation
{
    /// <summary>
    /// Representa una rotaci�n entre l�mites
    /// </summary>
    public class AnimationAxis : Animation
    {
        /// <summary>
        /// �ngulo inicial de la rotaci�n en radianes
        /// </summary>
        private float m_RotationFrom = 0f;
        /// <summary>
        /// �ngulo final de la rotaci�n en radianes
        /// </summary>
        private float m_RotationTo = 0f;
        /// <summary>
        /// �ngulo actual en radianes
        /// </summary>
        private float m_CurrentAngle = 0f;
        /// <summary>
        /// Velocidad angular de la animaci�n
        /// </summary>
        private float m_AngularVelocity = 0f;
        /// <summary>
        /// Velocidad angular en radianes
        /// </summary>
        private float m_CurrentAngularVelocity = 0f;
        /// <summary>
        /// Indica si se realizar� el giro de la animaci�n en sentido inverso
        /// </summary>
        private bool m_InverseAngle = false;

        /// <summary>
        /// Indica si la rotaci�n tiene l�mites establecidos
        /// </summary>
        public bool HasLimits
        {
            get
            {
                return (m_RotationFrom != m_RotationTo);
            }
        }
        /// <summary>
        /// Indica si se ha alcanzado el inicio de la rotaci�n
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
        /// Indica si se ha alcanzado el final de la rotaci�n
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
        /// <param name="name">Nombre</param>
        /// <param name="bone">Bone que se va a animar</param>
        public AnimationAxis(string name, ModelBone bone)
            : base(name, bone)
        {

        }

        /// <summary>
        /// Actualiza los datos de animaci�n
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //Animaci�n autom�tica
            if (m_CurrentAngularVelocity != 0f)
            {
                this.Rotate(m_CurrentAngularVelocity);

                if (RotationFromReached || RotationToReached)
                {
                    m_CurrentAngularVelocity = 0f;
                }
            }
        }
        /// <summary>
        /// Inicializa la animaci�n
        /// </summary>
        /// <param name="axis">Eje de rotaci�n</param>
        /// <param name="angleFrom">�ngulo desde</param>
        /// <param name="angleTo">�ngulo hasta</param>
        /// <param name="angularVelocity">Velocidad angular</param>
        public virtual void Initialize(Vector3 axis, float angleFrom, float angleTo, float angularVelocity, bool inverse)
        {
            base.Initialize(axis);

            m_RotationFrom = MathHelper.ToRadians(angleFrom);
            m_RotationTo = MathHelper.ToRadians(angleTo);
            m_AngularVelocity = angularVelocity;
            m_InverseAngle = inverse;
        }
        /// <summary>
        /// Reinicia la animaci�n
        /// </summary>
        public override void Reset()
        {
            base.Reset();

            m_CurrentAngle = 0f;
        }
        /// <summary>
        /// Establece el �ngulo de rotaci�n entre los l�mites informados
        /// </summary>
        /// <param name="angle">�ngulo de rotaci�n</param>
        public override void SetRotationAngle(float angle)
        {
            // Cortar el �ngulo si traspasa los l�mites
            m_CurrentAngle = Clamp(angle);

            base.SetRotationAngle(m_CurrentAngle);
        }
        /// <summary>
        /// A�ade el �ngulo a la rotaci�n entre los l�mites de rotaci�n
        /// </summary>
        /// <param name="angle">�ngulo de rotaci�n</param>
        public override void Rotate(float angle)
        {
            angle = MathHelper.Clamp(angle, -m_AngularVelocity, m_AngularVelocity);

            // A�adir el �ngulo
            m_CurrentAngle += angle;
            // Cortar el �ngulo si est� fuera de los l�mites de rotaci�n
            m_CurrentAngle = Clamp(m_CurrentAngle);

            base.SetRotationAngle(m_CurrentAngle);
        }
        /// <summary>
        /// Comienza la animaci�n
        /// </summary>
        /// <param name="reverse">Indica si debe realizarse hacia atr�s</param>
        public void Begin(bool reverse)
        {
            m_CurrentAngularVelocity = (reverse) ? -m_AngularVelocity : m_AngularVelocity;
        }
        /// <summary>
        /// Termina la animaci�n
        /// </summary>
        public void End()
        {
            m_CurrentAngularVelocity = 0f;
        }
        /// <summary>
        /// Corta el �ngulo especificado entre los l�mites establecidos
        /// </summary>
        /// <param name="angle">�ngulo a cortar</param>
        /// <returns>Devuelve el �ngulo cortado</returns>
        protected virtual float Clamp(float angle)
        {
            float newAngle = 0f;

            if (!HasLimits)
            {
                // Si no hay l�mites establecemos el �ngulo
                newAngle = angle;
            }
            else
            {
                // Si hay l�mites se corta el �ngulo
                if (!m_InverseAngle)
                {
                    newAngle = MathHelper.Clamp(angle, m_RotationFrom, m_RotationTo);
                }
                else
                {
                    if (angle < 0)
                    {
                        //Negativo
                        newAngle = -MathHelper.Clamp(-angle, m_RotationFrom, m_RotationTo);
                    }
                    else if (angle > 0)
                    {
                        //Positivo
                        newAngle = MathHelper.Clamp(angle, -m_RotationTo, -m_RotationFrom);
                    }
                }
            }

            return newAngle;
        }
    }
}
