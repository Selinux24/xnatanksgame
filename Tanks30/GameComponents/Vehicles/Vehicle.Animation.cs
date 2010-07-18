﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GameComponents.Vehicles
{
    using GameComponents.Animation;

    /// <summary>
    /// Información de animación de un vehículo
    /// </summary>
    public partial class Vehicle
    {
        /// <summary>
        /// Lista de transformaciones del modelo
        /// </summary>
        protected Matrix[] m_BoneTransforms;
        /// <summary>
        /// Controlador de animación
        /// </summary>
        protected AnimationController m_AnimationController = new AnimationController();
        /// <summary>
        /// Lista de posibles posiciones de jugador
        /// </summary>
        protected List<PlayerPosition> m_PlayerControlList = new List<PlayerPosition>();
        /// <summary>
        /// Posición actual del jugador en el modelo
        /// </summary>
        protected PlayerPosition m_CurrentPlayerControl = null;
        
        /// <summary>
        /// Obtiene un controlador de animación específico por nombre
        /// </summary>
        /// <param name="name">Nombre del controlador de animación</param>
        /// <returns>Devuelve el controlador de animación</returns>
        public Animation GetAnimation(string name)
        {
            foreach (Animation animation in m_AnimationController.AnimationList)
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

        /// <summary>
        /// Establecer el siguiente controlador de jugador
        /// </summary>
        public void SetNextPlayerPosition()
        {
            int index = m_PlayerControlList.IndexOf(m_CurrentPlayerControl);
            if (index == m_PlayerControlList.Count - 1)
            {
                m_CurrentPlayerControl = m_PlayerControlList[0];
            }
            else
            {
                m_CurrentPlayerControl = m_PlayerControlList[index + 1];
            }
        }
        /// <summary>
        /// Establecer el anterior controlador de jugador
        /// </summary>
        public void SetPreviousPlayerControl()
        {
            int index = m_PlayerControlList.IndexOf(m_CurrentPlayerControl);
            if (index == 0)
            {
                m_CurrentPlayerControl = m_PlayerControlList[m_PlayerControlList.Count - 1];
            }
            else
            {
                m_CurrentPlayerControl = m_PlayerControlList[index - 1];
            }
        }
    }
}
