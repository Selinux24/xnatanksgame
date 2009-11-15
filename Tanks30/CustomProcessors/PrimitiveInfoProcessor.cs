using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Physics;

namespace CustomProcessors
{
    [ContentProcessor(DisplayName = "Model - TriangleInfo")]
    public class PrimitiveInfoProcessor : ModelProcessor
    {
        TriangleInfo m_Info = new TriangleInfo();

        protected override void ProcessVertexChannel(
            GeometryContent geometry,
            int vertexChannelIndex,
            ContentProcessorContext context)
        {
            base.ProcessVertexChannel(geometry, vertexChannelIndex, context);

            List<Triangle> primitives = new List<Triangle>();

            for (int i = 0; i < (geometry.Indices.Count - 2); i += 3)
            {
                Vector3 vertex1 = geometry.Vertices.Positions[geometry.Indices[i]];
                Vector3 vertex2 = geometry.Vertices.Positions[geometry.Indices[i + 1]];
                Vector3 vertex3 = geometry.Vertices.Positions[geometry.Indices[i + 2]];

                Triangle triangle = new Triangle(vertex1, vertex2, vertex3);

                primitives.Add(triangle);
            }

            m_Info.AddTriangles(geometry.Parent.Name, primitives.ToArray());

            m_Info[geometry.Parent.Name].Update();
        }

        public override ModelContent Process(
            NodeContent input,
            ContentProcessorContext context)
        {
            ModelContent modelContent = base.Process(input, context);

            foreach (string index in m_Info.Indexes)
            {
                foreach (ModelMeshContent mesh in modelContent.Meshes)
                {
                    if (string.Compare(index, mesh.Name) == 0)
                    {
                        mesh.Tag = m_Info[index];
                    }
                }
            }

            m_Info.Update();

            modelContent.Tag = m_Info;

            return modelContent;
        }
    }
}
