using Physics;

namespace GameComponents.Particles
{
    /// <summary>
    /// Generador
    /// </summary>
    public class ParticleGenerator
    {
        /// <summary>
        /// Tipo de partícula
        /// </summary>
        public ParticleSystemTypes ParticleType;
        /// <summary>
        /// Duración del generador
        /// </summary>
        public float Duration;
        /// <summary>
        /// Objeto emisor
        /// </summary>
        public IPhysicObject Emitter;
    }
}
