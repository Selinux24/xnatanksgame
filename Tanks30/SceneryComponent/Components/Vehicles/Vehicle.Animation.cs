using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GameComponents.Vehicles
{
    using GameComponents.Vehicles.Animation;

    public partial class Vehicle
    {
        // Lista de transformaciones del modelo
        protected Matrix[] m_BoneTransforms;
        // Controlador de animación
        protected AnimationController m_AnimationController = new AnimationController();
        // Lista de posibles posiciones de jugador
        protected List<PlayerPosition> m_PlayerControlList = new List<PlayerPosition>();
        // Posición actual del jugador en el modelo
        protected PlayerPosition m_CurrentPlayerControl = null;
        
        /// <summary>
        /// Obtiene un controlador de animación específico por nombre
        /// </summary>
        /// <param name="name">Nombre del controlador de animación</param>
        /// <returns>Devuelve el controlador de animación</returns>
        public AnimationBase GetAnimation(string name)
        {
            foreach (AnimationBase animation in m_AnimationController.AnimationList)
            {
                if (string.Compare(animation.Name, name, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return animation;
                }
            }

            return null;
        }
        /// <summary>
        /// Obtiene una posición de jugador por nombre
        /// </summary>
        /// <param name="name">Nombre de la posición del jugador</param>
        /// <returns>Devuelve la posición del jugador</returns>
        public PlayerPosition GetPlayerPosition(string name)
        {
            foreach (PlayerPosition playerPosition in m_PlayerControlList)
            {
                if (string.Compare(playerPosition.Name, name, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return playerPosition;
                }
            }

            return null;
        }
    }
}
