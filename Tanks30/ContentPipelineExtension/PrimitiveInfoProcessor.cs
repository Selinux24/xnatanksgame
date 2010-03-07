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
        /// Informaci�n de primitivas del modelo
        /// </summary>
        private PrimitiveInfo m_PrimitiveInfo = new PrimitiveInfo();

        /// <summary>
        /// Procesar cada v�rtice del modelo
        /// </summary>
        /// <param name="geometry">Informaci�n de geometr�a</param>
        /// <param name="vertexChannelIndex">Indice del canal</param>
        /// <param name="context">Contexto</param>
        protected override void ProcessVertexChannel(GeometryContent geometry, int vertexChannelIndex, ContentProcessorContext context)
        {
            // M�todo base del procesador de modelos
            base.ProcessVertexChannel(geometry, vertexChannelIndex, context);

            // Extraer todos los tri�ngulos del modelo
            List<Triangle> primitives = new List<Triangle>();

            for (int i = 0; i < (geometry.Indices.Count - 2); i += 3)
            {
                // Ontener los v�rtices
                Vector3 vertex1 = geometry.Vertices.Positions[geometry.Indices[i]];
                Vector3 vertex2 = geometry.Vertices.Positions[geometry.Indices[i + 1]];
                Vector3 vertex3 = geometry.Vertices.Positions[geometry.Indices[i + 2]];

                // Crear el tri�ngulo que forman
                Triangle triangle = new Triangle(vertex1, vertex2, vertex3);

                // A�adir a la lista de tri�ngulos
                primitives.Add(triangle);
            }

            // A�adir la lista de tri�ngulos a la lista de primitivas
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
            // Llamada al m�todo base
            ModelContent modelContent = base.Process(input, context);

            // Recorrer cada �ndice
            foreach (string index in this.m_PrimitiveInfo.Indexes)
            {
                // Recorrer cada cuerpo del modelo
                foreach (ModelMeshContent mesh in modelContent.Meshes)
                {
                    // Comparar por �ndices
                    if (string.Compare(index, mesh.Name) == 0)
                    {
                        // A�adir la informaci�n al cuerpo
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
