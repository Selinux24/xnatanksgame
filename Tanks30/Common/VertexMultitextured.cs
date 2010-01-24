using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Common.Format
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
}
