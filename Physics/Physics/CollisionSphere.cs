
namespace Physics
{
    /// <summary>
    /// Represents a rigid body that can be treated as a sphere for collision detection.
    /// </summary>
    public class CollisionSphere : CollisionPrimitive
    {
        /// <summary>
        /// The radius of the sphere.
        /// </summary>
        public float Radius;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="radius">Radio de la esfera</param>
        /// <param name="mass">Masa</param>
        public CollisionSphere(float radius, float mass)
            : base(mass)
        {
            this.Radius = radius;
        }
    }
}