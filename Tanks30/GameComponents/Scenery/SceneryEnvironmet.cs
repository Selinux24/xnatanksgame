using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameComponents.Scenery
{
    using Common;

    /// <summary>
    /// Variables de entorno global
    /// </summary>
    public class SceneryEnvironment
    {
        /// <summary>
        /// Plano de corte cercano
        /// </summary>
        public static float GlobalNearClip = GlobalMatrices.NearClipPlane;
        /// <summary>
        /// Plano de corte lejano
        /// </summary>
        public static float GlobalFarClip = GlobalMatrices.FarClipPlane;
        /// <summary>
        /// Obtiene la distancia desde el punto de vista al plano lejano global
        /// </summary>
        public static float GlobalZoneDistance = GlobalFarClip - GlobalNearClip;

        /// <summary>
        /// Nivel de detalle
        /// </summary>
        public abstract class LevelOfDetail
        {
            public static float HighNearClip = GlobalNearClip;
            public static float HighFarClip = (GlobalFarClip - GlobalNearClip) * 0.20f;
            public static float HighZoneDistance = HighFarClip - GlobalNearClip;

            public static float MediumNearClip = HighFarClip;
            public static float MediumFarClip = (GlobalFarClip - GlobalNearClip) * 0.40f;
            public static float MediumZoneDistance = MediumFarClip - GlobalNearClip;

            public static float LowNearClip = MediumFarClip;
            public static float LowFarClip = GlobalFarClip;
            public static float LowZoneDistance = LowFarClip - GlobalNearClip;
        }

        /// <summary>
        /// Cielo
        /// </summary>
        public abstract class SkyBox
        {
            public static float NearClip = 1.0f;
            public static float FarClip = GlobalFarClip * 100.0f;
        }

        /// <summary>
        /// Ambiente
        /// </summary>
        public abstract class Ambient
        {
            /// <summary>
            /// Color ambiental
            /// </summary>
            public static Color AmbientColor = Color.OrangeRed;

            public static bool LightingEnabled = false;
            public static Vector3 LightDirection = Vector3.Normalize(new Vector3(-1, -2f, -1f));
        }

        /// <summary>
        /// Niebla
        /// </summary>
        public abstract class Fog
        {
            /// <summary>
            /// Obtiene el plano inicial de niebla
            /// </summary>
            public static float FogStart
            {
                get
                {
                    return GlobalFarClip - (GlobalFarClip * FogIntensity) + GlobalNearClip;
                }
            }
            /// <summary>
            /// Obtiene el plano final de niebla
            /// </summary>
            public static float FogEnd
            {
                get
                {
                    return GlobalFarClip;
                }
            }
            /// <summary>
            /// Intensidad de la niebla
            /// </summary>
            public static float FogIntensity = 0.9f;
            /// <summary>
            /// Indica si la niebla está activada
            /// </summary>
            public static bool FogEnabled = true;

            /// <summary>
            /// Establece la niebla en el efecto
            /// </summary>
            /// <param name="effect">Efecto</param>
            public static void SetFogToEffect(BasicEffect effect)
            {
                if (effect != null)
                {
                    effect.FogColor = SceneryEnvironment.Ambient.AmbientColor.ToVector3();
                    effect.FogStart = SceneryEnvironment.Fog.FogStart;
                    effect.FogEnd = SceneryEnvironment.Fog.FogEnd;
                    effect.FogEnabled = SceneryEnvironment.Fog.FogEnabled;
                }
            }
        }
    }
}
