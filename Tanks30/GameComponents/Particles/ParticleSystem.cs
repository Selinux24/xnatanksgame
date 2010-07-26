using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameComponents.Particles
{
    using Common;
    using GameComponents.MathComponents;

    /// <summary>
    /// Sistema de part�culas
    /// </summary>
    public abstract class ParticleSystem : DrawableGameComponent
    {
        /// <summary>
        /// Configuraci�n de la apariencia de las part�culas del sistema de part�culas
        /// </summary>
        protected ParticleSettings Settings = new ParticleSettings();

        /// <summary>
        /// Efecto para dibujar las part�culas
        /// </summary>
        private Effect m_ParticleEffect;
        /// <summary>
        /// Par�metros de vista del efecto
        /// </summary>
        private EffectParameter m_EffectViewParameter;
        /// <summary>
        /// Par�metro de projecci�n del efecto
        /// </summary>
        private EffectParameter m_EffectProjectionParameter;
        /// <summary>
        /// Par�metro de Altura del Viewport del efecto
        /// </summary>
        private EffectParameter m_EffectViewportHeightParameter;
        /// <summary>
        /// Par�metro de tiempo del efecto
        /// </summary>
        private EffectParameter m_EffectTimeParameter;
        /// <summary>
        /// Cola circular de part�culas
        /// </summary>
        private ParticleVertex[] m_Particles;
        /// <summary>
        /// Buffer de part�culas
        /// </summary>
        private VertexBuffer m_VertexBuffer;
        /// <summary>
        /// Declaraci�n de los v�rtices del buffer
        /// </summary>
        private VertexDeclaration m_VertexDeclaration;
        /// <summary>
        /// Indice de la primera part�cula activa
        /// </summary>
        private int m_FirstActiveParticle;
        /// <summary>
        /// Indice de la primera part�cula nueva
        /// </summary>
        private int m_FirstNewParticle;
        /// <summary>
        /// Indice de la primera part�cula libre
        /// </summary>
        private int m_FirstFreeParticle;
        /// <summary>
        /// Indice de la primera part�cula retirada
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
        protected ParticleSystem(Game game)
            : base(game)
        {

        }

        /// <summary>
        /// Initializes the component.
        /// </summary>
        public override void Initialize()
        {
            //Inicializaci�n
            this.InitializeSettings(Settings);

            //Lista de part�culas
            this.m_Particles = new ParticleVertex[this.Settings.MaxParticles];

            base.Initialize();
        }
        /// <summary>
        /// Establece la configuraci�n del sistema de part�culas
        /// </summary>
        /// <param name="settings">Configuraci�n</param>
        protected abstract void InitializeSettings(ParticleSettings settings);
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

            // Puntero a los par�metros que cambian en cada frame
            this.m_EffectViewParameter = parameters["View"];
            this.m_EffectProjectionParameter = parameters["Projection"];
            this.m_EffectViewportHeightParameter = parameters["ViewportHeight"];
            this.m_EffectTimeParameter = parameters["CurrentTime"];

            // Establecer los par�metros constantes
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

            //Obtener la t�cnica correcta del efecto para renderizar
            if ((this.Settings.MinRotateSpeed == 0) && (this.Settings.MaxRotateSpeed == 0))
            {
                this.m_ParticleEffect.CurrentTechnique = this.m_ParticleEffect.Techniques["NonRotatingParticles"];
            }
            else
            {
                this.m_ParticleEffect.CurrentTechnique = this.m_ParticleEffect.Techniques["RotatingParticles"];
            }

            //Declaraci�n de los v�rtices
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

            //Prevenir que el contador se pasa del valor m�ximo para float
            if (this.m_FirstActiveParticle == this.m_FirstFreeParticle)
            {
                this.m_CurrentTime = 0;
            }

            //Prevenir que el contador se pasa del valor m�ximo para int
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
                //Si hay part�culas reci�n a�adidas, es necesario introducirlas en el buffer
                this.AddNewParticlesToVertexBuffer();
            }

            //Si hay part�culas activas, renderizar
            if (this.m_FirstActiveParticle != this.m_FirstFreeParticle)
            {
                //Establecer la configuraci�n del renderizador
                this.SetParticleRenderStates(this.GraphicsDevice.RenderState);

                //Altura del ViewPort
                this.m_EffectViewportHeightParameter.SetValue(this.GraphicsDevice.Viewport.Height);

                //Tiempo actual
                this.m_EffectTimeParameter.SetValue(this.m_CurrentTime);

                //Matriz vista
                this.m_EffectViewParameter.SetValue(GlobalMatrices.gViewMatrix);

                //Matriz proyecci�n
                this.m_EffectProjectionParameter.SetValue(GlobalMatrices.gGlobalProjectionMatrix);

                //Establecer la declaraci�n y el buffer
                this.GraphicsDevice.Vertices[0].SetSource(this.m_VertexBuffer, 0, ParticleVertex.SizeInBytes);
                this.GraphicsDevice.VertexDeclaration = this.m_VertexDeclaration;

                //Comenzar a dibujar
                this.m_ParticleEffect.Begin();

                foreach (EffectPass pass in this.m_ParticleEffect.CurrentTechnique.Passes)
                {
                    pass.Begin();

                    if (this.m_FirstActiveParticle < this.m_FirstFreeParticle)
                    {
                        //Si las part�culas son consecutivas, dibujado de una sola vez
                        this.GraphicsDevice.DrawPrimitives(
                            PrimitiveType.PointList,
                            this.m_FirstActiveParticle,
                            this.m_FirstFreeParticle - this.m_FirstActiveParticle);
                    }
                    else
                    {
                        //Si las part�culas no son consecutivas, dibujado en dos pasos
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
                this.GraphicsDevice.RenderState.PointSpriteEnable = false;
                this.GraphicsDevice.RenderState.DepthBufferWriteEnable = true;
            }

            this.m_DrawCounter++;
        }

        /// <summary>
        /// Retira las part�culas inactivas
        /// </summary>
        void RetireActiveParticles()
        {
            float particleDuration = (float)this.Settings.Duration.TotalSeconds;

            while (this.m_FirstActiveParticle != this.m_FirstNewParticle)
            {
                //Obtener la edad de la part�cula
                float particleAge = this.m_CurrentTime - this.m_Particles[this.m_FirstActiveParticle].Time;

                if (particleAge < particleDuration)
                {
                    break;
                }

                //Almacenar el tiempo en el que se retira la part�cula
                this.m_Particles[this.m_FirstActiveParticle].Time = this.m_DrawCounter;

                //Aumentar el contador para dejar la part�cula en la lista de retiradas
                this.m_FirstActiveParticle++;

                //Si es la �ltima part�cula, se vuelve a empezar desde el principio
                if (this.m_FirstActiveParticle >= this.m_Particles.Length)
                {
                    this.m_FirstActiveParticle = 0;
                }
            }
        }
        /// <summary>
        /// Libera las part�culas retiradas
        /// </summary>
        void FreeRetiredParticles()
        {
            while (this.m_FirstRetiredParticle != this.m_FirstActiveParticle)
            {
                //Obtener la edad de la part�cula desde que se retir�
                int age = this.m_DrawCounter - (int)this.m_Particles[this.m_FirstRetiredParticle].Time;

                //La GPU nunca deber�a usar m�s de dos frames tras la CPU. Por si acaso, contamos 3
                if (age < 3)
                {
                    break;
                }

                //Aumentamos el contador para dejar la part�cula en la lista de part�culas libres
                this.m_FirstRetiredParticle++;

                //Si llegamos al final, comenzamos desde el principio
                if (this.m_FirstRetiredParticle >= this.m_Particles.Length)
                {
                    this.m_FirstRetiredParticle = 0;
                }
            }
        }
        /// <summary>
        /// A�adir nuevas part�culas al buffer
        /// </summary>
        void AddNewParticlesToVertexBuffer()
        {
            int stride = ParticleVertex.SizeInBytes;

            if (this.m_FirstNewParticle < this.m_FirstFreeParticle)
            {
                //Si todas las part�culas son consecutivas, se cargan de una sola vez
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

            //Establecer el indice para dejar las part�culas en el �rea activa
            this.m_FirstNewParticle = this.m_FirstFreeParticle;
        }
        /// <summary>
        /// Configura el renderizador para cada part�cula
        /// </summary>
        void SetParticleRenderStates(RenderState renderState)
        {
            //PointSprites
            renderState.PointSpriteEnable = true;
            renderState.PointSizeMax = 256;

            //AlphaBlend
            renderState.AlphaBlendEnable = true;
            renderState.AlphaBlendOperation = BlendFunction.Add;
            renderState.SourceBlend = Settings.SourceBlend;
            renderState.DestinationBlend = Settings.DestinationBlend;

            //AlphaTest
            renderState.AlphaTestEnable = true;
            renderState.AlphaFunction = CompareFunction.Greater;
            renderState.ReferenceAlpha = 0;

            //DepthBuffer para que se oculten tras otros objetos
            renderState.DepthBufferEnable = true;

            //DepthBufferWrite para que no influyan en otras part�culas
            renderState.DepthBufferWriteEnable = false;
        }

        /// <summary>
        /// A�ade una nueva part�cula al sistema
        /// </summary>
        /// <param name="position">Posici�n</param>
        /// <param name="velocity">Velocidad</param>
        public void AddParticle(Vector3 position, Vector3 velocity)
        {
            //Obtener el siguiente �ndice disponible
            int nextFreeParticle = this.m_FirstFreeParticle + 1;

            //Si se ha llegado al final, se empieza desde el principio
            if (nextFreeParticle >= this.m_Particles.Length)
            {
                nextFreeParticle = 0;
            }

            //Si no ha part�culas libres, se terminar el proceso
            if (nextFreeParticle == this.m_FirstRetiredParticle)
            {
                return;
            }

            //Ajustar la velocidad
            velocity *= this.Settings.EmitterVelocitySensitivity;

            //A�adir algo de velocidad horizontal
            float horizontalVelocity = MathHelper.Lerp(
                this.Settings.MinHorizontalVelocity,
                this.Settings.MaxHorizontalVelocity,                                                       
                RandomComponent.NextFloat());

            double horizontalAngle = RandomComponent.NextDouble() * MathHelper.TwoPi;

            velocity.X += horizontalVelocity * (float)Math.Cos(horizontalAngle);
            velocity.Z += horizontalVelocity * (float)Math.Sin(horizontalAngle);

            //A�adir algo de velocidad vertical
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

            //A�adir la part�cula
            this.m_Particles[this.m_FirstFreeParticle].Position = position;
            this.m_Particles[this.m_FirstFreeParticle].Velocity = velocity;
            this.m_Particles[this.m_FirstFreeParticle].Random = randomValues;
            this.m_Particles[this.m_FirstFreeParticle].Time = this.m_CurrentTime;

            this.m_FirstFreeParticle = nextFreeParticle;
        }
    }
}


