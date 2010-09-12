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
#if DEBUG
        public static BoundingBox[] m_DEBUGAABB = new BoundingBox[1024];
        public static int m_DEBUGAABBCOUNT = 0;
        public static Triangle[] m_DEBUGTRI = new Triangle[1024];
        public static int m_DEBUGTRICOUNT = 0;
        public static Vector3[] m_DEBUGEDGES = new Vector3[1024];
        public static int m_DEBUGEDGESCOUNT = 0;
#endif

        /// <summary>
        /// Detecta la intersección entre una esfera y un plano
        /// </summary>
        /// <param name="sphere">Esfera</param>
        /// <param name="plane">Plano</param>
        /// <returns>Devuelve verdadero si hay intersección</returns>
        public static bool SphereAndPlane(CollisionSphere sphere, CollisionPlane plane)
        {
            return SphereAndPlane(sphere, plane.Normal, plane.D) == 0;
        }
        /// <summary>
        /// Detecta la intersección entre una esfera y un plano
        /// </summary>
        /// <param name="sphere">Esfera</param>
        /// <param name="plane">Plano</param>
        /// <returns>Devuelve verdadero si hay intersección</returns>
        public static bool SphereAndPlane(CollisionSphere sphere, Plane plane)
        {
            return SphereAndPlane(sphere, plane.Normal, plane.D) == 0;
        }
        /// <summary>
        /// Detecta la intersección entre una esfera y un plano
        /// </summary>
        /// <param name="sphere">Esfera</param>
        /// <param name="normal">Normal del plano</param>
        /// <param name="d">Distancia</param>
        /// <returns>Devuelve verdadero si hay intersección</returns>
        public static int SphereAndPlane(CollisionSphere sphere, Vector3 normal, float d)
        {
            // Buscar la distancia al origen
            float distance = Vector3.Dot(normal, sphere.Position) + d;

            // Si la distancia es menor hay intersección
            if (distance > sphere.Radius)
            {
                // La esfera está sobre el plano
                return 1;
            }
            else if (distance < -sphere.Radius)
            {
                // La esfera está bajo el plano
                return -1;
            }
            else
            {
                // La esfera intersecta con el plano
                return 0;
            }
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
        public static bool SphereAndTri(CollisionSphere sphere, Triangle triangle)
        {
            // Obtener el punto más cercano al triángulo desde el centro
            Vector3 point = Triangle.ClosestPointInTriangle(triangle, sphere.Position);

            float distance = Vector3.Distance(point, sphere.Position);

            if (distance > sphere.Radius)
            {
                return false;
            }
            else
            {
                return true;
            }
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
            float projectedBox = box.ProyectToVector(normal);

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
            //Transformar la caja a coordenadas locales para poder usar un AABB-Triangle
            Quaternion orientation = box.Orientation;
            Quaternion orientationInv;
            Quaternion.Conjugate(ref orientation, out orientationInv);

            //Llevar el triángulo a las coordenadas de la caja
            Matrix minv = Matrix.CreateFromQuaternion(orientationInv);
            Vector3 localPoint1 = Vector3.TransformNormal(tri.Point1 - box.Position, minv);
            Vector3 localPoint2 = Vector3.TransformNormal(tri.Point2 - box.Position, minv);
            Vector3 localPoint3 = Vector3.TransformNormal(tri.Point3 - box.Position, minv);
            Triangle localTri = new Triangle(localPoint1, localPoint2, localPoint3);

            BoundingBox localTriBounds = PhysicsMathHelper.GenerateFromTriangle(localTri);
            Vector3 localTriBoundhalfExtent = localTriBounds.GetHalfSizes();
            Vector3 localTriBoundCenter = localTriBounds.GetCenter();

            float localTriBoundCenterX = Math.Abs(localTriBoundCenter.X);
            float localTriBoundCenterY = Math.Abs(localTriBoundCenter.Y);
            float localTriBoundCenterZ = Math.Abs(localTriBoundCenter.Z);

            if (localTriBoundhalfExtent.X + box.HalfSize.X <= localTriBoundCenterX ||
                localTriBoundhalfExtent.Y + box.HalfSize.Y <= localTriBoundCenterY ||
                localTriBoundhalfExtent.Z + box.HalfSize.Z <= localTriBoundCenterZ)
            {
                //El cuerpo está fuera de la caja
                return false;
            }

            if (localTriBoundhalfExtent.X + localTriBoundCenterX <= box.HalfSize.X &&
                localTriBoundhalfExtent.Y + localTriBoundCenterY <= box.HalfSize.Y &&
                localTriBoundhalfExtent.Z + localTriBoundCenterZ <= box.HalfSize.Z)
            {
                //El cuerpo está dentro de la caja
                return true;
            }

            Vector3 point1 = localTri.Point1;
            Vector3 point2 = localTri.Point2;
            Vector3 point3 = localTri.Point3;

            //Obtener eje 1, entre el punto 1 y el 2 del triángulo
            Vector3 edge1;
            Vector3.Subtract(ref point2, ref point1, out edge1);

            //Obtener eje 2, entre el punto 1 y el 3 del triángulo
            Vector3 edge2;
            Vector3.Subtract(ref point3, ref point1, out edge2);

            //Obtener el eje perpendicular entre los dos ejes
            Vector3 crossEdge;
            Vector3.Cross(ref edge1, ref edge2, out crossEdge);

            //Obtener la distancia del triángulo al eje
            float triangleDist = Vector3.Dot(localTri.Point1, crossEdge);
            if (Math.Abs(crossEdge.X * box.HalfSize.X) +
                Math.Abs(crossEdge.Y * box.HalfSize.Y) +
                Math.Abs(crossEdge.Z * box.HalfSize.Z) <= Math.Abs(triangleDist))
            {
                return false;
            }

            // No hay resultados en los tests con el AABB del triángulo, hay que probar los 9 casos, 3 por eje
            // Al usar la transformación local de la caja, cada plano calculado es paralelo a cada eje de la caja
            // Como son paralelos, el producto es siempre 0 y se puede omitir

            //Obtener eje 3, entre el punto 2 y el 3 del triángulo
            Vector3 edge3;
            Vector3.Subtract(ref point2, ref point3, out edge3);

            if (OverlapOnAxis(box, localTri, edge1, 0)) { return false; }
            if (OverlapOnAxis(box, localTri, edge2, 0)) { return false; }
            if (OverlapOnAxis(box, localTri, edge3, 0)) { return false; }

            if (OverlapOnAxis(box, localTri, edge1, 1)) { return false; }
            if (OverlapOnAxis(box, localTri, edge2, 1)) { return false; }
            if (OverlapOnAxis(box, localTri, edge3, 1)) { return false; }

            if (OverlapOnAxis(box, localTri, edge1, 2)) { return false; }
            if (OverlapOnAxis(box, localTri, edge2, 2)) { return false; }
            if (OverlapOnAxis(box, localTri, edge3, 2)) { return false; }

#if DEBUG
            m_DEBUGTRI[m_DEBUGTRICOUNT++] = tri;
            m_DEBUGAABB[m_DEBUGAABBCOUNT++] = PhysicsMathHelper.GenerateFromTriangle(tri);
#endif

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

        /// <summary>
        /// Obtiene si hay superposición de las cajas sobre el eje especificado
        /// </summary>
        /// <param name="one">Caja primera</param>
        /// <param name="two">Caja segunda</param>
        /// <param name="axis">Eje</param>
        /// <param name="toCentre">Vector de separación entre centros</param>
        /// <returns>Devuelve verdadero si hay superposición</returns>
        private static bool OverlapOnAxis(CollisionBox one, CollisionBox two, Vector3 axis, Vector3 toCentre)
        {
            // Proyectar las extensiones sobre el eje
            float oneProject = one.ProyectToVector(axis);
            float twoProject = two.ProyectToVector(axis);

            // Projectar el vector entre los centros en el eje
            float distance = Math.Abs(Vector3.Dot(toCentre, axis));

            // Si la distancia entre centros es mayor que la suma de las proyecciones, no hay intersección
            return (distance < oneProject + twoProject);
        }
        /// <summary>
        /// Ejecuta el test de intersección entre el OBB y el Triángulo
        /// </summary>
        /// <param name="box">OBB</param>
        /// <param name="tri">Triángulo</param>
        /// <param name="edge">Lado</param>
        /// <param name="axis">Eje a testear (0, 1 ó 2)</param>
        /// <returns>Devuelve verdadero si la intersección no ha sido descartada</returns>
        private static bool OverlapOnAxis(CollisionBox box, Triangle tri, Vector3 edge, int axis)
        {
            if (axis == 0)
            {
                // a.X ^ b.X = (1,0,0) ^ edge
                // axis = Vector3(0, -edge.Z, edge.Y);
                float dPoint1 = tri.Point1.Z * edge.Y - tri.Point1.Y * edge.Z;
                float dPoint2 = tri.Point2.Z * edge.Y - tri.Point2.Y * edge.Z;
                float dPoint3 = tri.Point3.Z * edge.Y - tri.Point3.Y * edge.Z;
                float dhalf = Math.Abs(box.HalfSize.Y * edge.Z) + Math.Abs(box.HalfSize.Z * edge.Y);
                if (Math.Min(dPoint1, Math.Min(dPoint2, dPoint3)) >= dhalf ||
                    Math.Max(dPoint1, Math.Max(dPoint2, dPoint3)) <= -dhalf)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (axis == 1)
            {
                // a.Y ^ b.X = (0,1,0) ^ edge
                // axis = Vector3(edge.Z, 0, -edge.X);
                float dPoint1 = tri.Point1.X * edge.Z - tri.Point1.Z * edge.X;
                float dPoint2 = tri.Point2.X * edge.Z - tri.Point2.Z * edge.X;
                float dPoint3 = tri.Point3.X * edge.Z - tri.Point3.Z * edge.X;
                float dhalf = Math.Abs(box.HalfSize.X * edge.Z) + Math.Abs(box.HalfSize.Z * edge.X);
                if (Math.Min(dPoint1, Math.Min(dPoint2, dPoint3)) >= dhalf ||
                    Math.Max(dPoint1, Math.Max(dPoint2, dPoint3)) <= -dhalf)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (axis == 2)
            {
                // a.Y ^ b.X = (0,0,1) ^ edge
                // axis = Vector3(-edge.Y, edge.X, 0);
                float dPoint1 = tri.Point1.Y * edge.X - tri.Point1.X * edge.Y;
                float dPoint2 = tri.Point2.Y * edge.X - tri.Point2.X * edge.Y;
                float dPoint3 = tri.Point3.Y * edge.X - tri.Point3.X * edge.Y;
                float dhalf = Math.Abs(box.HalfSize.Y * edge.X) + Math.Abs(box.HalfSize.X * edge.Y);
                if (Math.Min(dPoint1, Math.Min(dPoint2, dPoint3)) >= dhalf ||
                    Math.Max(dPoint1, Math.Max(dPoint2, dPoint3)) <= -dhalf)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                throw new Exception();
            }
        }
    }
}