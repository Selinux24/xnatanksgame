using System;
using System.Collections.Generic;
using System.Text;

namespace Physics
{
    public class CollisionTriangleSoup : CollisionPrimitive
    {
        public Triangle[] Triangles;

        public CollisionTriangleSoup(Triangle[] triangles)
        {
            this.Triangles = triangles;
        }
    }
}
