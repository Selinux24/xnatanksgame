using Microsoft.Xna.Framework;

namespace GameComponents.Buildings
{
    using GameComponents.Animation;

	public partial class Building
	{
        /// <summary>
        /// Lista de transformaciones del modelo
        /// </summary>
        protected Matrix[] m_BoneTransforms;
        /// <summary>
        /// Controlador de animación
        /// </summary>
        protected AnimationController m_AnimationController = new AnimationController();
	}
}
