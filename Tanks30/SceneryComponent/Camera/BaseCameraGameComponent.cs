using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GameComponents.Scenery;

namespace GameComponents.Camera
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public abstract class BaseCameraGameComponent : Microsoft.Xna.Framework.GameComponent
    {
        /// <summary>
        /// Dispositivo gr�fico
        /// </summary>
        protected GraphicsDevice device;

        #region Proyecci�n y vista

        // Matriz de proyecci�n global
        protected static Matrix m_GlobalProjectionMatrix;
        // Matriz de proyecci�n de alta definici�n
        protected static Matrix m_LODHighProjectionMatrix;
        // Matriz de proyecci�n de definici�n media
        protected static Matrix m_LODMediumProjectionMatrix;
        // Matriz de proyecci�n de baja definici�n
        protected static Matrix m_LODLowProjectionMatrix;
        /// <summary>
        /// Obtiene la matriz de proyecci�n global
        /// </summary>
        public static Matrix gGlobalProjectionMatrix
        {
            get
            {
                return m_GlobalProjectionMatrix;
            }
        }
        /// <summary>
        /// Obtiene la matriz de proyecci�n de alta definici�n
        /// </summary>
        public static Matrix gLODHighProjectionMatrix
        {
            get
            {
                return m_LODHighProjectionMatrix;
            }
        }
        /// <summary>
        /// Obtiene la matriz de proyecci�n de media definici�n
        /// </summary>
        public static Matrix gLODMediumProjectionMatrix
        {
            get
            {
                return m_LODMediumProjectionMatrix;
            }
        }
        /// <summary>
        /// Obtiene la matriz de proyecci�n de baja definici�n
        /// </summary>
        public static Matrix gLODLowProjectionMatrix
        {
            get
            {
                return m_LODLowProjectionMatrix;
            }
        }

        // Maatriz de vista
        protected static Matrix m_ViewMatrix;
        /// <summary>
        /// Obtiene la matriz de vista
        /// </summary>
        public static Matrix gViewMatrix
        {
            get
            {
                return m_ViewMatrix;
            }
        }
        
        /// <summary>
        /// Obtiene el cono de visi�n global de la c�mara
        /// </summary>
        public static BoundingFrustum gGlobalFrustum
        {
            get
            {
                return new BoundingFrustum(m_ViewMatrix * m_GlobalProjectionMatrix);
            }
        }
        /// <summary>
        /// Obtiene el cono de visi�n de alta definici�n de la c�mara
        /// </summary>
        public static BoundingFrustum gLODHighFrustum
        {
            get
            {
                return new BoundingFrustum(m_ViewMatrix * m_LODHighProjectionMatrix);
            }
        }
        /// <summary>
        /// Obtiene el cono de visi�n de media definici�n de la c�mara
        /// </summary>
        public static BoundingFrustum gLODMediumFrustum
        {
            get
            {
                return new BoundingFrustum(m_ViewMatrix * m_LODMediumProjectionMatrix);
            }
        }
        /// <summary>
        /// Obtiene el cono de visi�n de baja definici�n de la c�mara
        /// </summary>
        public static BoundingFrustum gLODLowFrustum
        {
            get
            {
                return new BoundingFrustum(m_ViewMatrix * m_LODLowProjectionMatrix);
            }
        }

        #endregion

        #region Din�mica

        // Posici�n
        protected static Vector3 m_Position = Vector3.Zero;
        // Direcci�n
        protected static Vector3 m_Direction = Vector3.Forward;
        // Rotaci�n en X
        protected static Quaternion m_Pitch = Quaternion.Identity;
        // Rotaci�n en Y
        protected static Quaternion m_Yaw = Quaternion.Identity;
        /// <summary>
        /// Obtiene o establece la posici�n
        /// </summary>
        public static Vector3 gPosition
        {
            get
            {
                return m_ViewMatrix.Translation;
            }
        }
        /// <summary>
        /// Obtiene la direcci�n
        /// </summary>
        public static Vector3 gDirection
        {
            get
            {
                return Matrix.Transpose(m_ViewMatrix).Forward;
            }
        }
        /// <summary>
        /// Obtiene la matriz de rotaci�n a partir de quaternion de rotaci�n
        /// </summary>
        protected static Matrix gRotationMatrix
        {
            get
            {
                return Matrix.CreateFromQuaternion(m_Pitch) * Matrix.CreateFromQuaternion(m_Yaw);
            }
        }

        #endregion

        /// <summary>
        /// Establece la posici�n de la c�mara
        /// </summary>
        public Vector3 Position
        {
            get
            {
                return m_Position;
            }
            set
            {
                m_Position = value;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game">Juego</param>
        public BaseCameraGameComponent(Game game)
            : base(game)
        {
            GraphicsDeviceManager deviceManager = (GraphicsDeviceManager)game.Services.GetService(typeof(IGraphicsDeviceManager));
            if (deviceManager != null)
            {
                device = deviceManager.GraphicsDevice;

                // Calcular la matriz de proyecci�n
                m_GlobalProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                    MathHelper.PiOver4,
                    (float)device.Viewport.Width / (float)device.Viewport.Height,
                    SceneryEnvironment.GlobalNearClip,
                    SceneryEnvironment.GlobalFarClip * 100);

                // Calcular la matriz de proyecci�n
                m_LODHighProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                    MathHelper.PiOver4,
                    (float)device.Viewport.Width / (float)device.Viewport.Height,
                    SceneryEnvironment.LevelOfDetail.HighNearClip,
                    SceneryEnvironment.LevelOfDetail.HighFarClip);

                // Calcular la matriz de proyecci�n
                m_LODMediumProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                    MathHelper.PiOver4,
                    (float)device.Viewport.Width / (float)device.Viewport.Height,
                    SceneryEnvironment.LevelOfDetail.MediumNearClip,
                    SceneryEnvironment.LevelOfDetail.MediumFarClip);

                // Calcular la matriz de proyecci�n
                m_LODLowProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                    MathHelper.PiOver4,
                    (float)device.Viewport.Width / (float)device.Viewport.Height,
                    SceneryEnvironment.LevelOfDetail.LowNearClip,
                    SceneryEnvironment.LevelOfDetail.LowFarClip);
            }
        }

        /// <summary>
        /// Actualiza el estado del componente
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
 
        /// <summary>
        /// Mueve la c�mara a lo largo de su eje de altura
        /// </summary>
        /// <param name="distance">Distancia</param>
        public void MoveUp(float distance)
        {
            this.MoveByAxis(gRotationMatrix.Up, distance);
        }
        /// <summary>
        /// Mueve la c�mara a lo largo de su eje derecho
        /// </summary>
        /// <param name="distance">Distancia</param>
        public void MoveRight(float distance)
        {
            this.MoveByAxis(gRotationMatrix.Right, distance);
        }
        /// <summary>
        /// Mueve la c�mara a lo largo de su eje de vista
        /// </summary>
        /// <param name="distance">Distancia</param>
        public void MoveForward(float distance)
        {
            this.MoveByAxis(gRotationMatrix.Forward, distance);
        }
        /// <summary>
        /// Mueve la c�mara a lo largo del eje especificado
        /// </summary>
        /// <param name="axis">Eje de tralaci�n</param>
        /// <param name="distance">Distancia</param>
        public void MoveByAxis(Vector3 axis, float distance)
        {
            Vector3 axisNorm = Vector3.Normalize(axis);

            this.MoveByVector(Vector3.Multiply(axisNorm, distance));
        }
        /// <summary>
        /// Aplica el vector especificado a la posici�n
        /// </summary>
        /// <param name="direction"></param>
        public void MoveByVector(Vector3 direction)
        {
            m_Position += direction;
        }

        /// <summary>
        /// Girar en el eje Y
        /// </summary>
        /// <param name="angle">�ngulo</param>
        public void Yaw(float angle)
        {
            m_Yaw *= Quaternion.CreateFromAxisAngle(Vector3.Up, angle);
        }
        /// <summary>
        /// Girar en el eje X
        /// </summary>
        /// <param name="angle">�ngulo</param>
        public void Pitch(float angle)
        {
            m_Pitch *= Quaternion.CreateFromAxisAngle(Vector3.Right, angle);
        }
    }
}


