using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Physics;

namespace Physics
{
    /// <summary>
    /// Información de triángulos de un modelo
    /// </summary>
    public class TriangleInfo
    {
        // Diccionario con los grupos de triángulos (meshes) del modelo
        private Dictionary<string, TriangleList> m_Triangles = new Dictionary<string, TriangleList>();

        /// <summary>
        /// BoundingBox de todo el modelo
        /// </summary>
        public BoundingBox AABB;
        /// <summary>
        /// BoundingSphere de todo el modelo
        /// </summary>
        public BoundingSphere BSph;
        /// <summary>
        /// Bbox Orientado de todo el modelo
        /// </summary>
        public OrientedBoundingBox OBB;
        /// <summary>
        /// Obtiene los nombres de grupos de triángulos (meshes)
        /// </summary>
        public string[] Indexes
        {
            get
            {
                string[] indexes = new string[m_Triangles.Keys.Count];

                m_Triangles.Keys.CopyTo(indexes, 0);

                return indexes;
            }
        }
        /// <summary>
        /// Obtiene la lista de triángulos del grupo especificado
        /// </summary>
        /// <param name="index">Grupo de triángulos</param>
        /// <returns>Devuelve la lista de triángulos</returns>
        public TriangleList this[string index]
        {
            get
            {
                return m_Triangles[index];
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public TriangleInfo()
        {

        }

        /// <summary>
        /// Añade una lista de triángulos al grupo especificado
        /// </summary>
        /// <param name="index">Grupo de triángulos</param>
        /// <param name="triangles">Lista de triángulos</param>
        public void AddTriangles(string index, Triangle[] triangles)
        {
            if (this.m_Triangles.ContainsKey(index))
            {
                this.m_Triangles[index].AddRange(triangles);
            }
            else
            {
                this.m_Triangles.Add(index, new TriangleList(triangles));
            }

            this.Update();
        }
        /// <summary>
        /// Actualiza el bbox del modelo
        /// </summary>
        public void Update()
        {
            foreach (TriangleList triList in m_Triangles.Values)
            {
                this.AABB = BoundingBox.CreateMerged(this.AABB, triList.AABB); 
                // TODO: La esfera debería crearse con las otras esferas, no con el AABB
                this.BSph = BoundingSphere.CreateFromBoundingBox(this.AABB);
                this.OBB = OrientedBoundingBox.CreateFromBoundingBox(this.AABB);
            }
        }
    }
}
