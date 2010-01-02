using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Physics;

namespace CustomProcessors
{
    [ContentTypeWriter]
    public class PrimitiveInfoWriter : ContentTypeWriter<TriangleInfo>
    {
        protected override void Write(ContentWriter output, TriangleInfo primitiveInfo)
        {
            // Cantidad de índices
            output.Write(primitiveInfo.Indexes.Length);
            foreach (string index in primitiveInfo.Indexes)
            {
                // El índice
                output.Write(index);
                // Cantidad de primitivas
                output.Write(primitiveInfo[index].Count);
                // Las primitivas
                foreach (Triangle triangle in primitiveInfo[index])
                {
                    output.Write(triangle.Point1);
                    output.Write(triangle.Point2);
                    output.Write(triangle.Point3);
                }
                // El AABB
                output.Write(primitiveInfo[index].AABB.Min);
                output.Write(primitiveInfo[index].AABB.Max);
                // El Bsph
                output.Write(primitiveInfo[index].BSph.Center);
                output.Write(primitiveInfo[index].BSph.Radius);
                // El OBB
                output.Write(primitiveInfo[index].OBB.HalfSize);
            }
        }

        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return typeof(Model).AssemblyQualifiedName;
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "CustomProcessors.PrimitiveInfoReader, CustomProcessors, Version=1.0.0.0, Culture=neutral";
        }
    }

}
