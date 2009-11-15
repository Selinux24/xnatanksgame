using System;

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
    }
}