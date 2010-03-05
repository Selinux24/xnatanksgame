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
        /// Obtiene el AABB que contiene al plano, que es infinito
        /// </summary>
        public override BoundingBox AABB { get; protected set; }
        /// <summary>
        /// Obtiene la esfera que contiene al plano, que es infinita
        /// </summary>
        public override BoundingSphere SPH { get; protected set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="plane">Plano</param>
        /// <param name="mass">Masa</param>
        public CollisionPlane(Plane plane, float mass)
            : this(plane.Normal, plane.D, mass)
        {

        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="normal">Normal</param>
        /// <param name="d">Distancia al origen de coordenadas</param>
        /// <param name="mass">Masa</param>
        public CollisionPlane(Vector3 normal, float d, float mass)
            : base(mass)
        {
            this.Normal = normal;
            this.D = d;
        }
    }
}