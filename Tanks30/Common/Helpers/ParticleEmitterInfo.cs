using System;

namespace Common.Helpers
{
    /// <summary>
    /// Información de emisión de partículas
    /// </summary>
    [Serializable]
    public class ParticleEmitterInfo
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
        public string BoneName;
        /// <summary>
        /// Indica si sólo se lanzará una partícula
        /// </summary>
        public bool UniqueParticle;
    }
}
