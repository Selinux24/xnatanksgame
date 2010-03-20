using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Microsoft.Xna.Framework.Graphics;

namespace ContentPipelineExtension
{
    using Common.Components;
    using Common.Drawing;

    /// <summary>
    /// Generador del fichero binario con la información del terreno
    /// </summary>
    [ContentTypeWriter]
    public class SceneryContentTypeWriter : ContentTypeWriter<SceneryInfo>
    {
        protected override void Write(ContentWriter output, SceneryInfo sceneryInfo)
        {
            // Escribir la textura del mapa de alturas
            output.WriteObject<Texture2DContent>(sceneryInfo.Terrain);
            // Escribir la definición de los vértices
            output.WriteObject<VertexElement[]>(VertexMultitextured.VertexElements);
            // Escribir el tamaño en bytes de cada vértice
            output.Write(VertexMultitextured.SizeInBytes);
            // Escribir el buffer de los vértices
            output.WriteObject<VertexBufferContent>(sceneryInfo.TerrainBuffer);
            // Escribir el número total de vértices
            output.Write(sceneryInfo.TerrainBufferVertexCount);

            // Obtener y escribir el número total de nodos del escenario
            int nodesCount = sceneryInfo.TerrainInfo.Nodes.Length;
            output.Write(nodesCount);

            for (int i = 0; i < nodesCount; i++)
            {
                // Escribir cada nodo
                output.WriteObject<SceneryTriangleNode>(sceneryInfo.TerrainInfo.Nodes[i]);
            }

            // Escribir los buffers de índices para cada nivel de detalle
            output.WriteObject<IndexCollection>(sceneryInfo.TerrainInfo.Indices[LOD.High]);
            output.WriteObject<IndexCollection>(sceneryInfo.TerrainInfo.Indices[LOD.Medium]);
            output.WriteObject<IndexCollection>(sceneryInfo.TerrainInfo.Indices[LOD.Low]);

            // Escribir el efecto para renderizar
            output.WriteObject<CompiledEffect>(sceneryInfo.Effect);

            // Escribir las texturas del terreno
            output.WriteObject<Texture2DContent>(sceneryInfo.Texture1);
            output.WriteObject<Texture2DContent>(sceneryInfo.Texture2);
            output.WriteObject<Texture2DContent>(sceneryInfo.Texture3);
            output.WriteObject<Texture2DContent>(sceneryInfo.Texture4);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return "GameComponents.Readers.SceneryReader, GameComponents, Version=1.0.0.0, Culture=neutral";
        }
    }
}
