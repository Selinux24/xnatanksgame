using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DrawingComponents
{
    /// <summary>
    /// Matrices maestras
    /// </summary>
    public abstract class GlobalMatrices
    {
        /// <summary>
        /// Matriz mundo
        /// </summary>
        public static Matrix World = Matrix.Identity;
        /// <summary>
        /// Matriz vista
        /// </summary>
        public static Matrix View = Matrix.Identity;
        /// <summary>
        /// Matriz proyección
        /// </summary>
        public static Matrix Projection = Matrix.Identity;

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
            GlobalMatrices.Projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4,
                aspectRatio,
                GlobalMatrices.NearClipPlane,
                GlobalMatrices.FarClipPlane);
        }
    }
}
