using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Common
{
    using Common.Primitives;

    /// <summary>
    /// Extensión de la caja alineada con los ejes
    /// </summary>
    public static class BoundingBoxEx
    {
        /// <summary>
        /// Obtiene la caja alineada con los ejes que contiene a todos los triángulos
        /// </summary>
        /// <param name="triangles">Lista de triángulos</param>
        /// <returns>Devuelve una caja alineada con los ejes a partir de todos los vértices de los triángulos</returns>
        public static BoundingBox CreateFromTriangles(Triangle[] triangles)
        {
            List<Vector3> vertices = new List<Vector3>();

            if (triangles != null && triangles.Length > 0)
            {
                for (int i = 0; i < triangles.Length; i++)
                {
                    vertices.Add(triangles[i].Point1);
                    vertices.Add(triangles[i].Point2);
                    vertices.Add(triangles[i].Point3);
                }
            }

            return BoundingBox.CreateFromPoints(vertices);
        }
    }
}
