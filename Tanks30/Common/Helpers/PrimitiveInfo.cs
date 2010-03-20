using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Common.Helpers
{
    using Common.Primitives;

    /// <summary>
    /// Información de triángulos de un modelo
    /// </summary>
    public class PrimitiveInfo
    {
        /// <summary>
        /// Diccionario con los grupos de triángulos (meshes) del modelo
        /// </summary>
        private Dictionary<string, Triangle[]> m_TriangleDictionary = new Dictionary<string, Triangle[]>();

        /// <summary>
        /// BoundingBox de todo el modelo
        /// </summary>
        public BoundingBox AABB { get; protected set; }
        /// <summary>
        /// BoundingSphere de todo el modelo
        /// </summary>
        public BoundingSphere SPH { get; protected set; }
        /// <summary>
        /// Obtiene los nombres de grupos de triángulos (meshes)
        /// </summary>
        public string[] Indexes
        {
            get
            {
                string[] indexes = new string[m_TriangleDictionary.Keys.Count];

                m_TriangleDictionary.Keys.CopyTo(indexes, 0);

                return indexes;
            }
        }
        /// <summary>
        /// Obtiene la lista de triángulos del grupo especificado
        /// </summary>
        /// <param name="index">Grupo de triángulos</param>
        /// <returns>Devuelve la lista de triángulos</returns>
        public Triangle[] this[string index]
        {
            get
            {
                return m_TriangleDictionary[index];
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public PrimitiveInfo()
        {

        }

        /// <summary>
        /// Añade una lista de triángulos al grupo especificado
        /// </summary>
        /// <param name="index">Grupo de triángulos</param>
        /// <param name="triangles">Lista de triángulos</param>
        public void AddTriangles(string index, Triangle[] triangles)
        {
            if (this.m_TriangleDictionary.ContainsKey(index))
            {
                List<Triangle> tmpList = new List<Triangle>();

                tmpList.AddRange(this.m_TriangleDictionary[index]);
                tmpList.AddRange(triangles);

                this.m_TriangleDictionary[index] = tmpList.ToArray();

            }
            else
            {
                this.m_TriangleDictionary.Add(index, triangles);
            }

            this.Update();
        }
        /// <summary>
        /// Actualiza el bbox del modelo
        /// </summary>
        public void Update()
        {
            // Obtener la lista de vértices
            List<Vector3> vertices = new List<Vector3>();

            foreach (Triangle[] triList in m_TriangleDictionary.Values)
            {
                foreach (Triangle tri in triList)
                {
                    vertices.Add(tri.Point1);
                    vertices.Add(tri.Point2);
                    vertices.Add(tri.Point3);
                }
            }

            // Crear los objetos circundantes
            this.AABB = BoundingBox.CreateFromPoints(vertices);
            this.SPH = BoundingSphere.CreateFromPoints(vertices);
        }
    }
}
