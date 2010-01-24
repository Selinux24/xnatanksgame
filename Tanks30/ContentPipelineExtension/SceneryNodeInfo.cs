using System;
using System.Collections.Generic;
using Common.Components;
using Common.Format;
using GameComponents.Scenery;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Physics;

namespace ContentPipelineExtension
{
    /// <summary>
    /// Información de nodos del terreno
    /// </summary>
    public class SceneryNodeInfo
    {
        /// <summary>
        /// Nodos del terreno
        /// </summary>
        public SceneryTriangleNode[] Nodes;
        /// <summary>
        /// Diccionario de índices del terreno por nivel de detalle
        /// </summary>
        public Dictionary<LOD, IndexCollection> Indices;

        /// <summary>
        /// Construye la información de nodos del terreno
        /// </summary>
        /// <param name="vertList">Lista de vértices</param>
        /// <param name="width">Anchura del terreno</param>
        /// <param name="deep">Profundidad del terreno</param>
        /// <param name="levels">Niveles distintos de detalle</param>
        /// <returns>Devuelve la información de nodos del terreno</returns>
        public static SceneryNodeInfo Build(VertexMultitextured[] vertList, int width, int deep, int levels)
        {
            // Lista resultante de nodos
            List<SceneryTriangleNode> nodes = new List<SceneryTriangleNode>();

            // Diccionario de índices
            Dictionary<LOD, IndexCollection> indices = new Dictionary<LOD, IndexCollection>();

            // Colección de índices de alta, media y baja resolución
            IndexCollection highIndices = new IndexCollection();
            IndexCollection mediumIndices = new IndexCollection();
            IndexCollection lowIndices = new IndexCollection();

            // Tamaño de la cuadrícula
            int totalVertexesX = width;
            int totalVertexesZ = deep;

            // Número de divisiones
            int divisions = Convert.ToInt32(Math.Pow(4, levels));
            int divisionsX = divisions / 2;
            int divisionsZ = divisions / 2;

            // Número de vértices en X y en Z
            int numVertexesX = (totalVertexesX / divisionsX);
            int numVertexesZ = (totalVertexesZ / divisionsZ);

            // Número total de vértices, triángulos e índices
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

                    // Crear los índices para alta resolución
                    int[] quadHighIndices = SceneryNodeInfo.BuildQuadtreeIndices(
                        LOD.High,
                        initialX,
                        initialZ,
                        nodeVertexesX,
                        nodeVertexesZ,
                        totalVertexesX,
                        totalVertexesZ);

                    // Crear los índices para media resolución
                    int[] quadMediumIndices = SceneryNodeInfo.BuildQuadtreeIndices(
                        LOD.Medium,
                        initialX,
                        initialZ,
                        nodeVertexesX,
                        nodeVertexesZ,
                        totalVertexesX,
                        totalVertexesZ);

                    // Crear los índices para baja resolución
                    int[] quadLowIndices = SceneryNodeInfo.BuildQuadtreeIndices(
                        LOD.Low,
                        initialX,
                        initialZ,
                        nodeVertexesX,
                        nodeVertexesZ,
                        totalVertexesX,
                        totalVertexesZ);

                    // Crear la lista de triángulos
                    Triangle[] quadTriangles = SceneryNodeInfo.BuildPrimitiveList(vertList, quadHighIndices);

                    // Diccionario de índice de inicio para cada resolución
                    Dictionary<LOD, int> startIndexes = new Dictionary<LOD, int>();
                    startIndexes.Add(LOD.High, highIndices.Count);
                    startIndexes.Add(LOD.Medium, mediumIndices.Count);
                    startIndexes.Add(LOD.Low, lowIndices.Count);

                    // Diccionario de cantidad de triángulos para cada resolución
                    Dictionary<LOD, int> triangleCount = new Dictionary<LOD, int>();
                    triangleCount.Add(LOD.High, quadHighIndices.Length / 3);
                    triangleCount.Add(LOD.Medium, quadMediumIndices.Length / 3);
                    triangleCount.Add(LOD.Low, quadLowIndices.Length / 3);

                    // Crear el nodo con los triángulos para la colisión, los índices de inicio y el número de triángulos para cada resolución
                    SceneryTriangleNode newQuadNode = new SceneryTriangleNode(quadTriangles, startIndexes, triangleCount);
                    nodes.Add(newQuadNode);

                    // Añadir los índices a la colección de índices para cada resolución
                    highIndices.AddRange(quadHighIndices);
                    mediumIndices.AddRange(quadMediumIndices);
                    lowIndices.AddRange(quadLowIndices);
                }
            }

            // Crear el diccionario de buffers de índices para cada resolución
            indices = new Dictionary<LOD, IndexCollection>();
            indices.Add(LOD.High, highIndices);
            indices.Add(LOD.Medium, mediumIndices);
            indices.Add(LOD.Low, lowIndices);

            // Devolver la información de nodos del escenario
            return new SceneryNodeInfo()
            {
                Nodes = nodes.ToArray(),
                Indices = indices,
            };
        }
        /// <summary>
        /// Construye la información de colisión con la lista de vértices y la lista de índices
        /// </summary>
        /// <param name="vertList">Lista de vértices</param>
        /// <param name="indices">Lista de índices</param>
        /// <returns>Devuelve la lista de triángulos</returns>
        private static Triangle[] BuildPrimitiveList(VertexMultitextured[] vertList, int[] indices)
        {
            List<Triangle> triangleList = new List<Triangle>();

            for (int i = 0; i < indices.Length; i += 3)
            {
                Vector3 vertex1 = vertList[indices[i + 0]].Position;
                Vector3 vertex2 = vertList[indices[i + 1]].Position;
                Vector3 vertex3 = vertList[indices[i + 2]].Position;

                Triangle newPrimitive = new Triangle(vertex1, vertex2, vertex3);

                triangleList.Add(newPrimitive);
            }

            return triangleList.ToArray();
        }
        /// <summary>
        /// Construye los índices de un solo nodo del quadtree
        /// </summary>
        /// <param name="lod">Nivel de detalle</param>
        /// <param name="initialVertexesX">Vértice de X desde el que se comienzan a leer los índices</param>
        /// <param name="initialVertexesZ">Vértice de Z desde el que se comienzan a leer los índices</param>
        /// <param name="numVertexesX">Vértices de X en el nodo</param>
        /// <param name="numVertexesZ">Vértices de Z en el nodo</param>
        /// <param name="totalVertexesX">Vertices totales en X</param>
        /// <param name="totalVertexesZ">Vertices totales en Z</param>
        /// <returns>Lista de índices para el nivel de detalle especificado</returns>
        private static int[] BuildQuadtreeIndices(
            LOD lod,
            int initialVertexesX,
            int initialVertexesZ,
            int numVertexesX,
            int numVertexesZ,
            int totalVertexesX,
            int totalVertexesZ)
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
            return indicesList.ToArray();
        }
    }
}
