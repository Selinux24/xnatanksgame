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

        // Posici�n
        private Vector3 m_Position = Vector3.Zero;
        // Rotaci�n
        private Quaternion m_Rotation = Quaternion.Identity;
        // Escala
        private float m_Scale = 1f;

        // Sistema de part�culas
        private ParticleSystem m_SmokePlumeParticles;

        /// <summary>
        /// Obtiene o establece la posici�n
        /// </summary>
        public virtual Vector3 Position
        {
            get
            {
                return this.m_Position;
            }
            set
            {
                this.m_Position = value;
            }
        }
        /// <summary>
        /// Obtiene o establece la rotaci�n
        /// </summary>
        public virtual Quaternion Rotation
        {
            get
            {
                return this.m_Rotation;
            }
            set
            {
                this.m_Rotation = value;

                this.m_Direction = Vector3.Transform(Vector3.Forward, this.m_Rotation);

                this.RotationHasChanged = true;
            }
        }
        /// <summary>
        /// Obtiene o establece la escala
        /// </summary>
        public virtual float Scale
        {
            get
            {
                return this.m_Scale;
            }
            set
            {
                this.m_Scale = value;
            }
        }

        /// <summary>
        /// Indica que el veh�culo se ha inicializado
        /// </summary>
        protected bool Initialized = false;
        /// <summary>
        /// Indica si la posici�n ha sido modificada desde la �ltima actualizaci�n
        /// </summary>
        protected bool PositionHasChanged = false;
        /// <summary>
        /// Indica si la rotaci�n ha sido modificada desde la �ltima actualizaci�n
        /// </summary>
        protected bool RotationHasChanged = false;
        /// <summary>
        /// Indica si la escala ha sido modificada desde la �ltimo actualizaci�n
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
        /// Obtiene la matriz de transformaci�n del modelo
        /// </summary>
        /// <returns>Devuelve la matriz de transformaci�n del modelo</returns>
        protected virtual Matrix GetTransform()
        {
            Matrix world = Matrix.CreateScale(this.Scale);
            world *= Matrix.CreateFromQuaternion(this.Rotation);
            world *= Matrix.CreateFromQuaternion(this.m_Inclination);
            world *= Matrix.CreateTranslation(this.Position);

            return world;
        }
        /// <summary>
        /// Obtiene la matriz de vista desde la posici�n actual del modelo
        /// </summary>
        /// <returns>Devuelve la matriz de vista desde la posici�n actual del modelo</returns>
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
        /// Carga de contenido gr�fico
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();

            // Informaci�n del componente
            VehicleComponentInfo componentInfo = VehicleComponentInfo.Load("Content/" + this.ComponentInfoName);

            // Modelo
            this.m_Model = ContentManager.Load<Model>("Content/" + componentInfo.Model);

            // Velocidad m�xima que puede alcanzar el tanque hacia delante
            this.MaxForwardVelocity = componentInfo.MaxForwardVelocity;
            // Velocidad m�xima que puede alcanzar el tanque marcha atr�s
            this.MaxBackwardVelocity = componentInfo.MaxBackwardVelocity;
            // Modificador de aceleraci�n
            this.AccelerationModifier = MaxForwardVelocity / componentInfo.AccelerationModifier;
            // Modificador de frenado
            this.BrakeModifier = AccelerationModifier * componentInfo.BrakeModifier;
            // Velocidad angular
            this.AngularVelocityModifier = MathHelper.ToRadians(componentInfo.AngularVelocityModifier);
            // Veh�culo volador
            this.Skimmer = componentInfo.Skimmer;
            // Altura m�xima
            this.MaxFlightHeight = componentInfo.MaxFlightHeight;
            // Altura m�nima
            this.MinFlightHeight = componentInfo.MinFlightHeight;
            // Rotaci�n ascendente del morro
            this.AscendingAngle = MathHelper.ToRadians(componentInfo.AscendingAngle);
            // Rotaci�n descendente del morro
            this.DescendingAngle = MathHelper.ToRadians(componentInfo.DescendingAngle);
            // Controles de animaci�n
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

            if (!this.Destroyed)
            {
                if (m_Autopilot.Enabled)
                {
                    // Piloto autom�tico
                    m_Autopilot.UpdateAutoPilot(this);
                }
            }

            // Controlador de animaci�n
            this.m_AnimationController.Update(gameTime);

            // Actualizar la rotaci�n
            this.UpdateRotation(gameTime);

            // Actualizar la posici�n
            this.UpdatePosition(gameTime);

            // Establecer la visibilidad del veh�culo
            this.Visible = this.TransformedBSph.Intersects(BaseCameraGameComponent.gLODHighFrustum);

            // Actualizar con el terreno
            this.UpdateWithScenery(gameTime);

            if (this.Damaged && this.Visible)
            {
                // Si el veh�culo ha sido da�ado mostrar humo
                this.m_SmokePlumeParticles.Visible = true;

                // A�adiendo humo
                this.m_SmokePlumeParticles.AddParticle(this.Position, Vector3.Zero);
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

            if (this.Damaged)
            {
                this.m_SmokePlumeParticles.SetCamera(
                    BaseCameraGameComponent.gViewMatrix,
                    BaseCameraGameComponent.gGlobalProjectionMatrix);
            }
        }

        /// <summary>
        /// Aumenta la velocidad de traslaci�n del modelo
        /// </summary>
        public virtual void Accelerate()
        {
            if (!this.Destroyed)
            {
                this.AddToVelocity(this.AccelerationModifier);
            }
        }
        /// <summary>
        /// Aumenta la velocidad de traslaci�n del modelo
        /// </summary>
        /// <param name="amount">Cantidad de movimiento a a�adir</param>
        public virtual void Accelerate(float amount)
        {
            if (!this.Destroyed)
            {
                this.AddToVelocity(amount);
            }
        }
        /// <summary>
        /// Disminuye la velocidad de traslaci�n del modelo
        /// </summary>
        public virtual void Brake()
        {
            if (!this.Destroyed)
            {
                this.AddToVelocity(-this.BrakeModifier);
            }
        }
        /// <summary>
        /// Disminuye la velocidad de traslaci�n del modelo
        /// </summary>
        /// <param name="amount">Cantidad de movimiento a disminuir</param>
        public virtual void Brake(float amount)
        {
            if (!this.Destroyed)
            {
                this.AddToVelocity(-amount);
            }
        }

        /// <summary>
        /// Gira el modelo a la izquierda
        /// </summary>
        public virtual void TurnLeft()
        {
            if (!this.Destroyed)
            {
                this.m_AngularVelocity = this.AngularVelocityModifier;
            }
        }
        /// <summary>
        /// Gira el modelo a la izquierda
        /// </summary>
        /// <param name="angle">�ngulo</param>
        public virtual void TurnLeft(float angle)
        {
            if (!this.Destroyed)
            {
                this.m_AngularVelocity = MathHelper.Clamp(angle, -this.AngularVelocityModifier, this.AngularVelocityModifier);
            }
        }
        /// <summary>
        /// Gira el modelo a la derecha
        /// </summary>
        public virtual void TurnRight()
        {
            if (!this.Destroyed)
            {
                this.m_AngularVelocity = -this.AngularVelocityModifier;
            }
        }
        /// <summary>
        /// Gira el modelo a la derecha
        /// </summary>
        /// <param name="angle">�ngulo</param>
        public virtual void TurnRight(float angle)
        {
            if (!this.Destroyed)
            {
                this.m_AngularVelocity = MathHelper.Clamp(-angle, -this.AngularVelocityModifier, this.AngularVelocityModifier);
            }
        }
    }
}