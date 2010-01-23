using System;
using Microsoft.Xna.Framework;

namespace Physics
{
    /// <summary>
    /// Primitiva
    /// </summary>
    public class CollisionPrimitive
    {
        /// <summary>
        /// Cuerpo representado por esta primitiva
        /// </summary>
        public RigidBody Body = new RigidBody();

        /// <summary>
        /// Obtiene la transformación resultante del cuerpo
        /// </summary>
        public Matrix Transform
        {
            get
            {
                if (this.Body != null)
                {
                    return this.Body.Transform;
                }

                return Matrix.Identity;
            }
        }
        /// <summary>
        /// Obtiene el eje X de la transformación
        /// </summary>
        public Vector3 XAxis
        {
            get
            {
                return this.Transform.Right;
            }
        }
        /// <summary>
        /// Obtiene el eje Y de la transformación
        /// </summary>
        public Vector3 YAxis
        {
            get
            {
                return this.Transform.Up;
            }
        }
        /// <summary>
        /// Obtiene el eje Z de la transformación
        /// </summary>
        public Vector3 ZAxis
        {
            get
            {
                return this.Transform.Backward;
            }
        }
        /// <summary>
        /// Obtiene la posición de la primitiva
        /// </summary>
        public Vector3 Position
        {
            get
            {
                return this.Transform.Translation;
            }
        }

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

            throw new NotImplementedException();
        }
    }
}