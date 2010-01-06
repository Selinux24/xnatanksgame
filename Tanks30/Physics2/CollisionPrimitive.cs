using System;
using Microsoft.Xna.Framework;

namespace Physics
{
    /// <summary>
    /// Primitiva
    /// </summary>
    public class CollisionPrimitive
    {
        /// <summary>
        /// Matriz de transformaci�n
        /// </summary>
        private Matrix m_Transform = Matrix.Identity;
        /// <summary>
        /// La transformaci�n de la primitiva con respecto al cuerpo r�gido.
        /// </summary>
        public Matrix Offset = Matrix.Identity;
        /// <summary>
        /// Cuerpo r�gido representado por esta primitiva
        /// </summary>
        public RigidBody Body = new RigidBody();

        /// <summary>
        /// Obtiene el eje X de la transformaci�n
        /// </summary>
        public Vector3 XAxis
        {
            get
            {
                return this.m_Transform.Right;
            }
        }
        /// <summary>
        /// Obtiene el eje Y de la transformaci�n
        /// </summary>
        public Vector3 YAxis
        {
            get
            {
                return this.m_Transform.Up;
            }
        }
        /// <summary>
        /// Obtiene el eje Z de la transformaci�n
        /// </summary>
        public Vector3 ZAxis
        {
            get
            {
                return this.m_Transform.Backward;
            }
        }
        /// <summary>
        /// Obtiene la posici�n de la primitiva
        /// </summary>
        public Vector3 Position
        {
            get
            {
                return this.m_Transform.Translation;
            }
        }
        /// <summary>
        /// Obtiene la transformaci�n resultante del cuerpo r�gido y la transformaci�n de la primitiva con respecto al cuerpo r�gido.
        /// </summary>
        public Matrix Transform
        {
            get
            {
                return this.m_Transform;
            }
        }

        /// <summary>
        /// Calcula las variables internas de la primitiva.
        /// </summary>
        public void CalculateInternals()
        {
            if (this.Body != null)
            {
                this.m_Transform = this.Body.Transform * this.Offset;
            }
        }
        /// <summary>
        /// Obtiene los ejes de transformaci�n de la primitiva.
        /// </summary>
        /// <param name="axis">Eje</param>
        public Vector3 GetAxis(TransformAxis axis)
        {
            if (axis == TransformAxis.X)
            {
                return this.XAxis;
            }
            else if (axis == TransformAxis.Y)
            {
                return this.YAxis;
            }
            else if (axis == TransformAxis.Z)
            {
                return this.ZAxis;
            }

            throw new NotImplementedException();
        }
    }
}