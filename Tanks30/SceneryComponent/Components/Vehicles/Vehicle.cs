using System;
using GameComponents.Camera;
using GameComponents.Components.Particles;
using GameComponents.Scenery;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameComponents.Vehicles
{
    /// <summary>
    /// Componente tanque
    /// </summary>
    public partial class Vehicle : DrawableGameComponent
    {
        /// <summary>
        /// Nombre del componente
        /// </summary>
        protected string ComponentInfoName;
        /// <summary>
        /// Administrador de contenidos
        /// </summary>
        protected ContentManager ContentManager;
        /// <summary>
        /// Escenario
        /// </summary>
        protected SceneryGameComponent Scenery = null;

        // Modelo
        private Model m_Model;

        // Posición
        private Vector3 m_Position = Vector3.Zero;
        // Rotación
        private Quaternion m_Rotation = Quaternion.Identity;
        // Escala
        private float m_Scale = 1f;

        // Sistema de partículas
        private ParticleSystem m_SmokePlumeParticles;

        /// <summary>
        /// Obtiene o establece la posición
        /// </summary>
        public virtual Vector3 Position
        {
            get
            {
                return m_Position;
            }
            set
            {
                m_Position = value;
            }
        }
        /// <summary>
        /// Obtiene o establece la rotación
        /// </summary>
        public virtual Quaternion Rotation
        {
            get
            {
                return m_Rotation;
            }
            set
            {
                m_Rotation = value;

                m_Direction = Vector3.Transform(Vector3.Forward, m_Rotation);

                RotationHasChanged = true;
            }
        }
        /// <summary>
        /// Obtiene o establece la escala
        /// </summary>
        public virtual float Scale
        {
            get
            {
                return m_Scale;
            }
            set
            {
                m_Scale = value;
            }
        }

        /// <summary>
        /// Indica que el vehículo se ha inicializado
        /// </summary>
        protected bool Initialized = false;
        /// <summary>
        /// Indica si la posición ha sido modificada desde la última actualización
        /// </summary>
        protected bool PositionHasChanged = false;
        /// <summary>
        /// Indica si la rotación ha sido modificada desde la última actualización
        /// </summary>
        protected bool RotationHasChanged = false;
        /// <summary>
        /// Indica si la escala ha sido modificada desde la último actualización
        /// </summary>
        protected bool ScaleHasChanged = false;

        /// <summary>
        /// Indica si el tanque tiene el foco
        /// </summary>
        public bool HasFocus = false;

        /// <summary>
        /// Obtiene la matriz mundo actual del modelo
        /// </summary>
        public virtual Matrix CurrentTransform
        {
            get
            {
                return GetTransform();
            }
        }
        /// <summary>
        /// Obtiene la vista actual desde el modelo
        /// </summary>
        public virtual Matrix CurrentView
        {
            get
            {
                return GetView();
            }
        }
        /// <summary>
        /// Obtiene la matriz de transformación del modelo
        /// </summary>
        /// <returns>Devuelve la matriz de transformación del modelo</returns>
        protected virtual Matrix GetTransform()
        {
            Matrix world = Matrix.CreateScale(this.Scale);
            world *= Matrix.CreateFromQuaternion(this.Rotation);
            world *= Matrix.CreateFromQuaternion(this.m_Inclination);
            world *= Matrix.CreateTranslation(this.Position);

            return world;
        }
        /// <summary>
        /// Obtiene la matriz de vista desde la posición actual del modelo
        /// </summary>
        /// <returns>Devuelve la matriz de vista desde la posición actual del modelo</returns>
        protected virtual Matrix GetView()
        {
            return m_CurrentPlayerControl.GetViewMatrix(this.m_AnimationController, this.GetTransform());
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game">Juego</param>
        public Vehicle(Game game)
            : base(game)
        {
            this.ContentManager = (ContentManager)game.Services.GetService(typeof(ContentManager));
            this.Scenery = (SceneryGameComponent)game.Services.GetService(typeof(SceneryGameComponent));

            this.m_SmokePlumeParticles = new SmokePlumeParticleSystem(game, this.ContentManager);
            this.m_SmokePlumeParticles.DrawOrder = 100;

            game.Components.Add(this.m_SmokePlumeParticles);
        }

        /// <summary>
        /// Carga de contenido gráfico
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();

            // Información del componente
            VehicleComponentInfo componentInfo = VehicleComponentInfo.Load("Content/" + this.ComponentInfoName);

            // Modelo
            this.m_Model = ContentManager.Load<Model>("Content/" + componentInfo.Model);

            // Velocidad máxima que puede alcanzar el tanque hacia delante
            this.MaxForwardVelocity = componentInfo.MaxForwardVelocity;
            // Velocidad máxima que puede alcanzar el tanque marcha atrás
            this.MaxBackwardVelocity = componentInfo.MaxBackwardVelocity;
            // Modificador de aceleración
            this.AccelerationModifier = MaxForwardVelocity / componentInfo.AccelerationModifier;
            // Modificador de frenado
            this.BrakeModifier = AccelerationModifier * componentInfo.BrakeModifier;
            // Velocidad angular
            this.AngularVelocityModifier = MathHelper.ToRadians(componentInfo.AngularVelocityModifier);
            // Vehículo volador
            this.Skimmer = componentInfo.Skimmer;
            // Altura
            this.FilghtHeight = componentInfo.FlightHeight;
            // Controles de animación
            this.m_AnimationController.AddRange(componentInfo.CreateAnimationList(this.m_Model));
            // Posiciones
            this.m_PlayerControlList.AddRange(componentInfo.CreatePlayerPositionList(this.m_Model));
            
            // Transformaciones iniciales
            this.m_BoneTransforms = new Matrix[m_Model.Bones.Count];
        }
        /// <summary>
        /// Actualiza el estado del componente
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (m_Autopilot.Enabled)
            {
                // Piloto automático
                m_Autopilot.UpdateAutoPilot(this);
            }

            // Controlador de animación
            this.m_AnimationController.Update(gameTime);

            // Actualizar la rotación
            this.UpdateRotation(gameTime);

            // Actualizar la posición
            this.UpdatePosition(gameTime);

            // Establecer la visibilidad del vehículo
            this.Visible = this.TransformedBSph.Intersects(BaseCameraGameComponent.gLODHighFrustum);

            m_SmokePlumeParticles.Visible = this.Visible;

            if (!this.Initialized)
            {
                // Actualizar inclinación y altura
                this.UpdateWithScenery(gameTime);

                this.Initialized = true;
            }
            else if (this.Visible)
            {
                if (this.PositionHasChanged || this.RotationHasChanged || this.ScaleHasChanged)
                {
                    // Actualizar inclinación y altura
                    this.UpdateWithScenery(gameTime);
                }

                // Añadiendo humo
                this.m_SmokePlumeParticles.AddParticle(this.Position, Vector3.Zero);
            }
            else
            {
                this.m_FramesSinceLastUpdate++;

                if (this.m_FramesSinceLastUpdate >= this.m_MaxFrames)
                {
                    // Actualizar inclinación y altura
                    this.UpdateWithScenery(gameTime);

                    this.m_FramesSinceLastUpdate = 0;
                }
            }

            this.PositionHasChanged = false;
            this.RotationHasChanged = false;
            this.ScaleHasChanged = false;
        }
        /// <summary>
        /// Dibujar
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (this.m_Model != null)
            {
                Matrix modelTransform = this.GetTransform();

                this.m_AnimationController.CopyAbsoluteBoneTransformsTo(this.m_Model, this.m_BoneTransforms);

                foreach (ModelMesh mesh in this.m_Model.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        SceneryEnvironment.Fog.SetFogToEffect(effect);
                        //SceneryEnvironment.Ambient.SetAmbientToEffect(effect);

                        effect.View = BaseCameraGameComponent.gViewMatrix;
                        effect.Projection = BaseCameraGameComponent.gGlobalProjectionMatrix;
                        effect.World = this.m_BoneTransforms[mesh.ParentBone.Index] * modelTransform;
                    }

                    mesh.Draw();
                }
            }

            this.m_SmokePlumeParticles.SetCamera(
                BaseCameraGameComponent.gViewMatrix,
                BaseCameraGameComponent.gGlobalProjectionMatrix);
        }

        /// <summary>
        /// Aumenta la velocidad de traslación del modelo
        /// </summary>
        public virtual void Accelerate()
        {
            this.AddToVelocity(this.AccelerationModifier);
        }
        /// <summary>
        /// Aumenta la velocidad de traslación del modelo
        /// </summary>
        /// <param name="amount">Cantidad de movimiento a añadir</param>
        public virtual void Accelerate(float amount)
        {
            this.AddToVelocity(amount);
        }
        /// <summary>
        /// Disminuye la velocidad de traslación del modelo
        /// </summary>
        public virtual void Brake()
        {
            this.AddToVelocity(-this.BrakeModifier);
        }
        /// <summary>
        /// Disminuye la velocidad de traslación del modelo
        /// </summary>
        /// <param name="amount">Cantidad de movimiento a disminuir</param>
        public virtual void Brake(float amount)
        {
            this.AddToVelocity(-amount);
        }

        /// <summary>
        /// Gira el modelo a la izquierda
        /// </summary>
        public virtual void TurnLeft()
        {
            this.m_AngularVelocity = this.AngularVelocityModifier;
        }
        /// <summary>
        /// Gira el modelo a la izquierda
        /// </summary>
        /// <param name="angle">Ángulo</param>
        public virtual void TurnLeft(float angle)
        {
            this.m_AngularVelocity = MathHelper.Clamp(angle, -this.AngularVelocityModifier, this.AngularVelocityModifier);
        }
        /// <summary>
        /// Gira el modelo a la derecha
        /// </summary>
        public virtual void TurnRight()
        {
            this.m_AngularVelocity = -this.AngularVelocityModifier;
        }
        /// <summary>
        /// Gira el modelo a la derecha
        /// </summary>
        /// <param name="angle">Ángulo</param>
        public virtual void TurnRight(float angle)
        {
            this.m_AngularVelocity = MathHelper.Clamp(-angle, -this.AngularVelocityModifier, this.AngularVelocityModifier);
        }
    }
}