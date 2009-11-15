using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Physics;

namespace Physics
{
    /// <summary>
    /// Lista de primitivas
    /// </summary>
    public class TriangleList : List<Triangle>
    {
        // Caja
        private BoundingBox m_AABB = new BoundingBox();
        // Esfera
        private BoundingSphere m_BSph = new BoundingSphere();
        // Caja Orientada
        private OrientedBoundingBox m_OBB = new OrientedBoundingBox();

        /// <summary>
        /// Obtiene el AABB circundante
        /// </summary>
        public BoundingBox AABB
        {
            get
            {
                return m_AABB;
            }
            set
            {
                m_AABB = value;
            }
        }
        /// <summary>
        /// Obtiene la esfera circundante
        /// </summary>
        public BoundingSphere BSph
        {
            get
            {
                return m_BSph;
            }
            set
            {
                m_BSph = value;
            }
        }
        /// <summary>
        /// Obtiene el OBB cicundante
        /// </summary>
        public OrientedBoundingBox OBB
        {
            get
            {
                return m_OBB;
            }
            set
            {
                m_OBB = value;
            }
        }
        /// <summary>
        /// Obtiene la lista de vértices de las primitivas
        /// </summary>
        public Vector3[] Vertexes
        {
            get
            {
                List<Vector3> vertexList = new List<Vector3>();

                foreach (Triangle tri in this)
                {
                    vertexList.Add(tri.Point1);
                    vertexList.Add(tri.Point2);
                    vertexList.Add(tri.Point3);
                }

                return vertexList.ToArray();
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public TriangleList()
            : base()
        {

        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="triangles">Lista de triángulos contenidos inicialmente</param>
        public TriangleList(Triangle[] triangles)
            : base(triangles)
        {
            this.Update();
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="capacity">Capacidad de la lista</param>
        public TriangleList(int capacity)
            : base(capacity)
        {

        }

        /// <summary>
        /// Actualizar el bbox
        /// </summary>
        public void Update()
        {
            // Crear el AABB usando los vértices
            m_AABB = BoundingBox.CreateFromPoints(this.Vertexes);

            // Crear la esfera usando los vértices
            m_BSph = BoundingSphere.CreateFromPoints(this.Vertexes);

            // Crear el OBB usando el AABB, a que inicialmente son iguales
            m_OBB = OrientedBoundingBox.CreateFromBoundingBox(m_AABB);
        }

        /// <summary>
        /// Obtiene la primera intersección entre el rayo y la lista de primitivas
        /// </summary>
        /// <param name="ray">Rayo</param>
        /// <param name="triangle">Devuelve el triángulo de intersección si existe</param>
        /// <param name="intersectionPoint">Devuelve el punto de intersección con el triángulo</param>
        /// <param name="distanceToPoint">Devuelve la distancia desde el origen del ray al punto de intersección</param>
        /// <returns>Devuelve verdadero si hay intersección o falso en el resto de los casos</returns>
        public bool Intersects(Ray ray, out Triangle? triangle, out Vector3? intersectionPoint, out float? distanceToPoint)
        {
            return Intersects(ray, out triangle, out intersectionPoint, out distanceToPoint, false);
        }
        /// <summary>
        /// Obtiene si hay intersección entre el rayo y la lista de primitivas
        /// </summary>
        /// <param name="ray">Rayo</param>
        /// <param name="triangle">Devuelve el triángulo de intersección si existe</param>
        /// <param name="intersectionPoint">Devuelve el punto de intersección con el triángulo</param>
        /// <param name="distanceToPoint">Devuelve la distancia desde el origen del ray al punto de intersección</param>
        /// <param name="findNearest">Indica si se debe testear la intersección hasta encontrar el más cercano</param>
        /// <returns>Devuelve verdadero si hay intersección o falso en el resto de los casos</returns>
        public bool Intersects(Ray ray, out Triangle? triangle, out Vector3? intersectionPoint, out float? distanceToPoint, bool findNearest)
        {
            triangle = null;
            intersectionPoint = null;
            distanceToPoint = null;

            // Primero ver si hay contacto con el bbox
            if (ray.Intersects(m_AABB).HasValue)
            {
                // Indica si ha habido intersección
                bool intersectionOccur = false;

                // Variables para almacenar la interscción más cercana
                Triangle? closestTriangle = null;
                Vector3? closestIntersectionPoint = null;
                float? closestDistanceToPoint = null;

                // Recorremos todos los triángulos de la lista realizando el test de intersección
                foreach (Triangle currentTriangle in this)
                {
                    // Variables para almacener la intersección actual
                    Vector3? currentIntersectionPoint = null;
                    float? currentDistanceToPoint = null;

                    if (currentTriangle.Intersects(ray, out currentIntersectionPoint, out currentDistanceToPoint))
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
            }

            return false;
        }
    }
}
