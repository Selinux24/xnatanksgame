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
            /// Color del cielo
            /// </summary>
            public static Color AtmosphericColor = Color.OrangeRed;
            /// <summary>
            /// Luz habilitada
            /// </summary>
            public static bool LightingEnabled = true;
            /// <summary>
            /// Dirección de la luz
            /// </summary>
            public static Vector3 LightDirection = Vector3.Normalize(new Vector3(-1, -2f, -1f));
            /// <summary>
            /// Intensidad de la luz
            /// </summary>
            public static float AmbientLightIntensity = 1f;
            /// <summary>
            /// Luz ambiental
            /// </summary>
            public static Color AmbientLightColor = Color.White;

            /// <summary>
            /// Establece los parámetros de iluminación al efecto
            /// </summary>
            /// <param name="effect">Efecto</param>
            public static void SetLightToEffect(BasicEffect effect)
            {
                SetLightToEffect(effect, AmbientLightIntensity);
            }
            /// <summary>
            /// Establece los parámetros de iluminación al efecto
            /// </summary>
            /// <param name="effect">Efecto</param>
            /// <param name="lightIntensity">Intensidad de la luz</param>
            public static void SetLightToEffect(BasicEffect effect, float lightIntensity)
            {
                effect.LightingEnabled = SceneryEnvironment.Ambient.LightingEnabled;
                effect.DirectionalLight0.Enabled = SceneryEnvironment.Ambient.LightingEnabled;
                effect.DirectionalLight0.Direction = SceneryEnvironment.Ambient.LightDirection * lightIntensity;
                effect.DirectionalLight0.DiffuseColor = SceneryEnvironment.Ambient.AmbientLightColor.ToVector3() * lightIntensity;
                effect.DirectionalLight0.SpecularColor = SceneryEnvironment.Ambient.AtmosphericColor.ToVector3() * lightIntensity;
            }
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
                    effect.FogColor = SceneryEnvironment.Ambient.AtmosphericColor.ToVector3();
                    effect.FogStart = SceneryEnvironment.Fog.FogStart;
                    effect.FogEnd = SceneryEnvironment.Fog.FogEnd;
                    effect.FogEnabled = SceneryEnvironment.Fog.FogEnabled;
                }
            }
            /// <summary>
            /// Establece los parámetros de niebla en el dispositivo
            /// </summary>
            /// <param name="device">Dispositivo</param>
            public static void SetFogToDevice(GraphicsDevice device)
            {
                device.RenderState.FogColor = SceneryEnvironment.Ambient.AtmosphericColor;
                device.RenderState.FogTableMode = FogMode.Linear;
                device.RenderState.FogStart = SceneryEnvironment.LevelOfDetail.HighFarClip;
                device.RenderState.FogEnd = SceneryEnvironment.LevelOfDetail.LowFarClip;
                device.RenderState.FogDensity = 0.5f;
                device.RenderState.FogEnable = true;
            }
        }
    }
}
