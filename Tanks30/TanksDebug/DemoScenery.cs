using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TanksDebug
{
    using Common;
    using Common.Components;
    using Common.Primitives;
    using Physics;

    public class DemoScenery : DrawableGameComponent, IScenery
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
            Vector3 A = new Vector3(size, 15, size) + position;
            Vector3 B = new Vector3(size, 15, 0) + position;
            Vector3 C = new Vector3(size, 15, -size) + position;
            Vector3 D = new Vector3(0, -15, size) + position;
            Vector3 E = new Vector3(0, 0, 0) + position;
            Vector3 F = new Vector3(0, -15, -size) + position;
            Vector3 G = new Vector3(-size, 15, size) + position;
            Vector3 H = new Vector3(-size, 15, 0) + position;
            Vector3 I = new Vector3(-size, 15, -size) + position;

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

            //FillMode previous = this.GraphicsDevice.RenderState.FillMode;
            //this.GraphicsDevice.RenderState.FillMode = FillMode.WireFrame;

            this.DrawFloor();

            //this.GraphicsDevice.RenderState.FillMode = previous;
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
            this.m_Effect.Projection = GlobalMatrices.gProjectionMatrix;

            this.m_Effect.Texture = this.m_Texture;
            this.m_Effect.TextureEnabled = true;

            this.m_Effect.Begin();

            foreach (EffectPass pass in this.m_Effect.CurrentTechnique.Passes)
            {
                pass.Begin();

                this.GraphicsDevice.DrawIndexedPrimitives(
                    PrimitiveType.TriangleList,
                    0,
                    0,
                    this.m_Geometry.Length,
                    0,
                    this.m_PrimitiveCount);

                pass.End();
            }

            this.m_Effect.End();
        }

        /// <summary>
        /// Obtiene la posición
        /// </summary>
        /// <returns>Devuelve la posición</returns>
        public Vector3 GetPosition()
        {
            return this.m_Position;
        }
        /// <summary>
        /// Obtiene la orientación
        /// </summary>
        /// <returns>Devuelve la orientación</returns>
        public Quaternion GetOrientation()
        {
            return this.m_Rotation;
        }
        /// <summary>
        /// Obtiene la primitiva de colisión
        /// </summary>
        /// <returns>Devuelve la primitiva de colisión del cuerpo</returns>
        public CollisionPrimitive GetPrimitive()
        {
            return this.m_CollisionPrimitive;
        }
        /// <summary>
        /// Obtiene la primitiva de colisión realizando el test de colisión con el cuerpo especificado
        /// </summary>
        /// <param name="physicObject">Cuerpo</param>
        /// <returns>Devuelve la primitiva de colisión en contacto con el cuerpo especificado</returns>
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
        /// <summary>
        /// Obtiene el AABB circundante del cuerpo
        /// </summary>
        /// <returns>Devuelve el AABB circundante del cuerpo</returns>
        public BoundingBox GetAABB()
        {
            return this.m_CollisionPrimitive.AABB;
        }
        /// <summary>
        /// Obtiene la esfera circundante del cuerpo
        /// </summary>
        /// <returns>Devuelve la esfera circundante del cuerpo</returns>
        public BoundingSphere GetSPH()
        {
            return this.m_CollisionPrimitive.SPH;
        }
        /// <summary>
        /// Indica si el cuerpo está activo
        /// </summary>
        /// <returns>Devuelve verdadero si el cuerpo está activo</returns>
        public bool IsActive()
        {
            return false;
        }
        /// <summary>
        /// Integra el cuerpo en el motor de físicas
        /// </summary>
        /// <param name="time">Tiempo</param>
        public void Integrate(float time)
        {

        }
   
        /// <summary>
        /// Evento que se produce cuando un cuerpo hace contacto con el objeto actual
        /// </summary>
        public event ObjectInContactDelegate Contacted;
        /// <summary>
        /// Ocurre cuando un objeto se activa
        /// </summary>
        public event ObjectStateHandler Activated;
        /// <summary>
        /// Ocurre cuando un objeto se desactiva
        /// </summary>
        public event ObjectStateHandler Deactivated;

        /// <summary>
        /// Cuando el vehículo es contactado por otro, se notifica el causante del contacto
        /// </summary>
        /// <param name="obj">Objeto que ha contactado con el vehículo actual</param>
        public void SetContactedWith(IPhysicObject obj)
        {
            if (this.Contacted != null)
            {
                this.Contacted(obj);
            }
        }
        /// <summary>
        /// Disparador del evento de activación
        /// </summary>
        private void FireActivated()
        {
            if (this.Activated != null)
            {
                this.Activated(this);
            }
        }
        /// <summary>
        /// Disparador del evento de desactivación
        /// </summary>
        private void FireDeactivated()
        {
            if (this.Deactivated != null)
            {
                this.Deactivated(this);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public float? GetHeigthAtPoint(float x, float z)
        {
            float maxY = 1000f;

            Vector3 point = new Vector3(x, maxY, z);

            // Crear un rayo desde el punto hacia abajo
            Ray ray = new Ray(point, Vector3.Down);

            // Intersectar con cada triángulo
            foreach (Triangle triangle in this.m_CollisionPrimitive.Triangles)
            {
                float? distance = ray.Intersects(triangle.Plane);
                if (distance.HasValue)
                {
                    Vector3 p = new Vector3(x, maxY - distance.Value, z);

                    if (Triangle.PointInTriangle(triangle, p))
                    {
                        return p.Y;
                    }
                }
            }

            return null;
        }
    }
}
