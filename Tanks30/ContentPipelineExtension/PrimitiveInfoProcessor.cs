using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

namespace ContentPipelineExtension
{
    using Common.Primitives;

    [ContentProcessor(DisplayName = "Model - TriangleInfo")]
    public class PrimitiveInfoProcessor : ModelProcessor
    {
        private PrimitiveInfo m_PrimitiveInfo = new PrimitiveInfo();

        protected override void ProcessVertexChannel(GeometryContent geometry, int vertexChannelIndex, ContentProcessorContext context)
        {
            base.ProcessVertexChannel(geometry, vertexChannelIndex, context);

            // Extraer todos los triángulos del modelo
            List<Triangle> primitives = new List<Triangle>();

            for (int i = 0; i < (geometry.Indices.Count - 2); i += 3)
            {
                Vector3 vertex1 = geometry.Vertices.Positions[geometry.Indices[i]];
                Vector3 vertex2 = geometry.Vertices.Positions[geometry.Indices[i + 1]];
                Vector3 vertex3 = geometry.Vertices.Positions[geometry.Indices[i + 2]];

                Triangle triangle = new Triangle(vertex1, vertex2, vertex3);

                primitives.Add(triangle);
            }

            this.m_PrimitiveInfo.AddTriangles(geometry.Parent.Name, primitives.ToArray());
        }

        public override ModelContent Process(NodeContent input, ContentProcessorContext context)
        {
            ModelContent modelContent = base.Process(input, context);

            foreach (string index in m_PrimitiveInfo.Indexes)
            {
                foreach (ModelMeshContent mesh in modelContent.Meshes)
                {
                    if (string.Compare(index, mesh.Name) == 0)
                    {
                        mesh.Tag = m_PrimitiveInfo[index];
                    }
                }
            }

            m_PrimitiveInfo.Update();

            modelContent.Tag = m_PrimitiveInfo;

            return modelContent;
        }
    }
}
