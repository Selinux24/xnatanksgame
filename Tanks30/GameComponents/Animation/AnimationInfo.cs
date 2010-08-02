using System;
using Microsoft.Xna.Framework;

namespace GameComponents.Animation
{
    /// <summary>
    /// Información de animación
    /// </summary>
    [Serializable]
    public class AnimationInfo
    {
        /// <summary>
        /// Nombre de la animación
        /// </summary>
        public string Name = null;
        /// <summary>
        /// Tipo
        /// </summary>
        public string Type = null;
        /// <summary>
        /// Nombre del nodo
        /// </summary>
        public string BoneName = null;
        /// <summary>
        /// Eje de giro
        /// </summary>
        public Vector3 Axis = Vector3.Up;
        /// <summary>
        /// Angulo desde
        /// </summary>
        public float AngleFrom;
        /// <summary>
        /// Angulo hasta
        /// </summary>
        public float AngleTo;
        /// <summary>
        /// Velocidad de giro
        /// </summary>
        public float Velocity;
        /// <summary>
        /// Indica si la animación se realizará en sentido inverso
        /// </summary>
        public bool Inverse;

        /// <summary>
        /// Constructor
        /// </summary>
        public AnimationInfo()
        {

        }
    }
}
