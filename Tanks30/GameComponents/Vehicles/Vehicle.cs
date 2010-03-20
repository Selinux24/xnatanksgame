using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameComponents.Vehicles
{
    using Common;
    using Common.Helpers;
    using GameComponents.Components.Particles;
    using GameComponents.Scenery;
    using Physics;
    using Physics.CollideCoarse;

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
        protected ContentManager Content;
        /// <summary>
        /// Controlador de físicas
        /// </summary>
        protected PhysicsController PhysicsController;

        /// <summary>
        /// Modelo
        /// </summary>
        private Model m_Model;
        /// <summary>
        /// Matriz relativa para posicionar el vehículo sobre el terreno
        /// </summary>
        private Matrix m_Offset = Matrix.Identity;
        // Posición
        //private Vector3 m_Position = Vector3.Zero;
        // Rotación
        //private Quaternion m_Rotation = Quaternion.Identity;
        // Escala
        private float m_Scale = 1f;

        /// <summary>
        /// Sistema de partículas
        /// </summary>
        private ParticleSystem m_SmokePlumeParticles;

        /// <summary>
        /// Obtiene o establece la posición
        /// </summary>
        public virtual Vector3 Position
        {
            get
            {
                if (this.m_OBB != null)
                {
                    return this.m_OBB.Position;
                }

                return Vector3.Zero;
            }
            set
            {
                if (this.m_OBB != null)
                {
                    this.m_OBB.SetPosition(value);
                }
            }
        }
        /// <summary>
        /// Obtiene o establece la rotación
        /// </summary>
        public virtual Quaternion Orientation
        {
            get
            {
                if (this.m_OBB != null)
                {
                    return this.m_OBB.Orientation;
                }

                return Quaternion.Identity;
            }
            set
            {
                if (this.m_OBB != null)
                {
                    this.m_OBB.SetOrientation(value);
                }

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
                if (this.m_OBB != null)
                {
                    return this.m_Offset * this.m_OBB.Transform;
                }

                return this.m_Offset * Matrix.Identity;
            }
        }
        /// <summary>
        /// Obtiene la vista actual desde el modelo
        /// </summary>
        public virtual Matrix CurrentPlayerControlTransform
        {
            get
            {
                if (this.m_CurrentPlayerControl != null)
                {
                    return this.m_CurrentPlayerControl.GetModelMatrix(
                        this.m_AnimationController,
                        this.CurrentTransform);
                }

                return this.CurrentTransform;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game">Juego</param>
        public Vehicle(Game game)
            : base(game)
        {
            this.Content = game.Content;
            this.PhysicsController = (PhysicsController)game.Services.GetService(typeof(PhysicsController));

            this.m_SmokePlumeParticles = new SmokePlumeParticleSystem(game);
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
            this.m_Model = Content.Load<Model>("Content/" + componentInfo.Model);
            this.m_TriangleInfo = this.m_Model.Tag as PrimitiveInfo;
            this.m_OBB = new CollisionBox(this.m_TriangleInfo.AABB, 1000f);
            this.m_Offset = Matrix.CreateTranslation(new Vector3(0f, -this.m_OBB.HalfSize.Y, 0f));

            // Velocidad máxima que puede alcanzar el tanque hacia delante
            this.MaxForwardVelocity = componentInfo.MaxForwardVelocity;
            // Velocidad máxima que puede alcanzar el tanque marcha atrás
            this.MaxBackwardVelocity = componentInfo.MaxBackwardVelocity;
            // Modificador de aceleración
            this.AccelerationModifier = this.MaxForwardVelocity / componentInfo.AccelerationModifier;
            // Modificador de frenado
            this.BrakeModifier = this.AccelerationModifier * componentInfo.BrakeModifier;
            // Velocidad angular
            this.AngularVelocityModifier = MathHelper.ToRadians(componentInfo.AngularVelocityModifier);
            // Vehículo volador
            this.Skimmer = componentInfo.Skimmer;
            // Altura máxima
            this.MaxFlightHeight = componentInfo.MaxFlightHeight;
            // Altura mínima
            this.MinFlightHeight = componentInfo.MinFlightHeight;
            // Rotación ascendente del morro
            this.AscendingAngle = MathHelper.ToRadians(componentInfo.AscendingAngle);
            // Rotación descendente del morro
            this.DescendingAngle = MathHelper.ToRadians(componentInfo.DescendingAngle);
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

            if (!this.Destroyed)
            {
                if (this.m_Autopilot.Enabled)
                {
                    // Piloto automático
                    this.m_Autopilot.UpdateAutoPilot(gameTime, this);
                }
            }

            // Controlador de animación
            this.m_AnimationController.Update(gameTime);

            // Actualizar la rotación
            //this.UpdateRotation(gameTime);

            // Actualizar la posición
            //this.UpdatePosition(gameTime);

            // Establecer la visibilidad del vehículo
            BoundingSphere sph = this.GetSPH();
            this.Visible = sph.Intersects(GlobalMatrices.gLODHighFrustum);

            // Actualizar con el terreno
            //this.UpdateWithScenery(gameTime);

            if (this.Damaged && this.Visible)
            {
                // Si el vehículo ha sido dañado mostrar humo
                this.m_SmokePlumeParticles.Visible = true;

                // Añadiendo humo
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
                Matrix modelTransform = this.CurrentTransform;

                this.m_AnimationController.CopyAbsoluteBoneTransformsTo(this.m_Model, this.m_BoneTransforms);

                foreach (ModelMesh mesh in this.m_Model.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        SceneryEnvironment.Fog.SetFogToEffect(effect);
                        //SceneryEnvironment.Ambient.SetAmbientToEffect(effect);

                        effect.View = GlobalMatrices.gViewMatrix;
                        effect.Projection = GlobalMatrices.gGlobalProjectionMatrix;
                        effect.World = this.m_BoneTransforms[mesh.ParentBone.Index] * modelTransform;
                    }

                    mesh.Draw();
                }
            }

            if (this.Damaged)
            {
                this.m_SmokePlumeParticles.SetCamera(
                    GlobalMatrices.gViewMatrix,
                    GlobalMatrices.gGlobalProjectionMatrix);
            }

#if DEBUG
            this.DrawDebug(gameTime);
#endif
        }
    }
}