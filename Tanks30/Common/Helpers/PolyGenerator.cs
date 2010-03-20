using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Common.Helpers
{
    /// <summary>
    /// Funciones de generación de polígonos
    /// </summary>
    public static class PolyGenerator
    {
        /// <summary>
        /// Obtiene los vértices que representan una esfera
        /// </summary>
        /// <param name="vertices">Lista de vértices</param>
        public static void InitializeSphere(out VertexPositionNormalTexture[] vertices, out short[] indices, float radius)
        {
            vertices = null;
            indices = null;

            List<VertexPositionNormalTexture> verticesList = new List<VertexPositionNormalTexture>();
            List<short> indicesList = new List<short>();

            float factor = 10;
            float pass = factor / 10f;
            float latitudeLimit = factor / 2f;
            float longitudeLimit = factor * 2f;

            // calculate the constant and the step we use
            float dToR = (float)System.Math.PI / factor;

            // loop around the sphere
            for (float latitude = -latitudeLimit; latitude < latitudeLimit; latitude += pass)
            {
                // loop the other way around it
                for (float longitude = 0; longitude <= longitudeLimit; longitude += pass)
                {
                    if (longitude < longitudeLimit)
                    {
                        int index = verticesList.Count;

                        int index0 = index;
                        int index1 = index + 1;
                        int index2 = index + 2;
                        int index3 = index + 3;

                        // triangulo con 1, 2 y 3
                        indicesList.Add((short)index0);
                        indicesList.Add((short)index2);
                        indicesList.Add((short)index1);

                        // triangulo con 2, 3 y 4
                        indicesList.Add((short)index1);
                        indicesList.Add((short)index2);
                        indicesList.Add((short)index3);
                    }

                    // coordenadas para 1
                    float x1 = (float)System.Math.Sin(longitude * dToR) * (float)System.Math.Cos(latitude * dToR);
                    float y1 = (float)System.Math.Sin(latitude * dToR);
                    float z1 = (float)System.Math.Cos(longitude * dToR) * (float)System.Math.Cos(latitude * dToR);

                    // coordenadas para 2
                    float x2 = (float)System.Math.Sin(longitude * dToR) * (float)System.Math.Cos((latitude + pass) * dToR);
                    float y2 = (float)System.Math.Sin((latitude + pass) * dToR);
                    float z2 = (float)System.Math.Cos(longitude * dToR) * (float)System.Math.Cos((latitude + pass) * dToR);

                    // vértice y normal para 1
                    Vector3 vertex1 = new Vector3(x1 * radius, y1 * radius, z1 * radius);
                    Vector3 normal1 = Vector3.Normalize(vertex1);
                    verticesList.Add(new VertexPositionNormalTexture(vertex1, normal1, Vector2.Zero));

                    // vértice y normal para 2
                    Vector3 vertex2 = new Vector3(x2 * radius, y2 * radius, z2 * radius);
                    Vector3 normal2 = Vector3.Normalize(vertex2);
                    verticesList.Add(new VertexPositionNormalTexture(vertex2, normal2, Vector2.Zero));
                }
            }

            vertices = verticesList.ToArray();
            indices = indicesList.ToArray();
        }
        /// <summary>
        /// Obtiene los vértices que representan un cubo
        /// </summary>
        /// <param name="vertices">Lista de vértices</param>
        public static void InitializeCube(out VertexPositionNormalTexture[] vertices, Vector3 min, Vector3 max)
        {
            float minX = min.X;
            float minY = min.Y;
            float minZ = min.Z;

            float maxX = max.X;
            float maxY = max.Y;
            float maxZ = max.Z;

            vertices = new VertexPositionNormalTexture[36];

            Vector3 topLeftFront = new Vector3(minX, maxY, maxZ);
            Vector3 bottomLeftFront = new Vector3(minX, minY, maxZ);
            Vector3 topRightFront = new Vector3(maxX, maxY, maxZ);
            Vector3 bottomRightFront = new Vector3(maxX, minY, maxZ);
            Vector3 topLeftBack = new Vector3(minX, maxY, minZ);
            Vector3 topRightBack = new Vector3(maxX, maxY, minZ);
            Vector3 bottomLeftBack = new Vector3(minX, minY, minZ);
            Vector3 bottomRightBack = new Vector3(maxX, minY, minZ);

            Vector2 textureTopLeft = new Vector2(0.0f, 0.0f);
            Vector2 textureTopRight = new Vector2(1.0f, 0.0f);
            Vector2 textureBottomLeft = new Vector2(0.0f, 1.0f);
            Vector2 textureBottomRight = new Vector2(1.0f, 1.0f);

            Vector3 frontNormal = new Vector3(0.0f, 0.0f, 1.0f);
            Vector3 backNormal = new Vector3(0.0f, 0.0f, -1.0f);
            Vector3 topNormal = new Vector3(0.0f, 1.0f, 0.0f);
            Vector3 bottomNormal = new Vector3(0.0f, -1.0f, 0.0f);
            Vector3 leftNormal = new Vector3(-1.0f, 0.0f, 0.0f);
            Vector3 rightNormal = new Vector3(1.0f, 0.0f, 0.0f);

            // Front face.
            vertices[0] = new VertexPositionNormalTexture(topLeftFront, frontNormal, textureTopLeft);
            vertices[2] = new VertexPositionNormalTexture(bottomLeftFront, frontNormal, textureBottomLeft);
            vertices[1] = new VertexPositionNormalTexture(topRightFront, frontNormal, textureTopRight);
            vertices[3] = new VertexPositionNormalTexture(bottomLeftFront, frontNormal, textureBottomLeft);
            vertices[5] = new VertexPositionNormalTexture(bottomRightFront, frontNormal, textureBottomRight);
            vertices[4] = new VertexPositionNormalTexture(topRightFront, frontNormal, textureTopRight);

            // Back face.
            vertices[6] = new VertexPositionNormalTexture(topLeftBack, backNormal, textureTopRight);
            vertices[8] = new VertexPositionNormalTexture(topRightBack, backNormal, textureTopLeft);
            vertices[7] = new VertexPositionNormalTexture(bottomLeftBack, backNormal, textureBottomRight);
            vertices[9] = new VertexPositionNormalTexture(bottomLeftBack, backNormal, textureBottomRight);
            vertices[11] = new VertexPositionNormalTexture(topRightBack, backNormal, textureTopLeft);
            vertices[10] = new VertexPositionNormalTexture(bottomRightBack, backNormal, textureBottomLeft);

            // Top face.
            vertices[12] = new VertexPositionNormalTexture(topLeftFront, topNormal, textureBottomLeft);
            vertices[14] = new VertexPositionNormalTexture(topRightBack, topNormal, textureTopRight);
            vertices[13] = new VertexPositionNormalTexture(topLeftBack, topNormal, textureTopLeft);
            vertices[15] = new VertexPositionNormalTexture(topLeftFront, topNormal, textureBottomLeft);
            vertices[17] = new VertexPositionNormalTexture(topRightFront, topNormal, textureBottomRight);
            vertices[16] = new VertexPositionNormalTexture(topRightBack, topNormal, textureTopRight);

            // Bottom face. 
            vertices[18] = new VertexPositionNormalTexture(bottomLeftFront, bottomNormal, textureTopLeft);
            vertices[20] = new VertexPositionNormalTexture(bottomLeftBack, bottomNormal, textureBottomLeft);
            vertices[19] = new VertexPositionNormalTexture(bottomRightBack, bottomNormal, textureBottomRight);
            vertices[21] = new VertexPositionNormalTexture(bottomLeftFront, bottomNormal, textureTopLeft);
            vertices[23] = new VertexPositionNormalTexture(bottomRightBack, bottomNormal, textureBottomRight);
            vertices[22] = new VertexPositionNormalTexture(bottomRightFront, bottomNormal, textureTopRight);

            // Left face.
            vertices[24] = new VertexPositionNormalTexture(topLeftFront, leftNormal, textureTopRight);
            vertices[26] = new VertexPositionNormalTexture(bottomLeftBack, leftNormal, textureBottomLeft);
            vertices[25] = new VertexPositionNormalTexture(bottomLeftFront, leftNormal, textureBottomRight);
            vertices[27] = new VertexPositionNormalTexture(topLeftBack, leftNormal, textureTopLeft);
            vertices[29] = new VertexPositionNormalTexture(bottomLeftBack, leftNormal, textureBottomLeft);
            vertices[28] = new VertexPositionNormalTexture(topLeftFront, leftNormal, textureTopRight);

            // Right face. 
            vertices[30] = new VertexPositionNormalTexture(topRightFront, rightNormal, textureTopLeft);
            vertices[32] = new VertexPositionNormalTexture(bottomRightFront, rightNormal, textureBottomLeft);
            vertices[31] = new VertexPositionNormalTexture(bottomRightBack, rightNormal, textureBottomRight);
            vertices[33] = new VertexPositionNormalTexture(topRightBack, rightNormal, textureTopRight);
            vertices[35] = new VertexPositionNormalTexture(topRightFront, rightNormal, textureTopLeft);
            vertices[34] = new VertexPositionNormalTexture(bottomRightBack, rightNormal, textureBottomRight);
        }
    }
}
