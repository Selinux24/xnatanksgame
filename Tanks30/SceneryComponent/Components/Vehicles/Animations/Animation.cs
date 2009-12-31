using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameComponents.Vehicles.Animations
{
    /// <summary>
    /// Representa una rotación sobre un eje específico
    /// </summary>
    public class Animation
    {
        /// <summary>
        /// Nombre de la animación
        /// </summary>
        public readonly string Name = null;
        // Indice del bone que se va a animar
        public readonly int Index = -1;
        // Nombre del bone que se va a animar
        public readonly string BoneName = null;

        // Bone que se va a animar
        private ModelBone m_Bone = null;
        // Transformación a aplicar al bone
        private Matrix m_Transform = Matrix.Identity;
        // Eje de sobre el que se realiza la rotación
        private Vector3 m_Axis = Vector3.Up;
        // Rotación
        private Quaternion m_Rotation = Quaternion.Identity;

        /// <summary>
        /// Obtiene la rotación
        /// </summary>
        public Quaternion Rotation
        {
            get
            {
                return m_Rotation;
            }
        }
        /// <summary>
        /// Obtiene la transformación
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
        /// Actualiza la información de rotación
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public virtual void Update(GameTime gameTime)
        {
            // La matriz de transformación es la representación del quaternion
            m_Transform = Matrix.CreateFromQuaternion(m_Rotation);
        }
        /// <summary>
        /// Inicializa la animación
        /// </summary>
        /// <param name="axis">Establece el eje de rotación</param>
        public virtual void Initialize(Vector3 axis)
        {
            m_Axis = axis;
        }
        /// <summary>
        /// Reinicia la animación al origen
        /// </summary>
        public virtual void Reset()
        {
            m_Rotation = Quaternion.Identity;
        }
        /// <summary>
        /// Establece el ángulo de rotación
        /// </summary>
        /// <param name="angle">Ángulo de rotación</param>
        public virtual void SetRotationAngle(float angle)
        {
            m_Rotation = Quaternion.CreateFromAxisAngle(m_Axis, angle);
        }
        /// <summary>
        /// Añade el ángulo a la rotación
        /// </summary>
        /// <param name="angle">Ángulo a añadir a la rotación</param>
        public virtual void Rotate(float angle)
        {
            m_Rotation *= Quaternion.CreateFromAxisAngle(m_Axis, angle);
        }
        /// <summary>
        /// Obtiene la representación en texto de la animación
        /// </summary>
        /// <returns>Devuelve la representación en texto de la animación</returns>
        public override string ToString()
        {
            string mask = "{0}:{1}";

            return string.Format(mask, this.GetType(), this.BoneName);
        }
    }
}
