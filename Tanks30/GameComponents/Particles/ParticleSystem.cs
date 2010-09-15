using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameComponents.Particles
{
    using Common;
    using GameComponents.MathComponents;

    /// <summary>
    /// Sistema de partículas
    /// </summary>
    public class ParticleSystem : DrawableGameComponent
    {
        /// <summary>
        /// Configuración de la apariencia de las partículas del sistema de partículas
        /// </summary>
        protected ParticleSettings Settings = new ParticleSettings();
        /// <summary>
        /// Tipo de partícula
        /// </summary>
        protected ParticleSystemTypes ParticleType = ParticleSystemTypes.None;

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
        /// Indice de la primera partícula activa
        /// </summary>
        private int m_FirstActiveParticle;
        /// <summary>
        /// Indice de la primera partícula nueva
        /// </summary>
        private int m_FirstNewParticle;
        /// <summary>
        /// Indice de la primera partícula libre
        /// </summary>
        private int m_FirstFreeParticle;
        /// <summary>
        /// Indice de la primera partícula retirada
        /// </summary>
        private int m_FirstRetiredParticle;
        /// <summary>
        /// Tiempo actual en segundos
        /// </summary>
        private float m_CurrentTime;
        /// <summary>
        /// Contador de las veces que se ha llamado a Draw
        /// </summary>
        private int m_DrawCounter;

        /// <summary>
        /// Obtiene la cantidad de partículas en uso
        /// </summary>
        public int UsedParticles
        {
            get
            {
                if (this.m_FirstActiveParticle < this.m_FirstFreeParticle)
                {
                    return this.m_FirstFreeParticle - this.m_FirstActiveParticle;
                }
                else
                {
                    return this.m_Particles.Length - this.m_FirstActiveParticle + this.m_FirstFreeParticle;
                }
            }
        }

        /// <summary>
        /// Gestor de contenidos
        /// </summary>
        protected ContentManager Content
        {
            get
            {
                return this.Game.Content;
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public ParticleSystem(Game game, ParticleSystemTypes particleType)
            : base(game)
        {
            this.ParticleType = particleType;
        }

        /// <summary>
        /// Initializes the component.
        /// </summary>
        public override void Initialize()
        {
            //Inicialización
            if (this.ParticleType == ParticleSystemTypes.Dust)
            {
                this.InitializeDust(Settings);
            }
            else if (this.ParticleType == ParticleSystemTypes.Explosion)
            {
                this.InitializeExplosion(Settings);
            }
            else if (this.ParticleType == ParticleSystemTypes.ExplosionSmoke)
            {
                this.InitializeExplosionSmoke(Settings);
            }
            else if (this.ParticleType == ParticleSystemTypes.Fire)
            {
                this.InitializeFire(Settings);
            }
            else if (this.ParticleType == ParticleSystemTypes.PlasmaEngine)
            {
                this.InitializePlasmaEngine(Settings);
            }
            else if (this.ParticleType == ParticleSystemTypes.ProjectileTrail)
            {
                this.InitializeProjectileTrail(Settings);
            }
            else if (this.ParticleType == ParticleSystemTypes.SmokeEngine)
            {
                this.InitializeSmokeEngine(Settings);
            }
            else if (this.ParticleType == ParticleSystemTypes.SmokePlume)
            {
                this.InitializeSmokePlume(Settings);
            }

            //Lista de partículas
            this.m_Particles = new ParticleVertex[this.Settings.MaxParticles];

            base.Initialize();
        }

        /// <summary> 
        /// Inicializar el sistema de partículas
        /// </summary> 
        /// <param name="settings">Propiedades</param>
        protected void InitializeDust(ParticleSettings settings)
        {
            settings.TextureName = "Content/Particles/smoke";

            settings.MaxParticles = 1000;

            settings.Duration = TimeSpan.FromSeconds(1);

            settings.MinHorizontalVelocity = 0;
            settings.MaxHorizontalVelocity = 2;

            settings.MinVerticalVelocity = 0;
            settings.MaxVerticalVelocity = 2;

            settings.Gravity = new Vector3(-0.15f, -0.15f, 0);

            settings.EndVelocity = 0.1f;

            settings.MinColor = Color.SandyBrown;
            settings.MaxColor = Color.SandyBrown;

            settings.MinRotateSpeed = -1;
            settings.MaxRotateSpeed = 1;

            settings.MinStartSize = 1;
            settings.MaxStartSize = 2;

            settings.MinEndSize = 5;
            settings.MaxEndSize = 10;
        }
        /// <summary> 
        /// Inicializar el sistema de partículas
        /// </summary> 
        /// <param name="settings">Propiedades</param>
        protected void InitializeExplosion(ParticleSettings settings)
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
        /// <summary> 
        /// Inicializar el sistema de partículas
        /// </summary> 
        /// <param name="settings">Propiedades</param>
        protected void InitializeExplosionSmoke(ParticleSettings settings)
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
        /// <summary> 
        /// Inicializar el sistema de partículas
        /// </summary> 
        /// <param name="settings">Propiedades</param>
        protected void InitializeFire(ParticleSettings settings)
        {
            settings.TextureName = "Content/Particles/fire";

            settings.MaxParticles = 500;

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
        /// <summary> 
        /// Inicializar el sistema de partículas
        /// </summary> 
        /// <param name="settings">Propiedades</param>
        protected void InitializePlasmaEngine(ParticleSettings settings)
        {
            settings.TextureName = "Content/Particles/fire";

            settings.MaxParticles = 500;

            settings.Duration = TimeSpan.FromSeconds(0.5f);

            settings.DurationRandomness = 0f;

            settings.MinHorizontalVelocity = 0;
            settings.MaxHorizontalVelocity = 0;

            settings.MinVerticalVelocity = 0;
            settings.MaxVerticalVelocity = 0;

            settings.Gravity = new Vector3(0, 0, 0);

            settings.MinColor = Color.AliceBlue;
            settings.MaxColor = Color.LightBlue;

            settings.MinStartSize = 1f;
            settings.MaxStartSize = 1f;

            settings.MinEndSize = 0.1f;
            settings.MaxEndSize = 0.1f;

            settings.SourceBlend = Blend.SourceAlpha;
            settings.DestinationBlend = Blend.One;
        }
        /// <summary> 
        /// Inicializar el sistema de partículas
        /// </summary> 
        /// <param name="settings">Propiedades</param>
        protected void InitializeProjectileTrail(ParticleSettings settings)
        {
            settings.TextureName = "Content/Particles/smoke";

            settings.MaxParticles = 250;

            settings.Duration = TimeSpan.FromSeconds(0.5f);

            settings.DurationRandomness = 1.5f;

            settings.EmitterVelocitySensitivity = 0.1f;

            settings.MinHorizontalVelocity = -1;
            settings.MaxHorizontalVelocity = 1;

            settings.MinVerticalVelocity = -1;
            settings.MaxVerticalVelocity = 1;

            settings.MinColor = Color.Gray;
            settings.MaxColor = Color.White;

            settings.MinRotateSpeed = 1;
            settings.MaxRotateSpeed = 1;

            settings.MinStartSize = 0.5f;
            settings.MaxStartSize = 1f;

            settings.MinEndSize = 1f;
            settings.MaxEndSize = 2f;
        }
        /// <summary>
        /// Humo de motor
        /// </summary>
        /// <param name="settings">Propiedades</param>
        protected void InitializeSmokeEngine(ParticleSettings settings)
        {
            settings.TextureName = "Content/Particles/smoke";

            settings.MaxParticles = 1000;

            settings.Duration = TimeSpan.FromSeconds(1);

            settings.MinHorizontalVelocity = 0;
            settings.MaxHorizontalVelocity = 2;

            settings.MinVerticalVelocity = 0;
            settings.MaxVerticalVelocity = 2;

            settings.Gravity = new Vector3(-1, -1, 0);

            settings.EndVelocity = 0.15f;

            settings.MinRotateSpeed = -1;
            settings.MaxRotateSpeed = 1;

            settings.MinStartSize = 1;
            settings.MaxStartSize = 2;

            settings.MinEndSize = 2;
            settings.MaxEndSize = 4;
        }
        /// <summary>
        /// Inicializar
        /// </summary>
        /// <param name="settings">Parámetros</param>
        protected void InitializeSmokePlume(ParticleSettings settings)
        {
            settings.TextureName = "Content/Particles/smoke";

            settings.MaxParticles = 5000;

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

        /// <summary>
        /// Carga del contenido
        /// </summary>
        protected override void LoadContent()
        {
            // Textura
            Texture2D texture = this.Content.Load<Texture2D>(this.Settings.TextureName);

            // Efecto
            Effect effect = this.Content.Load<Effect>("Content/Particles/Particles");

            this.m_ParticleEffect = effect.Clone(this.GraphicsDevice);

            EffectParameterCollection parameters = this.m_ParticleEffect.Parameters;

            // Puntero a los parámetros que cambian en cada frame
            this.m_EffectViewParameter = parameters["View"];
            this.m_EffectProjectionParameter = parameters["Projection"];
            this.m_EffectViewportHeightParameter = parameters["ViewportHeight"];
            this.m_EffectTimeParameter = parameters["CurrentTime"];

            // Establecer los parámetros constantes
            parameters["Duration"].SetValue((float)this.Settings.Duration.TotalSeconds);
            parameters["DurationRandomness"].SetValue(this.Settings.DurationRandomness);
            parameters["Gravity"].SetValue(this.Settings.Gravity);
            parameters["EndVelocity"].SetValue(this.Settings.EndVelocity);
            parameters["MinColor"].SetValue(this.Settings.MinColor.ToVector4());
            parameters["MaxColor"].SetValue(this.Settings.MaxColor.ToVector4());
            parameters["RotateSpeed"].SetValue(new Vector2(this.Settings.MinRotateSpeed, this.Settings.MaxRotateSpeed));
            parameters["StartSize"].SetValue(new Vector2(this.Settings.MinStartSize, this.Settings.MaxStartSize));
            parameters["EndSize"].SetValue(new Vector2(this.Settings.MinEndSize, this.Settings.MaxEndSize));
            parameters["Texture"].SetValue(texture);

            //Obtener la técnica correcta del efecto para renderizar
            if ((this.Settings.MinRotateSpeed == 0) && (this.Settings.MaxRotateSpeed == 0))
            {
                this.m_ParticleEffect.CurrentTechnique = this.m_ParticleEffect.Techniques["NonRotatingParticles"];
            }
            else
            {
                this.m_ParticleEffect.CurrentTechnique = this.m_ParticleEffect.Techniques["RotatingParticles"];
            }

            //Declaración de los vértices
            this.m_VertexDeclaration = new VertexDeclaration(this.GraphicsDevice, ParticleVertex.VertexElements);

            //Buffer
            this.m_VertexBuffer = new VertexBuffer(
                this.GraphicsDevice,
                ParticleVertex.SizeInBytes * this.m_Particles.Length,
                BufferUsage.WriteOnly | BufferUsage.Points);

            //Establecer el contenido del buffer
            this.m_VertexBuffer.SetData(this.m_Particles);
        }
        /// <summary>
        /// Actualiza el estado del componente.
        /// </summary>
        /// <param name="gameTime">Tiempo</param>
        public override void Update(GameTime gameTime)
        {
            this.m_CurrentTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

            this.RetireActiveParticles();

            this.FreeRetiredParticles();

            //Prevenir que el contador se pasa del valor máximo para float
            if (this.m_FirstActiveParticle == this.m_FirstFreeParticle)
            {
                this.m_CurrentTime = 0;
            }

            //Prevenir que el contador se pasa del valor máximo para int
            if (this.m_FirstRetiredParticle == this.m_FirstActiveParticle)
            {
                this.m_DrawCounter = 0;
            }
        }
        /// <summary>
        /// Dibuja el componente
        /// </summary>
        /// <param name="gameTime">Tiempo</param>
        public override void Draw(GameTime gameTime)
        {
            if (this.m_FirstNewParticle != this.m_FirstFreeParticle)
            {
                //Si hay partículas recién añadidas, es necesario introducirlas en el buffer
                this.AddNewParticlesToVertexBuffer();
            }

            //Si hay partículas activas, renderizar
            if (this.m_FirstActiveParticle != this.m_FirstFreeParticle)
            {
                //Guardar el estado anterior
                bool pointSpriteEnable = this.GraphicsDevice.RenderState.PointSpriteEnable;
                float pointSizeMax = this.GraphicsDevice.RenderState.PointSizeMax;
                bool alphaBlendEnable = this.GraphicsDevice.RenderState.AlphaBlendEnable;
                BlendFunction alphaBlendOperation = this.GraphicsDevice.RenderState.AlphaBlendOperation;
                Blend sourceBlend = this.GraphicsDevice.RenderState.SourceBlend;
                Blend destinationBlend = this.GraphicsDevice.RenderState.DestinationBlend;
                bool alphaTestEnable = this.GraphicsDevice.RenderState.AlphaTestEnable;
                CompareFunction alphaFunction = this.GraphicsDevice.RenderState.AlphaFunction;
                int referenceAlpha = this.GraphicsDevice.RenderState.ReferenceAlpha;
                bool depthBufferEnable = this.GraphicsDevice.RenderState.DepthBufferEnable;
                bool depthBufferWriteEnable = this.GraphicsDevice.RenderState.DepthBufferWriteEnable;

                //PointSprites
                this.GraphicsDevice.RenderState.PointSpriteEnable = true;
                this.GraphicsDevice.RenderState.PointSizeMax = 256;

                //AlphaBlend
                this.GraphicsDevice.RenderState.AlphaBlendEnable = true;
                this.GraphicsDevice.RenderState.AlphaBlendOperation = BlendFunction.Add;
                this.GraphicsDevice.RenderState.SourceBlend = Settings.SourceBlend;
                this.GraphicsDevice.RenderState.DestinationBlend = Settings.DestinationBlend;

                //AlphaTest
                this.GraphicsDevice.RenderState.AlphaTestEnable = true;
                this.GraphicsDevice.RenderState.AlphaFunction = CompareFunction.Greater;
                this.GraphicsDevice.RenderState.ReferenceAlpha = 0;

                //DepthBuffer para que se oculten tras otros objetos
                this.GraphicsDevice.RenderState.DepthBufferEnable = true;

                //DepthBufferWrite para que no influyan en otras partículas
                this.GraphicsDevice.RenderState.DepthBufferWriteEnable = false;

                //Niebla
                Scenery.SceneryEnvironment.Fog.SetFogToDevice(this.GraphicsDevice);

                //Establecer la declaración y el buffer
                this.GraphicsDevice.Vertices[0].SetSource(this.m_VertexBuffer, 0, ParticleVertex.SizeInBytes);
                this.GraphicsDevice.VertexDeclaration = this.m_VertexDeclaration;

                //Altura del ViewPort
                this.m_EffectViewportHeightParameter.SetValue(this.GraphicsDevice.Viewport.Height);
                //Tiempo actual
                this.m_EffectTimeParameter.SetValue(this.m_CurrentTime);
                //Matriz vista
                this.m_EffectViewParameter.SetValue(GlobalMatrices.gViewMatrix);
                //Matriz proyección
                this.m_EffectProjectionParameter.SetValue(GlobalMatrices.gProjectionMatrix);

                //Comenzar a dibujar
                this.m_ParticleEffect.Begin();

                foreach (EffectPass pass in this.m_ParticleEffect.CurrentTechnique.Passes)
                {
                    pass.Begin();

                    if (this.m_FirstActiveParticle < this.m_FirstFreeParticle)
                    {
                        //Si las partículas son consecutivas, dibujado de una sola vez
                        this.GraphicsDevice.DrawPrimitives(
                            PrimitiveType.PointList,
                            this.m_FirstActiveParticle,
                            this.m_FirstFreeParticle - this.m_FirstActiveParticle);
                    }
                    else
                    {
                        //Si las partículas no son consecutivas, dibujado en dos pasos
                        this.GraphicsDevice.DrawPrimitives(
                            PrimitiveType.PointList,
                            this.m_FirstActiveParticle,
                            this.m_Particles.Length - this.m_FirstActiveParticle);

                        if (this.m_FirstFreeParticle > 0)
                        {
                            this.GraphicsDevice.DrawPrimitives(
                                PrimitiveType.PointList,
                                0,
                                this.m_FirstFreeParticle);
                        }
                    }

                    pass.End();
                }

                //Fin del dibujado
                this.m_ParticleEffect.End();

                //Resetear el renderizador

                //PointSprites
                this.GraphicsDevice.RenderState.PointSpriteEnable = pointSpriteEnable;
                this.GraphicsDevice.RenderState.PointSizeMax = pointSizeMax;

                //AlphaBlend
                this.GraphicsDevice.RenderState.AlphaBlendEnable = alphaBlendEnable;
                this.GraphicsDevice.RenderState.AlphaBlendOperation = alphaBlendOperation;
                this.GraphicsDevice.RenderState.SourceBlend = sourceBlend;
                this.GraphicsDevice.RenderState.DestinationBlend = destinationBlend;

                //AlphaTest
                this.GraphicsDevice.RenderState.AlphaTestEnable = alphaTestEnable;
                this.GraphicsDevice.RenderState.AlphaFunction = alphaFunction;
                this.GraphicsDevice.RenderState.ReferenceAlpha = referenceAlpha;

                //DepthBuffer para que se oculten tras otros objetos
                this.GraphicsDevice.RenderState.DepthBufferEnable = depthBufferEnable;

                //DepthBufferWrite para que no influyan en otras partículas
                this.GraphicsDevice.RenderState.DepthBufferWriteEnable = depthBufferWriteEnable;

                //Restablecer la declaración 
                this.GraphicsDevice.VertexDeclaration = null;
            }

            this.m_DrawCounter++;
        }

        /// <summary>
        /// Retira las partículas inactivas
        /// </summary>
        void RetireActiveParticles()
        {
            float particleDuration = (float)this.Settings.Duration.TotalSeconds;

            while (this.m_FirstActiveParticle != this.m_FirstNewParticle)
            {
                //Obtener la edad de la partícula
                float particleAge = this.m_CurrentTime - this.m_Particles[this.m_FirstActiveParticle].Time;

                if (particleAge < particleDuration)
                {
                    break;
                }

                //Almacenar el tiempo en el que se retira la partícula
                this.m_Particles[this.m_FirstActiveParticle].Time = this.m_DrawCounter;

                //Aumentar el contador para dejar la partícula en la lista de retiradas
                this.m_FirstActiveParticle++;

                //Si es la última partícula, se vuelve a empezar desde el principio
                if (this.m_FirstActiveParticle >= this.m_Particles.Length)
                {
                    this.m_FirstActiveParticle = 0;
                }
            }
        }
        /// <summary>
        /// Libera las partículas retiradas
        /// </summary>
        void FreeRetiredParticles()
        {
            while (this.m_FirstRetiredParticle != this.m_FirstActiveParticle)
            {
                //Obtener la edad de la partícula desde que se retiró
                int age = this.m_DrawCounter - (int)this.m_Particles[this.m_FirstRetiredParticle].Time;

                //La GPU nunca debería usar más de dos frames tras la CPU. Por si acaso, contamos 3
                if (age < 3)
                {
                    break;
                }

                //Aumentamos el contador para dejar la partícula en la lista de partículas libres
                this.m_FirstRetiredParticle++;

                //Si llegamos al final, comenzamos desde el principio
                if (this.m_FirstRetiredParticle >= this.m_Particles.Length)
                {
                    this.m_FirstRetiredParticle = 0;
                }
            }
        }
        /// <summary>
        /// Añadir nuevas partículas al buffer
        /// </summary>
        void AddNewParticlesToVertexBuffer()
        {
            int stride = ParticleVertex.SizeInBytes;

            if (this.m_FirstNewParticle < this.m_FirstFreeParticle)
            {
                //Si todas las partículas son consecutivas, se cargan de una sola vez
                this.m_VertexBuffer.SetData(
                    this.m_FirstNewParticle * stride,
                    this.m_Particles,
                    this.m_FirstNewParticle,
                    this.m_FirstFreeParticle - this.m_FirstNewParticle,
                    stride);
            }
            else
            {
                //Si no son consecutivas, se cargan en dos pasos
                this.m_VertexBuffer.SetData(
                    this.m_FirstNewParticle * stride,
                    this.m_Particles,
                    this.m_FirstNewParticle,
                    this.m_Particles.Length - this.m_FirstNewParticle,
                    stride);

                if (this.m_FirstFreeParticle > 0)
                {
                    this.m_VertexBuffer.SetData(
                        0,
                        this.m_Particles,
                        0,
                        this.m_FirstFreeParticle,
                        stride);
                }
            }

            //Establecer el indice para dejar las partículas en el área activa
            this.m_FirstNewParticle = this.m_FirstFreeParticle;
        }

        /// <summary>
        /// Añade una nueva partícula al sistema
        /// </summary>
        /// <param name="position">Posición</param>
        /// <param name="velocity">Velocidad</param>
        public void AddParticle(Vector3 position, Vector3 velocity)
        {
            //Obtener el siguiente índice disponible
            int nextFreeParticle = this.m_FirstFreeParticle + 1;

            //Si se ha llegado al final, se empieza desde el principio
            if (nextFreeParticle >= this.m_Particles.Length)
            {
                nextFreeParticle = 0;
            }

            //Si no ha partículas libres, se terminar el proceso
            if (nextFreeParticle == this.m_FirstRetiredParticle)
            {
                return;
            }

            //Ajustar la velocidad
            velocity *= this.Settings.EmitterVelocitySensitivity;

            //Añadir algo de velocidad horizontal
            float horizontalVelocity = MathHelper.Lerp(
                this.Settings.MinHorizontalVelocity,
                this.Settings.MaxHorizontalVelocity,
                RandomComponent.NextFloat());

            double horizontalAngle = RandomComponent.NextDouble() * MathHelper.TwoPi;

            velocity.X += horizontalVelocity * (float)Math.Cos(horizontalAngle);
            velocity.Z += horizontalVelocity * (float)Math.Sin(horizontalAngle);

            //Añadir algo de velocidad vertical
            velocity.Y += MathHelper.Lerp(
                this.Settings.MinVerticalVelocity,
                this.Settings.MaxVerticalVelocity,
                RandomComponent.NextFloat());

            //Colores
            Color randomValues = new Color(
                (byte)RandomComponent.Next(255),
                (byte)RandomComponent.Next(255),
                (byte)RandomComponent.Next(255),
                (byte)RandomComponent.Next(255));

            //Añadir la partícula
            this.m_Particles[this.m_FirstFreeParticle].Position = position;
            this.m_Particles[this.m_FirstFreeParticle].Velocity = velocity;
            this.m_Particles[this.m_FirstFreeParticle].Random = randomValues;
            this.m_Particles[this.m_FirstFreeParticle].Time = this.m_CurrentTime;

            this.m_FirstFreeParticle = nextFreeParticle;
        }
    }
}


