using System;
using System.IO;
using System.Xml.Serialization;

namespace GameComponents.Vehicles
{
    using GameComponents.Animation;
    using GameComponents.Weapons;

    /// <summary>
    /// Información de animación de un vehículo
    /// </summary>
    [Serializable]
    public partial class VehicleComponentInfo
    {
        /// <summary>
        /// Nombre del modelo
        /// </summary>
        public string Model = null;
        /// <summary>
        /// Integridad
        /// </summary>
        public float Hull;
        /// <summary>
        /// Blindaje del casco
        /// </summary>
        public float Armor;
        /// <summary>
        /// Velocidad de avance
        /// </summary>
        public float MaxForwardVelocity;
        /// <summary>
        /// Velocidad de retroceso
        /// </summary>
        public float MaxBackwardVelocity;
        /// <summary>
        /// Modificador de aceleración
        /// </summary>
        public float AccelerationModifier;
        /// <summary>
        /// Modificador de desaceleración
        /// </summary>
        public float BrakeModifier;
        /// <summary>
        /// Modificador de giro
        /// </summary>
        public float AngularVelocityModifier;
        /// <summary>
        /// Indica si es un vehículo volador
        /// </summary>
        public bool Skimmer = false;
        /// <summary>
        /// Altura de vuelo máxima
        /// </summary>
        public float MaxFlightHeight;
        /// <summary>
        /// Altura de vuelo mínima
        /// </summary>
        public float MinFlightHeight;
        /// <summary>
        /// Angulo de inclinación del morro en el ascenso
        /// </summary>
        public float AscendingAngle;
        /// <summary>
        /// Angulo de inclinación del morro en el descenso
        /// </summary>
        public float DescendingAngle;

        /// <summary>
        /// Colección de armas
        /// </summary>
        public WeaponInfo[] Weapons;
        /// <summary>
        /// Colección de controladores de animación
        /// </summary>
        public AnimationInfo[] AnimationControlers;
        /// <summary>
        /// Colección de posiciones de jugador
        /// </summary>
        public PlayerPositionInfo[] PlayerPositions;

        /// <summary>
        /// Carga la información de animación de un vehículo desde un xml
        /// </summary>
        /// <param name="xml">Información XML</param>
        /// <returns>Devuelve la información leída</returns>
        public static VehicleComponentInfo Load(string xml)
        {
            StreamReader rd = new StreamReader(xml);
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(VehicleComponentInfo));

                VehicleComponentInfo result = serializer.Deserialize(rd) as VehicleComponentInfo;

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
        public VehicleComponentInfo()
        {

        }
    }
}
