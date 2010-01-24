﻿using System.Collections.Generic;
using Common.Components;
using GameComponents.Scenery;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Physics;

namespace ContentPipelineExtension
{
    class SceneryTriangleNodeReader : ContentTypeReader<SceneryTriangleNode>
    {
        protected override SceneryTriangleNode Read(ContentReader input, SceneryTriangleNode existingInstance)
        {
            //Triángulos
            Triangle[] triangles = input.ReadObject<Triangle[]>();

            //Indices
            int indexKeyCount = input.ReadInt32();

            Dictionary<LOD, int> startIndexes = new Dictionary<LOD, int>();

            for (int k = 0; k < indexKeyCount; k++)
            {
                LOD key = (LOD)input.ReadInt32();
                int index = input.ReadInt32();

                startIndexes.Add(key, index);
            }

            //Número de primitivas
            int primitiveKeyCount = input.ReadInt32();

            Dictionary<LOD, int> primitiveCount = new Dictionary<LOD, int>();

            for (int k = 0; k < primitiveKeyCount; k++)
            {
                LOD key = (LOD)input.ReadInt32();
                int index = input.ReadInt32();

                primitiveCount.Add(key, index);
            }

            return new SceneryTriangleNode(triangles, startIndexes, primitiveCount);
        }
    }
}
