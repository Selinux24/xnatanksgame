using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TanksDebug
{
    using Common;
    using Common.Primitives;
    using Physics;

    public class DemoScenery : DrawableGameComponent, IPhysicObject
    {
        /// <summary>
        /// Matriz de transformación del componente
        /// </summary>
        Matrix m_Transform = Matrix.Identity;
        /// <summary>
        /// Posición
        /// </summary>
        Vector3 m_Position = Vector3.Zero;
        /// <summary>
        /// Orientación
        /// </summary>
        Quaternion m_Rotation = Quaternion.Identity;

        /// <summary>
        /// Nombre de la textura del suelo
        /// </summary>
        string m_TextureName;
        /// <summary>
        /// Textura
        /// </summary>
        Texture2D m_Texture;

        /// <summary>
        /// UV
        /// </summary>
        VertexPositionTexture[] m_Geometry;
        /// <summary>
        /// Número de primitivas
        /// </summary>
        int m_PrimitiveCount = 0;

        /// <summary>
        /// Efecto
        /// </summary>
        BasicEffect m_Effect;
        /// <summary>
        /// Declaración
        /// </summary>
        VertexDeclaration m_Decal;
        /// <summary>
        /// Vértices
        /// </summary>
        VertexBuffer m_VertexBuffer;
        /// <summary>
        /// Indices
        /// </summary>
        IndexBuffer m_IndexBuffer;

        /// <summary>
        /// Primitiva de colisión
        /// </summary>
        private CollisionTriangleSoup m_CollisionPrimitive;

        /// <summary>
        /// Obtiene o establece la posición
        /// </summary>
        public Vector3 Position
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
        /// Obtiene o establece la orientación
        /// </summary>
        public Quaternion Rotation
        {
            get
            {
                return m_Rotation;
            }
            set
            {
                m_Rotation = value;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game">Juego</param>
        /// <param name="terrainSize">Tamaño del terreno</param>
        /// <param name="position">Posición</param>
        /// <param name="textureAssetName">Nombre de la textura</param>
        public DemoScenery(Game game, float terrainSize, Vector3 position, string textureAssetName)
            : base(game)
        {
            float size = terrainSize * 0.5f;
            Vector3 A = new Vector3(size, 10, size) + position;
            Vector3 B = new Vector3(size, 15, 0) + position;
            Vector3 C = new Vector3(size, 20, -size) + position;
            Vector3 D = new Vector3(0, 15, size) + position;
            Vector3 E = new Vector3(0, -10, 0) + position;
            Vector3 F = new Vector3(0, 15, -size) + position;
            Vector3 G = new Vector3(-size, 20, size) + position;
            Vector3 H = new Vector3(-size, 15, 0) + position;
            Vector3 I = new Vector3(-size, 10, -size) + position;

            Triangle tr1 = new Triangle(A, D, B);
            Triangle tr2 = new Triangle(B, D, E);

            Triangle tr3 = new Triangle(B, E, C);
            Triangle tr4 = new Triangle(C, E, F);

            Triangle tr5 = new Triangle(D, G, E);
            Triangle tr6 = new Triangle(E, G, H);

            Triangle tr7 = new Triangle(E, H, F);
            Triangle tr8 = new Triangle(F, H, I);

            CollisionTriangleSoup triangleSoup = new CollisionTriangleSoup(new Triangle[] { tr1, tr2, tr3, tr4, tr5, tr6, tr7, tr8 }, float.PositiveInfinity);

            this.m_CollisionPrimitive = triangleSoup;

            this.m_Geometry = new VertexPositionTexture[9];

            this.m_Geometry[0] = new VertexPositionTexture(A, new Vector2(1.0f, 0.0f));
            this.m_Geometry[1] = new VertexPositionTexture(B, new Vector2(0.5f, 0.0f));
            this.m_Geometry[2] = new VertexPositionTexture(C, new Vector2(0.0f, 0.0f));

            this.m_Geometry[3] = new VertexPositionTexture(D, new Vector2(1.0f, 0.5f));
            this.m_Geometry[4] = new VertexPositionTexture(E, new Vector2(0.5f, 0.5f));
            this.m_Geometry[5] = new VertexPositionTexture(F, new Vector2(0.0f, 0.5f));

            this.m_Geometry[6] = new VertexPositionTexture(G, new Vector2(1.0f, 1.0f));
            this.m_Geometry[7] = new VertexPositionTexture(H, new Vector2(0.5f, 1.0f));
            this.m_Geometry[8] = new VertexPositionTexture(I, new Vector2(0.0f, 1.0f));

            this.m_PrimitiveCount = 8;

            this.m_TextureName = textureAssetName;
        }

        /// <summary>
        /// Carga el contenido
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();

            this.m_Texture = this.Game.Content.Load<Texture2D>(this.m_TextureName);

            this.LoadFloor();
        }
        /// <summary>
        /// Actualiza el estado
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.m_Transform = Matrix.CreateFromQuaternion(this.m_Rotation) * Matrix.CreateTranslation(this.m_Position);
        }
        /// <summary>
        /// Dibuja el contenido
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            this.DrawFloor();
        }

        /// <summary>
        /// Carga un suelo
        /// </summary>
        private void LoadFloor()
        {
            this.m_Effect = new BasicEffect(this.GraphicsDevice, null);

            this.m_Decal = new VertexDeclaration(this.GraphicsDevice, VertexPositionTexture.VertexElements);

            List<VertexPositionTexture> points = new List<VertexPositionTexture>();

            foreach (VertexPositionTexture uv in this.m_Geometry)
            {
                points.Add(uv);
            }

            this.m_VertexBuffer = new VertexBuffer(
                GraphicsDevice,
                VertexPositionTexture.SizeInBytes * points.Count,
                BufferUsage.WriteOnly);

            this.m_VertexBuffer.SetData<VertexPositionTexture>(points.ToArray());

            List<Int16> indexes = new List<Int16>();

            if (m_Geometry.Length == 4)
            {
                indexes.Add(0);
                indexes.Add(1);
                indexes.Add(2);
                indexes.Add(1);
                indexes.Add(3);
                indexes.Add(2);
            }
            else if (m_Geometry.Length == 9)
            {
                indexes.Add(0);
                indexes.Add(3);
                indexes.Add(1);
                indexes.Add(1);
                indexes.Add(3);
                indexes.Add(4);

                indexes.Add(1);
                indexes.Add(4);
                indexes.Add(2);
                indexes.Add(2);
                indexes.Add(4);
                indexes.Add(5);

                indexes.Add(3);
                indexes.Add(6);
                indexes.Add(4);
                indexes.Add(4);
                indexes.Add(6);
                indexes.Add(7);

                indexes.Add(4);
                indexes.Add(7);
                indexes.Add(5);
                indexes.Add(5);
                indexes.Add(7);
                indexes.Add(8);
            }

            this.m_IndexBuffer = new IndexBuffer(
                this.GraphicsDevice,
                sizeof(Int16) * indexes.Count,
                BufferUsage.None,
                IndexElementSize.SixteenBits);

            this.m_IndexBuffer.SetData<Int16>(indexes.ToArray());
        }
        /// <summary>
        /// Dibuja un suelo
        /// </summary>
        private void DrawFloor()
        {
            this.GraphicsDevice.VertexDeclaration = this.m_Decal;
            this.GraphicsDevice.Vertices[0].SetSource(this.m_VertexBuffer, 0, VertexPositionTexture.SizeInBytes);
            this.GraphicsDevice.Indices = this.m_IndexBuffer;

            this.m_Effect.World = this.m_Transform * GlobalMatrices.gWorldMatrix;
            this.m_Effect.View = GlobalMatrices.gViewMatrix;
            this.m_Effect.Projection = GlobalMatrices.gGlobalProjectionMatrix;

            this.m_Effect.Texture = this.m_Texture;
            this.m_Effect.TextureEnabled = true;

            this.m_Effect.Begin();

            foreach (EffectPass pass in this.m_Effect.CurrentTechnique.Passes)
            {
                pass.Begin();

                this.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
                this.GraphicsDevice.RenderState.FillMode = FillMode.Solid;
                this.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, this.m_Geometry.Length, 0, this.m_PrimitiveCount);

                pass.End();
            }

            this.m_Effect.End();
        }

        public CollisionPrimitive GetPrimitive()
        {
            return this.m_CollisionPrimitive;
        }

        public CollisionPrimitive GetContactedPrimitive(IPhysicObject physicObject)
        {
            if (physicObject != null)
            {
                BoundingSphere otherSPH = physicObject.GetSPH();

                List<Triangle> triangleList = new List<Triangle>();

                foreach (Triangle tri in this.m_CollisionPrimitive.Triangles)
                {
                    if (otherSPH.Intersects(tri.Plane) == PlaneIntersectionType.Intersecting)
                    {
                        triangleList.Add(tri);
                    }
                }

                if (triangleList.Count > 0)
                {
                    return new CollisionTriangleSoup(triangleList.ToArray(), float.PositiveInfinity);
                }
            }

            return null;
        }

        public BoundingBox GetAABB()
        {
            return this.m_CollisionPrimitive.AABB;
        }

        public BoundingSphere GetSPH()
        {
            return this.m_CollisionPrimitive.SPH;
        }

        public bool IsActive()
        {
            return false;
        }

        public void Integrate(float time)
        {

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
