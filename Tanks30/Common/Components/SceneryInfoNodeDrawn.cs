using Microsoft.Xna.Framework;

namespace Common.Components
{
    /// <summary>
    /// Informaci�n de nodo dibujado
    /// </summary>
    public class SceneryInfoNodeDrawn
    {
        /// <summary>
        /// V�rtice arriba a la izquierda
        /// </summary>
        public readonly Vector2 UpperLeft;
        /// <summary>
        /// V�rtice abajo a la derecha
        /// </summary>
        public readonly Vector2 LowerRight;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="level">Nivel del nodo</param>
        /// <param name="upperLeftX">Componente X del v�rtice superior</param>
        /// <param name="upperLeftY">Componente Y del v�rtice superior</param>
        /// <param name="lowerRightX">Componente X del v�rtice inferior</param>
        /// <param name="lowerRightY">Componente Y del v�rtice inferior</param>
        public SceneryInfoNodeDrawn(float upperLeftX, float upperLeftY, float lowerRightX, float lowerRightY)
        {
            this.UpperLeft.X = upperLeftX;
            this.UpperLeft.Y = upperLeftY;

            this.LowerRight.X = lowerRightX;
            this.LowerRight.Y = lowerRightY;
        }
    }
}
