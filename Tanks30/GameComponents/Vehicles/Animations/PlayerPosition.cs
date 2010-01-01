using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameComponents.Vehicles.Animations
{
    /// <summary>
    /// Define la posici�n de un jugador
    /// </summary>
    public class PlayerPosition
    {
        // Nombre de la posici�n
        public readonly string Name = null;
        // Indice del bone que representa la posici�n del jugador
        public readonly int Index = -1;
        // Nombre del bone que representa la posici�n del jugador
        public readonly string BoneName = null;

        // Bone que representa la posici�n del jugador
        private ModelBone m_Bone = null;
        // Posici�n adicional a la posici�n del jugador
        private Vector3 m_AditionalTranslation = Vector3.Zero;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Nombre</param>
        /// <param name="bone">Bone que representa la posici�n del jugador</param>
        /// <param name="translation">Posici�n adicional a la posici�n marcada por el bone</param>
        public PlayerPosition(string name, ModelBone bone, Vector3 translation)
        {
            this.Name = name;
            this.Index = bone.Index;
            this.BoneName = bone.Name;

            m_Bone = bone;
            m_AditionalTranslation = translation;
        }

        /// <summary>
        /// Obtiene la transformaci�n absoluta de la posici�n
        /// </summary>
        /// <param name="controller">Controlador de animaci�n</param>
        /// <param name="modelTransform">Transformaci�n</param>
        /// <returns>Devuelve la transformaci�n absoluta del modelo</returns>
        public Matrix GetModelMatrix(AnimationController controller, Matrix modelTransform)
        {
            // Calcular la transformaci�n global compuesta por la transformaci�n adicional, la transformaci�n del bone y la transformaci�n del modelo
            Matrix transform =
                Matrix.CreateTranslation(m_AditionalTranslation) *
                controller.GetAbsoluteTransform(m_Bone) *
                modelTransform;

            return transform;
        }
        /// <summary>
        /// Obtiene la matriz de vista desde la posici�n del jugador
        /// </summary>
        /// <param name="controller">Controlador de animaci�n</param>
        /// <param name="modelTransform">Matriz de transformaci�n del modelo</param>
        /// <returns>Devuelve la matriz de vista desde la posici�n del jugador</returns>
        public Matrix GetViewMatrix(AnimationController controller, Matrix modelTransform)
        {
            // Obtener la transformaci�n del modelo
            Matrix transform = GetModelMatrix(controller, modelTransform);

            // Obtener los vectores para construir la matriz de vista
            Vector3 position = transform.Translation;
            Vector3 forward = transform.Forward;
            Vector3 up = transform.Up;

            // Construir la matriz de vista usando la posici�n, el eje de vista y la inclinaci�n final
            return Matrix.CreateLookAt(
                position,
                position + forward,
                up);
        }
    }
}
