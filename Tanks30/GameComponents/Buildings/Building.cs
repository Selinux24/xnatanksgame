using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameComponents.Buildings
{
    using Common;
    using Common.Helpers;
    using GameComponents.Animation;
    using GameComponents.Geometry;
    using GameComponents.Scenery;
    using Physics;

    public abstract partial class Building : DrawableGameComponent
    {
        /// <summary>
        /// Diccionario de modelos de vehículos
        /// </summary>
        private static Dictionary<string, GeometryInfo> g_ModelDictionary = new Dictionary<string, GeometryInfo>();

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
        /// Colección de triángulos del modelo
        /// </summary>
        private PrimitiveInfo TriangleInfo
        {
            get
            {
                return g_ModelDictionary[this.m_ModelName].Primitives;
            }
        }
        /// <summary>
        /// Matriz relativa para posicionar el modelo sobre el terreno
        /// </summary>
        private Matrix m_Offset = Matrix.Identity;
        /// <summary>
        /// Escala
        /// </summary>
        private float m_Scale = 1f;

        /// <summary>
        /// Obtiene o establece la posición
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
        /// Obtiene o establece la rotación
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
                if (this.m_CollisionPrimitive != null)
                {
                    return this.m_Offset * this.m_CollisionPrimitive.Transform;
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
            this.m_ModelName = componentInfo.Model;

            if (!g_ModelDictionary.ContainsKey(componentInfo.Model))
            {
                Model model = Content.Load<Model>(this.ComponentsDirectory + this.m_ModelName);
                PrimitiveInfo primitives = model.Tag as PrimitiveInfo;

                GeometryInfo geometry = new GeometryInfo()
                {
                    Model = model,
                    Primitives = primitives,
                };

                g_ModelDictionary.Add(this.m_ModelName, geometry);
            }

            CollisionBox obb = new CollisionBox(this.TriangleInfo.AABB, 1000000f);

            this.m_CollisionPrimitive = obb;
            this.m_Offset = Matrix.CreateTranslation(new Vector3(0f, -obb.HalfSize.Y, 0f));
         
            // Controles de animación
            this.m_AnimationController.AddRange(Animation.CreateAnimationList(this.Model, componentInfo.AnimationControlers));

            // Transformaciones iniciales
            this.m_BoneTransforms = new Matrix[Model.Bones.Count];
        }
        /// <summary>
        /// Actualiza el estado del componente
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Establecer la visibilidad
            BoundingSphere sph = this.SPH;
            this.Visible = sph.Intersects(GlobalMatrices.gLODHighFrustum);
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

                        SceneryEnvironment.Ambient.SetLightToEffect(effect);
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
