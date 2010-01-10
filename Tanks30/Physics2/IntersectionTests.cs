using System;
using Microsoft.Xna.Framework;

namespace Physics
{
    public class IntersectionTests
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

            // Ejes del triángulo
            Vector3 edge0 = Vector3.Subtract(trnTri.Point2, trnTri.Point1);
            Vector3 edge1 = Vector3.Subtract(trnTri.Point3, trnTri.Point2);
            Vector3 edge2 = Vector3.Subtract(trnTri.Point1, trnTri.Point3);

            // Eje 0
            float fex = Math.Abs(edge0.X);
            float fey = Math.Abs(edge0.Y);
            float fez = Math.Abs(edge0.Z);

            if (!AXISTEST_X01(trnTri.Point1, trnTri.Point3, box.HalfSize, edge0.Z, edge0.Y, fez, fey)) return false;
            if (!AXISTEST_Y02(trnTri.Point1, trnTri.Point3, box.HalfSize, edge0.Z, edge0.X, fez, fex)) return false;
            if (!AXISTEST_Z12(trnTri.Point2, trnTri.Point3, box.HalfSize, edge0.Y, edge0.X, fey, fex)) return false;

            // Eje 1
            fex = Math.Abs(edge1.X);
            fey = Math.Abs(edge1.Y);
            fez = Math.Abs(edge1.Z);

            if (!AXISTEST_X01(trnTri.Point1, trnTri.Point3, box.HalfSize, edge1.Z, edge1.Y, fez, fey)) return false;
            if (!AXISTEST_Y02(trnTri.Point1, trnTri.Point3, box.HalfSize, edge1.Z, edge1.X, fez, fex)) return false;
            if (!AXISTEST_Z0(trnTri.Point1, trnTri.Point2, box.HalfSize, edge1.Y, edge1.X, fey, fex)) return false;

            // Eje 2
            fex = Math.Abs(edge2.X);
            fey = Math.Abs(edge2.Y);
            fez = Math.Abs(edge2.Z);

            if (!AXISTEST_X2(trnTri.Point1, trnTri.Point2, box.HalfSize, edge2.Z, edge2.Y, fez, fey)) return false;
            if (!AXISTEST_Y1(trnTri.Point1, trnTri.Point2, box.HalfSize, edge2.Z, edge2.X, fez, fex)) return false;
            if (!AXISTEST_Z12(trnTri.Point2, trnTri.Point3, box.HalfSize, edge2.Y, edge2.X, fey, fex)) return false;

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
            if (PhysicsMathHelper.IsZero(denom))
            {
                return false;
            }

            float t = -(tri.Plane.D + Vector3.Dot(tri.Plane.Normal, ray.Position)) / denom;
            if (t <= 0.0f)
            {
                return false;
            }

            Vector3 intersection = ray.Position + (t * ray.Direction);
            if (PointInTriangle(tri, intersection))
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

        private static bool AXISTEST_Z0(Vector3 v0, Vector3 v1, Vector3 boxhalfsize, float a, float b, float fa, float fb)
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

            float rad = fa * boxhalfsize.X + fb * boxhalfsize.Y;
            if (min > rad || max < -rad)
            {
                return false;
            }

            return true;
        }

        private static bool AXISTEST_Y1(Vector3 v0, Vector3 v1, Vector3 boxhalfsize, float a, float b, float fa, float fb)
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

            float rad = fa * boxhalfsize.X + fb * boxhalfsize.Z;
            if (min > rad || max < -rad)
            {
                return false;
            }

            return true;
        }

        private static bool AXISTEST_Z12(Vector3 v1, Vector3 v2, Vector3 boxhalfsize, float a, float b, float fa, float fb)
        {
            float min, max;
            float p1 = a * v1.X - b * v1.Y;
            float p2 = a * v2.X - b * v2.Y;
            if (p2 < p1)
            {
                min = p2;
                max = p1;
            }
            else
            {
                min = p1;
                max = p2;
            }

            float rad = fa * boxhalfsize.X + fb * boxhalfsize.Y;
            if (min > rad || max < -rad)
            {
                return false;
            }

            return true;
        }

        private static bool AXISTEST_Y02(Vector3 v0, Vector3 v2, Vector3 boxhalfsize, float a, float b, float fa, float fb)
        {
            float min, max;
            float p0 = -a * v0.X + b * v0.Z;
            float p2 = -a * v2.X + b * v2.Z;
            if (p0 < p2)
            {
                min = p0;
                max = p2;
            }
            else
            {
                min = p2;
                max = p0;
            }

            float rad = fa * boxhalfsize.X + fb * boxhalfsize.Z;
            if (min > rad || max < -rad)
            {
                return false;
            }

            return true;
        }

        private static bool AXISTEST_X01(Vector3 v0, Vector3 v2, Vector3 boxhalfsize, float a, float b, float fa, float fb)
        {
            float min, max;
            float p0 = a * v0.Y - b * v0.Z;
            float p2 = a * v2.Y - b * v2.Z;
            if (p0 < p2)
            {
                min = p0;
                max = p2;
            }
            else
            {
                min = p2;
                max = p0;
            }

            float rad = fa * boxhalfsize.Y + fb * boxhalfsize.Z;
            if (min > rad || max < -rad)
            {
                return false;
            }

            return true;
        }

        private static bool AXISTEST_X2(Vector3 v0, Vector3 v1, Vector3 boxhalfsize, float a, float b, float fa, float fb)
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

            float rad = fa * boxhalfsize.Y + fb * boxhalfsize.Z;
            if (min > rad || max < -rad)
            {
                return false;
            }

            return true;
        }

        private static void FINDMINMAX(float x0, float x1, float x2, out float min, out float max)
        {
            min = max = x0;

            if (x1 < min) min = x1;
            if (x1 > max) max = x1;
            if (x2 < min) min = x2;
            if (x2 > max) max = x2;
        }
        /// <summary>
        /// Obtiene si el punto especificado está contenido en el triángulo
        /// </summary>
        /// <param name="point">Punto</param>
        /// <returns>Devuelve verdadero si el punto está contenido en el triángulo, o falso en el resto de los casos</returns>
        private static bool PointInTriangle(Triangle tri, Vector3 point)
        {
            Vector3 u = new Vector3(
                Triangle.PointFromVector3(point, tri.I1) - Triangle.PointFromVector3(tri.Point1, tri.I1),
                Triangle.PointFromVector3(tri.Point2, tri.I1) - Triangle.PointFromVector3(tri.Point1, tri.I1),
                Triangle.PointFromVector3(tri.Point3, tri.I1) - Triangle.PointFromVector3(tri.Point1, tri.I1));

            Vector3 v = new Vector3(
                Triangle.PointFromVector3(point, tri.I2) - Triangle.PointFromVector3(tri.Point1, tri.I2),
                Triangle.PointFromVector3(tri.Point2, tri.I2) - Triangle.PointFromVector3(tri.Point1, tri.I2),
                Triangle.PointFromVector3(tri.Point3, tri.I2) - Triangle.PointFromVector3(tri.Point1, tri.I2));

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
    }
}