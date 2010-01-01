using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameComponents.Vehicles.Animations
{
    /// <summary>
    /// Representa una rotaci�n sobre un eje espec�fico
    /// </summary>
    public class Animation
    {
        /// <summary>
        /// Nombre de la animaci�n
        /// </summary>
        public readonly string Name = null;
        // Indice del bone que se va a animar
        public readonly int Index = -1;
        // Nombre del bone que se va a animar
        public readonly string BoneName = null;

        // Bone que se va a animar
        private ModelBone m_Bone = null;
        // Transformaci�n a aplicar al bone
        private Matrix m_Transform = Matrix.Identity;
        // Eje de sobre el que se realiza la rotaci�n
        private Vector3 m_Axis = Vector3.Up;
        // Rotaci�n
        private Quaternion m_Rotation = Quaternion.Identity;

        /// <summary>
        /// Obtiene la rotaci�n
        /// </summary>
        public Quaternion Rotation
        {
            get
            {
                return m_Rotation;
            }
        }
        /// <summary>
        /// Obtiene la transformaci�n
        /// </summary>
        public Matrix Transform
        {
            get
            {
                return m_Transform;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Nombre</param>
        /// <param name="bone">Bone que se va a animar</param>
        public Animation(string name, ModelBone bone)
        {
            this.Name = name;

            if (bone != null)
            {
                this.m_Bone = bone;
                this.BoneName = m_Bone.Name;
                this.Index = m_Bone.Index;
            }
        }

        /// <summary>
        /// Actualiza la informaci�n de rotaci�n
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public virtual void Update(GameTime gameTime)
        {
            // La matriz de transformaci�n es la representaci�n del quaternion
            m_Transform = Matrix.CreateFromQuaternion(m_Rotation);
        }
        /// <summary>
        /// Inicializa la animaci�n
        /// </summary>
        /// <param name="axis">Establece el eje de rotaci�n</param>
        public virtual void Initialize(Vector3 axis)
        {
            m_Axis = axis;
        }
        /// <summary>
        /// Reinicia la animaci�n al origen
        /// </summary>
        public virtual void Reset()
        {
            m_Rotation = Quaternion.Identity;
        }
        /// <summary>
        /// Establece el �ngulo de rotaci�n
        /// </summary>
        /// <param name="angle">�ngulo de rotaci�n</param>
        public virtual void SetRotationAngle(float angle)
        {
            m_Rotation = Quaternion.CreateFromAxisAngle(m_Axis, angle);
        }
        /// <summary>
        /// A�ade el �ngulo a la rotaci�n
        /// </summary>
        /// <param name="angle">�ngulo a a�adir a la rotaci�n</param>
        public virtual void Rotate(float angle)
        {
            m_Rotation *= Quaternion.CreateFromAxisAngle(m_Axis, angle);
        }
        /// <summary>
        /// Obtiene la representaci�n en texto de la animaci�n
        /// </summary>
        /// <returns>Devuelve la representaci�n en texto de la animaci�n</returns>
        public override string ToString()
        {
            string mask = "{0}:{1}";

            return string.Format(mask, this.GetType(), this.BoneName);
        }
    }
}
