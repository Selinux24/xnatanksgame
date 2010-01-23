using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Physics;

namespace ContentPipelineExtension
{
    public class PrimitiveListReader : ContentTypeReader<TriangleList>
    {
        protected override TriangleList Read(ContentReader input, TriangleList existingInstance)
        {
            // Leer el número de primitivas
            int primitiveCount = input.ReadInt32();
            // Leer las primitivas
            Triangle[] primitives = new Triangle[primitiveCount];
            for (int primitiveIndex = 0; primitiveIndex < primitiveCount; primitiveIndex++)
            {
                Vector3 vertex1 = input.ReadVector3();
                Vector3 vertex2 = input.ReadVector3();
                Vector3 vertex3 = input.ReadVector3();

                primitives[primitiveIndex] = new Triangle(vertex1, vertex2, vertex3);
            }
            TriangleList primitiveList = new TriangleList(primitives);
            // Leer el AABB
            BoundingBox aabb = new BoundingBox(input.ReadVector3(), input.ReadVector3());
            primitiveList.AABB = aabb;
            // Leer el Bsph
            BoundingSphere bsph = new BoundingSphere(input.ReadVector3(), input.ReadSingle());
            primitiveList.BSph = bsph;
            // Leer el OBB
            CollisionBox obb = new CollisionBox() { HalfSize = input.ReadVector3() };
            primitiveList.OBB = obb;

            return primitiveList;
        }
    }
}
