
namespace ContentPipelineExtension
{
    /// <summary>
    /// Fichero de descripción de escenario
    /// </summary>
    public class SceneryFile
    {
        /// <summary>
        /// Fichero con el mapa de alturas
        /// </summary>
        public string HeightMapFile;
        /// <summary>
        /// Longitud del lado de cada casilla
        /// </summary>
        public float HeightMapCellSize;
        /// <summary>
        /// Escala de altura
        /// </summary>
        public float HeightMapCellScale;
        /// <summary>
        /// Proporción de textura 1
        /// </summary>
        public float ProportionTexture1;
        /// <summary>
        /// Proporción de textura 2
        /// </summary>
        public float ProportionTexture2;
        /// <summary>
        /// Proporción de textura 3
        /// </summary>
        public float ProportionTexture3;
        /// <summary>
        /// Fichero de efecto de renderizado
        /// </summary>
        public string EffectFile;
        /// <summary>
        /// Fichero de textura 1
        /// </summary>
        public string Texture1File;
        /// <summary>
        /// Fichero de textura 2
        /// </summary>
        public string Texture2File;
        /// <summary>
        /// Fichero de textura 3
        /// </summary>
        public string Texture3File;
        /// <summary>
        /// Fichero de textura 4
        /// </summary>
        public string Texture4File;
        /// <summary>
        /// Textura de detalle 1
        /// </summary>
        public string DetailTexture1File;
        /// <summary>
        /// Textura de detalle 2
        /// </summary>
        public string DetailTexture2File;
        /// <summary>
        /// Textura de detalle 3
        /// </summary>
        public string DetailTexture3File;
        /// <summary>
        /// Textura de detalle 4
        /// </summary>
        public string DetailTexture4File;

        /// <summary>
        /// Efecto para vegetación
        /// </summary>
        public string BillboardEffectFile;
        /// <summary>
        /// Textura para la hierba
        /// </summary>
        public string BillboardGrassTextureFile;
        /// <summary>
        /// Textura para los árboles
        /// </summary>
        public string BillboardTreeTextureFile;
        /// <summary>
        /// Elementos por triángulo
        /// </summary>
        public int BillboardsPerTriangle;
        /// <summary>
        /// Porcentaje de árboles
        /// </summary>
        public float BillboardTreesPercent;
    }
}
