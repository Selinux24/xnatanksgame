using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Common
{
    public abstract class GlobalMatrices
    {
        /// <summary>
        /// Matriz mundo
        /// </summary>
        public static Matrix gWorldMatrix = Matrix.Identity;

        /// <summary>
        /// Obtiene la matriz de proyección global
        /// </summary>
        public static Matrix gGlobalProjectionMatrix;
        /// <summary>
        /// Obtiene la matriz de proyección de alta definición
        /// </summary>
        public static Matrix gLODHighProjectionMatrix;
        /// <summary>
        /// Obtiene la matriz de proyección de media definición
        /// </summary>
        public static Matrix gLODMediumProjectionMatrix;
        /// <summary>
        /// Obtiene la matriz de proyección de baja definición
        /// </summary>
        public static Matrix gLODLowProjectionMatrix;

        /// <summary>
        /// Obtiene la matriz de vista
        /// </summary>
        public static Matrix gViewMatrix;

        /// <summary>
        /// Obtiene o establece la posición
        /// </summary>
        public static Vector3 gPosition
        {
            get
            {
                return GlobalMatrices.gViewMatrix.Translation;
            }
        }
        /// <summary>
        /// Obtiene la dirección
        /// </summary>
        public static Vector3 gDirection
        {
            get
            {
                return Matrix.Transpose(GlobalMatrices.gViewMatrix).Forward;
            }
        }

        /// <summary>
        /// Obtiene el cono de visión global de la cámara
        /// </summary>
        public static BoundingFrustum gGlobalFrustum
        {
            get
            {
                return new BoundingFrustum(gViewMatrix * gGlobalProjectionMatrix);
            }
        }
        /// <summary>
        /// Obtiene el cono de visión de alta definición de la cámara
        /// </summary>
        public static BoundingFrustum gLODHighFrustum
        {
            get
            {
                return new BoundingFrustum(gViewMatrix * gLODHighProjectionMatrix);
            }
        }
        /// <summary>
        /// Obtiene el cono de visión de media definición de la cámara
        /// </summary>
        public static BoundingFrustum gLODMediumFrustum
        {
            get
            {
                return new BoundingFrustum(gViewMatrix * gLODMediumProjectionMatrix);
            }
        }
        /// <summary>
        /// Obtiene el cono de visión de baja definición de la cámara
        /// </summary>
        public static BoundingFrustum gLODLowFrustum
        {
            get
            {
                return new BoundingFrustum(gViewMatrix * gLODLowProjectionMatrix);
            }
        }

        /// <summary>
        /// Plano lejano
        /// </summary>
        public static float NearClipPlane = 0.1f;
        /// <summary>
        /// Plano cercano
        /// </summary>
        public static float FarClipPlane = 1000000f;

        /// <summary>
        /// Actualiza la matriz de proyección
        /// </summary>
        /// <param name="device">Dispositivo</param>
        /// <remarks>Del dispositivo obtiene la relación de aspecto, según el tamaño del ViewPort</remarks>
        public static void UpdateProjection(GraphicsDevice device)
        {
            GlobalMatrices.UpdateProjection((float)device.Viewport.Width / (float)device.Viewport.Height);
        }
        /// <summary>
        /// Actualiza la matriz de proyección
        /// </summary>
        /// <param name="width">Ancho del ViewPort</param>
        /// <param name="height">Alto del ViewPort</param>
        public static void UpdateProjection(float width, float height)
        {
            GlobalMatrices.UpdateProjection(width / height);
        }
        /// <summary>
        /// Actualiza la matriz de proyección
        /// </summary>
        /// <param name="aspectRatio">Relación de aspecto</param>
        public static void UpdateProjection(float aspectRatio)
        {
            GlobalMatrices.gGlobalProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4,
                aspectRatio,
                GlobalMatrices.NearClipPlane,
                GlobalMatrices.FarClipPlane);
        }
    }
}
