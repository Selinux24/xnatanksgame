using Microsoft.Xna.Framework;

namespace Common.Components
{
    /// <summary>
    /// Información de nodo dibujado
    /// </summary>
    public class SceneryInfoNodeDrawn
    {
        /// <summary>
        /// Vértice arriba a la izquierda
        /// </summary>
        public readonly Vector2 UpperLeft;
        /// <summary>
        /// Vértice abajo a la derecha
        /// </summary>
        public readonly Vector2 LowerRight;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="level">Nivel del nodo</param>
        /// <param name="upperLeftX">Componente X del vértice superior</param>
        /// <param name="upperLeftY">Componente Y del vértice superior</param>
        /// <param name="lowerRightX">Componente X del vértice inferior</param>
        /// <param name="lowerRightY">Componente Y del vértice inferior</param>
        public SceneryInfoNodeDrawn(float upperLeftX, float upperLeftY, float lowerRightX, float lowerRightY)
        {
            this.UpperLeft.X = upperLeftX;
            this.UpperLeft.Y = upperLeftY;

            this.LowerRight.X = lowerRightX;
            this.LowerRight.Y = lowerRightY;
        }
    }
}
