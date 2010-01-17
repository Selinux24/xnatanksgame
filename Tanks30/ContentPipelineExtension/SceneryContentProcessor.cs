using System;
using System.Collections.Generic;
using GameComponents;
using GameComponents.Scenery;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Graphics;
using Physics;

namespace ContentPipelineExtension
{
    /// <summary>
    /// 
    /// </summary>
    [ContentProcessor(DisplayName = "Scenery Processor")]
    public class SceneryContentProcessor : ContentProcessor<SceneryFile, SceneryInfo>
    {
        public override SceneryInfo Process(SceneryFile input, ContentProcessorContext context)
        {
            // Cargar la textura del mapa de alturas
            Texture2DContent terrain = context.BuildAndLoadAsset<Texture2DContent, Texture2DContent>(new ExternalReference<Texture2DContent>(input.HeightMapFile), null);

            // Obtener el mapa de alturas
            HeightMap heightMap = this.BuildHeightMap(terrain, input.HeightMapCellScale, context);

            // Generar los vértices e inicializar el buffer
            VertexMultitextured[] vertList = this.BuildVertices(heightMap, input.HeightMapCellSize);
            VertexBufferContent vertexBuffer = new VertexBufferContent(VertexMultitextured.SizeInBytes * vertList.Length);
            vertexBuffer.Write<VertexMultitextured>(0, VertexMultitextured.SizeInBytes, vertList, context.TargetPlatform);

            // Generar los índices e inicializar los buffers
            double lowOrderLevels = Math.Sqrt(heightMap.Data.Length) / 2.0f;
            int levels = Convert.ToInt32(Math.Log(lowOrderLevels, 4.0d)) - 1;
            SceneryIndexInfo indices = this.BuildIndices(vertList, heightMap, levels);

            CompiledEffect effect = Effect.CompileEffectFromFile(input.EffectFile, null, null, CompilerOptions.None, context.TargetPlatform);
            Texture2DContent texture1 = context.BuildAndLoadAsset<Texture2DContent, Texture2DContent>(new ExternalReference<Texture2DContent>(input.Texture1File), null);
            Texture2DContent texture2 = context.BuildAndLoadAsset<Texture2DContent, Texture2DContent>(new ExternalReference<Texture2DContent>(input.Texture2File), null);
            Texture2DContent texture3 = context.BuildAndLoadAsset<Texture2DContent, Texture2DContent>(new ExternalReference<Texture2DContent>(input.Texture3File), null);
            Texture2DContent texture4 = context.BuildAndLoadAsset<Texture2DContent, Texture2DContent>(new ExternalReference<Texture2DContent>(input.Texture4File), null);

            return new SceneryInfo()
            {
                Terrain = terrain,
                TerrainBuffer = vertexBuffer,
                TerrainBufferVertexCount = vertList.Length,
                TerrainInfo = indices,
                Effect = effect,
                Texture1 = texture1,
                Texture2 = texture2,
                Texture3 = texture3,
                Texture4 = texture4,
            };
        }

        /// <summary>
        /// Contruye la información gráfica
        /// </summary>
        /// <param name="heightMap">Mapa de alturas</param>
        /// <param name="cellSize">Tamaño de la celda</param>
        private HeightMap BuildHeightMap(Texture2DContent terrain, float cellScale, ContentProcessorContext context)
        {
            if (terrain.Mipmaps.Count > 0)
            {
                BitmapContent heightMapContent = terrain.Mipmaps[0];

                // Sólo se soportan texturas cuadradas
                if (heightMapContent.Width == heightMapContent.Height)
                {
                    byte[] pixelData = heightMapContent.GetPixelData();

                    // Construir el mapa de alturas
                    return HeightMap.FromData(
                        pixelData,
                        heightMapContent.Height,
                        heightMapContent.Width,
                        cellScale);

                    //// Inicializar el buffer de índices
                    //m_LODIndexBuffers[LOD.High] = new IndexBuffer(
                    //    device,
                    //    sizeof(int) * indices[LOD.High].Length,
                    //    BufferUsage.None,
                    //    IndexElementSize.ThirtyTwoBits);

                    //// Establecer los índices
                    //m_LODIndexBuffers[LOD.High].SetData<int>(indices[LOD.High]);

                    //// Inicializar el buffer de índices
                    //m_LODIndexBuffers[LOD.Medium] = new IndexBuffer(
                    //    device,
                    //    sizeof(int) * indices[LOD.Medium].Length,
                    //    BufferUsage.None,
                    //    IndexElementSize.ThirtyTwoBits);

                    //// Establecer los índices
                    //m_LODIndexBuffers[LOD.Medium].SetData<int>(indices[LOD.Medium]);

                    //// Inicializar el buffer de índices
                    //m_LODIndexBuffers[LOD.Low] = new IndexBuffer(
                    //    device,
                    //    sizeof(int) * indices[LOD.Low].Length,
                    //    BufferUsage.None,
                    //    IndexElementSize.ThirtyTwoBits);

                    //// Establecer los índices
                    //m_LODIndexBuffers[LOD.Low].SetData<int>(indices[LOD.Low]);
                }
                else
                {
                    //Sólo se soportan texturas cuadradas
                    throw new NotImplementedException();
                }
            }
            else
            {
                throw new PipelineException("El archivo de mapa de alturas no tiene el formato correcto. No se encuentra la imagen");
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
            int vertexCountX = heightData.Width;
            int vertexCountZ = heightData.Deep;
            int vertexCount = vertexCountX * vertexCountZ;

            float level0 = heightData.Min;
            float level1 = heightData.Max * 0.1f;
            float level2 = heightData.Max * 0.4f;
            float level3 = heightData.Max * 0.5f;
            float level4 = heightData.Max;

            // Crear los vértices
            List<VertexMultitextured> vertList = new List<VertexMultitextured>(vertexCount);
            Vector3[,] normals = this.CreateNormals(heightData, cellSize);

            for (int width = 0; width < vertexCountX; width++)
            {
                for (int deep = 0; deep < vertexCountZ; deep++)
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

        private SceneryIndexInfo BuildIndices(VertexMultitextured[] vertList, HeightMap heightMap, int levels)
        {
            Dictionary<LOD, IndexCollection> indices = new Dictionary<LOD, IndexCollection>();

            List<SceneryTriangleNode> nodes = new List<SceneryTriangleNode>();

            IndexCollection highIndices = new IndexCollection();
            IndexCollection mediumIndices = new IndexCollection();
            IndexCollection lowIndices = new IndexCollection();

            int totalVertexesX = heightMap.Width;
            int totalVertexesZ = heightMap.Deep;

            int divisions = Convert.ToInt32(Math.Pow(4, levels));
            int divisionsX = divisions / 2;
            int divisionsZ = divisions / 2;

            int numVertexesX = (totalVertexesX / divisionsX);
            int numVertexesZ = (totalVertexesZ / divisionsZ);

            int totalVertices = totalVertexesX * totalVertexesZ;
            int totalTriangles = (totalVertexesX - 1) * (totalVertexesZ - 1) * 2;
            int totalIndices = totalTriangles * 3;

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

                    Dictionary<LOD, int> startIndexes = new Dictionary<LOD, int>();
                    startIndexes.Add(LOD.High, highIndices.Count);
                    startIndexes.Add(LOD.Medium, mediumIndices.Count);
                    startIndexes.Add(LOD.Low, lowIndices.Count);

                    Dictionary<LOD, int> triangleCount = new Dictionary<LOD, int>();
                    triangleCount.Add(LOD.High, quadHighIndices.Length / 3);
                    triangleCount.Add(LOD.Medium, quadMediumIndices.Length / 3);
                    triangleCount.Add(LOD.Low, quadLowIndices.Length / 3);

                    SceneryTriangleNode newQuadNode = new SceneryTriangleNode(quadPrimitives, startIndexes, triangleCount);
                    nodes.Add(newQuadNode);

                    highIndices.AddRange(quadHighIndices);
                    mediumIndices.AddRange(quadMediumIndices);
                    lowIndices.AddRange(quadLowIndices);
                }
            }

            indices = new Dictionary<LOD, IndexCollection>();
            indices.Add(LOD.High, highIndices);
            indices.Add(LOD.Medium, mediumIndices);
            indices.Add(LOD.Low, lowIndices);

            return new SceneryIndexInfo()
            {
                TerrainNodes = nodes.ToArray(),
                Indices = indices,
            };
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
    }
}