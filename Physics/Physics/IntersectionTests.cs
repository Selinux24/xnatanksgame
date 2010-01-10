using System;
using Microsoft.Xna.Framework;


namespace Physics
{
    public class IntersectionTests
    {
        public static bool SphereAndHalfSpace(CollisionSphere sphere, CollisionPlane plane)
        {
            // Find the distance from the origin
            float ballDistance = Vector3.Dot(plane.Normal, sphere.Position) - sphere.Radius;

            // Check for the intersection
            return ballDistance <= plane.D;
        }

        public static bool SphereAndSphere(CollisionSphere one, CollisionSphere two)
        {
            // Find the vector between the objects
            Vector3 midline = one.Position - two.Position;

            // See if it is large enough.
            return midline.LengthSquared() < (one.Radius + two.Radius) * (one.Radius + two.Radius);
        }

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

        static float TransformToAxis(CollisionBox box, Vector3 axis)
        {
            return
                box.HalfSize.X * Math.Abs(Vector3.Dot(axis, box.XAxis)) +
                box.HalfSize.Y * Math.Abs(Vector3.Dot(axis, box.YAxis)) +
                box.HalfSize.Z * Math.Abs(Vector3.Dot(axis, box.ZAxis));
        }

        static bool OverlapOnAxis(CollisionBox one, CollisionBox two, Vector3 axis, Vector3 toCentre)
        {
            // Project the half-size of one onto axis
            float oneProject = TransformToAxis(one, axis);
            float twoProject = TransformToAxis(two, axis);

            // Project this onto the axis
            float distance = Math.Abs(Vector3.Dot(toCentre, axis));

            // Check for overlap
            return (distance < oneProject + twoProject);
        }

        public static bool BoxAndBox(CollisionBox one, CollisionBox two)
        {
            // Find the vector between the two centres
            Vector3 toCentre = two.Position - one.Position;

            return (
                // Check on box one's axes first
                OverlapOnAxis(one, two, one.XAxis, toCentre) &&
                OverlapOnAxis(one, two, one.YAxis, toCentre) &&
                OverlapOnAxis(one, two, one.ZAxis, toCentre) &&

                // And on two's
                OverlapOnAxis(one, two, two.XAxis, toCentre) &&
                OverlapOnAxis(one, two, two.YAxis, toCentre) &&
                OverlapOnAxis(one, two, two.ZAxis, toCentre) &&

                // Now on the cross products
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
        /// Does an intersection test on an arbitrarily aligned box and a
        /// half-space.
        ///
        /// The box is given as a transform matrix, including
        /// position, and a vector of half-sizes for the extend of the
        /// box along each local axis.
        ///
        /// The half-space is given as a direction (i.e. unit) vector and the
        /// offset of the limiting plane from the origin, along the given
        /// direction.
        /// </summary>
        public static bool BoxAndHalfSpace(CollisionBox box, CollisionPlane plane)
        {
            // Work out the projected radius of the box onto the plane direction
            float projectedRadius = TransformToAxis(box, plane.Normal);

            // Work out how far the box is from the origin
            float boxDistance = Vector3.Dot(plane.Normal, box.Position) - projectedRadius;

            // Check for the intersection
            if (boxDistance <= plane.D)
            {
                return true;
            }

            return false;
        }


        public static bool BoxAndTri(CollisionBox box, Triangle tri)
        {
            // Pasar el triángulo a coordenadas de la caja
            Triangle trnTri = new Triangle(
                Vector3.Subtract(tri.Point1, box.Position),
                Vector3.Subtract(tri.Point2, box.Position),
                Vector3.Subtract(tri.Point3, box.Position));

            // Obtener los ejes del triángulo
            Vector3 edge0 = Vector3.Subtract(trnTri.Point2, trnTri.Point1);
            Vector3 edge1 = Vector3.Subtract(trnTri.Point3, trnTri.Point2);
            Vector3 edge2 = Vector3.Subtract(trnTri.Point1, trnTri.Point3);

            // Comprobar si la caja está sobre el triángulo
            float fex = Math.Abs(edge0.X);
            float fey = Math.Abs(edge0.Y);
            float fez = Math.Abs(edge0.Z);

            if (!AXISTEST_X01(trnTri.Point1, trnTri.Point3, box.HalfSize, edge0.Z, edge0.Y, fez, fey)) return false;
            if (!AXISTEST_Y02(trnTri.Point1, trnTri.Point3, box.HalfSize, edge0.Z, edge0.X, fez, fex)) return false;
            if (!AXISTEST_Z12(trnTri.Point2, trnTri.Point3, box.HalfSize, edge0.Y, edge0.X, fey, fex)) return false;

            fex = Math.Abs(edge1.X);
            fey = Math.Abs(edge1.Y);
            fez = Math.Abs(edge1.Z);

            if (!AXISTEST_X01(trnTri.Point1, trnTri.Point3, box.HalfSize, edge1.Z, edge1.Y, fez, fey)) return false;
            if (!AXISTEST_Y02(trnTri.Point1, trnTri.Point3, box.HalfSize, edge1.Z, edge1.X, fez, fex)) return false;
            if (!AXISTEST_Z0(trnTri.Point1, trnTri.Point2, box.HalfSize, edge1.Y, edge1.X, fey, fex)) return false;

            fex = Math.Abs(edge2.X);
            fey = Math.Abs(edge2.Y);
            fez = Math.Abs(edge2.Z);

            if (!AXISTEST_X2(trnTri.Point1, trnTri.Point2, box.HalfSize, edge2.Z, edge2.Y, fez, fey)) return false;
            if (!AXISTEST_Y1(trnTri.Point1, trnTri.Point2, box.HalfSize, edge2.Z, edge2.X, fez, fex)) return false;
            if (!AXISTEST_Z12(trnTri.Point2, trnTri.Point3, box.HalfSize, edge2.Y, edge2.X, fey, fex)) return false;

            // Hay intersección
            return true;
        }

        static bool AXISTEST_Z0(Vector3 v0, Vector3 v1, Vector3 boxhalfsize, float a, float b, float fa, float fb)
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

        static bool AXISTEST_Y1(Vector3 v0, Vector3 v1, Vector3 boxhalfsize, float a, float b, float fa, float fb)
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

        static bool AXISTEST_Z12(Vector3 v1, Vector3 v2, Vector3 boxhalfsize, float a, float b, float fa, float fb)
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

        static bool AXISTEST_Y02(Vector3 v0, Vector3 v2, Vector3 boxhalfsize, float a, float b, float fa, float fb)
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

        static bool AXISTEST_X01(Vector3 v0, Vector3 v2, Vector3 boxhalfsize, float a, float b, float fa, float fb)
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

        static bool AXISTEST_X2(Vector3 v0, Vector3 v1, Vector3 boxhalfsize, float a, float b, float fa, float fb)
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

        static void FINDMINMAX(float x0, float x1, float x2, out float min, out float max)
        {
            min = max = x0;

            if (x1 < min) min = x1;
            if (x1 > max) max = x1;
            if (x2 < min) min = x2;
            if (x2 > max) max = x2;
        }

        public static bool BoxAndPlane(Vector3 planeNormal, Vector3 planePoint, CollisionBox box)
        {
            Vector3 vmin;
            Vector3 vmax;

            if (planeNormal.X > 0.0f)
            {
                vmin.X = -box.HalfSize.X - planePoint.X;
                vmax.X = box.HalfSize.X - planePoint.X;
            }
            else
            {
                vmin.X = box.HalfSize.X - planePoint.X;
                vmax.X = -box.HalfSize.X - planePoint.X;
            }

            if (planeNormal.Y > 0.0f)
            {
                vmin.Y = -box.HalfSize.Y - planePoint.Y;
                vmax.Y = box.HalfSize.Y - planePoint.Y;
            }
            else
            {
                vmin.Y = box.HalfSize.Y - planePoint.Y;
                vmax.Y = -box.HalfSize.Y - planePoint.Y;
            }

            if (planeNormal.Z > 0.0f)
            {
                vmin.Z = -box.HalfSize.Z - planePoint.Z;
                vmax.Z = box.HalfSize.Z - planePoint.Z;
            }
            else
            {
                vmin.Z = box.HalfSize.Z - planePoint.Z;
                vmax.Z = -box.HalfSize.Z - planePoint.Z;
            }

            if (Vector3.Dot(planeNormal, vmin) > 0.0f) return false;

            if (Vector3.Dot(planeNormal, vmax) >= 0.0f) return true;

            return false;
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
            if (Core.IsZero(denom))
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

        public static bool TriAndTri(Triangle tri, Triangle triangle)
        {
            // TODO: Intersección con un triángulo

            return false;
        }

        /// <summary>
        /// Obtiene si el punto especificado está contenido en el triángulo
        /// </summary>
        /// <param name="point">Punto</param>
        /// <returns>Devuelve verdadero si el punto está contenido en el triángulo, o falso en el resto de los casos</returns>
        public static bool PointInTriangle(Triangle tri, Vector3 point)
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