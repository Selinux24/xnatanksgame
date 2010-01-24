using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Physics
{
    /// <summary>
    /// Colección de triángulos
    /// </summary>
    public class CollisionTriangleSoup : CollisionPrimitive
    {
        /// <summary>
        /// Lista de triángulos
        /// </summary>
        private List<Triangle> m_TriangleList;

        /// <summary>
        /// Obtiene la lista de triángulos
        /// </summary>
        public Triangle[] Triangles
        {
            get
            {
                return m_TriangleList.ToArray();
            }
        }
        /// <summary>
        /// Obtiene el AABB circundante
        /// </summary>
        public BoundingBox AABB { get; protected set; }
        /// <summary>
        /// Obtiene la esfera circundante
        /// </summary>
        public BoundingSphere BSph { get; protected set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="triangles">Lista de triángulos de la colección</param>
        public CollisionTriangleSoup(Triangle[] triangles)
        {
            // Establecer la lista de triángulos
            this.m_TriangleList = new List<Triangle>(triangles);

            this.Update();
        }
        /// <summary>
        /// Añade la lista de triángulos especificada
        /// </summary>
        /// <param name="triangles">Lista de triángulos</param>
        public void AddTriangles(Triangle[] triangles)
        {
            if (triangles != null && triangles.Length > 0)
            {
                this.m_TriangleList.AddRange(triangles);

                this.Update();
            }
        }
        /// <summary>
        /// Actualiza los cuerpos contenedores de las primitivas
        /// </summary>
        private void Update()
        {
            // Obtener todos los vértices de los triángulos
            List<Vector3> vertexList = new List<Vector3>();

            foreach (Triangle tri in this.m_TriangleList)
            {
                vertexList.Add(tri.Point1);
                vertexList.Add(tri.Point2);
                vertexList.Add(tri.Point3);
            }

            // Crear el AABB usando los vértices
            this.AABB = BoundingBox.CreateFromPoints(vertexList.ToArray());

            // Crear la esfera usando los vértices
            this.BSph = BoundingSphere.CreateFromPoints(vertexList.ToArray());
        }
    }
}
