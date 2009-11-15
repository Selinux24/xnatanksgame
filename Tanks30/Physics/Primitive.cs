using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Physics
{
    /// <summary>
    /// Una primitiva
    /// </summary>
    public struct Primitive
    {
        /// <summary>
        /// Vector 1
        /// </summary>
        public readonly Vector3 Vertex1;
        /// <summary>
        /// Vector 2
        /// </summary>
        public readonly Vector3 Vertex2;
        /// <summary>
        /// Vector 3
        /// </summary>
        public readonly Vector3 Vertex3;
        /// <summary>
        /// Plano en el que est� contenido el tri�ngulo
        /// </summary>
        public readonly Plane Plane;
        /// <summary>
        /// Obtiene la normal de la primitiva
        /// </summary>
        public readonly Vector3 Normal;
        /// <summary>
        /// Obtiene el baricentro
        /// </summary>
        public readonly Vector3 Barycentric;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="vertex1">Vector 1</param>
        /// <param name="vertex2">Vector 2</param>
        /// <param name="vertex3">Vector 3</param>
        public Primitive(Vector3 vertex1, Vector3 vertex2, Vector3 vertex3)
        {
            this.Vertex1 = vertex1;
            this.Vertex2 = vertex2;
            this.Vertex3 = vertex3;

            this.Plane = new Plane(Vertex1, Vertex3, Vertex2);
            this.Normal = Plane.Normal;

            float p = 1.0f / 3.0f;
            this.Barycentric = Vector3.Barycentric(Vertex1, Vertex2, Vertex3, p, p);
        }

        /// <summary>
        /// Obtiene la distancia entre el origen del rayo y el tri�ngulo especificados
        /// </summary>
        /// <param name="ray">Rayo</param>
        /// <param name="tri">Tri�ngulo</param>
        /// <returns>Devuelve la distancia de intersecci�n si existe o nada</returns>
        public static float? Intersects(Ray ray, Primitive tri)
        {
            float numer = Vector3.Dot(tri.Plane.Normal, ray.Position) + tri.Plane.D;
            float denom = Vector3.Dot(tri.Plane.Normal, ray.Direction);

            return -(numer / denom);
        }
        /// <summary>
        /// Obtiene el punto de intersecci�n del rayo y el tri�ngulo especificados
        /// </summary>
        /// <param name="ray">Rayo</param>
        /// <param name="tri">Tri�ngulo</param>
        /// <param name="point">Devuelve el punto de intersecci�n</param>
        /// <returns>Devuelve el punto de intersecci�n o nada</returns>
        public static float? Intersects(Ray ray, Primitive tri, out Vector3? point)
        {
            point = null;

            //Obtener la distancia del rayo al plano
            float? distance = null;
            distance = ray.Intersects(tri.Plane);
            //distance = Primitive.Intersects(ray, tri);
            if (distance.HasValue)
            {
                //Calcular el punto de colisi�n desde el punto usando el rayo multiplicado por la distancia
                Vector3 collisionPoint = ray.Position + Vector3.Multiply(ray.Direction, distance.Value);
                if (Primitive.PointInTriangle(collisionPoint, tri))
                {
                    point = collisionPoint;

                    return distance;
                }
            }

            return null;
        }

        public static float? Intersects(BoundingSphere bsph, out Vector3? point)
        {
            // TODO: Intersecci�n con una esfera

            point = Vector3.Zero;

            return null;
        }

        public static bool Intersects(Primitive tri)
        {
            // TODO: Intersecci�n con un tri�ngulo

            return false;
        }

        /// <summary>
        /// Obtiene si los puntos pasados est�n en la misma cara de un plano
        /// </summary>
        /// <param name="p1">Punto 1</param>
        /// <param name="p2">Punot 2</param>
        /// <param name="a">Punto 3</param>
        /// <param name="b">Punto 4</param>
        /// <returns></returns>
        public static bool SameSide(Vector3 p1, Vector3 p2, Vector3 a, Vector3 b)
        {
            // TODO: Aqu� est� el problema de rendimiento
            Vector3 cp1 = Vector3.Cross(b - a, p1 - a);
            Vector3 cp2 = Vector3.Cross(b - a, p2 - a);
            if (Vector3.Dot(cp1, cp2) >= 0.0f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Obtiene si el punto est� contenido en el tri�ngulo
        /// </summary>
        /// <param name="point">Punto</param>
        /// <param name="tri">Tri�ngulo</param>
        /// <returns>Verdadero si est� contenido en el tri�gulo, falso si no lo est�</returns>
        public static bool PointInTriangle(Vector3 point, Primitive tri)
        {
            if ((SameSide(point, tri.Vertex1, tri.Vertex2, tri.Vertex3)) &&
                (SameSide(point, tri.Vertex2, tri.Vertex1, tri.Vertex3)) &&
                (SameSide(point, tri.Vertex3, tri.Vertex1, tri.Vertex2)))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Obtiene la representaci�n en texto de la primitiva
        /// </summary>
        /// <returns>Devuelve la representaci�n en texto de la primitiva</returns>
        public override string ToString()
        {
            string mask = @"P1:{0} P2:{1} P3:{2} Normal:{3} Bar:{4}";

            return string.Format(mask, Vertex1, Vertex2, Vertex3, Normal, Barycentric);
        }
    }
}
