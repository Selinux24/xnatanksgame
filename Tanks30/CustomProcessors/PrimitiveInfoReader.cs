using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Physics;

namespace CustomProcessors
{
    public class PrimitiveInfoReader : ContentTypeReader<TriangleInfo>
    {
        protected override TriangleInfo Read(ContentReader input, TriangleInfo existingInstance)
        {
            TriangleInfo primitiveInfo;
            if (existingInstance != null)
            {
                primitiveInfo = existingInstance;
            }
            else
            {
                primitiveInfo = new TriangleInfo();
            }

            // El número de índices
            int indexes = input.ReadInt32();
            for (int index = 0; index < indexes; index++)
            {
                // Leer el índice actual
                string currentIndex = input.ReadString();
                // Leer el número de primitivas
                int primitives = input.ReadInt32();
                // Leer las primitivas
                Triangle[] primitiveList = new Triangle[primitives];
                for (int primitive = 0; primitive < primitives; primitive++)
                {
                    Vector3 vertex1 = input.ReadVector3();
                    Vector3 vertex2 = input.ReadVector3();
                    Vector3 vertex3 = input.ReadVector3();

                    primitiveList[primitive] = new Triangle(vertex1, vertex2, vertex3);
                }
                primitiveInfo.AddTriangles(currentIndex, primitiveList);
                // Leer el AABB
                primitiveInfo[currentIndex].AABB = new BoundingBox(input.ReadVector3(), input.ReadVector3());
                // Leer el BSph
                primitiveInfo[currentIndex].BSph = new BoundingSphere(input.ReadVector3(), input.ReadSingle());
                // Leer el OBB
                primitiveInfo[currentIndex].OBB = new OrientedBoundingBox(input.ReadVector3(), input.ReadVector3());
            }

            return primitiveInfo;
        }
    }
}
