using System.IO;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace ContentPipelineExtension
{
    /// <summary>
    /// Importador del fichero XML que define un terreno generado a partir de un mapa de alturas
    /// </summary>
    [ContentImporter(".scn", DisplayName = "Scenery - Terrain Importer", DefaultProcessor = "ContentPipelineExtension.SceneryContentProcessor")]
    public class SceneryContentImporter : ContentImporter<SceneryFile>
    {
        /// <summary>
        /// Importa la información de un fichero de mapa de alturas
        /// </summary>
        /// <param name="filename">Nombre del fichero</param>
        /// <param name="context">Contexto</param>
        /// <returns>Devuelve el descriptor del terreno cargado a partir del fichero</returns>
        public override SceneryFile Import(string filename, ContentImporterContext context)
        {
            // Obtiene el directorio del fichero de definición
            string directory = Path.GetDirectoryName(filename);

            // Importador XML
            XmlImporter importer = new XmlImporter();

            // Lectura del fichero
            SceneryFile info = importer.Import(filename, context) as SceneryFile;

            // Completar los nombres de los ficheros con el directorio del fichero de definición
            info.HeightMapFile = Path.Combine(directory, info.HeightMapFile);
            info.EffectFile = Path.Combine(directory, info.EffectFile);
            info.Texture1File = Path.Combine(directory, info.Texture1File);
            info.Texture2File = Path.Combine(directory, info.Texture2File);
            info.Texture3File = Path.Combine(directory, info.Texture3File);
            info.Texture4File = Path.Combine(directory, info.Texture4File);
            info.DetailTexture1File = Path.Combine(directory, info.DetailTexture1File);
            info.DetailTexture2File = Path.Combine(directory, info.DetailTexture2File);
            info.DetailTexture3File = Path.Combine(directory, info.DetailTexture3File);
            info.DetailTexture4File = Path.Combine(directory, info.DetailTexture4File);
            info.BillboardEffectFile = Path.Combine(directory, info.BillboardEffectFile);
            info.BillboardTreeTextureFile = Path.Combine(directory, info.BillboardTreeTextureFile);
            info.BillboardGrassTextureFile = Path.Combine(directory, info.BillboardGrassTextureFile);

            // Devuelve la definición
            return info;
        }
    }
}
