using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Microsoft.Xna.Framework.Graphics;
using Physics;

namespace ContentPipelineExtension
{
    [ContentTypeWriter]
    public class PrimitiveListWriter : ContentTypeWriter<Triangle[]>
    {
        protected override void Write(ContentWriter output, Triangle[] primitiveList)
        {
            // Escribir la cantidad de triángulos
            output.Write(primitiveList.Length);

            // Escribir cada vértice de cada triángulo
            foreach (Triangle triangle in primitiveList)
            {
                output.Write(triangle.Point1);
                output.Write(triangle.Point2);
                output.Write(triangle.Point3);
            }
        }

        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return typeof(Model).AssemblyQualifiedName;
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "ContentPipelineExtension.PrimitiveListReader, ContentPipelineExtension, Version=1.0.0.0, Culture=neutral";
        }
    }
}
