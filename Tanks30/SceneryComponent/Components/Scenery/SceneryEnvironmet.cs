using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GameComponents.Scenery
{
    /// <summary>
    /// Variables de entorno global
    /// </summary>
    public class SceneryEnvironment
    {
        /// <summary>
        /// Plano de corte cercano
        /// </summary>
        public static float GlobalNearClip = 0.1f;
        /// <summary>
        /// Plano de corte lejano
        /// </summary>
        public static float GlobalFarClip = 7000.0f;
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
            public static float HighFarClip = (GlobalFarClip - GlobalNearClip) / 7.0f * 1.0f;
            public static float HighZoneDistance = HighFarClip - GlobalNearClip;

            public static float MediumNearClip = HighFarClip;
            public static float MediumFarClip = (GlobalFarClip - GlobalNearClip) / 7.0f * 3.0f;
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
            public static Color AmbientColor = Color.Gray;

            public static bool LightingEnabled = true;
            public static Color AmbientLightColor = Color.White;
            public static Color AmbientDiffuseColor = Color.White;
            public static Color AmbientSpecularColor = new Color(1, 1, 1);
            public static float AmbientSpecularPower = 16.0f;

            public static bool Light0Enable = true;
            public static Color Light0DiffuseColor = Color.White;
            public static Color Light0SpecularColor = Color.White;
            public static Vector3 Light0Direction = new Vector3(0.0f, -10.0f, 10.0f);

            public static bool Light1Enable = true;
            public static Color Light1DiffuseColor = Color.White;
            public static Color Light1SpecularColor = Color.White;
            public static Vector3 Light1Direction = new Vector3(0.0f, -5.0f, 0.0f);

            public static bool Light2Enable = true;
            public static Color Light2DiffuseColor = Color.White;
            public static Color Light2SpecularColor = Color.White;
            public static Vector3 Light2Direction = new Vector3(0.0f, -10.0f, 0.0f);

            /// <summary>
            /// Establece los parámetros de luz y niebla al efecto
            /// </summary>
            /// <param name="effect">Efecto</param>
            public static void SetAmbientToEffect(BasicEffect effect)
            {
                effect.Alpha = 1.0f;

                effect.LightingEnabled = SceneryEnvironment.Ambient.LightingEnabled;

                effect.AmbientLightColor = SceneryEnvironment.Ambient.AmbientLightColor.ToVector3();
                effect.DiffuseColor = SceneryEnvironment.Ambient.AmbientDiffuseColor.ToVector3();
                effect.SpecularColor = SceneryEnvironment.Ambient.AmbientSpecularColor.ToVector3();
                effect.SpecularPower = SceneryEnvironment.Ambient.AmbientSpecularPower;

                effect.DirectionalLight0.Enabled = SceneryEnvironment.Ambient.Light0Enable;
                effect.DirectionalLight0.DiffuseColor = SceneryEnvironment.Ambient.Light0DiffuseColor.ToVector3();
                effect.DirectionalLight0.Direction = Vector3.Normalize(SceneryEnvironment.Ambient.Light0Direction);
                effect.DirectionalLight0.SpecularColor = SceneryEnvironment.Ambient.Light0SpecularColor.ToVector3();

                effect.DirectionalLight1.Enabled = SceneryEnvironment.Ambient.Light1Enable;
                effect.DirectionalLight1.DiffuseColor = SceneryEnvironment.Ambient.Light1DiffuseColor.ToVector3();
                effect.DirectionalLight1.Direction = Vector3.Normalize(SceneryEnvironment.Ambient.Light1Direction);
                effect.DirectionalLight1.SpecularColor = SceneryEnvironment.Ambient.Light1SpecularColor.ToVector3();

                effect.DirectionalLight2.Enabled = SceneryEnvironment.Ambient.Light2Enable;
                effect.DirectionalLight2.DiffuseColor = SceneryEnvironment.Ambient.Light2DiffuseColor.ToVector3();
                effect.DirectionalLight2.Direction = Vector3.Normalize(SceneryEnvironment.Ambient.Light2Direction);
                effect.DirectionalLight2.SpecularColor = SceneryEnvironment.Ambient.Light2SpecularColor.ToVector3();
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
                effect.FogColor = SceneryEnvironment.Ambient.AmbientColor.ToVector3();
                effect.FogStart = SceneryEnvironment.Fog.FogStart;
                effect.FogEnd = SceneryEnvironment.Fog.FogEnd;
                effect.FogEnabled = SceneryEnvironment.Fog.FogEnabled;
            }
        }
    }
}
