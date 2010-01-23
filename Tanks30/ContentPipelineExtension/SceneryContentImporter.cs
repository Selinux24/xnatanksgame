using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using System.IO;

namespace ContentPipelineExtension
{
    /// <summary>
    /// 
    /// </summary>
    [ContentImporter(".scn", DisplayName = "Scenery - Xml Importer", DefaultProcessor = "ContentPipelineExtension.SceneryContentProcessor")]
    public class SceneryContentImporter : ContentImporter<SceneryFile>
    {
        public override SceneryFile Import(string filename, ContentImporterContext context)
        {
            string directory = Path.GetDirectoryName(filename);

            XmlImporter importer = new XmlImporter();

            SceneryFile info = importer.Import(filename, context) as SceneryFile;

            info.HeightMapFile = Path.Combine(directory, info.HeightMapFile);
            info.EffectFile = Path.Combine(directory, info.EffectFile);
            info.Texture1File = Path.Combine(directory, info.Texture1File);
            info.Texture2File = Path.Combine(directory, info.Texture2File);
            info.Texture3File = Path.Combine(directory, info.Texture3File);
            info.Texture4File = Path.Combine(directory, info.Texture4File);

            return info;
        }
    }
}
