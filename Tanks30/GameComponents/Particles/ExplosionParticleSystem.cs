using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameComponents.Particles
{
    /// <summary> 
    /// Explosión
    /// </summary> 
    public class ExplosionParticleSystem : ParticleSystem
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game">Juego</param>
        public ExplosionParticleSystem(Game game)
            : base(game)
        {

        }

        /// <summary> 
        /// Inicializar el sistema de partículas
        /// </summary> 
        protected override void InitializeSettings(ParticleSettings settings)
        {
            settings.TextureName = "Content/Particles/explosion";

            settings.MaxParticles = 1000;

            settings.Duration = TimeSpan.FromSeconds(2);

            settings.DurationRandomness = 1;

            settings.MinHorizontalVelocity = 20;
            settings.MaxHorizontalVelocity = 30;

            settings.MinVerticalVelocity = -20;
            settings.MaxVerticalVelocity = 20;

            settings.EndVelocity = 0;

            settings.MinColor = Color.DarkGray;
            settings.MaxColor = Color.Gray;

            settings.MinRotateSpeed = -1;
            settings.MaxRotateSpeed = 1;

            settings.MinStartSize = 10;
            settings.MaxStartSize = 10;

            settings.MinEndSize = 100;
            settings.MaxEndSize = 200;

            settings.SourceBlend = Blend.SourceAlpha;
            settings.DestinationBlend = Blend.One;
        }
    }
}
