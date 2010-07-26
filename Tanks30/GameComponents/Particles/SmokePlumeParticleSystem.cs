using System;
using Microsoft.Xna.Framework;

namespace GameComponents.Particles
{
    /// <summary>
    /// Humo
    /// </summary>
    public class SmokePlumeParticleSystem : ParticleSystem
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game">Juego</param>
        public SmokePlumeParticleSystem(Game game)
            : base(game)
        {
        
        }

        /// <summary>
        /// Inicializar
        /// </summary>
        /// <param name="settings">Parámetros</param>
        protected override void InitializeSettings(ParticleSettings settings)
        {
            settings.TextureName = "Content/Particles/smoke";

            settings.MaxParticles = 600;

            settings.Duration = TimeSpan.FromSeconds(10);

            settings.MinHorizontalVelocity = 0;
            settings.MaxHorizontalVelocity = 5;

            settings.MinVerticalVelocity = 10;
            settings.MaxVerticalVelocity = 20;

            settings.Gravity = new Vector3(-20, -5, 0);

            settings.EndVelocity = 0.75f;

            settings.MinRotateSpeed = -1;
            settings.MaxRotateSpeed = 1;

            settings.MinStartSize = 5;
            settings.MaxStartSize = 10;

            settings.MinEndSize = 50;
            settings.MaxEndSize = 200;
        }
    }
}


