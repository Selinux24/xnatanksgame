using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;

namespace GameComponents.Readers
{
    using Common.Components;
    using Common.Drawing;
    using Common.Helpers;
    using Common.Primitives;

    class SceneryTriangleNodeReader : ContentTypeReader<SceneryTriangleNode>
    {
        protected override SceneryTriangleNode Read(ContentReader input, SceneryTriangleNode existingInstance)
        {
            //Triángulos
            Triangle[] triangles = input.ReadObject<Triangle[]>();

            //Indices
            int indexKeyCount = input.ReadInt32();

            Dictionary<LOD, SceneryNodeIndexInfo> indexesInfo = new Dictionary<LOD, SceneryNodeIndexInfo>();

            for (int k = 0; k < indexKeyCount; k++)
            {
                LOD key = (LOD)input.ReadInt32();
                SceneryNodeIndexInfo indexInfo = input.ReadObject<SceneryNodeIndexInfo>();

                indexesInfo.Add(key, indexInfo);
            }

            return new SceneryTriangleNode(triangles, indexesInfo);
        }
    }
}
