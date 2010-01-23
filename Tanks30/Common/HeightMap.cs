using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Common.Helpers
{
    using Common.Format;

    /// <summary>
    /// Mapa de alturas
    /// </summary>
    public class HeightMap
    {
        /// <summary>
        /// Alturas
        /// </summary>
        private float[,] m_Data;
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
                if (this.m_Data != null)
                {
                    return this.m_Data.GetLength(0);
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
                if (this.m_Data != null)
                {
                    return this.m_Data.GetLength(1);
                }

                return 0;
            }
        }
        /// <summary>
        /// Obtiene la cantidad de puntos de altura del mapa de alturas
        /// </summary>
        public int DataLength
        {
            get
            {
                if (this.m_Data != null)
                {
                    return this.m_Data.Length;
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
            this.m_Data = data;

            foreach (float height in data)
            {
                if (height < this.Min)
                {
                    this.Min = height;
                }

                if (height > this.Max)
                {
                    this.Max = height;
                }
            }
        }
        /// <summary>
        /// Crea un mapa de alturas a partir de buffer de bytes
        /// </summary>
        /// <param name="bitmapData">Buffer de bytes</param>
        /// <param name="bitmapHeight">Altura del contenido del buffer en pixels</param>
        /// <param name="bitmapWidth">Anchura del contenido del buffer en pixels</param>
        /// <param name="cellScale">Escala</param>
        /// <returns>Devuelve el mapa de alturas generado</returns>
        public static HeightMap FromData(byte[] bitmapData, int bitmapHeight, int bitmapWidth, float cellScale)
        {
            int height = bitmapHeight + 1;
            int width = bitmapWidth + 1;
            int stride = bitmapData.Length / (bitmapWidth * bitmapHeight);
            int size = bitmapData.Length / stride;

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
                        //Si llegamos al tope de ancho, usamos el índice anterior
                        dataX = x - 1;
                    }
                    if (z == height - 1)
                    {
                        //Si llegamos al tope de profundidad, usamos el índice anterior
                        dataZ = z - 1;
                    }

                    //Aumenta según el tamaño del pixel
                    int index = ((dataX * (width - 1)) + dataZ) * stride;

                    result[x, z] = (float)(bitmapData[index] & 0x000000FF) * cellScale;
                }
            }

            return new HeightMap(result);
        }

        /// <summary>
        /// Construye los vértices a partir del mapa de alturas especificado
        /// </summary>
        /// <param name="cellSize">Tamaño de la celda</param>
        /// <returns>Devuelve la colección de vértices</returns>
        public VertexMultitextured[] BuildVertices(float cellSize)
        {
            // Contador de vértices
            int vertexCountX = this.Width;
            int vertexCountZ = this.Deep;
            int vertexCount = vertexCountX * vertexCountZ;

            float level0 = this.Min;
            float level1 = this.Max * 0.1f;
            float level2 = this.Max * 0.4f;
            float level3 = this.Max * 0.5f;
            float level4 = this.Max;

            // Crear los vértices
            List<VertexMultitextured> vertList = new List<VertexMultitextured>(vertexCount);
            Vector3[,] normals = this.CreateNormals(cellSize);

            for (int width = 0; width < vertexCountX; width++)
            {
                for (int deep = 0; deep < vertexCountZ; deep++)
                {
                    VertexMultitextured newVertex = new VertexMultitextured();

                    float posX = width * cellSize;
                    float posY = this.m_Data[deep, width];
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
        /// Crea las normales de vértice a partir del mapa de alturas
        /// </summary>
        /// <param name="cellSize">Tamaño de la celda</param>
        /// <returns>Devuelve la lista de normales de vértice del escenario</returns>
        private Vector3[,] CreateNormals(float cellSize)
        {
            // Obtener las dimensiones del mapa de alturas
            int width = this.Width;
            int deep = this.Deep;

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
                            // Obtener la posición del vértice cuya normal se va a calcular
                            Vector3 pos = new Vector3(cellSize * x, this.m_Data[y, x], cellSize * y);
                            Vector3 pos2;
                            Vector3 pos3;
                            Vector3 norm1;
                            Vector3 norm2;
                            Vector3 norm3;
                            Vector3 norm4;
                            Vector3 norm5;
                            Vector3 norm6;

                            pos2 = new Vector3(cellSize * (x), this.m_Data[y - 1, x], cellSize * (y - 1));
                            pos3 = new Vector3(cellSize * (x - 1), this.m_Data[y, x - 1], cellSize * (y));
                            pos2 -= pos;
                            pos3 -= pos;
                            pos2.Normalize();
                            pos3.Normalize();
                            norm1 = Vector3.Cross(pos2, pos3);

                            pos2 = new Vector3(cellSize * (x - 1), this.m_Data[y, x - 1], cellSize * (y));
                            pos3 = new Vector3(cellSize * (x - 1), this.m_Data[y + 1, x - 1], cellSize * (y + 1));
                            pos2 -= pos;
                            pos3 -= pos;
                            pos2.Normalize();
                            pos3.Normalize();
                            norm2 = Vector3.Cross(pos2, pos3);

                            pos2 = new Vector3(cellSize * (x - 1), this.m_Data[y + 1, x - 1], cellSize * (y + 1));
                            pos3 = new Vector3(cellSize * (x), this.m_Data[y + 1, x], cellSize * (y + 1));
                            pos2 -= pos;
                            pos3 -= pos;
                            pos2.Normalize();
                            pos3.Normalize();
                            norm3 = Vector3.Cross(pos2, pos3);

                            pos2 = new Vector3(cellSize * (x), this.m_Data[y + 1, x], cellSize * (y + 1));
                            pos3 = new Vector3(cellSize * (x + 1), this.m_Data[y, x + 1], cellSize * (y));
                            pos2 -= pos;
                            pos3 -= pos;
                            pos2.Normalize();
                            pos3.Normalize();
                            norm4 = Vector3.Cross(pos2, pos3);

                            pos2 = new Vector3(cellSize * (x + 1), this.m_Data[y, x + 1], cellSize * (y));
                            pos3 = new Vector3(cellSize * (x + 1), this.m_Data[y - 1, x + 1], cellSize * (y - 1));
                            pos2 -= pos;
                            pos3 -= pos;
                            pos2.Normalize();
                            pos3.Normalize();
                            norm5 = Vector3.Cross(pos2, pos3);

                            pos2 = new Vector3(cellSize * (x + 1), this.m_Data[y - 1, x + 1], cellSize * (y - 1));
                            pos3 = new Vector3(cellSize * (x), this.m_Data[y - 1, x], cellSize * (y - 1));
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
    }
}
