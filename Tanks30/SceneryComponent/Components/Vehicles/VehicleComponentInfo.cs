using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Graphics;

namespace GameComponents.Vehicles
{
    using GameComponents.Vehicles.Animations;

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

        /// <summary>
        /// Crea la lista de animaciones usable por los componentes
        /// </summary>
        /// <param name="model">Modelo a animar</param>
        /// <returns>Devuelve una lista de animaciones</returns>
        public Animation[] CreateAnimationList(Model model)
        {
            List<Animation> animationList = new List<Animation>();

            foreach (AnimationInfo animationInfo in this.AnimationControlers)
            {
                if (animationInfo.Type == typeof(Animation).ToString())
                {
                    Animation animation = new Animation(animationInfo.Name, model.Bones[animationInfo.BoneName]);
                    animation.Initialize(animationInfo.Axis);

                    animationList.Add(animation);
                }
                else if (animationInfo.Type == typeof(AnimationAxis).ToString())
                {
                    AnimationAxis animation = new AnimationAxis(animationInfo.Name, model.Bones[animationInfo.BoneName]);
                    animation.Initialize(animationInfo.Axis, animationInfo.AngleFrom, animationInfo.AngleTo, animationInfo.Velocity, animationInfo.Inverse);

                    animationList.Add(animation);
                }
            }

            return animationList.ToArray();
        }
        /// <summary>
        /// Crea la lista de posiciones de jugador usable por los componentes
        /// </summary>
        /// <param name="model">Modelo que contiene las posiciones</param>
        /// <returns>Devuelve una lista de posiciones de jugador</returns>
        public PlayerPosition[] CreatePlayerPositionList(Model model)
        {
            List<PlayerPosition> m_PlayerControlList = new List<PlayerPosition>();

            foreach (PlayerPositionInfo positionInfo in this.PlayerPositions)
            {
                PlayerPosition position = new PlayerPosition(positionInfo.Name, model.Bones[positionInfo.BoneName], positionInfo.Translation);

                m_PlayerControlList.Add(position);
            }

            return m_PlayerControlList.ToArray();
        }
    }
}
