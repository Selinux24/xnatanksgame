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
            return boxDistance <= plane.D;
        }


        public static bool BoxAndTri(CollisionBox box, Triangle tri, ref CollisionData data)
        {
            Triangle trnTri = new Triangle(
                Vector3.Subtract(tri.Point1, box.Position),
                Vector3.Subtract(tri.Point2, box.Position),
                Vector3.Subtract(tri.Point3, box.Position));

            // Ejes del triángulo
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

            // Colisión
            CollisionPlane plane = new CollisionPlane(tri.Plane.Normal, tri.Plane.D);
            return CollisionDetector.BoxAndHalfSpace(box, plane, ref data);
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
    }
}