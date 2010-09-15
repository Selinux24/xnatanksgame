using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameComponents.Animation
{
    using Common.Helpers;

    /// <summary>
    /// Representa una rotaci�n sobre un eje espec�fico
    /// </summary>
    public class Animation
    {
        /// <summary>
        /// Nombre de la animaci�n
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
        /// Transformaci�n a aplicar al bone
        /// </summary>
        private Matrix m_Transform = Matrix.Identity;
        /// <summary>
        /// Eje de sobre el que se realiza la rotaci�n
        /// </summary>
        private Vector3 m_Axis = Vector3.Up;
        /// <summary>
        /// Rotaci�n
        /// </summary>
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
