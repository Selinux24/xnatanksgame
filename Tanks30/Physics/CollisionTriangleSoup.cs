using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Physics
{
    /// <summary>
    /// Colecci�n de tri�ngulos
    /// </summary>
    public class CollisionTriangleSoup : CollisionPrimitive
    {
        /// <summary>
        /// Lista de tri�ngulos
        /// </summary>
        private List<Triangle> m_TriangleList;

        /// <summary>
        /// Obtiene la lista de tri�ngulos
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
        /// <param name="triangles">Lista de tri�ngulos de la colecci�n</param>
        public CollisionTriangleSoup(Triangle[] triangles)
        {
            // Establecer la lista de tri�ngulos
            this.m_TriangleList = new List<Triangle>(triangles);

            this.Update();
        }
        /// <summary>
        /// A�ade la lista de tri�ngulos especificada
        /// </summary>
        /// <param name="triangles">Lista de tri�ngulos</param>
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
            // Obtener todos los v�rtices de los tri�ngulos
            List<Vector3> vertexList = new List<Vector3>();

            foreach (Triangle tri in this.m_TriangleList)
            {
                vertexList.Add(tri.Point1);
                vertexList.Add(tri.Point2);
                vertexList.Add(tri.Point3);
            }

            // Crear el AABB usando los v�rtices
            this.AABB = BoundingBox.CreateFromPoints(vertexList.ToArray());

            // Crear la esfera usando los v�rtices
            this.BSph = BoundingSphere.CreateFromPoints(vertexList.ToArray());
        }
    }
}
