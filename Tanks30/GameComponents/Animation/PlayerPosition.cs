using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameComponents.Animation
{
    using Common.Helpers;

    /// <summary>
    /// Define la posición de un jugador
    /// </summary>
    public class PlayerPosition
    {
        /// <summary>
        /// Nombre de la posición
        /// </summary>
        public readonly string Name = null;
        /// <summary>
        /// Indice del bone que representa la posición del jugador
        /// </summary>
        public readonly int Index = -1;
        /// <summary>
        /// Nombre del bone que representa la posición del jugador
        /// </summary>
        public readonly string BoneName = null;

        /// <summary>
        /// Bone que representa la posición del jugador
        /// </summary>
        private ModelBone m_Bone = null;
        /// <summary>
        /// Posición adicional a la posición del jugador
        /// </summary>
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
                Matrix.CreateTranslation(this.m_AditionalTranslation) *
                controller.GetAbsoluteTransform(this.m_Bone) *
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

        /// <summary>
        /// Crea la lista de posiciones de jugador usable por los componentes
        /// </summary>
        /// <param name="model">Modelo que contiene las posiciones</param>
        /// <returns>Devuelve una lista de posiciones de jugador</returns>
        public static PlayerPosition[] CreatePlayerPositionList(Model model, PlayerPositionInfo[] playerPositions)
        {
            List<PlayerPosition> m_PlayerControlList = new List<PlayerPosition>();

            if (playerPositions != null && playerPositions.Length > 0)
            {
                foreach (PlayerPositionInfo positionInfo in playerPositions)
                {
                    PlayerPosition position = new PlayerPosition(positionInfo.Name, model.Bones[positionInfo.BoneName], positionInfo.Translation);

                    m_PlayerControlList.Add(position);
                }
            }

            return m_PlayerControlList.ToArray();
        }
    }
}
