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
        public ParticleSystem ParticleSystem;
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
