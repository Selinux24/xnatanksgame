using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TanksDebug
{
    using Common;
    using Common.Helpers;
    using GameComponents.Geometry;
    using Physics;

    /// <summary>
    /// Componente que dibuja geometr�a
    /// </summary>
    public class CubeGameComponent : DrawableGameComponent, IPhysicObject
    {
        /// <summary>
        /// Esquina menor
        /// </summary>
        public readonly Vector3 Min;
        /// <summary>
        /// Esquina mayor
        /// </summary>
        public readonly Vector3 Max;
        /// <summary>
        /// Masa
        /// </summary>
        public readonly float Mass;
        /// <summary>
        /// Caja que representa este componente
        /// </summary>
        private CollisionBox m_Box = null;
        /// <summary>
        /// Informaci�n de geometr�a
        /// </summary>
        private BufferedGeometryInfo m_Geometry;
        /// <summary>
        /// Efecto
        /// </summary>
        private BasicEffect m_BasicEffect = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game">Juego</param>
        /// <param name="min">Esquina m�nima</param>
        /// <param name="max">Esquina m�xima</param>
        /// <param name="mass">Masa</param>
        public CubeGameComponent(Game game, Vector3 min, Vector3 max, float mass)
            : base(game)
        {
            this.Min = min;
            this.Max = max;
            this.Mass = mass;
        }

        /// <summary>
        /// Carga el contenido gr�fico del componente
        /// </summary>
        protected override void LoadContent()
        {
            Vector3 halfSize = (this.Max - this.Min) / 2f;

            this.m_Box = new CollisionBox(halfSize, this.Mass);

            VertexPositionNormalTexture[] buffer = null;

            PolyGenerator.InitializeCube(out buffer, this.Min, this.Max);

            int primitiveCount = buffer.Length / 3;

            this.m_Geometry = new BufferedGeometryInfo()
            {
                FillMode = FillMode.Solid,
                Indexed = false,
                PrimitiveType = PrimitiveType.TriangleList,
                PrimitiveCount = primitiveCount,
                Vertices = buffer,
                VertexDeclaration = new VertexDeclaration(this.GraphicsDevice, VertexPositionNormalTexture.VertexElements),
                Texture = this.Game.Content.Load<Texture2D>(@"Content/crate"),
            };
            
            this.m_BasicEffect = new BasicEffect(this.GraphicsDevice, null);
            this.m_BasicEffect.EnableDefaultLighting();

            base.LoadContent();
        }
        /// <summary>
        /// Dibuja la geometr�a
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            this.m_BasicEffect.World = this.m_Box.Transform * GlobalMatrices.gWorldMatrix;
            this.m_BasicEffect.View = GlobalMatrices.gViewMatrix;
            this.m_BasicEffect.Projection = GlobalMatrices.gProjectionMatrix;

            if (this.m_Box.IsAwake)
            {
                this.m_BasicEffect.DiffuseColor = Color.BurlyWood.ToVector3();
                this.m_BasicEffect.EmissiveColor = Color.Black.ToVector3();
                this.m_BasicEffect.SpecularColor = Color.Black.ToVector3();
                this.m_BasicEffect.SpecularPower = 0f;
            }
            else
            {
                this.m_BasicEffect.DiffuseColor = Color.Gray.ToVector3();
                this.m_BasicEffect.EmissiveColor = Color.Black.ToVector3();
                this.m_BasicEffect.SpecularColor = Color.Black.ToVector3();
                this.m_BasicEffect.SpecularPower = 0f;
            }

            this.m_Geometry.Draw(gameTime, this.GraphicsDevice, this.m_BasicEffect);

#if DEBUG
            // Dibujar el AABB
            GameComponents.Debug.DebugDrawer.DrawDebugAABB(this.GraphicsDevice, this.GetAABB());
#endif
        }
        /// <summary>
        /// Establece el estado inicial de posici�n y orientaci�n del componente
        /// </summary>
        /// <param name="position">Posici�n</param>
        /// <param name="orientation">Orientaci�n</param>
        public void SetState(Vector3 position, Quaternion orientation)
        {
            this.m_Box.SetInitialState(position, orientation);
        }

        public Vector3 GetPosition()
        {
            return this.m_Box.Position;
        }
        public Quaternion GetOrientation()
        {
            return this.m_Box.Orientation;
        }
        public CollisionPrimitive GetPrimitive()
        {
            return this.m_Box;
        }
        public CollisionPrimitive GetContactedPrimitive(IPhysicObject physicObject)
        {
            if (physicObject != null)
            {
                BoundingSphere sph = physicObject.GetSPH();
                BoundingSphere thisSph = this.GetSPH();

                if (sph.Contains(thisSph) != ContainmentType.Disjoint)
                {
                    return this.m_Box;
                }
            }

            return null;
        }
        public BoundingBox GetAABB()
        {
            return this.m_Box.AABB;
        }
        public BoundingSphere GetSPH()
        {
            return this.m_Box.SPH;
        }
        public bool IsActive()
        {
            return this.m_Box.IsAwake;
        }
        public void Integrate(float time)
        {
            if (this.m_Box != null)
            {
                this.m_Box.Integrate(time);
            }
        }
        public event ObjectInContactDelegate OnObjectContacted;
        public void Contacted(IPhysicObject obj)
        {
            if (this.OnObjectContacted != null)
            {
                this.OnObjectContacted(obj);
            }
        }
    }
}