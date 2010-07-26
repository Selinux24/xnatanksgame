using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameComponents.Particles
{
    public class FireParticleSystem : ParticleSystem
    {
        public FireParticleSystem(Game game)
            : base(game)
        { }

        protected override void InitializeSettings(ParticleSettings settings)
        {
            settings.TextureName = "fire";

            settings.MaxParticles = 800;

            settings.Duration = TimeSpan.FromSeconds(2);

            settings.DurationRandomness = 1;

            settings.MinHorizontalVelocity = 0;
            settings.MaxHorizontalVelocity = 15;

            settings.MinVerticalVelocity = -10;
            settings.MaxVerticalVelocity = 10;

            settings.Gravity = new Vector3(0, 15, 0);

            settings.MinColor = new Color(255, 255, 255, 10);
            settings.MaxColor = new Color(255, 255, 255, 40);

            settings.MinStartSize = 5;
            settings.MaxStartSize = 10;

            settings.MinEndSize = 10;
            settings.MaxEndSize = 40;

            settings.SourceBlend = Blend.SourceAlpha;
            settings.DestinationBlend = Blend.One;
        }
    }
}
