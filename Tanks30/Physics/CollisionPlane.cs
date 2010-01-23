using Microsoft.Xna.Framework;

namespace Physics
{
    /// <summary>
    /// Plano
    /// </summary>
    public class CollisionPlane : CollisionPrimitive
    {
        /// <summary>
        /// Normal del plano
        /// </summary>
        public Vector3 Normal;

        /// <summary>
        /// Distancia del plano al origen de coordenadas
        /// </summary>
        public float D;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="plane">Plano</param>
        public CollisionPlane(Plane plane)
            : this(plane.Normal, plane.D)
        {

        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="normal">Normal</param>
        /// <param name="d">Distancia al origen de coordenadas</param>
        public CollisionPlane(Vector3 normal, float d)
        {
            this.Normal = normal;
            this.D = d;
        }
    }
}