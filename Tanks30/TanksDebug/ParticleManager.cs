using System;
using GameComponents.Particles;
using Microsoft.Xna.Framework;

namespace TanksDebug
{
    public class ParticleManager : GameComponent
    {
        private ExplosionParticleSystem m_Explosion = null;
        private ExplosionSmokeParticleSystem m_ExplosionSmoke = null;
        private FireParticleSystem m_Fire = null;
        private SmokePlumeParticleSystem m_SmokePlume = null;
        private ProjectileTrailParticleSystem m_ProjectileTrail = null;

        public ParticleManager(Game game)
            : base(game)
        {
            this.m_Explosion = new ExplosionParticleSystem(this.Game);
            this.m_ExplosionSmoke = new ExplosionSmokeParticleSystem(this.Game);
            this.m_Fire = new FireParticleSystem(this.Game);
            this.m_SmokePlume = new SmokePlumeParticleSystem(this.Game);
            this.m_ProjectileTrail = new ProjectileTrailParticleSystem(this.Game);

            this.Game.Components.Add(this.m_Explosion);
            this.Game.Components.Add(this.m_ExplosionSmoke);
            this.Game.Components.Add(this.m_Fire);
            this.Game.Components.Add(this.m_SmokePlume);
            this.Game.Components.Add(this.m_ProjectileTrail);
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public void AddExplosionParticle(Vector3 position, Vector3 velocity)
        {
            this.m_Explosion.AddParticle(position, velocity);
        }

        public void AddExplosionSmokeParticle(Vector3 position, Vector3 velocity)
        {
            this.m_ExplosionSmoke.AddParticle(position, velocity);
        }

        public void AddFireParticle(Vector3 position, Vector3 velocity)
        {
            this.m_Fire.AddParticle(position, velocity);
        }

        public void AddSmokePlumeParticle(Vector3 position, Vector3 velocity)
        {
            this.m_SmokePlume.AddParticle(position, velocity);
        }

        public void AddProjectileTrailParticle(Vector3 position, Vector3 velocity)
        {
            this.m_ProjectileTrail.AddParticle(position, velocity);
        }
    }
}
