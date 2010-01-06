using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameComponents.Vehicles
{
    using GameComponents.Camera;
    using GameComponents.Components.Particles;
    using GameComponents.Scenery;
    using Physics;

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
        /// Escenario
        /// </summary>
        protected SceneryGameComponent Scenery = null;

        /// <summary>
        /// Modelo
        /// </summary>
        private Model m_Model;

        // Posici�n
        //private Vector3 m_Position = Vector3.Zero;
        // Rotaci�n
        //private Quaternion m_Rotation = Quaternion.Identity;
        // Escala
        private float m_Scale = 1f;

        /// <summary>
        /// Sistema de part�culas
        /// </summary>
        private ParticleSystem m_SmokePlumeParticles;

        /// <summary>
        /// Obtiene o establece la posici�n
        /// </summary>
        public virtual Vector3 Position
        {
            get
            {
                //return this.m_Position;
                if (this.OBB != null)
                {
                    return this.OBB.Body.Position;
                }

                return Vector3.Zero;
            }
            set
            {
                //this.m_Position = value;
                if (this.OBB != null)
                {
                    this.OBB.Body.Position = value;

                    this.OBB.Body.CalculateDerivedData();
                }
            }
        }
        /// <summary>
        /// Obtiene o establece la rotaci�n
        /// </summary>
        public virtual Quaternion Orientation
        {
            get
            {
                //return this.m_Rotation;
                if (this.OBB != null)
                {
                    return this.OBB.Body.Orientation;
                }

                return Quaternion.Identity;
            }
            set
            {
                //this.m_Rotation = value;

                //this.m_Direction = Vector3.Transform(Vector3.Forward, this.m_Rotation);

                if (this.OBB != null)
                {
                    this.OBB.Body.Orientation = value;

                    this.OBB.Body.CalculateDerivedData();
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
                //Matrix world = Matrix.CreateScale(this.Scale);
                //world *= Matrix.CreateFromQuaternion(this.Rotation);
                //world *= Matrix.CreateFromQuaternion(this.m_Inclination);
                //world *= Matrix.CreateTranslation(this.Position);

                //return world;

                if (this.OBB != null)
                {
                    return this.OBB.Transform;
                }

                return Matrix.Identity;
            }
        }
        /// <summary>
        /// Obtiene la vista actual desde el modelo
        /// </summary>
        public virtual Matrix CurrentView
        {
            get
            {
                return this.m_CurrentPlayerControl.GetViewMatrix(this.m_AnimationController, this.CurrentTransform);
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
            this.Scenery = (SceneryGameComponent)game.Services.GetService(typeof(SceneryGameComponent));

            this.m_SmokePlumeParticles = new SmokePlumeParticleSystem(game);
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
            this.m_Model = Content.Load<Model>("Content/" + componentInfo.Model);
            this.m_TriangleInfo = this.m_Model.Tag as TriangleInfo;
            this.m_Bsph = new BoundingSphere()
            {
                Center = this.m_TriangleInfo.BSph.Center,
                Radius = this.m_TriangleInfo.BSph.Radius,
            };
            this.m_OBB = new CollisionBox()
            {
                HalfSize = this.m_TriangleInfo.OBB.HalfSize,
            };

            this.m_OBB.Body.Mass = 100f;
            this.m_OBB.Body.SetDamping(0.8f, 0.8f);

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
                if (this.m_Autopilot.Enabled)
                {
                    // Piloto autom�tico
                    this.m_Autopilot.UpdateAutoPilot(this);
                }
            }

            // Controlador de animaci�n
            this.m_AnimationController.Update(gameTime);

            // Actualizar la rotaci�n
            //this.UpdateRotation(gameTime);

            // Actualizar la posici�n
            //this.UpdatePosition(gameTime);

            // Establecer la visibilidad del veh�culo
            this.Visible = this.TransformedBSph.Intersects(BaseCameraGameComponent.gLODHighFrustum);

            // Actualizar con el terreno
            //this.UpdateWithScenery(gameTime);

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
                Matrix modelTransform = this.CurrentTransform;

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
    }
}