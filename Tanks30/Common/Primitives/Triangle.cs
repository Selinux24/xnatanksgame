using System;
using Microsoft.Xna.Framework;

namespace Common.Primitives
{
    /// <summary>
    /// Un triángulo
    /// </summary>
    public struct Triangle
    {
        /// <summary>
        /// Puntos del triángulo
        /// </summary>
        public readonly Vector3 Point1, Point2, Point3;
        /// <summary>
        /// Centro del triángulo
        /// </summary>
        public readonly Vector3 Center;
        /// <summary>
        /// Indices
        /// </summary>
        public readonly int I1, I2;
        /// <summary>
        /// Plano en el que está contenido el triángulo
        /// </summary>
        public readonly Plane Plane;
        /// <summary>
        /// Obtiene la normal del plano que contiene el triángulo
        /// </summary>
        public Vector3 Normal
        {
            get
            {
                return this.Plane.Normal;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="point1">Punto 1</param>
        /// <param name="point2">Punto 2</param>
        /// <param name="point3">Punto 3</param>
        public Triangle(Vector3 point1, Vector3 point2, Vector3 point3)
        {
            this.Point1 = point1;
            this.Point2 = point2;
            this.Point3 = point3;
            this.Center = Vector3.Multiply(point1 + point2 + point3, 1.0f / 3.0f);
            this.Plane = new Plane(this.Point2, this.Point1, this.Point3);

            Vector3 n = this.Plane.Normal;
            float absX = (float)Math.Abs(n.X);
            float absY = (float)Math.Abs(n.Y);
            float absZ = (float)Math.Abs(n.Z);

            Vector3 a = new Vector3(absX, absY, absZ);
            if (a.X > a.Y)
            {
                if (a.X > a.Z)
                {
                    this.I1 = 1;
                    this.I2 = 2;
                }
                else
                {
                    this.I1 = 0;
                    this.I2 = 1;
                }
            }
            else
            {
                if (a.Y > a.Z)
                {
                    this.I1 = 0;
                    this.I2 = 2;
                }
                else
                {
                    this.I1 = 0;
                    this.I2 = 1;
                }
            }
        }
    }
}
