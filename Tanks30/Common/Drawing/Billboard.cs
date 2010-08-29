using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Common.Drawing
{
    /// <summary>
    /// Vértice para Billboard
    /// </summary>
    public struct Billboard
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
        public Vector2 TextureCoordinate;
        /// <summary>
        /// Efecto del viento
        /// </summary>
        public float WindEffect;

        /// <summary>
        /// Tamaño en bytes
        /// </summary>
        public static int SizeInBytes = (3 + 3 + 2 + 1) * 4;
        /// <summary>
        /// Elementos del vértice
        /// </summary>
        public static VertexElement[] VertexElements = new VertexElement[]
        {
            new VertexElement( 0, 0, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Position, 0 ),
            new VertexElement( 0, sizeof(float) * 3, VertexElementFormat.Vector3, VertexElementMethod.Default, VertexElementUsage.Normal, 0 ),
            new VertexElement( 0, sizeof(float) * 6, VertexElementFormat.Vector2, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0 ),
            new VertexElement( 0, sizeof(float) * 8, VertexElementFormat.Single, VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 1 ),
        };
    }
}
