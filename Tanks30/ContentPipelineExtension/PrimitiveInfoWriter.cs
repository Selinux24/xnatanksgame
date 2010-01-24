using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Microsoft.Xna.Framework.Graphics;
using Physics;

namespace ContentPipelineExtension
{
    [ContentTypeWriter]
    public class PrimitiveInfoWriter : ContentTypeWriter<PrimitiveInfo>
    {
        protected override void Write(ContentWriter output, PrimitiveInfo primitiveInfo)
        {
            // Escribir la cantidad de �ndices
            output.Write(primitiveInfo.Indexes.Length);

            // Escribir cada �ndice
            foreach (string index in primitiveInfo.Indexes)
            {
                // El �ndice
                output.Write(index);

                // Escibir los tri�ngulos del �ndice
                output.WriteObject<Triangle[]>(primitiveInfo[index]);
            }
        }

        public override string GetRuntimeType(TargetPlatform targetPlatform)
        {
            return typeof(Model).AssemblyQualifiedName;
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "ContentPipelineExtension.PrimitiveInfoReader, ContentPipelineExtension, Version=1.0.0.0, Culture=neutral";
        }
    }

}
