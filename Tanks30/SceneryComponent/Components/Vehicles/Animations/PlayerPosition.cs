using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameComponents.Vehicles.Animations
{
    /// <summary>
    /// Define la posición de un jugador
    /// </summary>
    public class PlayerPosition
    {
        // Nombre de la posición
        public readonly string Name = null;
        // Indice del bone que representa la posición del jugador
        public readonly int Index = -1;
        // Nombre del bone que representa la posición del jugador
        public readonly string BoneName = null;

        // Bone que representa la posición del jugador
        private ModelBone m_Bone = null;
        // Posición adicional a la posición del jugador
        private Vector3 m_AditionalTranslation = Vector3.Zero;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Nombre</param>
        /// <param name="bone">Bone que representa la posición del jugador</param>
        /// <param name="translation">Posición adicional a la posición marcada por el bone</param>
        public PlayerPosition(string name, ModelBone bone, Vector3 translation)
        {
            this.Name = name;
            this.Index = bone.Index;
            this.BoneName = bone.Name;

            m_Bone = bone;
            m_AditionalTranslation = translation;
        }

        /// <summary>
        /// Obtiene la transformación absoluta de la posición
        /// </summary>
        /// <param name="controller">Controlador de animación</param>
        /// <param name="modelTransform">Transformación</param>
        /// <returns>Devuelve la transformación absoluta del modelo</returns>
        public Matrix GetModelMatrix(AnimationController controller, Matrix modelTransform)
        {
            // Calcular la transformación global compuesta por la transformación adicional, la transformación del bone y la transformación del modelo
            Matrix transform =
                Matrix.CreateTranslation(m_AditionalTranslation) *
                controller.GetAbsoluteTransform(m_Bone) *
                modelTransform;

            return transform;
        }
        /// <summary>
        /// Obtiene la matriz de vista desde la posición del jugador
        /// </summary>
        /// <param name="controller">Controlador de animación</param>
        /// <param name="modelTransform">Matriz de transformación del modelo</param>
        /// <returns>Devuelve la matriz de vista desde la posición del jugador</returns>
        public Matrix GetViewMatrix(AnimationController controller, Matrix modelTransform)
        {
            // Obtener la transformación del modelo
            Matrix transform = GetModelMatrix(controller, modelTransform);

            // Obtener los vectores para construir la matriz de vista
            Vector3 position = transform.Translation;
            Vector3 forward = transform.Forward;
            Vector3 up = transform.Up;

            // Construir la matriz de vista usando la posición, el eje de vista y la inclinación final
            return Matrix.CreateLookAt(
                position,
                position + forward,
                up);
        }
    }
}
