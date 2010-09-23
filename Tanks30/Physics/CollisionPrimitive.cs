using System;
using Microsoft.Xna.Framework;

namespace Physics
{
    /// <summary>
    /// Primitiva
    /// </summary>
    public abstract class CollisionPrimitive : RigidBody
    {
        /// <summary>
        /// Obtiene el AABB del cuerpo
        /// </summary>
        public abstract BoundingBox AABB { get; protected set; }
        /// <summary>
        /// Obtiene la esfera circundate de la primitiva
        /// </summary>
        public abstract BoundingSphere SPH { get; protected set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="mass">Masa</param>
        public CollisionPrimitive(float mass)
            : base(mass)
        {

        }
    }
}