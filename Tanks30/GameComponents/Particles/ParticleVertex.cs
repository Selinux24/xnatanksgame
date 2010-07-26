using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameComponents.Particles
{
    /// <summary>
    /// Estructura para dibujar PointSprites
    /// </summary>
    struct ParticleVertex
    {
        /// <summary>
        /// Tamaño en bytes de la estructura
        /// </summary>
        public const int SizeInBytes = 32;
        
        /// <summary>
        /// Posición inicial
        /// </summary>
        public Vector3 Position;
        /// <summary>
        /// Velocidad inicial
        /// </summary>
        public Vector3 Velocity;
        /// <summary>
        /// Color
        /// </summary>
        public Color Random;
        /// <summary>
        /// Tiempo en segundos en que se creó la partícula
        /// </summary>
        public float Time;

        /// <summary>
        /// Elementos de la estructura
        /// </summary>
        public static readonly VertexElement[] VertexElements =
        {
            new VertexElement(
                0, 
                0, 
                VertexElementFormat.Vector3,
                VertexElementMethod.Default,
                VertexElementUsage.Position, 
                0),

            new VertexElement(
                0, 
                12, 
                VertexElementFormat.Vector3,
                VertexElementMethod.Default,
                VertexElementUsage.Normal, 
                0),

            new VertexElement(
                0, 
                24, 
                VertexElementFormat.Color,
                VertexElementMethod.Default,
                VertexElementUsage.Color, 
                0),

            new VertexElement(
                0,
                28,
                VertexElementFormat.Single,
                VertexElementMethod.Default,
                VertexElementUsage.TextureCoordinate, 
                0),
        };
    }
}


