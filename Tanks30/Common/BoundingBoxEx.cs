using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Common
{
    using Common.Primitives;

    public static class BoundingBoxEx
    {
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
