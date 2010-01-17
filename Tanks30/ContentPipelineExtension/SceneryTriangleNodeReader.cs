using System.Collections.Generic;
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
            int triangleCount = input.ReadInt32();

            List<Triangle> triangles = new List<Triangle>();

            for (int i = 0; i < triangleCount; i++)
            {
                Vector3 point1 = input.ReadVector3();
                Vector3 point2 = input.ReadVector3();
                Vector3 point3 = input.ReadVector3();

                Triangle tri = new Triangle(point1, point2, point3);

                triangles.Add(tri);
            }

            TriangleList triangleList = new TriangleList(triangles.ToArray());

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

            return new SceneryTriangleNode(triangleList, startIndexes, primitiveCount);
        }
    }
}
