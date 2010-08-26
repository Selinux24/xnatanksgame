using System;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Graphics;

namespace ContentPipelineExtension
{
    using Common.Drawing;
    using Common.Helpers;

    /// <summary>
    /// Procesador de contenidos de mapa de alturas
    /// </summary>
    [ContentProcessor(DisplayName = "Scenery - Terrain Processor")]
    public class SceneryContentProcessor : ContentProcessor<SceneryFile, SceneryInfo>
    {
        /// <summary>
        /// Procesar el mapa de alturas
        /// </summary>
        /// <param name="input">Información del escenario de entrada</param>
        /// <param name="context">Contexto de procesado</param>
        /// <returns>Devuelve la información de escenario leída</returns>
        public override SceneryInfo Process(SceneryFile input, ContentProcessorContext context)
        {
            // Cargar la textura del mapa de alturas
            Texture2DContent terrain = context.BuildAndLoadAsset<Texture2DContent, Texture2DContent>(new ExternalReference<Texture2DContent>(input.HeightMapFile), null);

            // Obtener el mapa de alturas
            HeightMap heightMap = this.BuildHeightMap(terrain, input.HeightMapCellScale, context);

            // Generar los vértices e inicializar el buffer de vértices
            VertexMultitextured[] vertList = heightMap.BuildVertices(
                input.HeightMapCellSize,
                input.ProportionTexture1,
                input.ProportionTexture2,
                input.ProportionTexture3);
            VertexBufferContent vertexBuffer = new VertexBufferContent(VertexMultitextured.SizeInBytes * vertList.Length);
            vertexBuffer.Write<VertexMultitextured>(0, VertexMultitextured.SizeInBytes, vertList, context.TargetPlatform);

            // Generar los índices e inicializar los buffers de índices
            double lowOrderLevels = (Math.Sqrt(heightMap.DataLength) - 1) * 0.5f;
            int levelCount = Convert.ToInt32(Math.Log(lowOrderLevels, 4.0d));
            SceneryNodeInfo sceneryIndexInfo = SceneryNodeInfo.Build(
                vertList, 
                heightMap.Width, 
                heightMap.Deep, 
                levelCount);

            // Efecto de renderización
            CompiledEffect effect = Effect.CompileEffectFromFile(
                input.EffectFile, 
                null, 
                null,
                CompilerOptions.None,
                context.TargetPlatform);

            // Texturas del terreno
            Texture2DContent texture1 = context.BuildAndLoadAsset<Texture2DContent, Texture2DContent>(new ExternalReference<Texture2DContent>(input.Texture1File), null);
            Texture2DContent texture2 = context.BuildAndLoadAsset<Texture2DContent, Texture2DContent>(new ExternalReference<Texture2DContent>(input.Texture2File), null);
            Texture2DContent texture3 = context.BuildAndLoadAsset<Texture2DContent, Texture2DContent>(new ExternalReference<Texture2DContent>(input.Texture3File), null);
            Texture2DContent texture4 = context.BuildAndLoadAsset<Texture2DContent, Texture2DContent>(new ExternalReference<Texture2DContent>(input.Texture4File), null);
            Texture2DContent detailTexture1 = context.BuildAndLoadAsset<Texture2DContent, Texture2DContent>(new ExternalReference<Texture2DContent>(input.DetailTexture1File), null);
            Texture2DContent detailTexture2 = context.BuildAndLoadAsset<Texture2DContent, Texture2DContent>(new ExternalReference<Texture2DContent>(input.DetailTexture2File), null);
            Texture2DContent detailTexture3 = context.BuildAndLoadAsset<Texture2DContent, Texture2DContent>(new ExternalReference<Texture2DContent>(input.DetailTexture3File), null);
            Texture2DContent detailTexture4 = context.BuildAndLoadAsset<Texture2DContent, Texture2DContent>(new ExternalReference<Texture2DContent>(input.DetailTexture4File), null);

            return new SceneryInfo()
            {
                Terrain = terrain,
                TerrainBuffer = vertexBuffer,
                TerrainBufferVertexCount = vertList.Length,
                TerrainInfo = sceneryIndexInfo,
                Effect = effect,
                Texture1 = texture1,
                Texture2 = texture2,
                Texture3 = texture3,
                Texture4 = texture4,
                DetailTexture1 = detailTexture1,
                DetailTexture2 = detailTexture2,
                DetailTexture3 = detailTexture3,
                DetailTexture4 = detailTexture4,
            };
        }

        /// <summary>
        /// Construye el mapa de alturas a partir de la textura especificada
        /// </summary>
        /// <param name="terrain">Textura con el mapa de alturas del terreno</param>
        /// <param name="cellScale">Escala de alturas</param>
        /// <param name="context">Contexto</param>
        /// <returns>Devuelve el mapa de alturas generado</returns>
        private HeightMap BuildHeightMap(Texture2DContent terrain, float cellScale, ContentProcessorContext context)
        {
            if (terrain.Mipmaps.Count > 0)
            {
                BitmapContent heightMapContent = terrain.Mipmaps[0];

                // Sólo se soportan texturas cuadradas
                if (heightMapContent.Width == heightMapContent.Height)
                {
                    byte[] pixelData = heightMapContent.GetPixelData();

                    // Construir el mapa de alturas
                    return HeightMap.FromData(
                        pixelData,
                        heightMapContent.Height,
                        heightMapContent.Width,
                        cellScale);
                }
                else
                {
                    //Sólo se soportan texturas cuadradas
                    throw new NotImplementedException();
                }
            }
            else
            {
                throw new PipelineException("El archivo de mapa de alturas no tiene el formato correcto. No se encuentra la imagen");
            }
        }
    }
}