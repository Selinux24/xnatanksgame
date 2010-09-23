using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameComponents.Vehicles
{
    using Common;
    using Common.Helpers;
    using GameComponents.Geometry;
    using GameComponents.Scenery;
    using GameComponents.Weapons;
    using Physics;
    using Physics.CollideCoarse;
    using GameComponents.Particles;

    /// <summary>
    /// Componente veh�culo
    /// </summary>
    public partial class Vehicle : DrawableGameComponent
    {
        /// <summary>
        /// Diccionario de modelos de veh�culos
        /// </summary>
        private static Dictionary<string, GeometryInfo> g_ModelDictionary = new Dictionary<string, GeometryInfo>();

        /// <summary>
        /// Directorio de los componentes
        /// </summary>
        protected string AssetsFolder;
        /// <summary>
        /// Nombre del componente
        /// </summary>
        protected string ComponentInfoName;
        /// <summary>
        /// Administrador de contenidos
        /// </summary>
        protected ContentManager Content;
        /// <summary>
        /// Controlador de f�sicas
        /// </summary>
        protected PhysicsController PhysicsController;

        /// <summary>
        /// Nombre del modelo
        /// </summary>
        private string m_ModelName = null;
        /// <summary>
        /// Modelo
        /// </summary>
        public Model Model
        {
            get
            {
                return g_ModelDictionary[this.m_ModelName].Model;
            }
        }
        /// <summary>
        /// Colecci�n de tri�ngulos del modelo
        /// </summary>
        private PrimitiveInfo TriangleInfo
        {
            get
            {
                return g_ModelDictionary[this.m_ModelName].Primitives;
            }
        }
        /// <summary>
        /// Matriz relativa para posicionar el veh�culo sobre el terreno
        /// </summary>
        private Matrix m_Offset = Matrix.Identity;
        // Posici�n
        //private Vector3 m_Position = Vector3.Zero;
        // Rotaci�n
        //private Quaternion m_Rotation = Quaternion.Identity;
        // Escala
        private float m_Scale = 1f;

        /// <summary>
        /// Obtiene o establece la posici�n
        /// </summary>
        public virtual Vector3 Position
        {
            get
            {
                if (this.m_CollisionPrimitive != null)
                {
                    return this.m_CollisionPrimitive.Position;
                }

                return Vector3.Zero;
            }
            set
            {
                if (this.m_CollisionPrimitive != null)
                {
                    this.m_CollisionPrimitive.SetPosition(value);
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
                if (this.m_CollisionPrimitive != null)
                {
                    return this.m_CollisionPrimitive.Orientation;
                }

                return Quaternion.Identity;
            }
            set
            {
                if (this.m_CollisionPrimitive != null)
                {
                    this.m_CollisionPrimitive.SetOrientation(value);
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
        /// Indica si el veh�culo tiene el foco
        /// </summary>
        public bool HasFocus = false;

        /// <summary>
        /// Obtiene la matriz mundo actual del modelo
        /// </summary>
        public virtual Matrix CurrentTransform
        {
            get
            {
                if (this.m_CollisionPrimitive != null)
                {
                    return this.m_Offset * this.m_CollisionPrimitive.Transform;
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
        }

        /// <summary>
        /// Carga de contenido gr�fico
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();

            // Informaci�n del componente
            VehicleComponentInfo componentInfo = VehicleComponentInfo.Load(Path.Combine(this.AssetsFolder, this.ComponentInfoName));

            // Modelo
            this.m_ModelName = componentInfo.Model;

            if (!g_ModelDictionary.ContainsKey(componentInfo.Model))
            {
                Model model = Content.Load<Model>(Path.Combine(this.AssetsFolder, this.m_ModelName));
                PrimitiveInfo primitives = model.Tag as PrimitiveInfo;

                GeometryInfo geometry = new GeometryInfo()
                {
                    Model = model,
                    Primitives = primitives,
                };

                g_ModelDictionary.Add(this.m_ModelName, geometry);
            }

            CollisionBox box = new CollisionBox(this.TriangleInfo.AABB, 1000f);

            this.m_CollisionPrimitive = box;
            this.m_Offset = Matrix.CreateTranslation(new Vector3(0f, -box.HalfSize.Y, 0f));

            // Integridad
            this.BaseHull = this.Hull = componentInfo.Hull;
            // Blindaje
            this.BaseArmor = this.Armor = componentInfo.Armor;

            // Altura m�xima de vuelo
            this.Engine.InitialMaxFlightHeight = componentInfo.MaxFlightHeight;
            // Altura m�nima de vuelo
            this.Engine.InitialMinFlightHeight = componentInfo.MinFlightHeight;

            // Velocidad m�xima que puede alcanzar el veh�culo hacia delante
            this.Engine.MaxForwardVelocity = componentInfo.MaxForwardVelocity;
            // Velocidad m�xima que puede alcanzar el veh�culo marcha atr�s
            this.Engine.MaxBackwardVelocity = componentInfo.MaxBackwardVelocity;
            // Modificador de aceleraci�n
            this.Engine.AccelerationModifier = componentInfo.AccelerationModifier;
            // Modificador de frenado
            this.Engine.BrakeModifier = componentInfo.BrakeModifier;
            // Velocidad angular
            this.Engine.AngularVelocityModifier = MathHelper.ToRadians(componentInfo.AngularVelocityModifier);
            // Veh�culo volador
            this.Engine.Skimmer = componentInfo.Skimmer;
            // Altura m�xima
            this.Engine.MaxFlightHeight = componentInfo.MaxFlightHeight;
            // Altura m�nima
            this.Engine.MinFlightHeight = componentInfo.MinFlightHeight;
            // Rotaci�n ascendente del morro
            this.Engine.AscendingAngle = MathHelper.ToRadians(componentInfo.AscendingAngle);
            // Rotaci�n descendente del morro
            this.Engine.DescendingAngle = MathHelper.ToRadians(componentInfo.DescendingAngle);
            // Controles de animaci�n
            this.m_AnimationController.AddRange(Animation.Animation.CreateAnimationList(this.Model, componentInfo.AnimationControlers));
            // Posiciones
            this.m_PlayerControlList.AddRange(Animation.PlayerPosition.CreatePlayerPositionList(this.Model, componentInfo.PlayerPositions));
            // Armas
            this.m_WeapontList.AddRange(Weapon.CreateWeaponList(this.Model, componentInfo.Weapons));
            // Emisores de part�culas
            this.m_ParticleEmitterList.AddRange(ParticleEmitter.CreateParticleEmitterList(this.Model, componentInfo.ParticleEmitters));

            // Transformaciones iniciales
            this.m_BoneTransforms = new Matrix[this.Model.Bones.Count];
        }
        /// <summary>
        /// Actualiza el estado del componente
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!this.IsDestroyed)
            {
                if (this.m_Autopilot.Enabled)
                {
                    // Piloto autom�tico
                    this.m_Autopilot.UpdateAutoPilot(gameTime, this);
                }
            }

            //Motor
            this.Engine.UpdateVehicle(this);
            if (this.Engine.Moving)
            {
                this.Primitive.Activate();

                this.FireAccelerating();
            }

            // Controlador de animaci�n
            this.m_AnimationController.Update(gameTime);

            // Establecer la visibilidad del veh�culo
            BoundingSphere sph = this.SPH;
            this.Visible = sph.Intersects(GlobalMatrices.gLODHighFrustum);

            this.PositionHasChanged = false;
            this.RotationHasChanged = false;
            this.ScaleHasChanged = false;

            //Part�culas
            ParticleManager particleManager = this.Game.Services.GetService<ParticleManager>();
            if (particleManager != null)
            {
                this.UpdateParticles(particleManager);
            }
        }
        /// <summary>
        /// Dibujar
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (this.Model != null)
            {
                Matrix modelTransform = this.CurrentTransform;

                this.m_AnimationController.CopyAbsoluteBoneTransformsTo(this.Model, this.m_BoneTransforms);

                foreach (ModelMesh mesh in this.Model.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.World = this.m_BoneTransforms[mesh.ParentBone.Index] * modelTransform;
                        effect.View = GlobalMatrices.gViewMatrix;
                        effect.Projection = GlobalMatrices.gProjectionMatrix;

                        SceneryEnvironment.Fog.SetFogToEffect(effect);

                        if (!this.IsDestroyed)
                        {
                            SceneryEnvironment.Ambient.SetLightToEffect(effect);
                        }
                        else
                        {
                            SceneryEnvironment.Ambient.SetLightToEffect(effect, 0.1f);
                        }
                    }

                    mesh.Draw();
                }
            }

#if DEBUG
            this.DrawDebug(gameTime);
#endif
        }
    }
}