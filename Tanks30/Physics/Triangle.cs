using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Physics
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

        // Indices
        private readonly int I1, I2;

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

        /// <summary>
        /// Obtiene si existe intersección entre el rayo y el triángulo
        /// </summary>
        /// <param name="ray">Rayo</param>
        /// <returns>Devuelve verdadero si hay intersección, y falso en el resto de los casos</returns>
        public bool Intersects(Ray ray)
        {
            float? distanceToPoint = null;
            Vector3? intersectionPoint = null;
            return Intersects(ray, out intersectionPoint, out distanceToPoint, false);
        }
        /// <summary>
        /// Obtiene si existe intersección entre el rayo y el triángulo
        /// </summary>
        /// <param name="ray">Rayo</param>
        /// <param name="segmentMode">Indica si usar el rayo como un segmento en vez de como un rayo</param>
        /// <returns>Devuelve verdadero si hay intersección, y falso en el resto de los casos</returns>
        public bool Intersects(Ray ray, bool segmentMode)
        {
            float? distanceToPoint = null;
            Vector3? intersectionPoint = null;
            return Intersects(ray, out intersectionPoint, out distanceToPoint, segmentMode);
        }
        /// <summary>
        /// Obtiene si existe intersección entre el rayo y el triángulo
        /// </summary>
        /// <param name="ray">Rayo</param>
        /// <param name="intersectionPoint">Punto de interseccion</param>
        /// <returns>Devuelve verdadero si hay intersección, y falso en el resto de los casos</returns>
        public bool Intersects(Ray ray, out Vector3? intersectionPoint)
        {
            float? distanceToPoint = null;
            return Intersects(ray, out intersectionPoint, out distanceToPoint, false);
        }
        /// <summary>
        /// Obtiene si existe intersección entre el rayo y el triángulo
        /// </summary>
        /// <param name="ray">Rayo</param>
        /// <param name="intersectionPoint">Punto de interseccion</param>
        /// <param name="segmentMode">Indica si usar el rayo como un segmento en vez de como un rayo</param>
        /// <returns>Devuelve verdadero si hay intersección, y falso en el resto de los casos</returns>
        public bool Intersects(Ray ray, out Vector3? intersectionPoint, bool segmentMode)
        {
            float? distanceToPoint = null;
            return Intersects(ray, out intersectionPoint, out distanceToPoint, segmentMode);
        }
        /// <summary>
        /// Obtiene si existe intersección entre el rayo y el triángulo
        /// </summary>
        /// <param name="ray">Rayo</param>
        /// <param name="intersectionPoint">Punto de interseccion</param>
        /// <param name="distanceToPoint">Distancia al punto de interscción desde el origen del rayo</param>
        /// <returns>Devuelve verdadero si hay intersección, y falso en el resto de los casos</returns>
        public bool Intersects(Ray ray, out Vector3? intersectionPoint, out float? distanceToPoint)
        {
            return Intersects(ray, out intersectionPoint, out distanceToPoint, false);
        }
        /// <summary>
        /// Obtiene si existe intersección entre el rayo y el triángulo
        /// </summary>
        /// <param name="ray">Rayo</param>
        /// <param name="intersectionPoint">Punto de interseccion</param>
        /// <param name="distanceToPoint">Distancia al punto de interscción desde el origen del rayo</param>
        /// <param name="segmentMode">Indica si usar el rayo como un segmento en vez de como un rayo</param>
        /// <returns>Devuelve verdadero si hay intersección, y falso en el resto de los casos</returns>
        public bool Intersects(Ray ray, out Vector3? intersectionPoint, out float? distanceToPoint, bool segmentMode)
        {
            intersectionPoint = null;
            distanceToPoint = null;

            float denom = Vector3.Dot(this.Plane.Normal, ray.Direction);
            if (PhysicsMathHelper.IsZero(denom))
            {
                return false;
            }

            float t = -(this.Plane.D + Vector3.Dot(this.Plane.Normal, ray.Position)) / denom;
            if (t <= 0.0f)
            {
                return false;
            }

            Vector3 intersection = ray.Position + (t * ray.Direction);
            if (this.PointInTriangle(intersection))
            {
                float distance = Vector3.Distance(ray.Position, intersection);

                if (segmentMode)
                {
                    if (distance <= ray.Direction.Length())
                    {
                        intersectionPoint = intersection;
                        distanceToPoint = distance;

                        return true;
                    }
                }
                else
                {
                    intersectionPoint = intersection;
                    distanceToPoint = distance;

                    return true;
                }
            }

            return false;
        }

        public bool Intersects(BoundingSphere bsph, out Vector3? intersectionPoint, out float? distanceToPoint)
        {
            // TODO: Intersección con una esfera

            intersectionPoint = null;
            distanceToPoint = null;

            return false;
        }

        public bool Intersects(Triangle triangle)
        {
            // TODO: Intersección con un triángulo

            return false;
        }
        
        /// <summary>
        /// Obtiene si el punto especificado está contenido en el triángulo
        /// </summary>
        /// <param name="point">Punto</param>
        /// <returns>Devuelve verdadero si el punto está contenido en el triángulo, o falso en el resto de los casos</returns>
        public bool PointInTriangle(Vector3 point)
        {
            Vector3 u = new Vector3(
                this.PointFromVector3(point, this.I1) - this.PointFromVector3(this.Point1, this.I1),
                this.PointFromVector3(this.Point2, this.I1) - this.PointFromVector3(this.Point1, this.I1),
                this.PointFromVector3(this.Point3, this.I1) - this.PointFromVector3(this.Point1, this.I1));

            Vector3 v = new Vector3(
                this.PointFromVector3(point, this.I2) - this.PointFromVector3(this.Point1, this.I2),
                this.PointFromVector3(this.Point2, this.I2) - this.PointFromVector3(this.Point1, this.I2),
                this.PointFromVector3(this.Point3, this.I2) - this.PointFromVector3(this.Point1, this.I2));

            float a, b;
            if (u.Y == 0.0f)
            {
                b = u.X / u.Z;
                if (b >= 0.0f && b <= 1.0f)
                {
                    a = (v.X - b * v.Z) / v.Y;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                b = (v.X * u.Y - u.X * v.Y) / (v.Z * u.Y - u.Z * v.Y);
                if (b >= 0.0f && b <= 1.0f)
                {
                    a = (u.X - b * u.Z) / u.Y;
                }
                else
                {
                    return false;
                }
            }

            return (a >= 0 && (a + b) <= 1);
        }

        /// <summary>
        /// Obtiene el valor de la componente del vector especificado por índice
        /// </summary>
        /// <param name="vector">Vector</param>
        /// <param name="index">Indice 0, 1 o 2 para obtener las componentes x, y o z</param>
        /// <returns>Devuelve la componente del vector especificada por el índice</returns>
        private float PointFromVector3(Vector3 vector, int index)
        {
            if (index == 0)
            {
                return vector.X;
            }
            else if (index == 1)
            {
                return vector.Y;
            }
            else if (index == 2)
            {
                return vector.Z;
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
