using System;
using System.Collections.Generic;
using Common.Format;
using Common.Helpers;
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
    /// Procesador de contenidos de mapa de alturas
    /// </summary>
    [ContentProcessor(DisplayName = "Scenery - Heightmap Processor")]
    public class SceneryContentProcessor : ContentProcessor<SceneryFile, SceneryInfo>
    {
        /// <summary>
        /// Procesar el mapa de alturas
        /// </summary>
        /// <param name="input">Información del escenario de entrada</param>
        /// <param name="context">Contexto de procesado</param>
        /// <returns>Devuelve la información de escenario leída</returns>
        public override SceneryInfo Process(SceneryFile input, ContentProcessorContext context)
        {
            // Cargar la textura del mapa de alturas
            Texture2DContent terrain = context.BuildAndLoadAsset<Texture2DContent, Texture2DContent>(new ExternalReference<Texture2DContent>(input.HeightMapFile), null);

            // Obtener el mapa de alturas
            HeightMap heightMap = this.BuildHeightMap(terrain, input.HeightMapCellScale, context);

            // Generar los vértices e inicializar el buffer
            VertexMultitextured[] vertList = heightMap.BuildVertices(input.HeightMapCellSize);
            VertexBufferContent vertexBuffer = new VertexBufferContent(VertexMultitextured.SizeInBytes * vertList.Length);
            vertexBuffer.Write<VertexMultitextured>(0, VertexMultitextured.SizeInBytes, vertList, context.TargetPlatform);

            // Generar los índices e inicializar los buffers
            double lowOrderLevels = Math.Sqrt(heightMap.DataLength) / 2.0f;
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
        /// Construye el mapa de alturas a partir de la textura especificada
        /// </summary>
        /// <param name="terrain">Textura con el mapa de alturas del terreno</param>
        /// <param name="cellScale">Escala de alturas</param>
        /// <param name="context">Contexto</param>
        /// <returns>Devuelve el mapa de alturas generado</returns>
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