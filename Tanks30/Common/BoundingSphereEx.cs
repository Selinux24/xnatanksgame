using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Common
{
    using Common.Primitives;

    /// <summary>
    /// Extensión de la esfera
    /// </summary>
    public static class BoundingSphereEx
    {
        /// <summary>
        /// Obtiene la esfera que contiene a todos los triángulos
        /// </summary>
        /// <param name="triangles">Lista de triángulos</param>
        /// <returns>Devuelve una esfera a partir de todos los vértices de los triángulos</returns>
        public static BoundingSphere CreateFromTriangles(Triangle[] triangles)
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

            return BoundingSphere.CreateFromPoints(vertices);
        }
    }
}
