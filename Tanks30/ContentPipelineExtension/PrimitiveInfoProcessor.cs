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
        /// <summary>
        /// Información de primitivas del modelo
        /// </summary>
        private PrimitiveInfo m_PrimitiveInfo = new PrimitiveInfo();

        /// <summary>
        /// Procesar cada vértice del modelo
        /// </summary>
        /// <param name="geometry">Información de geometría</param>
        /// <param name="vertexChannelIndex">Indice del canal</param>
        /// <param name="context">Contexto</param>
        protected override void ProcessVertexChannel(GeometryContent geometry, int vertexChannelIndex, ContentProcessorContext context)
        {
            // Método base del procesador de modelos
            base.ProcessVertexChannel(geometry, vertexChannelIndex, context);

            // Extraer todos los triángulos del modelo
            List<Triangle> primitives = new List<Triangle>();

            for (int i = 0; i < (geometry.Indices.Count - 2); i += 3)
            {
                // Ontener los vértices
                Vector3 vertex1 = geometry.Vertices.Positions[geometry.Indices[i]];
                Vector3 vertex2 = geometry.Vertices.Positions[geometry.Indices[i + 1]];
                Vector3 vertex3 = geometry.Vertices.Positions[geometry.Indices[i + 2]];

                // Crear el triángulo que forman
                Triangle triangle = new Triangle(vertex1, vertex2, vertex3);

                // Añadir a la lista de triángulos
                primitives.Add(triangle);
            }

            // Añadir la lista de triángulos a la lista de primitivas
            this.m_PrimitiveInfo.AddTriangles(geometry.Parent.Name, primitives.ToArray());
        }
        /// <summary>
        /// Procesar cada nodo
        /// </summary>
        /// <param name="input">Nodo</param>
        /// <param name="context">Contexto</param>
        /// <returns>Devuelve el modelo</returns>
        public override ModelContent Process(NodeContent input, ContentProcessorContext context)
        {
            // Llamada al método base
            ModelContent modelContent = base.Process(input, context);

            // Recorrer cada índice
            foreach (string index in this.m_PrimitiveInfo.Indexes)
            {
                // Recorrer cada cuerpo del modelo
                foreach (ModelMeshContent mesh in modelContent.Meshes)
                {
                    // Comparar por índices
                    if (string.Compare(index, mesh.Name) == 0)
                    {
                        // Añadir la información al cuerpo
                        mesh.Tag = this.m_PrimitiveInfo[index];
                    }
                }
            }

            this.m_PrimitiveInfo.Update();

            modelContent.Tag = m_PrimitiveInfo;

            return modelContent;
        }
    }
}
