using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GameComponents.Particles
{
    using Physics;

    /// <summary>
    /// Gestor de partículas
    /// </summary>
    public class ParticleManager : GameComponent
    {
        /// <summary>
        /// Diccionario de sistemas de partículas por tipo
        /// </summary>
        private Dictionary<ParticleSystemTypes, ParticleSystem> m_ParticleDictionary = new Dictionary<ParticleSystemTypes, ParticleSystem>();
        /// <summary>
        /// Lista de generadores de partículas estáticos
        /// </summary>
        private List<ParticleGenerator> m_ParticleGenerators = new List<ParticleGenerator>();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game">Juego</param>
        public ParticleManager(Game game)
            : base(game)
        {

        }

        /// <summary>
        /// Actualiza el componente
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (this.m_ParticleGenerators != null && this.m_ParticleGenerators.Count > 0)
            {
                List<ParticleGenerator> toDelete = new List<ParticleGenerator>();

                foreach (ParticleGenerator generator in this.m_ParticleGenerators)
                {
                    if (generator.Emitter != null)
                    {
                        this.AddParticle(generator.ParticleType, generator.Emitter.GetPosition(), Vector3.Up);
                    }

                    generator.Duration -= elapsed;

                    if (generator.Duration <= 0)
                    {
                        toDelete.Add(generator);
                    }
                }

                if (toDelete.Count > 0)
                {
                    foreach (ParticleGenerator generator in toDelete)
                    {
                        this.m_ParticleGenerators.Remove(generator);
                    }
                }
            }
        }

        /// <summary>
        /// Obtiene la cantidad de partículas en uso del tipo especificado
        /// </summary>
        /// <param name="particleType">Tipo de partículas</param>
        /// <returns>Devuelve el número de partículas en uso</returns>
        public int GetUsedParticlesCount(ParticleSystemTypes particleType)
        {
            if (particleType != ParticleSystemTypes.None)
            {
                if (this.m_ParticleDictionary.ContainsKey(particleType))
                {
                    return this.m_ParticleDictionary[particleType].UsedParticles;
                }
            }

            return 0;
        }
        /// <summary>
        /// Añade una partícula del tipo especificado
        /// </summary>
        /// <param name="particleType">Tipo de partícula</param>
        /// <param name="position">Posición</param>
        /// <param name="velocity">Velocidad</param>
        public void AddParticle(ParticleSystemTypes particleType, Vector3 position, Vector3 velocity)
        {
            if (particleType != ParticleSystemTypes.None)
            {
                if (!this.m_ParticleDictionary.ContainsKey(particleType))
                {
                    ParticleSystem particleSystem = new ParticleSystem(this.Game, particleType);
                    particleSystem.DrawOrder = 0;
                    this.Game.Components.Add(particleSystem);

                    this.m_ParticleDictionary.Add(particleType, particleSystem);
                }

                this.m_ParticleDictionary[particleType].AddParticle(position, velocity);
            }
        }
        /// <summary>
        /// Añade un generador estático de fuego
        /// </summary>
        /// <param name="particleType">Tipo de partícula</param>
        /// <param name="obj">Objeto que genera el fuego</param>
        /// <param name="duration">Duración</param>
        public void AddParticleGenerator(ParticleSystemTypes particleType, IPhysicObject obj, float duration)
        {
            if (particleType != ParticleSystemTypes.None)
            {
                this.m_ParticleGenerators.Add(new ParticleGenerator() { Emitter = obj, ParticleType = particleType, Duration = duration });
            }
        }
    }
}
