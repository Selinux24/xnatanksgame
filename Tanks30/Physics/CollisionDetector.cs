using System;
using Microsoft.Xna.Framework;

namespace Physics
{
    using Common.Primitives;

    /// <summary>
    /// Detector de colisiones
    /// </summary>
    public abstract class CollisionDetector
    {
        /// <summary>
        /// Detecta la colisión entre dos objetos
        /// </summary>
        /// <param name="obj1">Objeto primero</param>
        /// <param name="obj2">Objeto segundo</param>
        /// <param name="data">Información de colisión</param>
        /// <returns>Devuelve verdadero si ha habido colisión</returns>
        public static bool BetweenObjects(ref CollisionPrimitive primitive1, ref CollisionPrimitive primitive2, ref CollisionData data)
        {
            if (primitive1 != null && primitive2 != null)
            {
                if (primitive1 is CollisionBox)
                {
                    if (primitive2 is CollisionBox)
                    {
                        return BoxAndBox((CollisionBox)primitive1, (CollisionBox)primitive2, ref data);
                    }
                    else if (primitive2 is CollisionSphere)
                    {
                        return BoxAndSphere((CollisionBox)primitive1, (CollisionSphere)primitive2, ref data);
                    }
                    else if (primitive2 is CollisionPlane)
                    {
                        return BoxAndHalfSpace((CollisionBox)primitive1, (CollisionPlane)primitive2, ref data);
                    }
                    else if (primitive2 is CollisionTriangleSoup)
                    {
                        return BoxAndTriangleSoup((CollisionBox)primitive1, (CollisionTriangleSoup)primitive2, ref data);
                    }
                }
                else if (primitive1 is CollisionSphere)
                {
                    if (primitive2 is CollisionBox)
                    {
                        return BoxAndSphere((CollisionBox)primitive2, (CollisionSphere)primitive1, ref data);
                    }
                    else if (primitive2 is CollisionSphere)
                    {
                        return SphereAndSphere((CollisionSphere)primitive1, (CollisionSphere)primitive2, ref data);
                    }
                    else if (primitive2 is CollisionPlane)
                    {
                        return SphereAndHalfSpace((CollisionSphere)primitive1, (CollisionPlane)primitive2, ref data);
                    }
                    else if (primitive2 is CollisionTriangleSoup)
                    {
                        return SphereAndTriangleSoup((CollisionSphere)primitive1, (CollisionTriangleSoup)primitive2, ref data);
                    }
                }

                throw new Exception("Tipo de colisión no controlada");
            }

            return false;
        }

        /// <summary>
        /// Detecta la colisión entre una esfera y un plano
        /// </summary>
        /// <param name="sphere">Esfera</param>
        /// <param name="plane">Plano</param>
        /// <param name="data">Rellena los datos de colisión</param>
        /// <returns>Devuelve verdadero si hay colisión o falso en el resto de los casos</returns>
        private static bool SphereAndHalfSpace(CollisionSphere sphere, CollisionPlane plane, ref CollisionData data)
        {
            if (data.ContactsLeft <= 0)
            {
                // Si no hay más contactos disponibles se sale de la función.
                return false;
            }

            // Distancia del centro al plano
            float centerToPlane = Math.Abs(Vector3.Dot(plane.Normal, sphere.Position) + plane.D);

            // Obtener la penetración de la esfera en el plano.
            float penetration = centerToPlane - sphere.Radius;
            if (penetration >= 0)
            {
                return false;
            }

            // Crear el contacto. Tiene una normal en la dirección del plano.
            Contact contact = data.CurrentContact;
            contact.ContactNormal = plane.Normal;
            contact.Penetration = -penetration;
            contact.ContactPoint = sphere.Position - plane.Normal * centerToPlane;

            // No hay cuerpo para el plano. Se considera escenario.
            RigidBody one = sphere;
            RigidBody two = null;
            contact.SetBodyData(ref one, ref two, data.Friction, data.Restitution);

            // Añadir el contacto
            data.AddContact();

            return true;
        }
        /// <summary>
        /// Detecta la colisión entre una esfera y un triángulo
        /// </summary>
        /// <param name="sphere">Esfera</param>
        /// <param name="tri">Triángulo</param>
        /// <param name="data">Rellena los datos de colisión</param>
        /// <returns>Devuelve verdadero si hay colisión o falso en el resto de los casos</returns>
        private static bool SphereAndTriangle(CollisionSphere sphere, Triangle tri, ref CollisionData data)
        {
            if (data.ContactsLeft <= 0)
            {
                // Si no hay más contactos disponibles se sale de la función.
                return false;
            }

            // Obtener el punto del triángulo más cercano al centro de la esfera
            Vector3 closestPoint = Triangle.ClosestPointInTriangle(tri, sphere.Position);

            // Obtener la distancia del punto obtenido al centro de la esfera
            float distance = Vector3.Distance(closestPoint, sphere.Position);

            if (distance <= sphere.Radius)
            {
                // Crear el contacto. Tiene una normal en la dirección del plano.
                Contact contact = data.CurrentContact;
                contact.ContactNormal = tri.Normal;
                contact.Penetration = sphere.Radius - distance;
                contact.ContactPoint = closestPoint;

                RigidBody one = sphere;
                RigidBody two = null;
                contact.SetBodyData(ref one, ref two, data.Friction, data.Restitution);

                data.AddContact();

                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Detecta la colisión entre dos esferas
        /// </summary>
        /// <param name="one">Esfera uno</param>
        /// <param name="two">Esfera dos</param>
        /// <param name="data">Datos de la colisión</param>
        /// <returns>Devuelve verdadero si hay colisión, o falso en el resto de los casos</returns>
        private static bool SphereAndSphere(CollisionSphere one, CollisionSphere two, ref CollisionData data)
        {
            if (data.ContactsLeft <= 0)
            {
                // Si no hay más contactos disponibles se sale de la función.
                return false;
            }

            // Almacenar las posiciones de las esferas
            Vector3 positionOne = one.Position;
            Vector3 positionTwo = two.Position;

            // Encontrar el vector entre los objetos
            Vector3 midline = positionOne - positionTwo;
            float size = midline.Length();

            if (size <= 0.0f || size >= one.Radius + two.Radius)
            {
                return false;
            }

            // Obtener la normal de forma manual
            Vector3 normal = midline * (1.0f / size);

            Contact contact = data.CurrentContact;
            contact.ContactNormal = normal;
            contact.ContactPoint = positionOne + midline * 0.5f;
            contact.Penetration = (one.Radius + two.Radius - size);
            RigidBody rbOne = one;
            RigidBody rbTwo = two;
            contact.SetBodyData(ref rbOne, ref rbTwo, data.Friction, data.Restitution);

            data.AddContact();

            return true;
        }
        /// <summary>
        /// Detecta la colisión entre una esfera y una lista de triángulos
        /// </summary>
        /// <param name="sphere">Esfera</param>
        /// <param name="triangleSoup">Lista de triángulos</param>
        /// <param name="data">Datos de la colisión</param>
        /// <returns>Devuelve verdadero si hay colisión, o falso en el resto de los casos</returns>
        private static bool SphereAndTriangleSoup(CollisionSphere sphere, CollisionTriangleSoup triangleSoup, ref CollisionData data)
        {
            return SphereAndTriangleList(sphere, triangleSoup.Triangles, ref data);
        }
        /// <summary>
        /// Detecta la colisión entre una esfera y una lista de triángulos
        /// </summary>
        /// <param name="sphere">Esfera</param>
        /// <param name="triangleList">Lista de triángulos</param>
        /// <param name="data">Datos de la colisión</param>
        /// <returns>Devuelve verdadero si hay colisión, o falso en el resto de los casos</returns>
        private static bool SphereAndTriangleList(CollisionSphere sphere, Triangle[] triangleList, ref CollisionData data)
        {
            bool contact = false;

            if (data.ContactsLeft > 0)
            {
                if (triangleList != null && triangleList.Length > 0)
                {
                    foreach (Triangle triangle in triangleList)
                    {
                        if (data.ContactsLeft > 0)
                        {
                            // Comprobar la intersección
                            if (CollisionDetector.SphereAndTriangle(sphere, triangle, ref data))
                            {
                                contact = true;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }

            return contact;
        }
        /// <summary>
        /// Detecta la colisión entre caja y plano
        /// </summary>
        /// <param name="box">Caja</param>
        /// <param name="plane">Plano</param>
        /// <param name="data">Datos de la colisión</param>
        /// <returns>Devuelve verdadero si hay colisión, o falso en el resto de los casos</returns>
        private static bool BoxAndHalfSpace(CollisionBox box, CollisionPlane plane, ref CollisionData data)
        {
            if (data.ContactsLeft <= 0)
            {
                // Si no hay más contactos disponibles se sale de la función.
                return false;
            }

            // Comprobar la intersección
            if (!IntersectionTests.BoxAndPlane(box, plane))
            {
                return false;
            }

            // Hay intersección, ahora hay que encontrar los puntos de intersección.
            // Podemos hacerlo únicamente chequeando los vértices.
            // Si la caja está descansando sobre el plano o un eje, se reportarán cuatro o dos puntos de contacto.

            uint contactsUsed = 0;
            for (int i = 0; i < 8; i++)
            {
                // Calcular la positición de cada vértice
                Vector3 vertexPos = box.GetCorner(i);

                // Calcular la distancia al plano
                float vertexDistance = plane.D + Vector3.Dot(vertexPos, plane.Normal);

                // Comparar las distancias
                if (vertexDistance <= 0f)
                {
                    // Crear la información del contacto.

                    // El punto de contacto está a medio camino entre el vértice y el plano.
                    // Se obtiene multiplicando la dirección por la mitad de la distancia de separación, y añadiendo la posición del vértice.
                    Contact contact = data.CurrentContact;
                    contact.ContactPoint = vertexPos;
                    contact.ContactNormal = plane.Normal;
                    contact.Penetration = -vertexDistance;

                    // Establecer los datos del contacto
                    RigidBody one = box;
                    RigidBody two = null;
                    contact.SetBodyData(ref one, ref two, data.Friction, data.Restitution);

                    // Añadir contacto
                    data.AddContact();

                    contactsUsed++;
                    if (contactsUsed == data.ContactsLeft)
                    {
                        return true;
                    }
                }
            }

            return true;
        }
        /// <summary>
        /// Detecta la colisión entre caja y triángulo
        /// </summary>
        /// <param name="box">Caja</param>
        /// <param name="tri">Triángulo</param>
        /// <param name="data">Datos de la colisión</param>
        /// <returns>Devuelve verdadero si hay colisión, o falso en el resto de los casos</returns>
        private static bool BoxAndTriangle(CollisionBox box, Triangle tri, ref CollisionData data)
        {
            bool intersectionExists = false;

            if (data.ContactsLeft <= 0)
            {
                // Si no hay más contactos disponibles se sale de la función.
                return false;
            }

            // Hay intersección, ahora hay que encontrar los puntos de intersección.
            // Podemos hacerlo únicamente chequeando los vértices.
            // Si la caja está descansando sobre el plano o un eje, se reportarán cuatro o dos puntos de contacto.
            for (int i = 0; i < 8; i++)
            {
                // Calcular la positición de cada vértice
                Vector3 vertexPos = box.GetCorner(i);

                // Calcular la distancia al plano
                float distanceToPlane = tri.Plane.Distance(vertexPos);
                if (distanceToPlane <= 0f)
                {
                    // Si la distancia es negativa está tras el plano. Si es 0, está en el plano

                    // Intersección entre línea y triángulo
                    Vector3 direction = Vector3.Normalize(box.Position - vertexPos);
                    Ray r = new Ray(vertexPos, direction);
                    if (IntersectionTests.TriAndRay(tri, r))
                    {
                        intersectionExists = true;

                        // Crear la información del contacto.

                        // El punto de contacto está a medio camino entre el vértice y el plano.
                        // Se obtiene multiplicando la dirección por la mitad de la distancia de separación, y añadiendo la posición del vértice.
                        Contact contact = data.CurrentContact;
                        contact.ContactPoint = vertexPos;
                        contact.ContactNormal = tri.Plane.Normal;
                        contact.Penetration = -distanceToPlane;

                        // Establecer los datos del contacto
                        RigidBody one = box;
                        RigidBody two = null;
                        contact.SetBodyData(ref one, ref two, data.Friction, data.Restitution);

                        // Añadir contacto
                        data.AddContact();

                        if (data.ContactsLeft <= 0)
                        {
                            return true;
                        }
                    }
                }
            }

            return intersectionExists;
        }
        /// <summary>
        /// Detecta la colisión entre cajas
        /// </summary>
        /// <param name="one">Caja uno</param>
        /// <param name="two">Caja dos</param>
        /// <param name="data">Datos de colisión</param>
        /// <returns>Devuelve verdadero si hay colisión, o falso en el resto de los casos</returns>
        private static bool BoxAndBox(CollisionBox one, CollisionBox two, ref CollisionData data)
        {
            if (data.ContactsLeft <= 0)
            {
                // Si no hay más contactos disponibles se sale de la función.
                return false;
            }

            // Encontrar el vector entre los dos centros
            Vector3 toCentre = two.Position - one.Position;

            // Se asume que no hay contacto
            float pen = float.MaxValue;
            uint best = uint.MaxValue;

            // Chequear cada eje, almacenando penetración y el mejor eje
            if (!TryAxis(one, two, one.XAxis, toCentre, 0, ref pen, ref best)) return false;
            if (!TryAxis(one, two, one.YAxis, toCentre, 1, ref pen, ref best)) return false;
            if (!TryAxis(one, two, one.ZAxis, toCentre, 2, ref pen, ref best)) return false;

            if (!TryAxis(one, two, two.XAxis, toCentre, 3, ref pen, ref best)) return false;
            if (!TryAxis(one, two, two.YAxis, toCentre, 4, ref pen, ref best)) return false;
            if (!TryAxis(one, two, two.ZAxis, toCentre, 5, ref pen, ref best)) return false;

            // Almacenar el mejor eje hasta ahora, en el caso de estar en una colisión de ejes paralelos más adelante.
            uint bestSingleAxis = best;

            if (!TryAxis(one, two, Vector3.Cross(one.XAxis, two.XAxis), toCentre, 6, ref pen, ref best)) return false;
            if (!TryAxis(one, two, Vector3.Cross(one.XAxis, two.YAxis), toCentre, 7, ref pen, ref best)) return false;
            if (!TryAxis(one, two, Vector3.Cross(one.XAxis, two.ZAxis), toCentre, 8, ref pen, ref best)) return false;
            if (!TryAxis(one, two, Vector3.Cross(one.YAxis, two.XAxis), toCentre, 9, ref pen, ref best)) return false;
            if (!TryAxis(one, two, Vector3.Cross(one.YAxis, two.YAxis), toCentre, 10, ref pen, ref best)) return false;
            if (!TryAxis(one, two, Vector3.Cross(one.YAxis, two.ZAxis), toCentre, 11, ref pen, ref best)) return false;
            if (!TryAxis(one, two, Vector3.Cross(one.ZAxis, two.XAxis), toCentre, 12, ref pen, ref best)) return false;
            if (!TryAxis(one, two, Vector3.Cross(one.ZAxis, two.YAxis), toCentre, 13, ref pen, ref best)) return false;
            if (!TryAxis(one, two, Vector3.Cross(one.ZAxis, two.ZAxis), toCentre, 14, ref pen, ref best)) return false;

            // Asegurarse de que tenemos un resultado.
            if (best != uint.MaxValue)
            {
                // Tenemos colisión, y tenemos el eje de colisión con menor penetración
                if (best < 3)
                {
                    // Hay un vértice la caja dos en una cara de la caja uno.
                    FillPointFaceBoxBox(one, two, toCentre, best, pen, ref data);

                    data.AddContact();

                    return true;
                }
                else if (best < 6)
                {
                    // Hay un vértice de la caja uno en una cara de la caja dos.
                    FillPointFaceBoxBox(two, one, toCentre * -1.0f, best - 3, pen, ref data);

                    data.AddContact();

                    return true;
                }
                else
                {
                    // Contacto canto a canto. Obtener el eje común.
                    best -= 6;
                    uint oneAxisIndex = best / 3;
                    uint twoAxisIndex = best % 3;
                    Vector3 oneAxis = one.GetAxis((TransformAxis)oneAxisIndex);
                    Vector3 twoAxis = two.GetAxis((TransformAxis)twoAxisIndex);
                    Vector3 axis = Vector3.Cross(oneAxis, twoAxis);
                    axis.Normalize();

                    // El eje debería apuntar desde la caja uno a la dos.
                    if (Vector3.Dot(axis, toCentre) > 0f)
                    {
                        axis = axis * -1.0f;
                    }

                    // Tenemos los ejes, pero no los cantos.

                    // Cada eje tiene 4 cantos paralelos a él, tenemos que encontrar los 4 de cada caja.
                    // Buscaremos el punto en el centro del canto. Sabemos que su componente en el eje de colisión es 0 y
                    // determinamos cual de los extremos en cada uno de los otros ejes es el más cercano.
                    Vector3 vOne = one.HalfSize;
                    Vector3 vTwo = two.HalfSize;
                    float[] ptOnOneEdge = new float[] { vOne.X, vOne.Y, vOne.Z };
                    float[] ptOnTwoEdge = new float[] { vTwo.X, vTwo.Y, vTwo.Z };
                    for (uint i = 0; i < 3; i++)
                    {
                        if (i == oneAxisIndex)
                        {
                            ptOnOneEdge[i] = 0;
                        }
                        else if (Vector3.Dot(one.GetAxis((TransformAxis)i), axis) > 0f)
                        {
                            ptOnOneEdge[i] = -ptOnOneEdge[i];
                        }

                        if (i == twoAxisIndex)
                        {
                            ptOnTwoEdge[i] = 0;
                        }
                        else if (Vector3.Dot(two.GetAxis((TransformAxis)i), axis) < 0f)
                        {
                            ptOnTwoEdge[i] = -ptOnTwoEdge[i];
                        }
                    }

                    vOne.X = ptOnOneEdge[0];
                    vOne.Y = ptOnOneEdge[1];
                    vOne.Z = ptOnOneEdge[2];

                    vTwo.X = ptOnTwoEdge[0];
                    vTwo.Y = ptOnTwoEdge[1];
                    vTwo.Z = ptOnTwoEdge[2];

                    // Pasar a coordenadas del mundo
                    vOne = Vector3.Transform(vOne, one.Transform);
                    vTwo = Vector3.Transform(vTwo, two.Transform);

                    // Tenemos un punto y una dirección para los cantos que colisionan.
                    // Necesitamos encontrar el punto de mayor cercanía de los dos segmentos.
                    float[] vOneAxis = new float[] { one.HalfSize.X, one.HalfSize.Y, one.HalfSize.Z };
                    float[] vTwoAxis = new float[] { two.HalfSize.X, two.HalfSize.Y, two.HalfSize.Z };
                    Vector3 vertex = ContactPoint(
                        vOne, oneAxis, vOneAxis[oneAxisIndex],
                        vTwo, twoAxis, vTwoAxis[twoAxisIndex],
                        bestSingleAxis > 2);

                    // Llenar el contacto.
                    Contact contact = data.CurrentContact;
                    contact.Penetration = pen;
                    contact.ContactNormal = axis;
                    contact.ContactPoint = vertex;
                    RigidBody rbOne = one;
                    RigidBody rbTwo = two;
                    contact.SetBodyData(ref rbOne, ref rbTwo, data.Friction, data.Restitution);

                    data.AddContact();

                    return true;
                }
            }

            return false;
        }
        /// <summary>
        /// Detecta la colisión entre una caja y un punto
        /// </summary>
        /// <param name="box">Caja</param>
        /// <param name="point">Punto</param>
        /// <param name="data">Datos de colisión</param>
        /// <returns>Devuelve verdadero si hay colisión, o falso en el resto de los casos</returns>
        private static bool BoxAndPoint(CollisionBox box, Vector3 point, ref CollisionData data)
        {
            if (data.ContactsLeft <= 0)
            {
                // Si no hay más contactos disponibles se sale de la función.
                return false;
            }

            // Transformar el punto
            Vector3 relPt = Vector3.Transform(point, Matrix.Invert(box.Transform));

            // Chequear cada eje buscando el eje en el que la penetración es menos profunda.
            float min_depth = box.HalfSize.X - Math.Abs(relPt.X);
            if (min_depth < 0)
            {
                return false;
            }
            Vector3 normal = box.XAxis * ((relPt.X < 0) ? -1 : 1);

            float depth = box.HalfSize.Y - Math.Abs(relPt.Y);
            if (depth < 0)
            {
                return false;
            }
            else if (depth < min_depth)
            {
                min_depth = depth;
                normal = box.YAxis * ((relPt.Y < 0) ? -1 : 1);
            }

            depth = box.HalfSize.Z - Math.Abs(relPt.Z);
            if (depth < 0)
            {
                return false;
            }
            else if (depth < min_depth)
            {
                min_depth = depth;
                normal = box.ZAxis * ((relPt.Z < 0) ? -1 : 1);
            }

            Contact contact = data.CurrentContact;
            contact.ContactNormal = normal;
            contact.ContactPoint = point;
            contact.Penetration = min_depth;

            RigidBody one = box;
            RigidBody two = null;
            contact.SetBodyData(ref one, ref two, data.Friction, data.Restitution);

            data.AddContact();

            return true;
        }
        /// <summary>
        /// Detecta la colisión entre una caja y una esfera
        /// </summary>
        /// <param name="box">Caja</param>
        /// <param name="sphere">Esfera</param>
        /// <param name="data">Datos de colisión a llenar</param>
        /// <returns>Devuelve verdadero si existe colisión, falso en el resto de los casos</returns>
        private static bool BoxAndSphere(CollisionBox box, CollisionSphere sphere, ref CollisionData data)
        {
            if (data.ContactsLeft <= 0)
            {
                // Si no hay más contactos disponibles se sale de la función.
                return false;
            }

            // Obtener el punto de la caja más cercano al centro de la esfera
            Vector3 closestPoint = CollisionBox.ClosestPointInBox(box, sphere.Position);

            // Obtener la distancia entre los puntos
            float distance = Vector3.Distance(sphere.Position, closestPoint);
            if (distance <= sphere.Radius)
            {
                Vector3 normal = Vector3.Normalize(box.Position - closestPoint);

                Contact contact = data.CurrentContact;
                contact.ContactNormal = normal;
                contact.ContactPoint = closestPoint;
                contact.Penetration = sphere.Radius - distance;

                RigidBody one = box;
                RigidBody two = sphere;
                contact.SetBodyData(ref one, ref two, data.Friction, data.Restitution);

                data.AddContact();

                return true;
            }

            return false;
        }
        /// <summary>
        /// Detecta la colisión entre una caja y una colección de triángulos
        /// </summary>
        /// <param name="box">Caja</param>
        /// <param name="triangleSoup">Colección de triángulos</param>
        /// <param name="data">Datos de colisión a llenar</param>
        /// <returns>Devuelve verdadero si existe colisión, falso en el resto de los casos</returns>
        private static bool BoxAndTriangleSoup(CollisionBox box, CollisionTriangleSoup triangleSoup, ref CollisionData data)
        {
            //CollisionPlane plane = new CollisionPlane(triangleSoup.Triangles[0].Plane, triangleSoup.Mass);

            //return BoxAndHalfSpace(box, plane, ref data);

            return BoxAndTriangleList(box, triangleSoup.Triangles, ref data);
        }
        /// <summary>
        /// Detecta la colisión entre una caja y una lista de triángulos
        /// </summary>
        /// <param name="box">Caja</param>
        /// <param name="triangleList">Lista de triángulos</param>
        /// <param name="data">Datos de colisión a llenar</param>
        /// <returns>Devuelve verdadero si existe colisión, falso en el resto de los casos</returns>
        private static bool BoxAndTriangleList(CollisionBox box, Triangle[] triangleList, ref CollisionData data)
        {
            if (data.ContactsLeft <= 0)
            {
                // Si no hay más contactos disponibles se sale de la función.
                return false;
            }

            bool intersection = false;

            if (data.ContactsLeft > 0)
            {
                int firstContact = data.ContactCount;

                foreach (Triangle triangle in triangleList)
                {
                    // Comprobar la intersección con el triángulo
                    if (IntersectionTests.BoxAndTri(box, triangle))
                    {
                        if (data.ContactsLeft > 0)
                        {
                            if (CollisionDetector.BoxAndTriangle(box, triangle, ref data))
                            {
                                intersection = true;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                //int contactCount = data.ContactCount - firstContact;

                //if (intersection && contactCount > 1)
                //{
                //    //Agrupar los contactos
                //    Vector3 contactPoint = Vector3.Zero;
                //    Vector3 contactNormal = Vector3.Zero;
                //    float penetration = 0f;

                //    for (int i = firstContact; i < data.ContactCount; i++)
                //    {
                //        contactPoint += data.ContactArray[i].ContactPoint;
                //        contactNormal += data.ContactArray[i].ContactNormal;
                //        penetration += data.ContactArray[i].Penetration;
                //    }

                //    contactPoint /= contactCount;
                //    contactNormal /= contactCount;
                //    penetration /= contactCount;

                //    Contact newContact = new Contact();
                //    data.ContactArray[firstContact].ContactPoint = contactPoint;
                //    data.ContactArray[firstContact].ContactNormal = Vector3.Normalize(contactNormal);
                //    data.ContactArray[firstContact].Penetration = penetration;

                //    data.SetContactIndex(firstContact + 1);
                //}
            }

            return intersection;
        }

        /// <summary>
        /// Obtiene si hay penetración entre las proyecciones de las cajas en el eje especificado
        /// </summary>
        /// <param name="one">Caja uno</param>
        /// <param name="two">Caja dos</param>
        /// <param name="axis">Eje</param>
        /// <param name="toCentre">Distancia entre centros</param>
        /// <param name="index">Índice</param>
        /// <param name="smallestPenetration">Penetración menor</param>
        /// <param name="smallestCase">Caso menor</param>
        /// <returns>Devuelve verdadero si hay penetración, falso en el resto de los casos</returns>
        private static bool TryAxis(CollisionBox one, CollisionBox two, Vector3 axis, Vector3 toCentre, uint index, ref float smallestPenetration, ref uint smallestCase)
        {
            // No chequear ejes paralelos
            if (axis.LengthSquared() < 0.0001)
            {
                return true;
            }

            // Asegurar que se trata de un eje normalizado
            axis.Normalize();

            // Obtener la penetración
            float penetration = PenetrationOnAxis(one, two, axis, toCentre);

            if (penetration < 0)
            {
                return false;
            }

            if (penetration < smallestPenetration)
            {
                smallestPenetration = penetration;
                smallestCase = index;
            }

            return true;
        }
        /// <summary>
        /// Obtiene la penetración de las proyecciones de las cajas en el eje especificado
        /// </summary>
        /// <param name="one">Caja uno</param>
        /// <param name="two">Caja dos</param>
        /// <param name="axis">Eje</param>
        /// <param name="toCentre">Distancia entre centros</param>
        /// <returns>Devuelve la penetración de las proyecciones de las cajas sobre el eje.</returns>
        private static float PenetrationOnAxis(CollisionBox one, CollisionBox two, Vector3 axis, Vector3 toCentre)
        {
            // Proyectar las extensiones de cada caja sobre el eje
            float oneProject = one.ProyectToVector(axis);
            float twoProject = two.ProyectToVector(axis);

            // Obtener la distancia entre centros de las cajas sobre el eje
            float distance = Convert.ToSingle(Math.Abs(Vector3.Dot(toCentre, axis)));

            // Positivo indica solapamiento, negativo separación
            return (oneProject + twoProject - distance);
        }
        /// <summary>
        /// Llena la información de colisión entre dos cajas, una vez se conoce que hay contacto del tipo vértice - cara
        /// </summary>
        /// <param name="one">Caja uno</param>
        /// <param name="two">Caja dos</param>
        /// <param name="toCentre">Distancia entre centros</param>
        /// <param name="data">Información de colisión</param>
        /// <param name="best">Eje de penetración menor</param>
        /// <param name="pen">Pentración menor</param>
        private static void FillPointFaceBoxBox(CollisionBox one, CollisionBox two, Vector3 toCentre, uint best, float pen, ref CollisionData data)
        {
            // Sabemos cual es el eje de la colisión, pero tenemos que conocer con qué cara tenemos que trabjar
            Vector3 normal = one.GetAxis((TransformAxis)best);
            if (Vector3.Dot(one.GetAxis((TransformAxis)best), toCentre) > 0f)
            {
                normal = normal * -1.0f;
            }

            // Obtenemos el vértice
            Vector3 vertex = two.HalfSize;
            if (Vector3.Dot(two.XAxis, normal) < 0f) vertex.X = -vertex.X;
            if (Vector3.Dot(two.YAxis, normal) < 0f) vertex.Y = -vertex.Y;
            if (Vector3.Dot(two.ZAxis, normal) < 0f) vertex.Z = -vertex.Z;

            Contact contact = data.CurrentContact;
            contact.ContactNormal = normal;
            contact.Penetration = pen;
            contact.ContactPoint = Vector3.Transform(vertex, two.Transform);
            RigidBody rbOne = one;
            RigidBody rbTwo = two;
            contact.SetBodyData(ref rbOne, ref rbTwo, data.Friction, data.Restitution);
        }
        /// <summary>
        /// Obtiene el punto de mayor cercanía a los segmentos implicados en una colisión canto a canto o cara a canto, o cara a cara
        /// </summary>
        /// <param name="pOne"></param>
        /// <param name="dOne"></param>
        /// <param name="oneSize"></param>
        /// <param name="pTwo"></param>
        /// <param name="dTwo"></param>
        /// <param name="twoSize"></param>
        /// <param name="useOne">Si es verdadero, y el punto de contacto está fuera de canto (colisión cara a canto), se usará sólo la caja uno, si no, la caja dos.</param>
        /// <returns>Devuelve el punto de mayor cercanía a los dos segmentos implicados en una colisión canto a canto</returns>
        private static Vector3 ContactPoint(Vector3 pOne, Vector3 dOne, float oneSize, Vector3 pTwo, Vector3 dTwo, float twoSize, bool useOne)
        {
            Vector3 toSt, cOne, cTwo;
            float dpStaOne, dpStaTwo, dpOneTwo, smOne, smTwo;
            float denom, mua, mub;

            smOne = dOne.LengthSquared();
            smTwo = dTwo.LengthSquared();
            dpOneTwo = Vector3.Dot(dTwo, dOne);

            toSt = pOne - pTwo;
            dpStaOne = Vector3.Dot(dOne, toSt);
            dpStaTwo = Vector3.Dot(dTwo, toSt);

            denom = smOne * smTwo - dpOneTwo * dpOneTwo;

            // Denomiador cero indica líneas paralelas
            if (Math.Abs(denom) < 0.0001f)
            {
                return useOne ? pOne : pTwo;
            }

            mua = (dpOneTwo * dpStaTwo - smTwo * dpStaOne) / denom;
            mub = (smOne * dpStaTwo - dpOneTwo * dpStaOne) / denom;

            // Si alguno de los cantos tiene el punto más cercano fuera de los límites, los cantos no están cerrados, y tenemos una colisión canto a cara.
            // El punto es en el canto, lo que sabemos por el parámetro useOne.
            if (mua > oneSize ||
                mua < -oneSize ||
                mub > twoSize ||
                mub < -twoSize)
            {
                return useOne ? pOne : pTwo;
            }
            else
            {
                cOne = pOne + dOne * mua;
                cTwo = pTwo + dTwo * mub;

                return cOne * 0.5f + cTwo * 0.5f;
            }
        }

        /// <summary>
        /// Obtiene la penetración de las proyecciones de las cajas en el eje especificado
        /// </summary>
        /// <param name="one">Caja uno</param>
        /// <param name="two">Caja dos</param>
        /// <param name="axis">Eje</param>
        /// <param name="toCentre">Distancia entre centros</param>
        /// <returns>Devuelve la penetración de las proyecciones de las cajas sobre el eje.</returns>
        private static float PenetrationOnAxis(CollisionBox box, Triangle tri, Vector3 edge, Vector3 toCentre)
        {
            // Proyectar las extensiones de cada caja sobre el eje
            float oneProject = box.ProyectToVector(edge);
            float twoProject = tri.ProyectToVector(edge);

            // Obtener la distancia entre centros de las cajas sobre el eje
            float distance = Convert.ToSingle(Math.Abs(Vector3.Dot(toCentre, edge)));

            // Positivo indica solapamiento, negativo separación
            return (oneProject + twoProject - distance);
        }
    }
}