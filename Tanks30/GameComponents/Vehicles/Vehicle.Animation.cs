using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GameComponents.Vehicles
{
    using GameComponents.Animation;
using GameComponents.Particles;

    public delegate void PlayerControlChangedHandler(PlayerPosition position);

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
        /// Lista de emisores de partículas
        /// </summary>
        protected List<ParticleEmitter> m_ParticleEmitterList = new List<ParticleEmitter>();

        /// <summary>
        /// Evento que se produce cuando se cambia la posición del jugador
        /// </summary>
        public event PlayerControlChangedHandler OnPlayerControlChanged;
        /// <summary>
        /// Disparador del evento de posición de jugador cambiada
        /// </summary>
        /// <param name="position">Posición</param>
        protected virtual void PlayerControlChanged(PlayerPosition position)
        {
            if (OnPlayerControlChanged != null)
            {
                OnPlayerControlChanged(position);
            }
        }
   
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
        /// Obtiene un emisor de partículas específico por nombre
        /// </summary>
        /// <param name="name">Nombre del emisor de partículas</param>
        /// <returns>Devuelve el emisor de partículas</returns>
        public ParticleEmitter GetParticleEmitter(string name)
        {
            foreach (ParticleEmitter emitter in m_ParticleEmitterList)
            {
                if (string.Compare(emitter.Name, name, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return emitter;
                }
            }

            return null;
        }
        /// <summary>
        /// Obtiene una posición de jugador por nombre
        /// </summary>
        /// <param name="name">Nombre de la posición del jugador</param>
        /// <returns>Devuelve la posición del jugador</returns>
        public PlayerPosition GetPlayerControl(string name)
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
        /// Establece la posición del jugador
        /// </summary>
        /// <param name="position">Nueva posición</param>
        public void SetPlaterControl(PlayerPosition position)
        {
            m_CurrentPlayerControl = position;

            this.PlayerControlChanged(position);
        }
        /// <summary>
        /// Establecer el siguiente controlador de jugador
        /// </summary>
        public void SetNextPlayerControl()
        {
            int index = this.m_PlayerControlList.IndexOf(this.m_CurrentPlayerControl);
            if (index == this.m_PlayerControlList.Count - 1)
            {
                this.SetPlaterControl(this.m_PlayerControlList[0]);
            }
            else
            {
                this.SetPlaterControl(this.m_PlayerControlList[index + 1]);
            }
        }
        /// <summary>
        /// Establecer el anterior controlador de jugador
        /// </summary>
        public void SetPreviousPlayerControl()
        {
            int index = m_PlayerControlList.IndexOf(this.m_CurrentPlayerControl);
            if (index == 0)
            {
                this.SetPlaterControl(this.m_PlayerControlList[this.m_PlayerControlList.Count - 1]);
            }
            else
            {
                this.SetPlaterControl(this.m_PlayerControlList[index - 1]);
            }
        }

        /// <summary>
        /// Actualiza las partículas activas en el gestor de partículas
        /// </summary>
        /// <param name="particleManager">Gestor de partículas</param>
        protected void UpdateParticles(ParticleManager particleManager)
        {
            foreach (ParticleEmitter emitter in this.m_ParticleEmitterList)
            {
                if (emitter.Active)
                {
                    Matrix mtr = emitter.GetModelMatrix(this.m_AnimationController, this.CurrentTransform);

                    particleManager.AddParticle(emitter.ParticleType, mtr.Translation, mtr.Backward);

                    if (emitter.UniqueParticle)
                    {
                        emitter.Active = false;
                    }
                }
            }
        }
    }
}
