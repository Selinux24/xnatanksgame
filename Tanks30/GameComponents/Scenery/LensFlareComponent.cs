using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameComponents.Scenery
{
    using Common;

    public class LensFlareComponent : DrawableGameComponent
    {
        /// <summary>
        /// Gestor de contenidos
        /// </summary>
        protected ContentManager Content;

        public float GlowSize = 400;
        public float QuerySize = 100;
        public Vector3 LightDirection = SceneryEnvironment.Ambient.LightDirection;

        private SpriteBatch m_SpriteBatch;
        private Texture2D m_GlowSprite;
        private BasicEffect m_BasicEffect;
        private VertexDeclaration m_VertexDeclaration;
        private VertexPositionColor[] m_QueryVertices;

        private OcclusionQuery m_OcclusionQuery;
        private bool m_OcclusionQueryActive;
        private float m_OcclusionAlpha;
        private Flare[] m_Flares = Flare.Default;

        private void UpdateOcclusion(Vector2 lightPosition)
        {
            // Give up if the current graphics card does not support occlusion queries.
            if (!this.m_OcclusionQuery.IsSupported)
            {
                return;
            }

            if (this.m_OcclusionQueryActive)
            {
                // If the previous query has not yet completed, wait until it does.
                if (!this.m_OcclusionQuery.IsComplete)
                {
                    return;
                }

                // Use the occlusion query pixel count to work out what percentage of the sun is visible.
                float queryArea = this.QuerySize * this.QuerySize;

                this.m_OcclusionAlpha = Math.Min(this.m_OcclusionQuery.PixelCount / queryArea, 1);
            }

            // Set renderstates for drawing the occlusion query geometry. We want depth
            // tests enabled, but depth writes disabled, and we set ColorWriteChannels
            // to None to prevent this query polygon actually showing up on the screen.
            RenderState renderState = this.GraphicsDevice.RenderState;

            renderState.DepthBufferEnable = true;
            renderState.DepthBufferWriteEnable = false;
            renderState.AlphaTestEnable = false;
            renderState.ColorWriteChannels = ColorWriteChannels.None;

            // Set up our BasicEffect to center on the current 2D light position.
            Viewport viewport = this.GraphicsDevice.Viewport;

            this.m_BasicEffect.World = Matrix.CreateTranslation(lightPosition.X, lightPosition.Y, 0);

            this.m_BasicEffect.Projection = Matrix.CreateOrthographicOffCenter(
                0,
                viewport.Width,
                viewport.Height,
                0,
                0,
                1);

            this.m_BasicEffect.Begin();
            this.m_BasicEffect.CurrentTechnique.Passes[0].Begin();

            this.GraphicsDevice.VertexDeclaration = this.m_VertexDeclaration;

            // Issue the occlusion query.
            this.m_OcclusionQuery.Begin();

            this.GraphicsDevice.DrawUserPrimitives(
                PrimitiveType.TriangleFan,
                this.m_QueryVertices,
                0,
                2);

            this.m_OcclusionQuery.End();

            this.m_BasicEffect.CurrentTechnique.Passes[0].End();
            this.m_BasicEffect.End();

            renderState.ColorWriteChannels = ColorWriteChannels.All;
            renderState.DepthBufferWriteEnable = true;

            this.m_OcclusionQueryActive = true;
        }

        private void DrawGlow(Vector2 lightPosition)
        {
            Vector4 color = new Vector4(1, 1, 1, this.m_OcclusionAlpha);
            Vector2 origin = new Vector2(this.m_GlowSprite.Width, this.m_GlowSprite.Height) / 2;
            float scale = GlowSize * 2 / this.m_GlowSprite.Width;

            this.m_SpriteBatch.Begin(SpriteBlendMode.AlphaBlend);

            this.m_SpriteBatch.Draw(
                this.m_GlowSprite,
                lightPosition,
                null,
                new Color(color),
                0,
                origin,
                scale,
                SpriteEffects.None,
                0);

            this.m_SpriteBatch.End();
        }

        private void DrawFlares(Vector2 lightPosition)
        {
            Viewport viewport = this.GraphicsDevice.Viewport;

            // Lensflare sprites are positioned at intervals along a line that
            // runs from the 2D light position toward the center of the screen.
            Vector2 screenCenter = new Vector2(viewport.Width, viewport.Height) / 2;

            Vector2 flareVector = screenCenter - lightPosition;

            // Draw the flare sprites using additive blending.
            this.m_SpriteBatch.Begin(SpriteBlendMode.Additive);

            foreach (Flare flare in m_Flares)
            {
                // Compute the position of this flare sprite.
                Vector2 flarePosition = lightPosition + flareVector * flare.Position;

                // Set the flare alpha based on the previous occlusion query result.
                Vector4 flareColor = flare.Color.ToVector4();

                flareColor.W *= this.m_OcclusionAlpha;

                // Center the sprite texture.
                Vector2 flareOrigin = new Vector2(flare.Texture.Width, flare.Texture.Height) / 2;

                // Draw the flare.
                this.m_SpriteBatch.Draw(
                    flare.Texture,
                    flarePosition,
                    null,
                    new Color(flareColor),
                    1,
                    flareOrigin,
                    flare.Scale,
                    SpriteEffects.None,
                    0);
            }

            this.m_SpriteBatch.End();
        }

        private void RestoreRenderStates()
        {
            RenderState renderState = this.GraphicsDevice.RenderState;

            renderState.DepthBufferEnable = true;
            renderState.AlphaTestEnable = false;
            renderState.AlphaBlendEnable = false;

            SamplerState samplerState = this.GraphicsDevice.SamplerStates[0];

            samplerState.AddressU = TextureAddressMode.Wrap;
            samplerState.AddressV = TextureAddressMode.Wrap;
        }

        public LensFlareComponent(Game game)
            : base(game)
        {
            this.Content = game.Content;
        }

        protected override void LoadContent()
        {
            this.m_SpriteBatch = new SpriteBatch(this.GraphicsDevice);

            this.m_GlowSprite = this.Content.Load<Texture2D>(@"Content\LensFlare\glow");
            foreach (Flare flare in m_Flares)
            {
                flare.Texture = this.Content.Load<Texture2D>(@"Content\LensFlare\" + flare.TextureName);
            }

            this.m_BasicEffect = new BasicEffect(this.GraphicsDevice, null);
            this.m_BasicEffect.View = Matrix.Identity;
            this.m_BasicEffect.VertexColorEnabled = true;

            this.m_VertexDeclaration = new VertexDeclaration(this.GraphicsDevice, VertexPositionColor.VertexElements);

            this.m_QueryVertices = new VertexPositionColor[4];
            this.m_QueryVertices[0].Position = new Vector3(-QuerySize / 2, -QuerySize / 2, -1);
            this.m_QueryVertices[1].Position = new Vector3(QuerySize / 2, -QuerySize / 2, -1);
            this.m_QueryVertices[2].Position = new Vector3(QuerySize / 2, QuerySize / 2, -1);
            this.m_QueryVertices[3].Position = new Vector3(-QuerySize / 2, QuerySize / 2, -1);

            this.m_OcclusionQuery = new OcclusionQuery(this.GraphicsDevice);
        }

        public override void Draw(GameTime gameTime)
        {
            // The sun is infinitely distant, so it should not be affected by the
            // position of the camera. Floating point math doesn't support infinitely
            // distant vectors, but we can get the same result by making a copy of our
            // view matrix, then resetting the view translation to zero. Pretending the
            // camera has not moved position gives the same result as if the camera
            // was moving, but the light was infinitely far away. If our flares came
            // from a local object rather than the sun, we would use the original view
            // matrix here.
            Matrix infiniteView = GlobalMatrices.gViewMatrix;

            infiniteView.Translation = Vector3.Zero;

            // Project the light position into 2D screen space.
            Viewport viewport = this.GraphicsDevice.Viewport;

            Vector3 projectedPosition = viewport.Project(
                -this.LightDirection,
                GlobalMatrices.gGlobalProjectionMatrix,
                infiniteView,
                Matrix.Identity);

            // Don't draw any flares if the light is behind the camera.
            if ((projectedPosition.Z < 0) || (projectedPosition.Z > 1))
            {
                return;
            }

            Vector2 lightPosition = new Vector2(projectedPosition.X, projectedPosition.Y);

            // Check whether the light is hidden behind the scenery.
            this.UpdateOcclusion(lightPosition);

            // If it is visible, draw the flare effect.
            if (this.m_OcclusionAlpha > 0)
            {
                this.DrawGlow(lightPosition);

                this.DrawFlares(lightPosition);
            }

            this.RestoreRenderStates();
        }
    }
}
