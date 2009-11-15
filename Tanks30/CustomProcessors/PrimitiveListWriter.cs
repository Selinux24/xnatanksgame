using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Physics;

namespace CustomProcessors
{
    [ContentTypeWriter]
    public class PrimitiveListWriter : ContentTypeWriter<TriangleList>
    {
        protected override void Write(ContentWriter output, TriangleList primitiveList)
        {
            // Cantidad de primitivas
            output.Write(primitiveList.Count);
            // Las primitivas
            foreach (Triangle triangle in primitiveList)
            {
                output.Write(triangle.Point1);
                output.Write(triangle.Point2);
                output.Write(triangle.Point3);
            }
            // El AAABB
            output.Write(primitiveList.AABB.Max);
            output.Write(primitiveList.AABB.Min);
            // El BSph
            output.Write(primitiveList.BSph.Center);
            output.Write(primitiveList.BSph.Radius);
            // El OBB
            output.Write(primitiveList.OBB.Max);
            output.Write(primitiveList.OBB.Min);
        }

        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return typeof(Model).AssemblyQualifiedName;
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "CustomProcessors.PrimitiveListReader, CustomProcessors, Version=1.0.0.0, Culture=neutral";
        }
    }
}
