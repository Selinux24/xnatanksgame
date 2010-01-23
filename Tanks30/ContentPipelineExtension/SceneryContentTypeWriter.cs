using Common.Format;
using GameComponents.Scenery;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Microsoft.Xna.Framework.Graphics;

namespace ContentPipelineExtension
{
    /// <summary>
    /// 
    /// </summary>
    [ContentTypeWriter]
    public class SceneryContentTypeWriter : ContentTypeWriter<SceneryInfo>
    {
        protected override void Write(ContentWriter output, SceneryInfo value)
        {
            output.WriteObject<Texture2DContent>(value.Terrain);
            output.WriteObject<VertexElement[]>(VertexMultitextured.VertexElements);
            output.Write(VertexMultitextured.SizeInBytes);
            output.WriteObject<VertexBufferContent>(value.TerrainBuffer);
            output.Write(value.TerrainBufferVertexCount);

            int nodesCount = value.TerrainInfo.TerrainNodes.Length;
            output.Write(nodesCount);

            for (int i = 0; i < nodesCount; i++)
            {
                output.WriteObject<SceneryTriangleNode>(value.TerrainInfo.TerrainNodes[i]);
            }

            output.WriteObject<IndexCollection>(value.TerrainInfo.Indices[LOD.High]);
            output.WriteObject<IndexCollection>(value.TerrainInfo.Indices[LOD.Medium]);
            output.WriteObject<IndexCollection>(value.TerrainInfo.Indices[LOD.Low]);

            output.WriteObject<CompiledEffect>(value.Effect);
            output.WriteObject<Texture2DContent>(value.Texture1);
            output.WriteObject<Texture2DContent>(value.Texture2);
            output.WriteObject<Texture2DContent>(value.Texture3);
            output.WriteObject<Texture2DContent>(value.Texture4);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "ContentPipelineExtension.SceneryReader, ContentPipelineExtension, Version=1.0.0.0, Culture=neutral";
        }
    }
}
