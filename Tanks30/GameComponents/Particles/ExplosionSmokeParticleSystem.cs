using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameComponents.Particles
{
    /// <summary> 
    /// Explosión con humo
    /// </summary> 
    public class ExplosionSmokeParticleSystem : ParticleSystem
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game">Juego</param>
        public ExplosionSmokeParticleSystem(Game game)
            : base(game)
        {

        }

        /// <summary> 
        /// Inicializar el sistema de partículas
        /// </summary> 
        protected override void InitializeSettings(ParticleSettings settings)
        {
            settings.TextureName = "Content/Particles/smoke";

            settings.MaxParticles = 1000;

            settings.Duration = TimeSpan.FromSeconds(4);

            settings.MinHorizontalVelocity = 0;
            settings.MaxHorizontalVelocity = 50;

            settings.MinVerticalVelocity = -10;
            settings.MaxVerticalVelocity = 50;

            settings.Gravity = new Vector3(0, -20, 0);

            settings.EndVelocity = 0;

            settings.MinColor = Color.LightGray;
            settings.MaxColor = Color.White;

            settings.MinRotateSpeed = -2;
            settings.MaxRotateSpeed = 2;

            settings.MinStartSize = 10;
            settings.MaxStartSize = 10;

            settings.MinEndSize = 100;
            settings.MaxEndSize = 200;
        }
    }
}
