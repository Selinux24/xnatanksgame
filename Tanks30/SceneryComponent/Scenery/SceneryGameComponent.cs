using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Physics;
using CustomProcessors;
using GameComponents.Camera;

namespace GameComponents.Scenery
{
    /// <summary>
    /// Escenario
    /// </summary>
    public class SceneryGameComponent : DrawableGameComponent
    {
        /// <summary>
        /// Vértice para multitextura
        /// </summary>
        public struct VertexMultitextured
        {
            /// <summary>
            /// Posición
            /// </summary>
            public Vector3 Position;
            /// <summary>
            /// Normal
            /// </summary>
            public Vector3 Normal;
            /// <summary>
            /// Coordenadas de Textura
            /// </summary>
            public Vector4 TextureCoordinate;
            /// <summary>
            /// Pesos de textura
            /// </summary>
            public Vector4 TexWeights;

            /// <summary>
            /// Tamaño en bytes
            /// </summary>
            public static int SizeInBytes = (3 + 3 + 4 + 4) * 4;
            /// <summary>
            /// Elementos del vértice
            /// </summary>
            public static VertexElement[] VertexElements = new VertexElement[]
             {
                 new VertexElement( 0, 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, 0 ),
                 new VertexElement( 0, sizeof(float) * 3, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Normal, 0 ),
                 new VertexElement( 0, sizeof(float) * 6, VertexElementFormat.Vector4, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0 ),
                 new VertexElement( 0, sizeof(float) * 10, VertexElementFormat.Vector4, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 1 ),
             };
        }

        /// <summary>
        /// Mapa de alturas
        /// </summary>
        public class HeightMap
        {
            /// <summary>
            /// Alturas
            /// </summary>
            public readonly float[,] Data;
            /// <summary>
            /// Altura mínima
            /// </summary>
            public readonly float Min;
            /// <summary>
            /// Altura máxima
            /// </summary>
            public readonly float Max;

            /// <summary>
            /// Anchura
            /// </summary>
            public int Width
            {
                get
                {
                    if (this.Data != null)
                    {
                        return this.Data.GetLength(0);
                    }

                    return 0;
                }
            }
            /// <summary>
            /// Profundidad
            /// </summary>
            public int Deep
            {
                get
                {
                    if (this.Data != null)
                    {
                        return this.Data.GetLength(1);
                    }

                    return 0;
                }
            }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="data">Mapa de alturas</param>
            public HeightMap(float[,] data)
            {
                this.Data = data;

                foreach (float height in data)
                {
                    if (height < Min)
                    {
                        Min = height;
                    }

                    if (height > Max)
                    {
                        Max = height;
                    }
                }
            }
        }

        // Contenidos
        private ContentManager content = null;
        // Efecto para renderizar
        private Effect m_Effect;

        // Declaración de los vértices
        private VertexDeclaration m_VertexDeclaration;
        // Buffer de vértices
        private VertexBuffer m_VertexBuffer = null;
        // Buffers de índices
        Dictionary<LOD, IndexBuffer> m_LODIndexBuffers = new Dictionary<LOD, IndexBuffer>();
        // Cantidad total de vértices
        private int m_VertexCount = 0;
        // Cantidad total de vértices de ancho
        private int m_VertexCountX = 0;
        // Cantidad total de vértices de largo
        private int m_VertexCountZ = 0;

        // Nodo principal del escenario
        private SceneryNode m_Root = null;

        /// <summary>
        /// Obtiene la anchura del escenario
        /// </summary>
        public float Width
        {
            get
            {
                if (m_Root != null)
                {
                    Vector3 pointA = new Vector3(m_Root.BoundingBox.Min.X, 0.0f, 0.0f);
                    Vector3 pointB = new Vector3(m_Root.BoundingBox.Max.X, 0.0f, 0.0f);

                    return Vector3.Distance(pointA, pointB);
                }

                return 0.0f;
            }
        }
        /// <summary>
        /// Obtiene la largura del escenario
        /// </summary>
        public float Long
        {
            get
            {
                if (m_Root != null)
                {
                    Vector3 pointA = new Vector3(0.0f, 0.0f, m_Root.BoundingBox.Min.Z);
                    Vector3 pointB = new Vector3(0.0f, 0.0f, m_Root.BoundingBox.Max.Z);

                    return Vector3.Distance(pointA, pointB);
                }

                return 0.0f;
            }
        }
        /// <summary>
        /// Obtiene la altura del escenario
        /// </summary>
        public float Height
        {
            get
            {
                if (m_Root != null)
                {
                    Vector3 pointA = new Vector3(0.0f, m_Root.BoundingBox.Min.Y, 0.0f);
                    Vector3 pointB = new Vector3(0.0f, m_Root.BoundingBox.Max.Y, 0.0f);

                    return Vector3.Distance(pointA, pointB);
                }

                return 0.0f;
            }
        }

        /// <summary>
        /// Obtiene el límite mínimo de anchura
        /// </summary>
        public float MinWidth
        {
            get
            {
                if (m_Root != null)
                {
                    return m_Root.BoundingBox.Min.X;
                }

                return 0.0f;
            }
        }
        /// <summary>
        /// Obtiene el límite máximo de anchura
        /// </summary>
        public float MaxWidth
        {
            get
            {
                if (m_Root != null)
                {
                    return m_Root.BoundingBox.Max.X;
                }

                return 0.0f;
            }
        }
        /// <summary>
        /// Obtiene el límite mínimo de profundidad
        /// </summary>
        public float MinLong
        {
            get
            {
                if (m_Root != null)
                {
                    return m_Root.BoundingBox.Min.Z;
                }

                return 0.0f;
            }
        }
        /// <summary>
        /// Obtiene el límite máximo de profundidad
        /// </summary>
        public float MaxLong
        {
            get
            {
                if (m_Root != null)
                {
                    return m_Root.BoundingBox.Max.Z;
                }

                return 0.0f;
            }
        }
        /// <summary>
        /// Obtiene el límite mínimo de altura
        /// </summary>
        public float MinHeight
        {
            get
            {
                if (m_Root != null)
                {
                    return m_Root.BoundingBox.Min.Y;
                }

                return 0.0f;
            }
        }
        /// <summary>
        /// Obtiene el límite máximo de altura
        /// </summary>
        public float MaxHeight
        {
            get
            {
                if (m_Root != null)
                {
                    return m_Root.BoundingBox.Max.Y;
                }

                return 0.0f;
            }
        }
        /// <summary>
        /// Obtiene el punto central del terreno
        /// </summary>
        public Vector3 Center
        {
            get
            {
                return new Vector3((MaxWidth - MinWidth) / 2.0f, (MaxHeight - MinHeight) / 2.0f, (MaxLong - MinLong) / 2.0f);
            }
        }

        #region DEV

        // Indica el nivel de detalle a dibujar
        private LOD m_Lod = LOD.None;
        /// <summary>
        /// Obtiene o establece el nivel de detalle a dibujar
        /// </summary>
        public LOD Lod
        {
            get
            {
                return m_Lod;
            }
            set
            {
                m_Lod = value;
            }
        }
        // Indica el modo de relleno a usar
        private FillMode m_FillMode = FillMode.Solid;
        /// <summary>
        /// Obtiene o establece el modo de relleno a usar
        /// </summary>
        public FillMode FillMode
        {
            get
            {
                return m_FillMode;
            }
            set
            {
                m_FillMode = value;
            }
        }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public SceneryGameComponent(Game game)
            : base(game)
        {
            content = (ContentManager)game.Services.GetService(typeof(ContentManager));
        }

        /// <summary>
        /// Inicializar
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }
        /// <summary>
        /// Cargar el contenito del componente
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();

            // Crear el terreno
            this.BuildGeometry(content.Load<Texture2D>(@"Content\Terrain\HM2"), 32.0f);

            // Crear el efecto para renderizar
            if (m_Effect == null)
            {
                this.m_Effect = content.Load<Effect>(@"Content\Terrain\terrain");

                this.m_Effect.Parameters["xSandTexture"].SetValue(content.Load<Texture2D>(@"Content\Terrain\sand"));
                this.m_Effect.Parameters["xGrassTexture"].SetValue(content.Load<Texture2D>(@"Content\Terrain\grass"));
                this.m_Effect.Parameters["xRockTexture"].SetValue(content.Load<Texture2D>(@"Content\Terrain\rock"));
                this.m_Effect.Parameters["xSnowTexture"].SetValue(content.Load<Texture2D>(@"Content\Terrain\snow"));
            }
        }
        /// <summary>
        /// Actualizar el estado del componente
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
        /// <summary>
        /// Dibujar el contenido gráfico del componente
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public override void Draw(GameTime gameTime)
        {
            if (m_Effect != null)
            {
                if (m_Root != null)
                {
                    FillMode currentMode = this.GraphicsDevice.RenderState.FillMode;
                    this.GraphicsDevice.RenderState.FillMode = m_FillMode;

                    this.GraphicsDevice.VertexDeclaration = m_VertexDeclaration;
                    this.GraphicsDevice.Vertices[0].SetSource(m_VertexBuffer, 0, VertexMultitextured.SizeInBytes);

                    // Establecer los parámetros en el efecto
                    this.m_Effect.CurrentTechnique = m_Effect.Techniques["MultiTextured"];

                    this.m_Effect.Parameters["xProjection"].SetValue(BaseCameraGameComponent.gGlobalProjectionMatrix);
                    this.m_Effect.Parameters["xView"].SetValue(BaseCameraGameComponent.gViewMatrix);
                    this.m_Effect.Parameters["xWorld"].SetValue(Matrix.Identity);

                    this.m_Effect.Parameters["xEnableLighting"].SetValue(SceneryEnvironment.Ambient.Light0Enable);
                    this.m_Effect.Parameters["xLightDirection"].SetValue(SceneryEnvironment.Ambient.Light0Direction);
                    this.m_Effect.Parameters["xAmbient"].SetValue(0.2f);

                    //SceneryEnvironment.Fog.SetFogToEffect(m_Effect as BasicEffect);

                    // Inicializar los nodos para el dibujado
                    m_Root.PrepareForDrawing();

                    // Dibujar el nivel de detall máximo
                    LODDraw(gameTime, LOD.High);
                    // Dibujar el nivel de detalle medio
                    LODDraw(gameTime, LOD.Medium);
                    // Dibujar el nivel de menos detalle
                    LODDraw(gameTime, LOD.Low);

                    this.GraphicsDevice.RenderState.FillMode = currentMode;
                }
            }
        }
        /// <summary>
        /// Dibuja el contenido gráfico del componente atendiendo al nivel de detalle
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        /// <param name="lod">Nivel de detalle</param>
        private void LODDraw(GameTime gameTime, LOD lod)
        {
            SceneryNode[] nodesToDraw = m_Root.GetNodesToDraw(lod);
            if (nodesToDraw.Length > 0)
            {
                if ((m_Lod != LOD.None) && (m_Lod != lod))
                {
                    return;
                }

                this.GraphicsDevice.Indices = m_LODIndexBuffers[lod];

                m_Effect.Begin();

                foreach (EffectPass pass in m_Effect.CurrentTechnique.Passes)
                {
                    pass.Begin();

                    foreach (SceneryNode node in nodesToDraw)
                    {
                        SceneryTriangleNode triNode = node as SceneryTriangleNode;
                        if (triNode != null)
                        {
                            this.GraphicsDevice.DrawIndexedPrimitives(
                                PrimitiveType.TriangleList,
                                0,
                                0,
                                m_VertexCount,
                                triNode.GetStartIndex(lod),
                                triNode.GetPrimitiveCount(lod));
                        }
                    }

                    pass.End();
                }

                m_Effect.End();
            }
        }

        /// <summary>
        /// Obtiene el nodo bajo las coordenadas especificadas
        /// </summary>
        /// <param name="x">Componente x</param>
        /// <param name="z">Componente z</param>
        /// <returns>Devuelve el nodo bajo las coordenadas especificadas o null si no existe</returns>
        internal SceneryTriangleNode GetNode(float x, float z)
        {
            return GetNode(x, z, m_Root);
        }
        /// <summary>
        /// Obtiene el subnodo bajo las coordenadas especificadas en el nodo especificado
        /// </summary>
        /// <param name="x">Coordenada x</param>
        /// <param name="z">Coordenada z</param>
        /// <param name="node">Nodo en el que buscar el subnodo</param>
        /// <returns>Devuelve el subnodo del nodo especificado bajo las coordenadas x, z o null si no existe</returns>
        private SceneryTriangleNode GetNode(float x, float z, SceneryNode node)
        {
            if (node != null)
            {
                if (node.HasChilds)
                {
                    if (node.NorthEast.Contains(x, z) != ContainmentType.Disjoint)
                    {
                        return GetNode(x, z, node.NorthEast);
                    }
                    else if (node.NorthWest.Contains(x, z) != ContainmentType.Disjoint)
                    {
                        return GetNode(x, z, node.NorthWest);
                    }
                    else if (node.SouthEast.Contains(x, z) != ContainmentType.Disjoint)
                    {
                        return GetNode(x, z, node.SouthEast);
                    }
                    else if (node.SouthWest.Contains(x, z) != ContainmentType.Disjoint)
                    {
                        return GetNode(x, z, node.SouthWest);
                    }
                }
                else
                {
                    return node as SceneryTriangleNode;
                }
            }

            return null;
        }
        /// <summary>
        /// Obtiene si existe intersección entre el ray y el escenario
        /// </summary>
        /// <param name="x">Componente x</param>
        /// <param name="z">Componente z</param>
        /// <param name="triangle">Devuelve el triángulo de intersección si existe</param>
        /// <param name="intersectionPoint">Devuelve el punto de intersección en el triángulo</param>
        /// <param name="distanceToPoint">Devuelve la distancia entre el punto de intersección y el origen del rayo</param>
        /// <returns>Devuelve verdadero si existe intersección o falso en el resto de los casos</returns>
        public bool Intersects(float x, float z, out Triangle? triangle, out Vector3? intersectionPoint, out float? distanceToPoint)
        {
            Vector3 point = new Vector3(x, MinHeight, z);

            Ray ray = new Ray(point, Vector3.Up);

            return Intersects(ray, out triangle, out intersectionPoint, out distanceToPoint);
        }
        /// <summary>
        /// Obtiene si existe intersección entre el ray y el escenario
        /// </summary>
        /// <param name="ray">Rayo</param>
        /// <param name="triangle">Devuelve el triángulo de intersección si existe</param>
        /// <param name="intersectionPoint">Devuelve el punto de intersección en el triángulo</param>
        /// <param name="distanceToPoint">Devuelve la distancia entre el punto de intersección y el origen del rayo</param>
        /// <returns>Devuelve verdadero si existe intersección o falso en el resto de los casos</returns>
        public bool Intersects(Ray ray, out Triangle? triangle, out Vector3? intersectionPoint, out float? distanceToPoint)
        {
            triangle = null;
            intersectionPoint = null;
            distanceToPoint = null;

            SceneryTriangleNode node = this.GetNode(ray.Position.X, ray.Position.Z);
            if (node != null)
            {
                return node.Intersects(ray, out triangle, out intersectionPoint, out distanceToPoint);
            }

            return false;
        }

        /// <summary>
        /// Obtiene una lista de nodos dibujados en el último frame
        /// </summary>
        /// <returns>Devuelve la lista de nodos dibujados en el último frame, o null si no se ha inicializado</returns>
        internal SceneryInfoNodeDrawn[] GetNodesDrawn(LOD lod)
        {
            if (m_Root != null)
            {
                return m_Root.GetNodesDrawn(lod);
            }

            return new SceneryInfoNodeDrawn[] { };
        }

        /// <summary>
        /// Contruye la información gráfica
        /// </summary>
        /// <param name="heightMap">Mapa de alturas</param>
        /// <param name="cellSize">Tamaño de la celda</param>
        private void BuildGeometry(Texture2D heightMap, float cellSize)
        {
            // Sólo se soportan texturas cuadradas
            if (heightMap.Width == heightMap.Height)
            {
                // Extraer el mapa de alturas desde la textura
                Int32[] data = new Int32[heightMap.Width * heightMap.Height];
                heightMap.GetData<Int32>(data);
                HeightMap heightData = ConvertARGBtoFloatHeightData(data, 2.5f);

                double lowOrderLevels = Math.Sqrt(heightData.Data.Length) / 2.0f;
                int levels = Convert.ToInt32(Math.Log(lowOrderLevels, 4.0d));

                // Construir los vértices
                VertexMultitextured[] vertList = this.BuildVertices(heightData, cellSize);

                // Crear la declaración de los vértices
                m_VertexDeclaration = new VertexDeclaration(
                    this.GraphicsDevice,
                    VertexMultitextured.VertexElements);

                // Inicializar el buffer de vértices
                //m_VertexBuffer = new VertexBuffer(
                //    this.GraphicsDevice,
                //    VertexMultitextured.SizeInBytes * vertList.Length,
                //    ResourceUsage.WriteOnly,
                //    ResourceManagementMode.Automatic);
                m_VertexBuffer = new VertexBuffer(
                    this.GraphicsDevice,
                    VertexMultitextured.SizeInBytes * vertList.Length,
                    BufferUsage.WriteOnly);

                // Establecer los vértices del buffer de vértices
                m_VertexBuffer.SetData<VertexMultitextured>(vertList);

                // Construir los índices
                Dictionary<LOD, int[]> indices = null;
                this.BuildIndices(
                    vertList,
                    LOD.Medium,
                    m_VertexCountX,
                    m_VertexCountZ,
                    levels,
                    out indices);

                // Inicializar el buffer de índices
                //m_LODIndexBuffers[LOD.High] = new IndexBuffer(
                //    this.GraphicsDevice,
                //    sizeof(int) * indices[LOD.High].Length,
                //    ResourceUsage.None,
                //    ResourceManagementMode.Automatic,
                //    IndexElementSize.ThirtyTwoBits);
                m_LODIndexBuffers[LOD.High] = new IndexBuffer(
                    this.GraphicsDevice,
                    sizeof(int) * indices[LOD.High].Length,
                    BufferUsage.None,
                    IndexElementSize.ThirtyTwoBits);

                // Establecer los índices
                m_LODIndexBuffers[LOD.High].SetData<int>(indices[LOD.High]);

                // Inicializar el buffer de índices
                //m_LODIndexBuffers[LOD.Medium] = new IndexBuffer(
                //    this.GraphicsDevice,
                //    sizeof(int) * indices[LOD.Medium].Length,
                //    ResourceUsage.None,
                //    ResourceManagementMode.Automatic,
                //    IndexElementSize.ThirtyTwoBits);
                m_LODIndexBuffers[LOD.Medium] = new IndexBuffer(
                    this.GraphicsDevice,
                    sizeof(int) * indices[LOD.Medium].Length,
                    BufferUsage.None,
                    IndexElementSize.ThirtyTwoBits);

                // Establecer los índices
                m_LODIndexBuffers[LOD.Medium].SetData<int>(indices[LOD.Medium]);

                // Inicializar el buffer de índices
                //m_LODIndexBuffers[LOD.Low] = new IndexBuffer(
                //    this.GraphicsDevice,
                //    sizeof(int) * indices[LOD.Low].Length,
                //    ResourceUsage.None,
                //    ResourceManagementMode.Automatic,
                //    IndexElementSize.ThirtyTwoBits);
                m_LODIndexBuffers[LOD.Low] = new IndexBuffer(
                    this.GraphicsDevice,
                    sizeof(int) * indices[LOD.Low].Length,
                    BufferUsage.None,
                    IndexElementSize.ThirtyTwoBits);

                // Establecer los índices
                m_LODIndexBuffers[LOD.Low].SetData<int>(indices[LOD.Low]);
            }
            else
            {
                //Sólo se soportan texturas cuadradas
                throw new NotImplementedException();
            }
        }
        /// <summary>
        /// Construye los vértices
        /// </summary>
        /// <param name="heightData">Mapa de alturas</param>
        /// <param name="cellSize">Tamaño de la celda</param>
        /// <returns>Devuelve la colección de vértices</returns>
        private VertexMultitextured[] BuildVertices(HeightMap heightData, float cellSize)
        {
            // Contador de vértices
            m_VertexCountX = heightData.Width;
            m_VertexCountZ = heightData.Deep;
            m_VertexCount = m_VertexCountX * m_VertexCountZ;

            float level0 = heightData.Min;
            float level1 = heightData.Max * 0.1f;
            float level2 = heightData.Max * 0.4f;
            float level3 = heightData.Max * 0.5f;
            float level4 = heightData.Max;

            // Crear los vértices
            List<VertexMultitextured> vertList = new List<VertexMultitextured>(m_VertexCount);
            Vector3[,] normals = CreateNormals(heightData, cellSize);

            for (int width = 0; width < m_VertexCountX; width++)
            {
                for (int deep = 0; deep < m_VertexCountZ; deep++)
                {
                    VertexMultitextured newVertex = new VertexMultitextured();

                    float posX = width * cellSize;
                    float posY = heightData.Data[deep, width];
                    float posZ = deep * cellSize;

                    newVertex.Position = new Vector3(posX, posY, posZ);
                    newVertex.Normal = normals[deep, width];
                    newVertex.TextureCoordinate.X = (float)width / 10.0f;
                    newVertex.TextureCoordinate.Y = (float)deep / 10.0f;

                    float twX = (posY < level1) ? (posY / level1) : 0.0f;
                    float twY = (posY < level2) ? (posY / level2) : 0.0f;
                    float twZ = (posY < level3) ? (posY / level3) : 0.0f;
                    float twW = (posY < level4) ? (posY / level4) : 0.0f;
                    float totalWeight = twX + twY + twZ + twW;

                    newVertex.TexWeights.X = twX / totalWeight;
                    newVertex.TexWeights.Y = twY / totalWeight;
                    newVertex.TexWeights.Z = twZ / totalWeight;
                    newVertex.TexWeights.W = twW / totalWeight;

                    vertList.Add(newVertex);
                }
            }

            return vertList.ToArray();
        }
        /// <summary>
        /// Crea las normales del escenario
        /// </summary>
        /// <param name="heightData">Mapa de alturas</param>
        /// <param name="cellSize">Tamaño de la celda</param>
        /// <returns>Devuelve la lista de normales del escenario</returns>
        private Vector3[,] CreateNormals(HeightMap heightData, float cellSize)
        {
            // Obtener las dimensiones del mapa de alturas
            int width = heightData.Width;
            int deep = heightData.Deep;

            // Inicalizar la lista de normales
            Vector3[,] normals = new Vector3[width, deep];
            if (normals != null)
            {
                // Recorrer e inicializar la lista de normales
                for (int y = 0; y < width; y++)
                {
                    for (int x = 0; x < deep; x++)
                    {
                        if (x == 0 || y == 0 || x == (deep - 1) || y == (width - 1))
                        {
                            // Los vértices del borde son siempre el vector de altura uno
                            normals[y, x] = Vector3.Up;
                        }
                        else
                        {
                            Vector3 pos = new Vector3(cellSize * x, heightData.Data[y, x], cellSize * y);
                            Vector3 pos2;
                            Vector3 pos3;
                            Vector3 norm1;
                            Vector3 norm2;
                            Vector3 norm3;
                            Vector3 norm4;
                            Vector3 norm5;
                            Vector3 norm6;

                            pos2 = new Vector3(cellSize * (x), heightData.Data[y - 1, x], cellSize * (y - 1));
                            pos3 = new Vector3(cellSize * (x - 1), heightData.Data[y, x - 1], cellSize * (y));
                            pos2 -= pos;
                            pos3 -= pos;
                            pos2.Normalize();
                            pos3.Normalize();
                            norm1 = Vector3.Cross(pos2, pos3);

                            pos2 = new Vector3(cellSize * (x - 1), heightData.Data[y, x - 1], cellSize * (y));
                            pos3 = new Vector3(cellSize * (x - 1), heightData.Data[y + 1, x - 1], cellSize * (y + 1));
                            pos2 -= pos;
                            pos3 -= pos;
                            pos2.Normalize();
                            pos3.Normalize();
                            norm2 = Vector3.Cross(pos2, pos3);

                            pos2 = new Vector3(cellSize * (x - 1), heightData.Data[y + 1, x - 1], cellSize * (y + 1));
                            pos3 = new Vector3(cellSize * (x), heightData.Data[y + 1, x], cellSize * (y + 1));
                            pos2 -= pos;
                            pos3 -= pos;
                            pos2.Normalize();
                            pos3.Normalize();
                            norm3 = Vector3.Cross(pos2, pos3);

                            pos2 = new Vector3(cellSize * (x), heightData.Data[y + 1, x], cellSize * (y + 1));
                            pos3 = new Vector3(cellSize * (x + 1), heightData.Data[y, x + 1], cellSize * (y));
                            pos2 -= pos;
                            pos3 -= pos;
                            pos2.Normalize();
                            pos3.Normalize();
                            norm4 = Vector3.Cross(pos2, pos3);

                            pos2 = new Vector3(cellSize * (x + 1), heightData.Data[y, x + 1], cellSize * (y));
                            pos3 = new Vector3(cellSize * (x + 1), heightData.Data[y - 1, x + 1], cellSize * (y - 1));
                            pos2 -= pos;
                            pos3 -= pos;
                            pos2.Normalize();
                            pos3.Normalize();
                            norm5 = Vector3.Cross(pos2, pos3);

                            pos2 = new Vector3(cellSize * (x + 1), heightData.Data[y - 1, x + 1], cellSize * (y - 1));
                            pos3 = new Vector3(cellSize * (x), heightData.Data[y - 1, x], cellSize * (y - 1));
                            pos2 -= pos;
                            pos3 -= pos;
                            pos2.Normalize();
                            pos3.Normalize();
                            norm6 = Vector3.Cross(pos2, pos3);

                            Vector3 norm = (norm1 + norm2 + norm3 + norm4 + norm5 + norm6) / 6.0f;

                            normals[y, x] = Vector3.Normalize(norm);
                        }
                    }
                }

                return normals;
            }
            else
            {
                throw new Exception("La memoria es insuficiente");
            }
        }
        /// <summary>
        /// Convierte los datos de una textura en formato ARGB en un mapa de alturas
        /// </summary>
        /// <param name="data">Datos</param>
        /// <param name="width">Anchura</param>
        /// <param name="height">Altura</param>
        /// <param name="scale">Escala</param>
        /// <returns>Devuelve el mapa de alturas obtenido</returns>
        /// <remarks>El resultado es siempre un número de vértices que forman una rejilla de cuadrados que son potencia de 2. Por ejemplo, una imagen de 512x512 se convierte en 513x513 vértices que forman 512x512 cuadrados</remarks>
        private HeightMap ConvertARGBtoFloatHeightData(Int32[] data, float scale)
        {
            int size = data.Length;
            int width = Convert.ToInt32(Math.Sqrt(size)) + 1;
            int height = Convert.ToInt32(Math.Sqrt(size)) + 1;

            float[,] result = new float[height, width];

            int dataX = 0;
            int dataZ = 0;
            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < height; z++)
                {
                    dataX = x;
                    dataZ = z;

                    if (x == width - 1)
                    {
                        dataX = x - 1;
                    }
                    if (z == height - 1)
                    {
                        dataZ = z - 1;
                    }

                    int index = (dataX * (width - 1)) + dataZ;

                    result[x, z] = (float)(data[index] & 0x000000FF) * scale;
                }
            }

            return new HeightMap(result);
        }
        /// <summary>
        /// Construye los índices
        /// </summary>
        /// <param name="vertList">Lista de vértices</param>
        /// <param name="totalVertexesZ">Vertices totales en Z</param>
        /// <param name="totalVertexesX">Vertices totales en X</param>
        /// <param name="levels">Niveles del quadtree</param>
        /// <returns>Devuelve la lista de índices</returns>
        private void BuildIndices(
            VertexMultitextured[] vertList,
            LOD lod,
            int totalVertexesX,
            int totalVertexesZ,
            int levels,
            out Dictionary<LOD, int[]> indices)
        {
            int divisions = Convert.ToInt32(Math.Pow(4, levels - 1));
            int divisionsX = divisions / 2;
            int divisionsZ = divisions / 2;

            int numVertexesX = (totalVertexesX / divisionsX);
            int numVertexesZ = (totalVertexesZ / divisionsZ);

            int totalVertices = totalVertexesX * totalVertexesZ;
            int totalTriangles = (totalVertexesX - 1) * (totalVertexesZ - 1) * 2;
            int totalIndices = totalTriangles * 3;

            List<SceneryNode> nodes = new List<SceneryNode>();

            List<int> highIndices = new List<int>();
            List<int> mediumIndices = new List<int>();
            List<int> lowIndices = new List<int>();

            for (int x = 0; x < divisionsX; x++)
            {
                int initialX = x * numVertexesX;
                int nodeVertexesX = numVertexesX;

                for (int z = 0; z < divisionsZ; z++)
                {
                    int initialZ = z * numVertexesZ;
                    int nodeVertexesZ = numVertexesZ;

                    int[] quadHighIndices = null;
                    this.BuildQuadtreeIndices(
                        LOD.High,
                        initialX,
                        initialZ,
                        nodeVertexesX,
                        nodeVertexesZ,
                        totalVertexesX,
                        totalVertexesZ,
                        out quadHighIndices);

                    int[] quadMediumIndices = null;
                    this.BuildQuadtreeIndices(
                        LOD.Medium,
                        initialX,
                        initialZ,
                        nodeVertexesX,
                        nodeVertexesZ,
                        totalVertexesX,
                        totalVertexesZ,
                        out quadMediumIndices);

                    int[] quadLowIndices = null;
                    this.BuildQuadtreeIndices(
                        LOD.Low,
                        initialX,
                        initialZ,
                        nodeVertexesX,
                        nodeVertexesZ,
                        totalVertexesX,
                        totalVertexesZ,
                        out quadLowIndices);

                    TriangleList quadPrimitives = null;
                    this.BuildPrimitiveList(
                        vertList,
                        quadHighIndices,
                        out quadPrimitives);

                    SceneryTriangleNode newQuadNode = new SceneryTriangleNode(this);
                    newQuadNode.Build(quadPrimitives);
                    newQuadNode.SetRenderParams(LOD.High, highIndices.Count, quadHighIndices.Length / 3);
                    newQuadNode.SetRenderParams(LOD.Medium, mediumIndices.Count, quadMediumIndices.Length / 3);
                    newQuadNode.SetRenderParams(LOD.Low, lowIndices.Count, quadLowIndices.Length / 3);

                    nodes.Add(newQuadNode);

                    highIndices.AddRange(quadHighIndices);
                    mediumIndices.AddRange(quadMediumIndices);
                    lowIndices.AddRange(quadLowIndices);
                }
            }

            this.m_Root = BuildQuadtree(nodes.ToArray());

            indices = new Dictionary<LOD, int[]>();
            indices.Add(LOD.High, highIndices.ToArray());
            indices.Add(LOD.Medium, mediumIndices.ToArray());
            indices.Add(LOD.Low, lowIndices.ToArray());
        }
        /// <summary>
        /// Construye los índices de un solo nodo del quadtree
        /// </summary>
        /// <param name="vertList">Lista de vértices</param>
        /// <param name="initialVertexesZ">Vértice de Z desde el que se comienzan a leer los índices</param>
        /// <param name="initialVertexesX">Vértice de X desde el que se comienzan a leer los índices</param>
        /// <param name="numVertexesZ">Vértices de Z en el nodo</param>
        /// <param name="numVertexesX">Vértices de X en el nodo</param>
        /// <param name="totalVertexesZ">Vertices totales en Z</param>
        /// <param name="totalVertexesX">Vertices totales en X</param>
        /// <param name="indices">Lista de índices resultado</param>
        /// <param name="primitives">LIsta de primitivas resultado</param>
        private void BuildQuadtreeIndices(
            LOD lod,
            int initialVertexesX,
            int initialVertexesZ,
            int numVertexesX,
            int numVertexesZ,
            int totalVertexesX,
            int totalVertexesZ,
            out int[] indices)
        {
            int level = (int)lod;

            int numTriangles = ((numVertexesX - 1) / level) * ((numVertexesZ - 1) / level) * 2;
            int numIndices = numTriangles * 3;

            List<int> indicesList = new List<int>(numIndices);

            for (int x = initialVertexesX; x < (initialVertexesX + numVertexesX); x += level)
            {
                for (int z = initialVertexesZ; z < (initialVertexesZ + numVertexesZ); z += level)
                {
                    int vertStart = (z * totalVertexesX) + x;

                    // Indices del triángulo superior: Cuadrado A, B, C, D
                    int vertexA = (vertStart);
                    int vertexB = (vertStart) + (1 * level);
                    int vertexC = (vertStart) + (totalVertexesX * level);
                    int vertexD = (vertStart) + ((totalVertexesX + 1) * level);

                    // Establecer los índices para el triángulo superior
                    indicesList.Add(vertexA);
                    indicesList.Add(vertexC);
                    indicesList.Add(vertexB);

                    // Establecer los índices para el triángulo inferior
                    indicesList.Add(vertexB);
                    indicesList.Add(vertexC);
                    indicesList.Add(vertexD);
                }
            }

            // Informar la lista resultado de índices
            indices = indicesList.ToArray();
        }
        /// <summary>
        /// Construye la información de colisión con la lista de vértices y la lista de índices
        /// </summary>
        /// <param name="vertList"></param>
        /// <param name="indices"></param>
        /// <param name="primitives"></param>
        private void BuildPrimitiveList(
            VertexMultitextured[] vertList,
            int[] indices,
            out TriangleList primitives)
        {
            List<Triangle> primitiveList = new List<Triangle>();

            for (int i = 0; i < indices.Length; i += 3)
            {
                Vector3 vertex1 = vertList[indices[i + 0]].Position;
                Vector3 vertex2 = vertList[indices[i + 1]].Position;
                Vector3 vertex3 = vertList[indices[i + 2]].Position;

                Triangle newPrimitive = new Triangle(vertex1, vertex2, vertex3);

                primitiveList.Add(newPrimitive);
            }

            // Crear la información de colisión
            primitives = new TriangleList(primitiveList.ToArray());
        }
        /// <summary>
        /// Construye el quadtree
        /// </summary>
        /// <param name="nodes">Nodos</param>
        /// <returns>Devuelve el nodo raíz del quadtree construido</returns>
        private SceneryNode BuildQuadtree(SceneryNode[] nodes)
        {
            SceneryNode[] resultNodes = nodes;

            while (resultNodes.Length > 1)
            {
                resultNodes = BuildQuadtreeNodes(resultNodes);
            }

            return resultNodes[0];
        }
        /// <summary>
        /// Construye los nodos de un quadtree según el nivel
        /// </summary>
        /// <param name="nodes">Nodos</param>
        /// <returns>Devuelve la lista de nodos superiores del quadtree</returns>
        private SceneryNode[] BuildQuadtreeNodes(SceneryNode[] nodes)
        {
            int upperNodeCount = nodes.Length / 4;
            int upperNodeSide = Convert.ToInt32(Math.Sqrt(nodes.Length));

            List<SceneryNode> resultNodes = new List<SceneryNode>();
            for (int y = 0; y < upperNodeSide; y += 2)
            {
                for (int x = 0; x < upperNodeSide; x += 2)
                {
                    int start = (y * upperNodeSide) + x;

                    int indexA = start;
                    int indexB = start + 1;
                    int indexC = start + upperNodeSide;
                    int indexD = start + upperNodeSide + 1;

                    //Asignar nodos al nuevo nodo
                    SceneryNode newNode = new SceneryNode(this);

                    newNode.Build(
                        nodes[indexA],
                        nodes[indexB],
                        nodes[indexC],
                        nodes[indexD]);

                    resultNodes.Add(newNode);
                }
            }

            return resultNodes.ToArray();
        }
    }
}
