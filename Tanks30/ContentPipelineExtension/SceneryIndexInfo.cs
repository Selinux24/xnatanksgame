using GameComponents.Scenery;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

namespace ContentPipelineExtension
{
    public class SceneryIndexInfo
    {
        public SceneryTriangleNode[] TerrainNodes;
        public Dictionary<LOD, IndexCollection> Indices;
    }
}
