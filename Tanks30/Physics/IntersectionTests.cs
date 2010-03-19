using System;
using Microsoft.Xna.Framework;

namespace Physics
{
    using Common.Primitives;

    /// <summary>
    /// Conjunto de test de detección de intersección
    /// </summary>
    public abstract class IntersectionTests
    {
        /// <summary>
        /// Detecta la intersección entre una esfera y un plano
        /// </summary>
        /// <param name="sphere">Esfera</param>
        /// <param name="plane">Plano</param>
        /// <returns>Devuelve verdadero si hay intersección</returns>
        public static bool SphereAndPlane(CollisionSphere sphere, CollisionPlane plane)
        {
            return SphereAndPlane(sphere, plane.Normal, plane.D);
        }
        /// <summary>
        /// Detecta la intersección entre una esfera y un plano
        /// </summary>
        /// <param name="sphere">Esfera</param>
        /// <param name="plane">Plano</param>
        /// <returns>Devuelve verdadero si hay intersección</returns>
        public static bool SphereAndPlane(CollisionSphere sphere, Plane plane)
        {
            return SphereAndPlane(sphere, plane.Normal, plane.D);
        }
        /// <summary>
        /// Detecta la intersección entre una esfera y un plano
        /// </summary>
        /// <param name="sphere">Esfera</param>
        /// <param name="normal">Normal del plano</param>
        /// <param name="d">Distancia</param>
        /// <returns>Devuelve verdadero si hay intersección</returns>
        public static bool SphereAndPlane(CollisionSphere sphere, Vector3 normal, float d)
        {
            // Buscar la distancia al origen
            float ballDistance = Vector3.Dot(normal, sphere.Position) - sphere.Radius;

            // Si la distancia es menor hay intersección
            return ballDistance <= d;
        }
        /// <summary>
        /// Detecta la intersección entre dos esferas
        /// </summary>
        /// <param name="one">Esfera</param>
        /// <param name="two">Esfera</param>
        /// <returns>Devuelve verdadero si hay intersección</returns>
        public static bool SphereAndSphere(CollisionSphere one, CollisionSphere two)
        {
            // Vector entre ambas esferas
            Vector3 midline = one.Position - two.Position;

            // Si el vector es más largo que la suma de los radios, no hay intersección
            return midline.LengthSquared() < (one.Radius + two.Radius) * (one.Radius + two.Radius);
        }
        /// <summary>
        /// Detecta la intersección entre una esfera y un triángulo
        /// </summary>
        /// <param name="sphere">Esfera</param>
        /// <param name="triangle">Triángulo</param>
        /// <param name="halfSpace">Indica si toma el triángulo como un HalfSpace, para tener en cuenta la dirección de la normal</param>
        /// <returns>Devuelve verdadero si hay intersección</returns>
        public static bool SphereAndTri(CollisionSphere sphere, Triangle triangle, bool halfSpace)
        {
            bool axisTest = true;

            if (!halfSpace)
            {
                // Find the distance from the origin
                float ballDistance = Math.Abs(Vector3.Dot(triangle.Plane.Normal, sphere.Position)) - sphere.Radius;

                // Check for the intersection
                if (ballDistance > triangle.Plane.D)
                {
                    axisTest = false;
                }
            }

            if (axisTest)
            {
                // Obtener los ejes del triángulo
                Vector3 edge0 = Vector3.Subtract(triangle.Point2, triangle.Point1);
                Vector3 edge1 = Vector3.Subtract(triangle.Point3, triangle.Point2);
                Vector3 edge2 = Vector3.Subtract(triangle.Point1, triangle.Point3);

                // Proyectar desde el punto 1
                Vector3 toPoint1 = sphere.Position - triangle.Point1;
                float distanceTo1 = toPoint1.Length();
                toPoint1.Normalize();
                float f1a = Math.Abs(Vector3.Dot(edge0, toPoint1));
                float f1b = Math.Abs(Vector3.Dot(edge2, toPoint1));
                if (distanceTo1 > f1a + sphere.Radius) return false;
                if (distanceTo1 > f1a + sphere.Radius) return false;

                // Proyectar desde el punto 2
                Vector3 toPoint2 = sphere.Position - triangle.Point2;
                float distanceTo2 = toPoint2.Length();
                toPoint2.Normalize();
                float f2a = Math.Abs(Vector3.Dot(edge0, toPoint2));
                float f2b = Math.Abs(Vector3.Dot(edge1, toPoint2));
                if (distanceTo2 > f2a + sphere.Radius) return false;
                if (distanceTo2 > f2a + sphere.Radius) return false;

                // Proyectar desde el punto 3
                Vector3 toPoint3 = sphere.Position - triangle.Point3;
                float distanceTo3 = toPoint3.Length();
                toPoint3.Normalize();
                float f3a = Math.Abs(Vector3.Dot(edge1, toPoint3));
                float f3b = Math.Abs(Vector3.Dot(edge2, toPoint3));
                if (distanceTo3 > f3a + sphere.Radius) return false;
                if (distanceTo3 > f3a + sphere.Radius) return false;

                return true;
            }

            return false;
        }
        /// <summary>
        /// Detecta la intersección entre dos cajas
        /// </summary>
        /// <param name="one">Caja</param>
        /// <param name="two">Caja</param>
        /// <returns>Devuelve verdadero si hay intersección</returns>
        public static bool BoxAndBox(CollisionBox one, CollisionBox two)
        {
            // Encontrar el vector entre los centros
            Vector3 toCentre = two.Position - one.Position;

            return (
                // Primero los ejes de la primera caja
                OverlapOnAxis(one, two, one.XAxis, toCentre) &&
                OverlapOnAxis(one, two, one.YAxis, toCentre) &&
                OverlapOnAxis(one, two, one.ZAxis, toCentre) &&

                // Segundo, los ejes de la segunda caja
                OverlapOnAxis(one, two, two.XAxis, toCentre) &&
                OverlapOnAxis(one, two, two.YAxis, toCentre) &&
                OverlapOnAxis(one, two, two.ZAxis, toCentre) &&

                // Tercero, los ejes cruzados
                OverlapOnAxis(one, two, Vector3.Cross(one.XAxis, two.XAxis), toCentre) &&
                OverlapOnAxis(one, two, Vector3.Cross(one.XAxis, two.YAxis), toCentre) &&
                OverlapOnAxis(one, two, Vector3.Cross(one.XAxis, two.ZAxis), toCentre) &&
                OverlapOnAxis(one, two, Vector3.Cross(one.YAxis, two.XAxis), toCentre) &&
                OverlapOnAxis(one, two, Vector3.Cross(one.YAxis, two.YAxis), toCentre) &&
                OverlapOnAxis(one, two, Vector3.Cross(one.YAxis, two.ZAxis), toCentre) &&
                OverlapOnAxis(one, two, Vector3.Cross(one.ZAxis, two.XAxis), toCentre) &&
                OverlapOnAxis(one, two, Vector3.Cross(one.ZAxis, two.YAxis), toCentre) &&
                OverlapOnAxis(one, two, Vector3.Cross(one.ZAxis, two.ZAxis), toCentre)
            );
        }
        /// <summary>
        /// Detecta la intersección entre una caja y un plano
        /// </summary>
        /// <param name="box">Caja</param>
        /// <param name="plane">Plano</param>
        /// <returns>Devuelve verdadero si hay intersección</returns>
        public static bool BoxAndPlane(CollisionBox box, CollisionPlane plane)
        {
            return BoxAndPlane(box, plane.Normal, plane.D);
        }
        /// <summary>
        /// Detecta la intersección entre una caja y un plano
        /// </summary>
        /// <param name="box">Caja</param>
        /// <param name="plane">Plano</param>
        /// <returns>Devuelve verdadero si hay intersección</returns>
        public static bool BoxAndPlane(CollisionBox box, Plane plane)
        {
            return BoxAndPlane(box, plane.Normal, plane.D);
        }
        /// <summary>
        /// Detecta la intersección entre una caja y un plano
        /// </summary>
        /// <param name="box">Caja</param>
        /// <param name="normal">Normal del plano</param>
        /// <param name="d">Distancia del plano</param>
        /// <returns>Devuelve verdadero si hay intersección</returns>
        public static bool BoxAndPlane(CollisionBox box, Vector3 normal, float d)
        {
            // Proyectar la caja sobre la normal del plano
            float projectedBox = TransformToAxis(box, normal);

            // Obtener la distancia del centro de la caja al plano sobre la normal
            float boxDistance = Vector3.Dot(normal, box.Position) - projectedBox;

            // Si la distancia es menor a d, hay intersección
            return boxDistance <= d;
        }
        /// <summary>
        /// Detecta la intersección entre una caja y un triángulo
        /// </summary>
        /// <param name="box">Caja</param>
        /// <param name="tri">Triángulo</param>
        /// <returns>Devuelve verdadero si hay intersección</returns>
        public static bool BoxAndTri(CollisionBox box, Triangle tri)
        {
            // Pasar el triángulo al espacio de coordenadas de la caja
            Triangle trnTri = new Triangle(
                Vector3.Subtract(tri.Point1, box.Position),
                Vector3.Subtract(tri.Point2, box.Position),
                Vector3.Subtract(tri.Point3, box.Position));

            // Eje 0
            Vector3 edge0 = Vector3.Subtract(trnTri.Point2, trnTri.Point1);
            if (!AxisTest_X(trnTri.Point1, trnTri.Point3, box.HalfSize, edge0.Z, edge0.Y)) return false;
            if (!AxisTest_Y(trnTri.Point1, trnTri.Point3, box.HalfSize, edge0.Z, edge0.X)) return false;
            if (!AxisTest_Z(trnTri.Point2, trnTri.Point3, box.HalfSize, edge0.Y, edge0.X)) return false;

            // Eje 1
            Vector3 edge1 = Vector3.Subtract(trnTri.Point3, trnTri.Point2);
            if (!AxisTest_X(trnTri.Point1, trnTri.Point3, box.HalfSize, edge1.Z, edge1.Y)) return false;
            if (!AxisTest_Y(trnTri.Point1, trnTri.Point3, box.HalfSize, edge1.Z, edge1.X)) return false;
            if (!AxisTest_Z(trnTri.Point1, trnTri.Point2, box.HalfSize, edge1.Y, edge1.X)) return false;

            // Eje 2
            Vector3 edge2 = Vector3.Subtract(trnTri.Point1, trnTri.Point3);
            if (!AxisTest_X(trnTri.Point1, trnTri.Point2, box.HalfSize, edge2.Z, edge2.Y)) return false;
            if (!AxisTest_Y(trnTri.Point1, trnTri.Point2, box.HalfSize, edge2.Z, edge2.X)) return false;
            if (!AxisTest_Z(trnTri.Point2, trnTri.Point3, box.HalfSize, edge2.Y, edge2.X)) return false;

            // Hay intersección
            return true;
        }
        /// <summary>
        /// Obtiene si existe intersección entre el rayo y el triángulo
        /// </summary>
        /// <param name="ray">Rayo</param>
        /// <returns>Devuelve verdadero si hay intersección, y falso en el resto de los casos</returns>
        public static bool TriAndRay(Triangle tri, Ray ray)
        {
            float? distanceToPoint = null;
            Vector3? intersectionPoint = null;
            return TriAndRay(tri, ray, out intersectionPoint, out distanceToPoint, false);
        }
        /// <summary>
        /// Obtiene si existe intersección entre el rayo y el triángulo
        /// </summary>
        /// <param name="ray">Rayo</param>
        /// <param name="segmentMode">Indica si usar el rayo como un segmento en vez de como un rayo</param>
        /// <returns>Devuelve verdadero si hay intersección, y falso en el resto de los casos</returns>
        public static bool TriAndRay(Triangle tri, Ray ray, bool segmentMode)
        {
            float? distanceToPoint = null;
            Vector3? intersectionPoint = null;
            return TriAndRay(tri, ray, out intersectionPoint, out distanceToPoint, segmentMode);
        }
        /// <summary>
        /// Obtiene si existe intersección entre el rayo y el triángulo
        /// </summary>
        /// <param name="ray">Rayo</param>
        /// <param name="intersectionPoint">Punto de interseccion</param>
        /// <returns>Devuelve verdadero si hay intersección, y falso en el resto de los casos</returns>
        public static bool TriAndRay(Triangle tri, Ray ray, out Vector3? intersectionPoint)
        {
            float? distanceToPoint = null;
            return TriAndRay(tri, ray, out intersectionPoint, out distanceToPoint, false);
        }
        /// <summary>
        /// Obtiene si existe intersección entre el rayo y el triángulo
        /// </summary>
        /// <param name="ray">Rayo</param>
        /// <param name="intersectionPoint">Punto de interseccion</param>
        /// <param name="segmentMode">Indica si usar el rayo como un segmento en vez de como un rayo</param>
        /// <returns>Devuelve verdadero si hay intersección, y falso en el resto de los casos</returns>
        public static bool TriAndRay(Triangle tri, Ray ray, out Vector3? intersectionPoint, bool segmentMode)
        {
            float? distanceToPoint = null;
            return TriAndRay(tri, ray, out intersectionPoint, out distanceToPoint, segmentMode);
        }
        /// <summary>
        /// Obtiene si existe intersección entre el rayo y el triángulo
        /// </summary>
        /// <param name="ray">Rayo</param>
        /// <param name="intersectionPoint">Punto de interseccion</param>
        /// <param name="distanceToPoint">Distancia al punto de interscción desde el origen del rayo</param>
        /// <returns>Devuelve verdadero si hay intersección, y falso en el resto de los casos</returns>
        public static bool TriAndRay(Triangle tri, Ray ray, out Vector3? intersectionPoint, out float? distanceToPoint)
        {
            return TriAndRay(tri, ray, out intersectionPoint, out distanceToPoint, false);
        }
        /// <summary>
        /// Obtiene si existe intersección entre el rayo y el triángulo
        /// </summary>
        /// <param name="ray">Rayo</param>
        /// <param name="intersectionPoint">Punto de interseccion</param>
        /// <param name="distanceToPoint">Distancia al punto de interscción desde el origen del rayo</param>
        /// <param name="segmentMode">Indica si usar el rayo como un segmento en vez de como un rayo</param>
        /// <returns>Devuelve verdadero si hay intersección, y falso en el resto de los casos</returns>
        public static bool TriAndRay(Triangle tri, Ray ray, out Vector3? intersectionPoint, out float? distanceToPoint, bool segmentMode)
        {
            intersectionPoint = null;
            distanceToPoint = null;

            float denom = Vector3.Dot(tri.Plane.Normal, ray.Direction);
            if (denom.IsZero())
            {
                return false;
            }

            float t = -(tri.Plane.D + Vector3.Dot(tri.Plane.Normal, ray.Position)) / denom;
            if (t <= 0.0f)
            {
                return false;
            }

            Vector3 intersection = ray.Position + (t * ray.Direction);
            if (Triangle.PointInTriangle(tri, intersection))
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
        /// <summary>
        /// Obtiene la primera intersección entre el rayo y la lista de triángulos
        /// </summary>
        /// <param name="triangleSoup">Lista de triángulos</param>
        /// <param name="ray">Rayo</param>
        /// <param name="triangle">Devuelve el triángulo de intersección si existe</param>
        /// <param name="intersectionPoint">Devuelve el punto de intersección con el triángulo</param>
        /// <param name="distanceToPoint">Devuelve la distancia desde el origen del ray al punto de intersección</param>
        /// <returns>Devuelve verdadero si hay intersección o falso en el resto de los casos</returns>
        public static bool TriangleSoupAndRay(CollisionTriangleSoup triangleSoup, Ray ray, out Triangle? triangle, out Vector3? intersectionPoint, out float? distanceToPoint)
        {
            return TriangleSoupAndRay(triangleSoup, ray, out triangle, out intersectionPoint, out distanceToPoint, false);
        }
        /// <summary>
        /// Obtiene si hay intersección entre el rayo y la lista de triángulos
        /// </summary>
        /// <param name="triangleSoup">Lista de triángulos</param>
        /// <param name="ray">Rayo</param>
        /// <param name="triangle">Devuelve el triángulo de intersección si existe</param>
        /// <param name="intersectionPoint">Devuelve el punto de intersección con el triángulo</param>
        /// <param name="distanceToPoint">Devuelve la distancia desde el origen del ray al punto de intersección</param>
        /// <param name="findNearest">Indica si se debe testear la intersección hasta encontrar el más cercano</param>
        /// <returns>Devuelve verdadero si hay intersección o falso en el resto de los casos</returns>
        public static bool TriangleSoupAndRay(CollisionTriangleSoup triangleSoup, Ray ray, out Triangle? triangle, out Vector3? intersectionPoint, out float? distanceToPoint, bool findNearest)
        {
            triangle = null;
            intersectionPoint = null;
            distanceToPoint = null;

            // Primero ver si hay contacto con el bbox
            if (ray.Intersects(triangleSoup.AABB).HasValue)
            {
                return TriangleListAndRay(
                    triangleSoup.Triangles, 
                    ray, 
                    out triangle, 
                    out intersectionPoint, 
                    out distanceToPoint, 
                    findNearest);
            }

            return false;
        }
        /// <summary>
        /// Obtiene si hay intersección entre el rayo y la lista de triángulos
        /// </summary>
        /// <param name="triangleList">Lista de triángulos</param>
        /// <param name="ray">Rayo</param>
        /// <param name="triangle">Devuelve el triángulo de intersección si existe</param>
        /// <param name="intersectionPoint">Devuelve el punto de intersección con el triángulo</param>
        /// <param name="distanceToPoint">Devuelve la distancia desde el origen del ray al punto de intersección</param>
        /// <param name="findNearest">Indica si se debe testear la intersección hasta encontrar el más cercano</param>
        /// <returns>Devuelve verdadero si hay intersección o falso en el resto de los casos</returns>
        public static bool TriangleListAndRay(Triangle[] triangleList, Ray ray, out Triangle? triangle, out Vector3? intersectionPoint, out float? distanceToPoint, bool findNearest)
        {
            triangle = null;
            intersectionPoint = null;
            distanceToPoint = null;

            // Indica si ha habido intersección
            bool intersectionOccur = false;

            // Variables para almacenar la interscción más cercana
            Triangle? closestTriangle = null;
            Vector3? closestIntersectionPoint = null;
            float? closestDistanceToPoint = null;

            // Recorremos todos los triángulos de la lista realizando el test de intersección
            foreach (Triangle currentTriangle in triangleList)
            {
                // Variables para almacener la intersección actual
                Vector3? currentIntersectionPoint = null;
                float? currentDistanceToPoint = null;

                if (IntersectionTests.TriAndRay(currentTriangle, ray, out currentIntersectionPoint, out currentDistanceToPoint))
                {
                    bool update = false;

                    if (closestDistanceToPoint.HasValue)
                    {
                        // Comprobar si la distancia obtenida en la intersección es más cercana a la obtenida anteriormente
                        if (closestDistanceToPoint.Value > currentDistanceToPoint.Value)
                        {
                            // Actualizar la intersección más cercana
                            update = true;
                        }
                    }
                    else
                    {
                        // No hay intersección todavía, se debe actualizar la intersección más cercana con la obtenida
                        update = true;
                    }

                    if (update)
                    {
                        // Indicar que ha habido una intersección
                        intersectionOccur = true;

                        // Actualizar la información de intersección más cercana
                        closestTriangle = currentTriangle;
                        closestIntersectionPoint = currentIntersectionPoint;
                        closestDistanceToPoint = currentDistanceToPoint;

                        if (!findNearest)
                        {
                            // Salimos en la primera intersección
                            break;
                        }
                    }
                }
            }

            if (intersectionOccur)
            {
                // Si ha habido intersección se establece el resultado de la intersección más cercana
                triangle = closestTriangle;
                intersectionPoint = closestIntersectionPoint;
                distanceToPoint = closestDistanceToPoint;

                return true;
            }

            return false;
        }

        private static float TransformToAxis(CollisionBox box, Vector3 axis)
        {
            return
                box.HalfSize.X * Math.Abs(Vector3.Dot(axis, box.XAxis)) +
                box.HalfSize.Y * Math.Abs(Vector3.Dot(axis, box.YAxis)) +
                box.HalfSize.Z * Math.Abs(Vector3.Dot(axis, box.ZAxis));
        }

        private static bool OverlapOnAxis(CollisionBox one, CollisionBox two, Vector3 axis, Vector3 toCentre)
        {
            // Proyectar las extensiones sobre el eje
            float oneProject = TransformToAxis(one, axis);
            float twoProject = TransformToAxis(two, axis);

            // Projectar el vector entre los centros en el eje
            float distance = Math.Abs(Vector3.Dot(toCentre, axis));

            // Si la distancia entre centros es mayor que la suma de las proyecciones, no hay intersección
            return (distance < oneProject + twoProject);
        }

        private static bool AxisTest_X(Vector3 v0, Vector3 v1, Vector3 boxhalfsize, float a, float b)
        {
            if (a != 0f && b != 0f)
            {
                float min, max;
                float p0 = a * v0.Y - b * v0.Z;
                float p1 = a * v1.Y - b * v1.Z;
                if (p0 < p1)
                {
                    min = p0;
                    max = p1;
                }
                else
                {
                    min = p1;
                    max = p0;
                }

                float fa = Math.Abs(a);
                float fb = Math.Abs(b);

                float rad = fa * boxhalfsize.Y + fb * boxhalfsize.Z;
                if (min > rad || max < -rad)
                {
                    return false;
                }
            }

            return true;
        }

        private static bool AxisTest_Y(Vector3 v0, Vector3 v1, Vector3 boxhalfsize, float a, float b)
        {
            if (a != 0f && b != 0f)
            {
                float min, max;
                float p0 = -a * v0.X + b * v0.Z;
                float p1 = -a * v1.X + b * v1.Z;
                if (p0 < p1)
                {
                    min = p0;
                    max = p1;
                }
                else
                {
                    min = p1;
                    max = p0;
                }

                float fa = Math.Abs(a);
                float fb = Math.Abs(b);

                float rad = fa * boxhalfsize.X + fb * boxhalfsize.Z;
                if (min > rad || max < -rad)
                {
                    return false;
                }
            }

            return true;
        }

        private static bool AxisTest_Z(Vector3 v0, Vector3 v1, Vector3 boxhalfsize, float a, float b)
        {
            if (a != 0f && b != 0f)
            {
                float min, max;
                float p0 = a * v0.X - b * v0.Y;
                float p1 = a * v1.X - b * v1.Y;
                if (p0 < p1)
                {
                    min = p0;
                    max = p1;
                }
                else
                {
                    min = p1;
                    max = p0;
                }

                float fa = Math.Abs(a);
                float fb = Math.Abs(b);

                float rad = fa * boxhalfsize.X + fb * boxhalfsize.Y;
                if (min > rad || max < -rad)
                {
                    return false;
                }
            }

            return true;
        }
    }
}