
using System;
namespace GameComponents.Scenery
{
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
        /// <summary>
        /// Crea un mapa de alturas a partir de buffer de bytes
        /// </summary>
        /// <param name="bitmapData">Buffer de bytes</param>
        /// <param name="bitmapHeight">Altura del contenido del buffer en pixels</param>
        /// <param name="bitmapWidth">Anchura del contenido del buffer en pixels</param>
        /// <param name="scale">Escala</param>
        /// <returns>Devuelve el mapa de alturas generado</returns>
        public static HeightMap FromData(byte[] bitmapData, int bitmapHeight, int bitmapWidth, float scale)
        {
            int height = bitmapHeight + 1;
            int width = bitmapWidth + 1;
            int stride = bitmapData.Length / (bitmapWidth * bitmapHeight);
            int size = bitmapData.Length / stride;

            float[,] result = new float[height, width];

            int dataX = 0;
            int dataZ = 0;
            for (int x = 0; x < width; x ++)
            {
                for (int z = 0; z < height; z ++)
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

                    result[x, z] = (float)(bitmapData[index] & 0x000000FF) * scale;
                }
            }

            return new HeightMap(result);
        }
    }
}
