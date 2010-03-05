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
        /// Obtiene los ejes de transformación de la primitiva.
        /// </summary>
        /// <param name="axis">Eje</param>
        public Vector3 GetAxis(TransformAxis axis)
        {
            if (axis == TransformAxis.X)
            {
                return this.XAxis;
            }
            else if (axis == TransformAxis.Y)
            {
                return this.YAxis;
            }
            else if (axis == TransformAxis.Z)
            {
                return this.ZAxis;
            }

            throw new NotSupportedException("Tipo de eje no soportado: " + axis.ToString());
        }

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