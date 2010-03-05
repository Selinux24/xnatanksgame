using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Graphics;

namespace ContentPipelineExtension
{
    public class SceneryInfo
    {
        public Texture2DContent Terrain;

        public VertexBufferContent TerrainBuffer;

        public int TerrainBufferVertexCount;

        public SceneryNodeInfo TerrainInfo;

        public CompiledEffect Effect;

        public Texture2DContent Texture1;

        public Texture2DContent Texture2;

        public Texture2DContent Texture3;

        public Texture2DContent Texture4;
    }
}
