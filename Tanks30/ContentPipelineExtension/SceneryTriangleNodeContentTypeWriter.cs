using Common.Components;
using GameComponents.Scenery;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Physics;

namespace ContentPipelineExtension
{
    /// <summary>
    /// 
    /// </summary>
    [ContentTypeWriter]
    public class SceneryTriangleNodeContentTypeWriter : ContentTypeWriter<SceneryTriangleNode>
    {
        protected override void Write(ContentWriter output, SceneryTriangleNode value)
        {
            //Triángulos
            output.WriteObject<Triangle[]>(value.TriangleSoup.Triangles);

            //Indices
            LOD[] indexKeys = new LOD[value.StartIndexes.Keys.Count];
            value.StartIndexes.Keys.CopyTo(indexKeys, 0);

            int indexKeyCount = indexKeys.Length;
            output.Write(indexKeyCount);

            for (int k = 0; k < indexKeyCount; k++)
            {
                LOD key = indexKeys[k];

                output.Write((int)key);
                output.Write(value.StartIndexes[key]);
            }

            //Número de primitivas
            LOD[] primitiveKeys = new LOD[value.PrimitiveCount.Keys.Count];
            value.PrimitiveCount.Keys.CopyTo(primitiveKeys, 0);

            int primitiveKeyCount = primitiveKeys.Length;
            output.Write(primitiveKeyCount);

            for (int k = 0; k < primitiveKeyCount; k++)
            {
                LOD key = primitiveKeys[k];

                output.Write((int)key);
                output.Write(value.PrimitiveCount[key]);
            }
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "ContentPipelineExtension.SceneryTriangleNodeReader, ContentPipelineExtension, Version=1.0.0.0, Culture=neutral";
        }
    }
}
