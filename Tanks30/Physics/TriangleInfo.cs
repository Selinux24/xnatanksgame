using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Physics;

namespace Physics
{
    /// <summary>
    /// Informaci�n de tri�ngulos de un modelo
    /// </summary>
    public class TriangleInfo
    {
        // Diccionario con los grupos de tri�ngulos (meshes) del modelo
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
        /// Obtiene los nombres de grupos de tri�ngulos (meshes)
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
        /// Obtiene la lista de tri�ngulos del grupo especificado
        /// </summary>
        /// <param name="index">Grupo de tri�ngulos</param>
        /// <returns>Devuelve la lista de tri�ngulos</returns>
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
        /// A�ade una lista de tri�ngulos al grupo especificado
        /// </summary>
        /// <param name="index">Grupo de tri�ngulos</param>
        /// <param name="triangles">Lista de tri�ngulos</param>
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
                // TODO: La esfera deber�a crearse con las otras esferas, no con el AABB
                this.BSph = BoundingSphere.CreateFromBoundingBox(this.AABB);
                this.OBB = OrientedBoundingBox.CreateFromBoundingBox(this.AABB);
            }
        }
    }
}
