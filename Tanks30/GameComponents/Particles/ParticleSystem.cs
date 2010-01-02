#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using GameComponents.MathComponents;
#endregion

namespace GameComponents.Components.Particles
{
    /// <summary>
    /// The main component in charge of displaying particles.
    /// </summary>
    public abstract class ParticleSystem : DrawableGameComponent
    {
        /// <summary>
        /// Configuración de la apariencia de las partículas del sistema de partículas
        /// </summary>
        protected ParticleSettings Settings = new ParticleSettings();
        /// <summary>
        /// Gestor de contenidos
        /// </summary>
        protected ContentManager Content;
        /// <summary>
        /// Efecto para dibujar las partículas
        /// </summary>
        private Effect m_ParticleEffect;
        /// <summary>
        /// Parámetros de vista del efecto
        /// </summary>
        private EffectParameter m_EffectViewParameter;
        /// <summary>
        /// Parámetro de projección del efecto
        /// </summary>
        private EffectParameter m_EffectProjectionParameter;
        /// <summary>
        /// Parámetro de Altura del Viewport del efecto
        /// </summary>
        private EffectParameter m_EffectViewportHeightParameter;
        /// <summary>
        /// Parámetro de tiempo del efecto
        /// </summary>
        private EffectParameter m_EffectTimeParameter;
        /// <summary>
        /// Cola circular de partículas
        /// </summary>
        private ParticleVertex[] m_Particles;
        /// <summary>
        /// Buffer de partículas
        /// </summary>
        private VertexBuffer m_VertexBuffer;
        /// <summary>
        /// Declaración de los vértices del buffer
        /// </summary>
        private VertexDeclaration m_VertexDeclaration;
        /// <summary>
        /// The particles array and vertex buffer are treated as a circular queue.
        /// Initially, the entire contents of the array are free, because no particles
        /// are in use. When a new particle is created, this is allocated from the
        /// beginning of the array. If more than one particle is created, these will
        /// always be stored in a consecutive block of array elements. Because all
        /// particles last for the same amount of time, old particles will always be
        /// removed in order from the start of this active particle region, so the
        /// active and free regions will never be intermingled. Because the queue is
        /// circular, there can be times when the active particle region wraps from the
        /// end of the array back to the start. The queue uses modulo arithmetic to
        /// handle these cases. For instance with a four entry queue we could have:
        ///
        ///      0
        ///      1 - first active particle
        ///      2 
        ///      3 - first free particle
        ///
        /// In this case, particles 1 and 2 are active, while 3 and 4 are free.
        /// Using modulo arithmetic we could also have:
        ///
        ///      0
        ///      1 - first free particle
        ///      2 
        ///      3 - first active particle
        ///
        /// Here, 3 and 0 are active, while 1 and 2 are free.
        ///
        /// But wait! The full story is even more complex.
        ///
        /// When we create a new particle, we add them to our managed particles array.
        /// We also need to copy this new data into the GPU vertex buffer, but we don't
        /// want to do that straight away, because setting new data into a vertex buffer
        /// can be an expensive operation. If we are going to be adding several particles
        /// in a single frame, it is faster to initially just store them in our managed
        /// array, and then later upload them all to the GPU in one single call. So our
        /// queue also needs a region for storing new particles that have been added to
        /// the managed array but not yet uploaded to the vertex buffer.
        ///
        /// Another issue occurs when old particles are retired. The CPU and GPU run
        /// asynchronously, so the GPU will often still be busy drawing the previous
        /// frame while the CPU is working on the next frame. This can cause a
        /// synchronization problem if an old particle is retired, and then immediately
        /// overwritten by a new one, because the CPU might try to change the contents
        /// of the vertex buffer while the GPU is still busy drawing the old data from
        /// it. Normally the graphics driver will take care of this by waiting until
        /// the GPU has finished drawing inside the VertexBuffer.SetData call, but we
        /// don't want to waste time waiting around every time we try to add a new
        /// particle! To avoid this delay, we can specify the SetDataOptions.NoOverwrite
        /// flag when we write to the vertex buffer. This basically means "I promise I
        /// will never try to overwrite any data that the GPU might still be using, so
        /// you can just go ahead and update the buffer straight away". To keep this
        /// promise, we must avoid reusing vertices immediately after they are drawn.
        ///
        /// So in total, our queue contains four different regions:
        ///
        /// Vertices between firstActiveParticle and firstNewParticle are actively
        /// being drawn, and exist in both the managed particles array and the GPU
        /// vertex buffer.
        ///
        /// Vertices between firstNewParticle and firstFreeParticle are newly created,
        /// and exist only in the managed particles array. These need to be uploaded
        /// to the GPU at the start of the next draw call.
        ///
        /// Vertices between firstFreeParticle and firstRetiredParticle are free and
        /// waiting to be allocated.
        ///
        /// Vertices between firstRetiredParticle and firstActiveParticle are no longer
        /// being drawn, but were drawn recently enough that the GPU could still be
        /// using them. These need to be kept around for a few more frames before they
        /// can be reallocated.
        /// </summary>
        private int m_FirstActiveParticle, m_FirstNewParticle, m_FirstFreeParticle, m_FirstRetiredParticle;
        /// <summary>
        /// Tiempo actual en segundos
        /// </summary>
        private float m_CurrentTime;
        /// <summary>
        /// Contador de las veces que se ha llamado a Draw
        /// </summary>
        private int m_DrawCounter;

        /// <summary>
        /// Constructor.
        /// </summary>
        protected ParticleSystem(Game game)
            : base(game)
        {
            this.Content = game.Content;
        }

        /// <summary>
        /// Initializes the component.
        /// </summary>
        public override void Initialize()
        {
            InitializeSettings(Settings);

            m_Particles = new ParticleVertex[Settings.MaxParticles];

            base.Initialize();
        }
        /// <summary>
        /// Derived particle system classes should override this method
        /// and use it to initalize their tweakable settings.
        /// </summary>
        protected abstract void InitializeSettings(ParticleSettings settings);
        /// <summary>
        /// Loads graphics for the particle system.
        /// </summary>
        protected override void LoadContent()
        {
            LoadParticleEffect();

            m_VertexDeclaration = new VertexDeclaration(
                GraphicsDevice,
                ParticleVertex.VertexElements);

            // Create a dynamic vertex buffer.
            //ResourceUsage usage = ResourceUsage.Dynamic | ResourceUsage.WriteOnly | ResourceUsage.Points;
            BufferUsage usage = BufferUsage.WriteOnly | BufferUsage.Points;

            int size = ParticleVertex.SizeInBytes * m_Particles.Length;

            m_VertexBuffer = new VertexBuffer(
                GraphicsDevice,
                size,
                usage);

            // Initialize the vertex buffer contents. This is necessary in order to correctly restore any existing particles after a lost device.
            m_VertexBuffer.SetData(m_Particles);
        }
        /// <summary>
        /// Updates the particle system.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            if (gameTime == null)
            {
                throw new ArgumentNullException("gameTime");
            }

            m_CurrentTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

            RetireActiveParticles();

            FreeRetiredParticles();

            // If we let our timer go on increasing for ever, it would eventually
            // run out of floating point precision, at which point the particles
            // would render incorrectly. An easy way to prevent this is to notice
            // that the time value doesn't matter when no particles are being drawn,
            // so we can reset it back to zero any time the active queue is empty.

            if (m_FirstActiveParticle == m_FirstFreeParticle)
            {
                m_CurrentTime = 0;
            }

            if (m_FirstRetiredParticle == m_FirstActiveParticle)
            {
                m_DrawCounter = 0;
            }
        }
        /// <summary>
        /// Draws the particle system.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice device = GraphicsDevice;

            // If there are any particles waiting in the newly added queue,
            // we'd better upload them to the GPU ready for drawing.
            if (m_FirstNewParticle != m_FirstFreeParticle)
            {
                AddNewParticlesToVertexBuffer();
            }

            // If there are any active particles, draw them now!
            if (m_FirstActiveParticle != m_FirstFreeParticle)
            {
                SetParticleRenderStates(device.RenderState);

                // Set an effect parameter describing the viewport size. This is needed
                // to convert particle sizes into screen space point sprite sizes.
                m_EffectViewportHeightParameter.SetValue(device.Viewport.Height);

                // Set an effect parameter describing the current time. All the vertex
                // shader particle animation is keyed off this value.
                m_EffectTimeParameter.SetValue(m_CurrentTime);

                // Set the particle vertex buffer and vertex declaration.
                device.Vertices[0].SetSource(
                    m_VertexBuffer,
                    0,
                    ParticleVertex.SizeInBytes);

                device.VertexDeclaration = m_VertexDeclaration;

                // Activate the particle effect.
                m_ParticleEffect.Begin();

                foreach (EffectPass pass in m_ParticleEffect.CurrentTechnique.Passes)
                {
                    pass.Begin();

                    if (m_FirstActiveParticle < m_FirstFreeParticle)
                    {
                        // If the active particles are all in one consecutive range,
                        // we can draw them all in a single call.
                        device.DrawPrimitives(
                            PrimitiveType.PointList,
                            m_FirstActiveParticle,
                            m_FirstFreeParticle - m_FirstActiveParticle);
                    }
                    else
                    {
                        // If the active particle range wraps past the end of the queue
                        // back to the start, we must split them over two draw calls.
                        device.DrawPrimitives(
                            PrimitiveType.PointList,
                            m_FirstActiveParticle,
                            m_Particles.Length - m_FirstActiveParticle);

                        if (m_FirstFreeParticle > 0)
                        {
                            device.DrawPrimitives(
                                PrimitiveType.PointList,
                                0,
                                m_FirstFreeParticle);
                        }
                    }

                    pass.End();
                }

                m_ParticleEffect.End();

                // Reset a couple of the more unusual renderstates that we changed, so as not to mess up any other subsequent drawing.
                device.RenderState.PointSpriteEnable = false;
                device.RenderState.DepthBufferWriteEnable = true;
            }

            m_DrawCounter++;
        }

        /// <summary>
        /// Helper for loading and initializing the particle effect.
        /// </summary>
        void LoadParticleEffect()
        {
            Effect effect = Content.Load<Effect>("Content/Particles/Particles");

            // If we have several particle systems, the content manager will return
            // a single shared effect instance to them all. But we want to preconfigure
            // the effect with parameters that are specific to this particular
            // particle system. By cloning the effect, we prevent one particle system
            // from stomping over the parameter settings of another.

            m_ParticleEffect = effect.Clone(GraphicsDevice);

            EffectParameterCollection parameters = m_ParticleEffect.Parameters;

            // Look up shortcuts for parameters that change every frame.
            m_EffectViewParameter = parameters["View"];
            m_EffectProjectionParameter = parameters["Projection"];
            m_EffectViewportHeightParameter = parameters["ViewportHeight"];
            m_EffectTimeParameter = parameters["CurrentTime"];

            // Set the values of parameters that do not change.
            parameters["Duration"].SetValue((float)Settings.Duration.TotalSeconds);
            parameters["DurationRandomness"].SetValue(Settings.DurationRandomness);
            parameters["Gravity"].SetValue(Settings.Gravity);
            parameters["EndVelocity"].SetValue(Settings.EndVelocity);
            parameters["MinColor"].SetValue(Settings.MinColor.ToVector4());
            parameters["MaxColor"].SetValue(Settings.MaxColor.ToVector4());

            parameters["RotateSpeed"].SetValue(
                new Vector2(Settings.MinRotateSpeed, Settings.MaxRotateSpeed));

            parameters["StartSize"].SetValue(new Vector2(Settings.MinStartSize, Settings.MaxStartSize));

            parameters["EndSize"].SetValue(new Vector2(Settings.MinEndSize, Settings.MaxEndSize));

            // Load the particle texture, and set it onto the effect.
            Texture2D texture = Content.Load<Texture2D>(Settings.TextureName);

            parameters["Texture"].SetValue(texture);

            // Choose the appropriate effect technique. If these particles will never
            // rotate, we can use a simpler pixel shader that requires less GPU power.
            string techniqueName;

            if ((Settings.MinRotateSpeed == 0) && (Settings.MaxRotateSpeed == 0))
            {
                techniqueName = "NonRotatingParticles";
            }
            else
            {
                techniqueName = "RotatingParticles";
            }

            m_ParticleEffect.CurrentTechnique = m_ParticleEffect.Techniques[techniqueName];
        }
        /// <summary>
        /// Helper for checking when active particles have reached the end of
        /// their life. It moves old particles from the active area of the queue
        /// to the retired section.
        /// </summary>
        void RetireActiveParticles()
        {
            float particleDuration = (float)Settings.Duration.TotalSeconds;

            while (m_FirstActiveParticle != m_FirstNewParticle)
            {
                // Is this particle old enough to retire?
                float particleAge = m_CurrentTime - m_Particles[m_FirstActiveParticle].Time;

                if (particleAge < particleDuration)
                {
                    break;
                }

                // Remember the time at which we retired this particle.
                m_Particles[m_FirstActiveParticle].Time = m_DrawCounter;

                // Move the particle from the active to the retired queue.
                m_FirstActiveParticle++;

                if (m_FirstActiveParticle >= m_Particles.Length)
                {
                    m_FirstActiveParticle = 0;
                }
            }
        }
        /// <summary>
        /// Helper for checking when retired particles have been kept around long
        /// enough that we can be sure the GPU is no longer using them. It moves
        /// old particles from the retired area of the queue to the free section.
        /// </summary>
        void FreeRetiredParticles()
        {
            while (m_FirstRetiredParticle != m_FirstActiveParticle)
            {
                // Has this particle been unused long enough that
                // the GPU is sure to be finished with it?
                int age = m_DrawCounter - (int)m_Particles[m_FirstRetiredParticle].Time;

                // The GPU is never supposed to get more than 2 frames behind the CPU.
                // We add 1 to that, just to be safe in case of buggy drivers that
                // might bend the rules and let the GPU get further behind.
                if (age < 3)
                {
                    break;
                }

                // Move the particle from the retired to the free queue.
                m_FirstRetiredParticle++;

                if (m_FirstRetiredParticle >= m_Particles.Length)
                {
                    m_FirstRetiredParticle = 0;
                }
            }
        }
        /// <summary>
        /// Helper for uploading new particles from our managed
        /// array to the GPU vertex buffer.
        /// </summary>
        void AddNewParticlesToVertexBuffer()
        {
            int stride = ParticleVertex.SizeInBytes;

            if (m_FirstNewParticle < m_FirstFreeParticle)
            {
                // If the new particles are all in one consecutive range,
                // we can upload them all in a single call.
                //vertexBuffer.SetData(
                //    firstNewParticle * stride, particles,
                //    firstNewParticle,
                //    firstFreeParticle - firstNewParticle,
                //    stride,
                //    SetDataOptions.NoOverwrite);
                m_VertexBuffer.SetData(
                    m_FirstNewParticle * stride, m_Particles,
                    m_FirstNewParticle,
                    m_FirstFreeParticle - m_FirstNewParticle,
                    stride);
            }
            else
            {
                // If the new particle range wraps past the end of the queue
                // back to the start, we must split them over two upload calls.
                //vertexBuffer.SetData(
                //    firstNewParticle * stride, particles,
                //    firstNewParticle,
                //    particles.Length - firstNewParticle,
                //    stride,
                //    SetDataOptions.NoOverwrite);
                m_VertexBuffer.SetData(
                    m_FirstNewParticle * stride, m_Particles,
                    m_FirstNewParticle,
                    m_Particles.Length - m_FirstNewParticle,
                    stride);

                if (m_FirstFreeParticle > 0)
                {
                    //vertexBuffer.SetData(
                    //    0, particles,
                    //    0, firstFreeParticle,
                    //    stride,
                    //    SetDataOptions.NoOverwrite);
                    m_VertexBuffer.SetData(
                        0, m_Particles,
                        0, m_FirstFreeParticle,
                        stride);
                }
            }

            // Move the particles we just uploaded from the new to the active queue.
            m_FirstNewParticle = m_FirstFreeParticle;
        }
        /// <summary>
        /// Helper for setting the renderstates used to draw particles.
        /// </summary>
        void SetParticleRenderStates(RenderState renderState)
        {
            // Enable point sprites.
            renderState.PointSpriteEnable = true;
            renderState.PointSizeMax = 256;

            // Set the alpha blend mode.
            renderState.AlphaBlendEnable = true;
            renderState.AlphaBlendOperation = BlendFunction.Add;
            renderState.SourceBlend = Settings.SourceBlend;
            renderState.DestinationBlend = Settings.DestinationBlend;

            // Set the alpha test mode.
            renderState.AlphaTestEnable = true;
            renderState.AlphaFunction = CompareFunction.Greater;
            renderState.ReferenceAlpha = 0;

            // Enable the depth buffer (so particles will not be visible through
            // solid objects like the ground plane), but disable depth writes
            // (so particles will not obscure other particles).
            renderState.DepthBufferEnable = true;
            renderState.DepthBufferWriteEnable = false;
        }

        /// <summary>
        /// Sets the camera view and projection matrices
        /// that will be used to draw this particle system.
        /// </summary>
        public void SetCamera(Matrix view, Matrix projection)
        {
            m_EffectViewParameter.SetValue(view);
            m_EffectProjectionParameter.SetValue(projection);
        }
        /// <summary>
        /// Adds a new particle to the system.
        /// </summary>
        public void AddParticle(Vector3 position, Vector3 velocity)
        {
            // Figure out where in the circular queue to allocate the new particle.
            int nextFreeParticle = m_FirstFreeParticle + 1;

            if (nextFreeParticle >= m_Particles.Length)
                nextFreeParticle = 0;

            // If there are no free particles, we just have to give up.
            if (nextFreeParticle == m_FirstRetiredParticle)
                return;

            // Adjust the input velocity based on how much
            // this particle system wants to be affected by it.
            velocity *= Settings.EmitterVelocitySensitivity;

            // Add in some random amount of horizontal velocity.
            float horizontalVelocity = MathHelper.Lerp(Settings.MinHorizontalVelocity,
                                                       Settings.MaxHorizontalVelocity,
                                                       RandomComponent.NextFloat());

            double horizontalAngle = RandomComponent.NextDouble() * MathHelper.TwoPi;

            velocity.X += horizontalVelocity * (float)Math.Cos(horizontalAngle);
            velocity.Z += horizontalVelocity * (float)Math.Sin(horizontalAngle);

            // Add in some random amount of vertical velocity.
            velocity.Y += MathHelper.Lerp(Settings.MinVerticalVelocity,
                                          Settings.MaxVerticalVelocity,
                                          RandomComponent.NextFloat());

            // Choose four random control values. These will be used by the vertex
            // shader to give each particle a different size, rotation, and color.
            Color randomValues = new Color((byte)RandomComponent.Next(255),
                                           (byte)RandomComponent.Next(255),
                                           (byte)RandomComponent.Next(255),
                                           (byte)RandomComponent.Next(255));

            // Fill in the particle vertex structure.
            m_Particles[m_FirstFreeParticle].Position = position;
            m_Particles[m_FirstFreeParticle].Velocity = velocity;
            m_Particles[m_FirstFreeParticle].Random = randomValues;
            m_Particles[m_FirstFreeParticle].Time = m_CurrentTime;

            m_FirstFreeParticle = nextFreeParticle;
        }
    }
}


