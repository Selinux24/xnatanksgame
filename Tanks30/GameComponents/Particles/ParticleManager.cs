using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Physics;

namespace GameComponents.Particles
{
    /// <summary>
    /// Gestor de partículas
    /// </summary>
    public class ParticleManager : GameComponent
    {
        /// <summary>
        /// Explosión
        /// </summary>
        private ExplosionParticleSystem m_Explosion = null;
        /// <summary>
        /// Explosión con humo
        /// </summary>
        private ExplosionSmokeParticleSystem m_ExplosionSmoke = null;
        /// <summary>
        /// Fuego
        /// </summary>
        private FireParticleSystem m_Fire = null;
        /// <summary>
        /// Humo
        /// </summary>
        private SmokePlumeParticleSystem m_SmokePlume = null;
        /// <summary>
        /// Traza de proyectil
        /// </summary>
        private ProjectileTrailParticleSystem m_ProjectileTrail = null;
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
        /// Inicialización
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            this.m_Explosion = new ExplosionParticleSystem(this.Game);
            this.m_ExplosionSmoke = new ExplosionSmokeParticleSystem(this.Game);
            this.m_Fire = new FireParticleSystem(this.Game);
            this.m_SmokePlume = new SmokePlumeParticleSystem(this.Game);
            this.m_ProjectileTrail = new ProjectileTrailParticleSystem(this.Game);

            this.m_Explosion.DrawOrder = 0;
            this.m_ExplosionSmoke.DrawOrder = 0;
            this.m_Fire.DrawOrder = 0;
            this.m_SmokePlume.DrawOrder = 0;
            this.m_ProjectileTrail.DrawOrder = 0;

            this.Game.Components.Add(this.m_Explosion);
            this.Game.Components.Add(this.m_ExplosionSmoke);
            this.Game.Components.Add(this.m_Fire);
            this.Game.Components.Add(this.m_SmokePlume);
            this.Game.Components.Add(this.m_ProjectileTrail);
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
                        generator.ParticleSystem.AddParticle(generator.Emitter.GetPosition(), Vector3.Up);
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
        /// Añade una partícula de explosión
        /// </summary>
        /// <param name="position">Posición</param>
        /// <param name="velocity">Velocidad</param>
        public void AddExplosionParticle(Vector3 position, Vector3 velocity)
        {
            this.m_Explosion.AddParticle(position, velocity);
        }
        /// <summary>
        /// Añade una partícula de explosión con humo
        /// </summary>
        /// <param name="position">Posición</param>
        /// <param name="velocity">Velocidad</param>
        public void AddExplosionSmokeParticle(Vector3 position, Vector3 velocity)
        {
            this.m_ExplosionSmoke.AddParticle(position, velocity);
        }
        /// <summary>
        /// Añade una partícula de fuego
        /// </summary>
        /// <param name="position">Posición</param>
        /// <param name="velocity">Velocidad</param>
        public void AddFireParticle(Vector3 position, Vector3 velocity)
        {
            this.m_Fire.AddParticle(position, velocity);
        }
        /// <summary>
        /// Añade una partícula de humo
        /// </summary>
        /// <param name="position">Posición</param>
        /// <param name="velocity">Velocidad</param>
        public void AddSmokePlumeParticle(Vector3 position, Vector3 velocity)
        {
            this.m_SmokePlume.AddParticle(position, velocity);
        }
        /// <summary>
        /// Añade una partícula de traza de proyectil
        /// </summary>
        /// <param name="position">Posición</param>
        /// <param name="velocity">Velocidad</param>
        public void AddProjectileTrailParticle(Vector3 position, Vector3 velocity)
        {
            this.m_ProjectileTrail.AddParticle(position, velocity);
        }

        /// <summary>
        /// Añade un generador estático de fuego
        /// </summary>
        /// <param name="obj">Objeto que genera el fuego</param>
        /// <param name="duration">Duración</param>
        public void AddFireParticleGenerator(IPhysicObject obj, float duration)
        {
            this.m_ParticleGenerators.Add(new ParticleGenerator() { Emitter = obj, ParticleSystem = this.m_Fire, Duration = duration });
        }
        /// <summary>
        /// Añade un generador estático de humo
        /// </summary>
        /// <param name="obj">Objeto que genera el humo</param>
        /// <param name="duration">Duración</param>
        public void AddSmokePlumeParticleGenerator(IPhysicObject obj, float duration)
        {
            this.m_ParticleGenerators.Add(new ParticleGenerator() { Emitter = obj, ParticleSystem = this.m_SmokePlume, Duration = duration });
        }
    }
}
