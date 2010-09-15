using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameComponents.Animation
{
    using Common.Helpers;

    /// <summary>
    /// Representa una rotación sobre un eje específico
    /// </summary>
    public class Animation
    {
        /// <summary>
        /// Nombre de la animación
        /// </summary>
        public readonly string Name = null;
        /// <summary>
        /// Indice del bone que se va a animar
        /// </summary>
        public readonly int Index = -1;
        /// <summary>
        /// Nombre del bone que se va a animar
        /// </summary>
        public readonly string BoneName = null;

        /// <summary>
        /// Bone que se va a animar
        /// </summary>
        private ModelBone m_Bone = null;
        /// <summary>
        /// Transformación a aplicar al bone
        /// </summary>
        private Matrix m_Transform = Matrix.Identity;
        /// <summary>
        /// Eje de sobre el que se realiza la rotación
        /// </summary>
        private Vector3 m_Axis = Vector3.Up;
        /// <summary>
        /// Rotación
        /// </summary>
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

        /// <summary>
        /// Crea la lista de animaciones usable por los componentes
        /// </summary>
        /// <param name="model">Modelo a animar</param>
        /// <returns>Devuelve una lista de animaciones</returns>
        public static Animation[] CreateAnimationList(Model model, AnimationInfo[] animationControlers)
        {
            List<Animation> animationList = new List<Animation>();

            if (animationControlers != null && animationControlers.Length > 0)
            {
                foreach (AnimationInfo animationInfo in animationControlers)
                {
                    if (animationInfo.Type == typeof(Animation).ToString())
                    {
                        Animation animation = new Animation(animationInfo.Name, model.Bones[animationInfo.BoneName]);
                        animation.Initialize(animationInfo.Axis);

                        animationList.Add(animation);
                    }
                    else if (animationInfo.Type == typeof(AnimationAxis).ToString())
                    {
                        AnimationAxis animation = new AnimationAxis(animationInfo.Name, model.Bones[animationInfo.BoneName]);
                        animation.Initialize(animationInfo.Axis, animationInfo.AngleFrom, animationInfo.AngleTo, animationInfo.Velocity, animationInfo.Inverse);

                        animationList.Add(animation);
                    }
                }
            }

            return animationList.ToArray();
        }
    }
}
