using System;
using Microsoft.Xna.Framework;

namespace DrawingComponents
{
    using Common;

    /// <summary>
    /// Componente dibujador de geometr�a
    /// </summary>
    public abstract class GeometryDrawer : DrawableGameComponent
    {
        // Indica si se debe actualizar la matriz con los elementos de posici�n, orientaci�n y escala
        bool updateMatrix = true;

        /// <summary>
        /// Posici�n
        /// </summary>
        private Vector3 position = Vector3.Zero;
        /// <summary>
        /// Orientaci�n
        /// </summary>
        private Quaternion orientation = Quaternion.Identity;
        /// <summary>
        /// Escala
        /// </summary>
        private Vector3 scale = Vector3.One;
        /// <summary>
        /// Transformaci�n
        /// </summary>
        private Matrix transform = Matrix.Identity;

        /// <summary>
        /// Obtiene o establece la posici�n
        /// </summary>
        public Vector3 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;

                updateMatrix = true;
            }
        }
        /// <summary>
        /// Obtiene o establece la orientaci�n
        /// </summary>
        public Quaternion Orientation
        {
            get
            {
                return orientation;
            }
            set
            {
                orientation = value;

                updateMatrix = true;
            }
        }
        /// <summary>
        /// Obtiene o establece la escala
        /// </summary>
        public Vector3 Scale
        {
            get
            {
                return scale;
            }
            set
            {
                scale = value;

                updateMatrix = true;
            }
        }
        /// <summary>
        /// Obtiene o establece la transformaci�n
        /// </summary>
        public Matrix Transform 
        {
            get
            {
                return transform;
            }
            set
            {
                transform = value;
            }
        }

        /// <summary>
        /// Geometr�a que dibuja el componente
        /// </summary>
        public Geometry Geometry;

        /// <summary>
        /// Constructor
        /// </summary>
        public GeometryDrawer(Game game)
            : base(game)
        {
            
        }

        /// <summary>
        /// Actualiza el componente
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (updateMatrix)
            {
                // Actualizar la matriz con los componentes de escala, orientaci�n y posici�n
                transform = Matrix.CreateScale(scale) * Matrix.CreateFromQuaternion(orientation) * Matrix.CreateTranslation(position);

                updateMatrix = false;
            }
        }
        /// <summary>
        /// Dibuja la geometr�a del componente
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            if (this.Geometry != null)
            {
                // Dibujar
                this.Geometry.Draw(
                    this.GraphicsDevice, 
                    this.transform * GlobalMatrices.gWorldMatrix, 
                    GlobalMatrices.gViewMatrix, 
                    GlobalMatrices.gGlobalProjectionMatrix);
            }
        }
    }
}
