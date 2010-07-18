using System;
using System.IO;
using System.Xml.Serialization;

namespace GameComponents.Buildings
{
    using GameComponents.Animation;

    /// <summary>
    /// Información de animación de un edificio
    /// </summary>
    [Serializable]
    public partial class BuildingComponentInfo
    {
        /// <summary>
        /// Nombre del modelo
        /// </summary>
        public string Model = null;

        /// <summary>
        /// Colección de controladores de animación
        /// </summary>
        public AnimationInfo[] AnimationControlers;

        /// <summary>
        /// Carga la información de animación de un edificio desde un xml
        /// </summary>
        /// <param name="xml">Información XML</param>
        /// <returns>Devuelve la información leída</returns>
        public static BuildingComponentInfo Load(string xml)
        {
            StreamReader rd = new StreamReader(xml);
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(BuildingComponentInfo));

                BuildingComponentInfo result = serializer.Deserialize(rd) as BuildingComponentInfo;

                return result;
            }
            finally
            {
                rd.Close();
                rd.Dispose();
                rd = null;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public BuildingComponentInfo()
        {

        }
    }
}
