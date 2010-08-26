using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Graphics;

namespace ContentPipelineExtension
{
    /// <summary>
    /// Elementos del terreno
    /// </summary>
    public class SceneryInfo
    {
        /// <summary>
        /// Textura del mapa de alturas del terreno
        /// </summary>
        public Texture2DContent Terrain;
        /// <summary>
        /// Buffer de vértices del terreno
        /// </summary>
        public VertexBufferContent TerrainBuffer;
        /// <summary>
        /// Número de vértices del terreno
        /// </summary>
        public int TerrainBufferVertexCount;
        /// <summary>
        /// Información de nodos del terreno
        /// </summary>
        public SceneryNodeInfo TerrainInfo;
        /// <summary>
        /// Efecto de renderización
        /// </summary>
        public CompiledEffect Effect;
        /// <summary>
        /// Textura de la altura 1
        /// </summary>
        public Texture2DContent Texture1;
        /// <summary>
        /// Textura de la altura 2
        /// </summary>
        public Texture2DContent Texture2;
        /// <summary>
        /// Textura de la altura 3
        /// </summary>
        public Texture2DContent Texture3;
        /// <summary>
        /// Textura de la altura 4
        /// </summary>
        public Texture2DContent Texture4;

        public Texture2DContent DetailTexture1;
        public Texture2DContent DetailTexture2;
        public Texture2DContent DetailTexture3;
        public Texture2DContent DetailTexture4;
    }
}
