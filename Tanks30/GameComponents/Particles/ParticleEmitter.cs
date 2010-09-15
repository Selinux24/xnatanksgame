using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameComponents.Particles
{
    using Common;
    using Common.Helpers;
    using GameComponents.Animation;

    /// <summary>
    /// Emisor de partículas
    /// </summary>
    public class ParticleEmitter
    {
        /// <summary>
        /// Nombre del emisor
        /// </summary>
        public string Name;
        /// <summary>
        /// Tipo de partícula
        /// </summary>
        public ParticleSystemTypes ParticleType;
        /// <summary>
        /// Nodo de emisión
        /// </summary>
        public ModelBone Bone;
        /// <summary>
        /// Indica si el emisor está activo
        /// </summary>
        public bool Active = false;
        /// <summary>
        /// Indica si se debe lanzar una sola partícula
        /// </summary>
        public bool UniqueParticle = false;

        /// <summary>
        /// Obtiene una lista de emisores de partículas a partir de la información de emisor
        /// </summary>
        /// <param name="model">Modelo</param>
        /// <param name="particleEmitterInfo">Información de emisor</param>
        /// <returns>Devuelve la lista de emisores de partículas</returns>
        public static ParticleEmitter[] CreateParticleEmitterList(Model model, ParticleEmitterInfo[] particleEmitterInfo)
        {
            List<ParticleEmitter> list = new List<ParticleEmitter>();

            if (particleEmitterInfo != null && particleEmitterInfo.Length > 0)
            {
                foreach (ParticleEmitterInfo info in particleEmitterInfo)
                {
                    ParticleEmitter newEmitter = new ParticleEmitter()
                    {
                        Name = info.Name,
                        ParticleType = info.ParticleType,
                        Bone = (string.IsNullOrEmpty(info.BoneName)) ? null : model.Bones[info.BoneName],
                        UniqueParticle = info.UniqueParticle,
                    };

                    list.Add(newEmitter);
                }
            }

            return list.ToArray();
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
            if (this.Bone != null)
            {
                return controller.GetAbsoluteTransform(this.Bone) * modelTransform;
            }
            else
            {
                return modelTransform;
            }
        }
    }
}
