using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameComponents.Buildings
{
    using Common;
    using Common.Helpers;
    using GameComponents.Animation;
    using GameComponents.Components.Particles;
    using GameComponents.Scenery;
    using Physics;

    public abstract partial class Building : DrawableGameComponent
    {
        /// <summary>
        /// Ruta de los componentes
        /// </summary>
        protected string ComponentsDirectory;
        /// <summary>
        /// Nombre del componente
        /// </summary>
        protected string ComponentInfoName;
        /// <summary>
        /// Administrador de contenidos
        /// </summary>
        protected ContentManager Content;

        /// <summary>
        /// Modelo
        /// </summary>
        private Model m_Model;
        /// <summary>
        /// Matriz relativa para posicionar el modelo sobre el terreno
        /// </summary>
        private Matrix m_Offset = Matrix.Identity;
        /// <summary>
        /// Escala
        /// </summary>
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
        /// Indica que el componente se ha inicializado
        /// </summary>
        protected bool Initialized = false;

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
        /// Constructor
        /// </summary>
        /// <param name="game">Juego</param>
        public Building(Game game)
            : base(game)
        {
            this.Content = game.Content;

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
            BuildingComponentInfo componentInfo = BuildingComponentInfo.Load(this.ComponentsDirectory + this.ComponentInfoName);

            // Modelo
            this.m_Model = Content.Load<Model>(this.ComponentsDirectory + componentInfo.Model);
            this.m_TriangleInfo = this.m_Model.Tag as PrimitiveInfo;
            this.m_OBB = new CollisionBox(this.m_TriangleInfo.AABB, 1000000f);
            this.m_Offset = Matrix.CreateTranslation(new Vector3(0f, -this.m_OBB.HalfSize.Y, 0f));
         
            // Controles de animación
            this.m_AnimationController.AddRange(Animation.CreateAnimationList(this.m_Model, componentInfo.AnimationControlers));

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

            // Establecer la visibilidad
            BoundingSphere sph = this.GetSPH();
            this.Visible = sph.Intersects(GlobalMatrices.gLODHighFrustum);

            // Mostrar humo
            this.m_SmokePlumeParticles.Visible = true;

            // Añadiendo humo
            this.m_SmokePlumeParticles.AddParticle(this.Position, Vector3.Zero);
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

            this.m_SmokePlumeParticles.SetCamera(
                GlobalMatrices.gViewMatrix,
                GlobalMatrices.gGlobalProjectionMatrix);

#if DEBUG
            this.DrawDebug(gameTime);
#endif
        }
    }
}
