using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

namespace ContentPipelineExtension
{
    using Common.Components;
    using Common.Format;
    using Common.Primitives;

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

            IndexCollection highIndices = new IndexCollection();
            IndexCollection mediumIndices = new IndexCollection();
            IndexCollection lowIndices = new IndexCollection();

            // Tamaño de la cuadrícula
            int totalCellsX = width;
            int totalCellsZ = deep;

            // Número de divisiones
            int divisions = Convert.ToInt32(Math.Pow(4, levels));
            int divisionsX = divisions / 2;
            int divisionsZ = divisions / 2;

            // Número de vértices en X y en Z
            int numVertexesX = (totalCellsX / divisionsX);
            int numVertexesZ = (totalCellsZ / divisionsZ);

            // Número total de vértices, triángulos e índices
            int totalVertices = totalCellsX * totalCellsZ;
            int totalTriangles = (totalCellsX - 1) * (totalCellsZ - 1) * 2;
            int totalIndices = totalTriangles * 3;

            for (int x = 0; x < divisionsX; x++)
            {
                int offsetX = x * numVertexesX;
                int cellsX = numVertexesX;

                for (int z = 0; z < divisionsZ; z++)
                {
                    int offsetZ = z * numVertexesZ;
                    int cellsZ = numVertexesZ;

                    // Crear índices para generar la lista de triángulos de colisión
                    int[] collisionIndices = SceneryNodeInfo.CreateCollisionIndices(
                        offsetX,
                        offsetZ,
                        cellsX,
                        cellsZ,
                        totalCellsX,
                        totalCellsZ);

                    // Crear la lista de triángulos
                    Triangle[] quadTriangles = SceneryNodeInfo.BuildPrimitiveList(vertList, collisionIndices);

                    // Crear los índices para alta resolución
                    SceneryNodeIndexInfo quadHighInfo = null;
                    Int32[] quadHighIndices = Build(
                        LOD.High,
                        highIndices.Count,
                        offsetX,
                        offsetZ,
                        cellsX,
                        cellsZ,
                        totalCellsX,
                        totalCellsZ,
                        out quadHighInfo);

                    // Crear los índices para media resolución
                    SceneryNodeIndexInfo quadMediumInfo = null;
                    Int32[] quadMediumIndices = Build(
                        LOD.Medium,
                        mediumIndices.Count,
                        offsetX,
                        offsetZ,
                        cellsX,
                        cellsZ,
                        totalCellsX,
                        totalCellsZ,
                        out quadMediumInfo);

                    // Crear los índices para baja resolución
                    SceneryNodeIndexInfo quadLowInfo = null;
                    Int32[] quadLowIndices = Build(
                        LOD.Low,
                        lowIndices.Count,
                        offsetX,
                        offsetZ,
                        cellsX,
                        cellsZ,
                        totalCellsX,
                        totalCellsZ,
                        out quadLowInfo);

                    // Añadir los índices a la colección de índices para cada resolución
                    highIndices.AddRange(quadHighIndices);
                    mediumIndices.AddRange(quadMediumIndices);
                    lowIndices.AddRange(quadLowIndices);

                    Dictionary<LOD, SceneryNodeIndexInfo> info = new Dictionary<LOD, SceneryNodeIndexInfo>();
                    info.Add(LOD.High, quadHighInfo);
                    info.Add(LOD.Medium, quadMediumInfo);
                    info.Add(LOD.Low, quadLowInfo);

                    // Crear el nodo con los triángulos para la colisión, los índices de inicio y el número de triángulos para cada resolución
                    SceneryTriangleNode newQuadNode = new SceneryTriangleNode(quadTriangles, info);
                    nodes.Add(newQuadNode);
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
        /// Construye los índices del nivel de detalle especificado, dentro de los límites del nodo
        /// </summary>
        /// <param name="lodType">Nivel de detalle</param>
        /// <param name="indexOffset">Posición del primer índice a generar, informado desde la colección global de índices</param>
        /// <param name="offsetX">Coordenada X</param>
        /// <param name="offsetZ">Coordenada Z</param>
        /// <param name="cellsX">Número de celdas del nodo en X</param>
        /// <param name="cellsZ">Número de celdas del nodo en Y</param>
        /// <param name="totalCellsX">Número total de celdas en X</param>
        /// <param name="totalCellsZ">Número total de celdas en Z</param>
        /// <param name="info">Devuelve la información de los índices del nodo</param>
        /// <returns>Devuelve la colección de índices del nodo</returns>
        /// <remarks>La colección contiene los nodos centrales, los cuatro bordes normales y los cuatro bordes para conectar con niveles de detalle menores</remarks>
        private static Int32[] Build(
            LOD lodType,
            int indexOffset,
            int offsetX,
            int offsetZ,
            int cellsX,
            int cellsZ,
            int totalCellsX,
            int totalCellsZ,
            out SceneryNodeIndexInfo info)
        {
            info = new SceneryNodeIndexInfo();

            int level = (int)lodType;

            List<Int32> indices = new List<Int32>();

            if (lodType == LOD.Low)
            {
                Int32[] allIndices = CreateAllQuadIndices(level, offsetX, offsetZ, cellsX, cellsZ, totalCellsX, totalCellsZ);

                indices.AddRange(allIndices);

                info.CenterPrimitiveCount = allIndices.Length / 3;
                info.BorderPrimitiveCount = 0;
                info.BorderConnectionPrimitiveCount = 0;

                info.CenterOffset = indexOffset;
                info.WestOffset = 0;
                info.EastOffset = 0;
                info.NorthOffset = 0;
                info.SouthOffset = 0;
                info.WestConnectionOffset = 0;
                info.EastConnectionOffset = 0;
                info.NorthConnectionOffset = 0;
                info.SouthConnectionOffset = 0;
            }
            else
            {
                Int32[] centralIndices = CreateCentralQuadIndices(level, offsetX, offsetZ, cellsX, cellsZ, totalCellsX, totalCellsZ);
                Int32[] northQuadIndices = CreateNorthQuadIndices(level, offsetX, offsetZ, cellsX, cellsZ, totalCellsX, totalCellsZ);
                Int32[] southQuadIndices = CreateSouthQuadIndices(level, offsetX, offsetZ, cellsX, cellsZ, totalCellsX, totalCellsZ);
                Int32[] eastQuadIndices = CreateEastQuadIndices(level, offsetX, offsetZ, cellsX, cellsZ, totalCellsX, totalCellsZ);
                Int32[] westQuadIndices = CreateWestQuadIndices(level, offsetX, offsetZ, cellsX, cellsZ, totalCellsX, totalCellsZ);
                Int32[] northConnectionQuadIndices = CreateConnectionNorthQuadIndices(level, offsetX, offsetZ, cellsX, cellsZ, totalCellsX, totalCellsZ);
                Int32[] southConnectionQuadIndices = CreateConnectionSouthQuadIndices(level, offsetX, offsetZ, cellsX, cellsZ, totalCellsX, totalCellsZ);
                Int32[] eastConnectionQuadIndices = CreateConnectionEastQuadIndices(level, offsetX, offsetZ, cellsX, cellsZ, totalCellsX, totalCellsZ);
                Int32[] westConnectionQuadIndices = CreateConnectionWestQuadIndices(level, offsetX, offsetZ, cellsX, cellsZ, totalCellsX, totalCellsZ);

                indices.AddRange(centralIndices);
                indices.AddRange(northQuadIndices);
                indices.AddRange(southQuadIndices);
                indices.AddRange(eastQuadIndices);
                indices.AddRange(westQuadIndices);
                indices.AddRange(northConnectionQuadIndices);
                indices.AddRange(southConnectionQuadIndices);
                indices.AddRange(eastConnectionQuadIndices);
                indices.AddRange(westConnectionQuadIndices);

                info.CenterPrimitiveCount = centralIndices.Length / 3;
                info.BorderPrimitiveCount = westQuadIndices.Length / 3;
                info.BorderConnectionPrimitiveCount = westConnectionQuadIndices.Length / 3;

                info.CenterOffset = indexOffset;
                info.NorthOffset = info.CenterOffset + centralIndices.Length;
                info.SouthOffset = info.NorthOffset + northQuadIndices.Length;
                info.EastOffset = info.SouthOffset + southQuadIndices.Length;
                info.WestOffset = info.EastOffset + eastQuadIndices.Length;
                info.NorthConnectionOffset = info.WestOffset + westQuadIndices.Length;
                info.SouthConnectionOffset = info.NorthConnectionOffset + northConnectionQuadIndices.Length;
                info.EastConnectionOffset = info.SouthConnectionOffset + southConnectionQuadIndices.Length;
                info.WestConnectionOffset = info.EastConnectionOffset + eastConnectionQuadIndices.Length;
            }

            return indices.ToArray();
        }

        /// <summary>
        /// Construye los índices de un solo nodo del quadtree
        /// </summary>
        /// <param name="offsetX">Coordenada X</param>
        /// <param name="offsetZ">Coordenada Z</param>
        /// <param name="cellsX">Número de celdas del nodo en X</param>
        /// <param name="cellsZ">Número de celdas del nodo en Y</param>
        /// <param name="totalCellsX">Número total de celdas en X</param>
        /// <param name="totalCellsZ">Número total de celdas en Z</param>
        /// <returns>Lista de índices para el nivel de detalle especificado</returns>
        private static Int32[] CreateCollisionIndices(
            int offsetX,
            int offsetZ,
            int cellsX,
            int cellsZ,
            int totalCellsX,
            int totalCellsZ)
        {
            int level = (int)LOD.High;

            int numTriangles = ((cellsX - 1) / level) * ((cellsZ - 1) / level) * 2;
            int numIndices = numTriangles * 3;

            List<Int32> indicesList = new List<Int32>(numIndices);

            for (int x = offsetX; x < (offsetX + cellsX); x += level)
            {
                for (int z = offsetZ; z < (offsetZ + cellsZ); z += level)
                {
                    int vertStart = (z * totalCellsX) + x;

                    // Indices del triángulo superior: Cuadrado A, B, C, D
                    int vertexA = (vertStart);
                    int vertexB = (vertStart) + (1 * level);
                    int vertexC = (vertStart) + (totalCellsX * level);
                    int vertexD = (vertStart) + ((totalCellsX + 1) * level);

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

        /// <summary>
        /// Crea todos los índices del nodo
        /// </summary>
        /// <param name="level">Nivel</param>
        /// <param name="offsetX">Coordenada X</param>
        /// <param name="offsetZ">Coordenada Z</param>
        /// <param name="cellsX">Número de celdas del nodo en X</param>
        /// <param name="cellsZ">Número de celdas del nodo en Y</param>
        /// <param name="totalCellsX">Número total de celdas en X</param>
        /// <param name="totalCellsZ">Número total de celdas en Z</param>
        /// <returns>Devuelve la lista de índices de todo el nodo</returns>
        private static Int32[] CreateAllQuadIndices(
            int level,
            int offsetX,
            int offsetZ,
            int cellsX,
            int cellsZ,
            int totalCellsX,
            int totalCellsZ)
        {
            List<Int32> indices = new List<int>();

            for (int x = 0; x < cellsX; x += level)
            {
                for (int z = 0; z < cellsZ; z += level)
                {
                    int start = ((z + offsetZ) * totalCellsX) + (x + offsetX);

                    // Indices del triángulo superior: Cuadrado A, B, C, D
                    int posA = (start);
                    int posB = (start) + (level);
                    int posC = (start) + (level * (totalCellsX));
                    int posD = (start) + (level * (totalCellsX + 1));

                    // Establecer los índices para el triángulo superior
                    indices.Add(posA);
                    indices.Add(posC);
                    indices.Add(posB);

                    // Establecer los índices para el triángulo inferior
                    indices.Add(posB);
                    indices.Add(posC);
                    indices.Add(posD);
                }
            }

            return indices.ToArray();
        }

        /// <summary>
        /// Crea los índices centrales del nodo
        /// </summary>
        /// <param name="level">Nivel</param>
        /// <param name="offsetX">Coordenada X</param>
        /// <param name="offsetZ">Coordenada Z</param>
        /// <param name="cellsX">Número de celdas del nodo en X</param>
        /// <param name="cellsZ">Número de celdas del nodo en Y</param>
        /// <param name="totalCellsX">Número total de celdas en X</param>
        /// <param name="totalCellsZ">Número total de celdas en Z</param>
        /// <returns>Devuelve la lista de índices centrales del nodo</returns>
        private static Int32[] CreateCentralQuadIndices(
            int level,
            int offsetX,
            int offsetZ,
            int cellsX,
            int cellsZ,
            int totalCellsX,
            int totalCellsZ)
        {
            List<Int32> indices = new List<int>();

            for (int x = level; x < cellsX - level; x += level)
            {
                for (int z = level; z < cellsZ - level; z += level)
                {
                    int start = ((z + offsetZ) * totalCellsX) + (x + offsetX);

                    int posA = (start);
                    int posB = (start) + (level);
                    int posC = (start) + (level * (totalCellsX));
                    int posD = (start) + (level * (totalCellsX + 1));

                    // Establecer los índices para el triángulo superior
                    indices.Add(posA);
                    indices.Add(posC);
                    indices.Add(posB);

                    // Establecer los índices para el triángulo inferior
                    indices.Add(posB);
                    indices.Add(posC);
                    indices.Add(posD);
                }
            }

            return indices.ToArray();
        }

        /// <summary>
        /// Crea la lista de índices del borde oeste
        /// </summary>
        /// <param name="level">Nivel</param>
        /// <param name="offsetX">Coordenada X</param>
        /// <param name="offsetZ">Coordenada Z</param>
        /// <param name="cellsX">Número de celdas del nodo en X</param>
        /// <param name="cellsZ">Número de celdas del nodo en Y</param>
        /// <param name="totalCellsX">Número total de celdas en X</param>
        /// <param name="totalCellsZ">Número total de celdas en Z</param>
        /// <returns>Devuelve la lista de índices del borde oeste</returns>
        private static Int32[] CreateWestQuadIndices(
            int level,
            int offsetX,
            int offsetZ,
            int cellsX,
            int cellsZ,
            int totalCellsX,
            int totalCellsZ)
        {
            List<Int32> indices = new List<int>();

            // Sólo el primer Z
            int z = 0;

            for (int x = 0; x < cellsX; x += level)
            {
                int start = ((z + offsetZ) * totalCellsX) + (x + offsetX);

                int posA = (start);
                int posB = (start) + (level);
                int posC = (start) + (level * (totalCellsX));
                int posD = (start) + (level * (totalCellsX + 1));

                if (x == 0)
                {
                    // Primera celda
                    indices.Add(posA);
                    indices.Add(posD);
                    indices.Add(posB);
                }
                else if (x < (cellsX - level))
                {
                    indices.Add(posA);
                    indices.Add(posC);
                    indices.Add(posB);

                    indices.Add(posD);
                    indices.Add(posB);
                    indices.Add(posC);
                }
                else
                {
                    // Última celda
                    indices.Add(posA);
                    indices.Add(posC);
                    indices.Add(posB);
                }
            }

            return indices.ToArray();
        }
        /// <summary>
        /// Crea la lista de índices del borde este
        /// </summary>
        /// <param name="level">Nivel</param>
        /// <param name="offsetX">Coordenada X</param>
        /// <param name="offsetZ">Coordenada Z</param>
        /// <param name="cellsX">Número de celdas del nodo en X</param>
        /// <param name="cellsZ">Número de celdas del nodo en Y</param>
        /// <param name="totalCellsX">Número total de celdas en X</param>
        /// <param name="totalCellsZ">Número total de celdas en Z</param>
        /// <returns>Devuelve la lista de índices del borde este</returns>
        private static Int32[] CreateEastQuadIndices(
            int level,
            int offsetX,
            int offsetZ,
            int cellsX,
            int cellsZ,
            int totalCellsX,
            int totalCellsZ)
        {
            List<Int32> indices = new List<int>();

            // Sólo el último z
            int z = (cellsZ - level);

            for (int x = 0; x < cellsX; x += level)
            {
                int start = ((z + offsetZ) * totalCellsX) + (x + offsetX);

                int posA = (start);
                int posB = (start) + (level);
                int posC = (start) + (level * (totalCellsX));
                int posD = (start) + (level * (totalCellsX + 1));

                if (x == 0)
                {
                    // Primera celda
                    indices.Add(posB);
                    indices.Add(posC);
                    indices.Add(posD);
                }
                else if (x < (cellsX - level))
                {
                    indices.Add(posA);
                    indices.Add(posC);
                    indices.Add(posB);

                    indices.Add(posD);
                    indices.Add(posB);
                    indices.Add(posC);
                }
                else
                {
                    // Última celda
                    indices.Add(posA);
                    indices.Add(posC);
                    indices.Add(posD);
                }
            }

            return indices.ToArray();
        }
        /// <summary>
        /// Crea la lista de índices del borde norte
        /// </summary>
        /// <param name="level">Nivel</param>
        /// <param name="offsetX">Coordenada X</param>
        /// <param name="offsetZ">Coordenada Z</param>
        /// <param name="cellsX">Número de celdas del nodo en X</param>
        /// <param name="cellsZ">Número de celdas del nodo en Y</param>
        /// <param name="totalCellsX">Número total de celdas en X</param>
        /// <param name="totalCellsZ">Número total de celdas en Z</param>
        /// <returns>Obtiene la lista de índices del borde norte</returns>
        private static Int32[] CreateNorthQuadIndices(
            int level,
            int offsetX,
            int offsetZ,
            int cellsX,
            int cellsZ,
            int totalCellsX,
            int totalCellsZ)
        {
            List<Int32> indices = new List<int>();

            // Sólo el primer X
            int x = 0;

            for (int z = 0; z < cellsZ; z += level)
            {
                int start = ((z + offsetZ) * totalCellsX) + (x + offsetX);

                int posA = (start);
                int posB = (start) + (level);
                int posC = (start) + (level * (totalCellsX));
                int posD = (start) + (level * (totalCellsX + 1));

                if (z == 0)
                {
                    // Primera celda
                    indices.Add(posA);
                    indices.Add(posC);
                    indices.Add(posD);
                }
                else if (z < (cellsZ - level))
                {
                    indices.Add(posA);
                    indices.Add(posC);
                    indices.Add(posB);

                    indices.Add(posD);
                    indices.Add(posB);
                    indices.Add(posC);
                }
                else
                {
                    // Última celda
                    indices.Add(posA);
                    indices.Add(posC);
                    indices.Add(posB);
                }
            }

            return indices.ToArray();
        }
        /// <summary>
        /// Crea la lista de índices del borde sur
        /// </summary>
        /// <param name="level">Nivel</param>
        /// <param name="offsetX">Coordenada X</param>
        /// <param name="offsetZ">Coordenada Z</param>
        /// <param name="cellsX">Número de celdas del nodo en X</param>
        /// <param name="cellsZ">Número de celdas del nodo en Y</param>
        /// <param name="totalCellsX">Número total de celdas en X</param>
        /// <param name="totalCellsZ">Número total de celdas en Z</param>
        /// <returns>Devuelve la lista de índices del borde sur</returns>
        private static Int32[] CreateSouthQuadIndices(
            int level,
            int offsetX,
            int offsetZ,
            int cellsX,
            int cellsZ,
            int totalCellsX,
            int totalCellsZ)
        {
            List<Int32> indices = new List<int>();

            // Sólo el último X
            int x = (cellsX - level);

            for (int z = 0; z < cellsZ; z += level)
            {
                int start = ((z + offsetZ) * totalCellsX) + (x + offsetX);

                int posA = (start);
                int posB = (start) + (level);
                int posC = (start) + (level * (totalCellsX));
                int posD = (start) + (level * (totalCellsX + 1));

                if (z == 0)
                {
                    // Primera celda
                    indices.Add(posB);
                    indices.Add(posC);
                    indices.Add(posD);
                }
                else if (z < (cellsZ - level))
                {
                    indices.Add(posA);
                    indices.Add(posC);
                    indices.Add(posB);

                    indices.Add(posD);
                    indices.Add(posB);
                    indices.Add(posC);
                }
                else
                {
                    // Última celda
                    indices.Add(posA);
                    indices.Add(posD);
                    indices.Add(posB);
                }
            }

            return indices.ToArray();
        }

        /// <summary>
        /// Crea la lista de índices de conexión del borde oeste
        /// </summary>
        /// <param name="level">Nivel</param>
        /// <param name="offsetX">Coordenada X</param>
        /// <param name="offsetZ">Coordenada Z</param>
        /// <param name="cellsX">Número de celdas del nodo en X</param>
        /// <param name="cellsZ">Número de celdas del nodo en Y</param>
        /// <param name="totalCellsX">Número total de celdas en X</param>
        /// <param name="totalCellsZ">Número total de celdas en Z</param>
        /// <returns>Devuelve la lista de índices de conexión del borde oeste</returns>
        private static Int32[] CreateConnectionWestQuadIndices(
            int level,
            int offsetX,
            int offsetZ,
            int cellsX,
            int cellsZ,
            int totalCellsX,
            int totalCellsZ)
        {
            List<Int32> indices = new List<int>();

            // Sólo el primer Z
            int z = 0;

            // Primero los triángulos del borde
            bool calc = true;
            for (int x = 0; x < cellsX; x += level)
            {
                if (calc)
                {
                    int start = ((z + offsetZ) * totalCellsX) + (x + offsetX);

                    int posA = (start);
                    int posB = (start) + (level * 2);
                    int posC = (start) + (level * (totalCellsX + 1));

                    indices.Add(posA);
                    indices.Add(posC);
                    indices.Add(posB);
                }

                calc = !calc;
            }

            // Después los triángulos pequeños
            calc = false;
            for (int x = level; x < cellsX - level; x += level)
            {
                int start = ((z + offsetZ) * totalCellsX) + (x + offsetX);

                int posA = (start);
                int posB = (start) + (level);
                int posC = (start) + (level * (totalCellsX));
                int posD = (start) + (level * (totalCellsX + 1));

                if (!calc)
                {
                    indices.Add(posD);
                    indices.Add(posB);
                    indices.Add(posC);
                }
                else
                {
                    indices.Add(posA);
                    indices.Add(posC);
                    indices.Add(posD);
                }

                calc = !calc;
            }

            return indices.ToArray();
        }
        /// <summary>
        /// Crea la lista de índices de conexión del borde este
        /// </summary>
        /// <param name="level">Nivel</param>
        /// <param name="offsetX">Coordenada X</param>
        /// <param name="offsetZ">Coordenada Z</param>
        /// <param name="cellsX">Número de celdas del nodo en X</param>
        /// <param name="cellsZ">Número de celdas del nodo en Y</param>
        /// <param name="totalCellsX">Número total de celdas en X</param>
        /// <param name="totalCellsZ">Número total de celdas en Z</param>
        /// <returns>Devuelve la lista de índices de conexión del borde este</returns>
        private static Int32[] CreateConnectionEastQuadIndices(
            int level,
            int offsetX,
            int offsetZ,
            int cellsX,
            int cellsZ,
            int totalCellsX,
            int totalCellsZ)
        {
            List<Int32> indices = new List<int>();

            // Sólo el último Z
            int z = (cellsZ - level);

            // Primero los triángulos del borde
            bool calc = false;
            for (int x = 0; x < cellsX; x += level)
            {
                if (calc)
                {
                    int start = ((z + offsetZ) * totalCellsX) + (x + offsetX);

                    int posA = (start);
                    int posB = (start) + (level * (totalCellsX - 1));
                    int posC = (start) + (level * (totalCellsX + 1));

                    indices.Add(posA);
                    indices.Add(posB);
                    indices.Add(posC);
                }

                calc = !calc;
            }

            // Después los triángulos pequeños
            calc = false;
            for (int x = level; x < cellsX - level; x += level)
            {
                int start = ((z + offsetZ) * totalCellsX) + (x + offsetX);

                int posA = (start);
                int posB = (start) + (level);
                int posC = (start) + (level * (totalCellsX));
                int posD = (start) + (level * (totalCellsX + 1));

                if (!calc)
                {
                    indices.Add(posD);
                    indices.Add(posB);
                    indices.Add(posA);
                }
                else
                {
                    indices.Add(posA);
                    indices.Add(posC);
                    indices.Add(posB);
                }

                calc = !calc;
            }

            return indices.ToArray();
        }
        /// <summary>
        /// Crea la lista de índices de conexión del borde norte
        /// </summary>
        /// <param name="level">Nivel</param>
        /// <param name="offsetX">Coordenada X</param>
        /// <param name="offsetZ">Coordenada Z</param>
        /// <param name="cellsX">Número de celdas del nodo en X</param>
        /// <param name="cellsZ">Número de celdas del nodo en Y</param>
        /// <param name="totalCellsX">Número total de celdas en X</param>
        /// <param name="totalCellsZ">Número total de celdas en Z</param>
        /// <returns>Devuelve la lista de índices de conexión del borde norte</returns>
        private static Int32[] CreateConnectionNorthQuadIndices(
            int level,
            int offsetX,
            int offsetZ,
            int cellsX,
            int cellsZ,
            int totalCellsX,
            int totalCellsZ)
        {
            List<Int32> indices = new List<int>();

            // Sólo el primer X
            int x = 0;

            // Primero los triángulos del borde
            bool calc = true;
            for (int z = 0; z < cellsZ; z += level)
            {
                if (calc)
                {
                    int start = ((z + offsetZ) * totalCellsX) + (x + offsetX);

                    int posA = (start);
                    int posB = (start) + (level * (totalCellsX * 2));
                    int posC = (start) + (level * (totalCellsX + 1));

                    indices.Add(posA);
                    indices.Add(posB);
                    indices.Add(posC);
                }

                calc = !calc;
            }

            // Después los triángulos pequeños
            calc = false;
            for (int z = level; z < cellsZ - level; z += level)
            {
                int start = ((z + offsetZ) * totalCellsX) + (x + offsetX);

                int posA = (start);
                int posB = (start) + (level);
                int posC = (start) + (level * (totalCellsX));
                int posD = (start) + (level * (totalCellsX + 1));

                if (!calc)
                {
                    indices.Add(posD);
                    indices.Add(posB);
                    indices.Add(posC);
                }
                else
                {
                    indices.Add(posA);
                    indices.Add(posD);
                    indices.Add(posB);
                }

                calc = !calc;
            }

            return indices.ToArray();
        }
        /// <summary>
        /// Crea la lista de índices de conexión del borde sur
        /// </summary>
        /// <param name="level">Nivel</param>
        /// <param name="offsetX">Coordenada X</param>
        /// <param name="offsetZ">Coordenada Z</param>
        /// <param name="cellsX">Número de celdas del nodo en X</param>
        /// <param name="cellsZ">Número de celdas del nodo en Y</param>
        /// <param name="totalCellsX">Número total de celdas en X</param>
        /// <param name="totalCellsZ">Número total de celdas en Z</param>
        /// <returns>Devuelve la lista de índices de conexión del borde sur</returns>
        private static Int32[] CreateConnectionSouthQuadIndices(
            int level,
            int offsetX,
            int offsetZ,
            int cellsX,
            int cellsZ,
            int totalCellsX,
            int totalCellsZ)
        {
            List<Int32> indices = new List<int>();

            // Sólo el último X
            int x = (cellsX - level);

            // Primero los triángulos del borde
            bool calc = true;
            for (int z = 0; z < cellsZ; z += level)
            {
                if (calc)
                {
                    int start = ((z + offsetZ) * totalCellsX) + (x + offsetX);

                    int posA = (start) + (level);
                    int posB = (start) + (level * (totalCellsX));
                    int posC = (start) + (level * ((totalCellsX * 2) + 1));

                    indices.Add(posA);
                    indices.Add(posB);
                    indices.Add(posC);
                }

                calc = !calc;
            }

            // Después los triángulos pequeños
            calc = false;
            for (int z = level; z < cellsZ - level; z += level)
            {
                int start = ((z + offsetZ) * totalCellsX) + (x + offsetX);

                int posA = (start);
                int posB = (start) + (level);
                int posC = (start) + (level * (totalCellsX));
                int posD = (start) + (level * (totalCellsX + 1));

                if (!calc)
                {
                    indices.Add(posD);
                    indices.Add(posA);
                    indices.Add(posC);
                }
                else
                {
                    indices.Add(posA);
                    indices.Add(posC);
                    indices.Add(posB);
                }

                calc = !calc;
            }

            return indices.ToArray();
        }
    }
}
