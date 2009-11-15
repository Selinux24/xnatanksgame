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
        /// Matriz de transformación
        /// </summary>
        private Matrix m_Transform;
        /// <summary>
        /// La transformación de la primitiva con respecto al cuerpo rígido.
        /// </summary>
        public Matrix Offset = Matrix.Identity;
        /// <summary>
        /// Cuerpo rígido representado por esta primitiva
        /// </summary>
        public RigidBody Body = new RigidBody();

        /// <summary>
        /// Obtiene el eje X de la transformación
        /// </summary>
        public Vector3 XAxis
        {
            get
            {
                return this.m_Transform.Right;
            }
        }
        /// <summary>
        /// Obtiene el eje Y de la transformación
        /// </summary>
        public Vector3 YAxis
        {
            get
            {
                return this.m_Transform.Up;
            }
        }
        /// <summary>
        /// Obtiene el eje Z de la transformación
        /// </summary>
        public Vector3 ZAxis
        {
            get
            {
                return this.m_Transform.Backward;
            }
        }
        /// <summary>
        /// Obtiene la posición de la primitiva
        /// </summary>
        public Vector3 Position
        {
            get
            {
                return this.m_Transform.Translation;
            }
        }
        /// <summary>
        /// Obtiene la transformación resultante del cuerpo rígido y la transformación de la primitiva con respecto al cuerpo rígido.
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
        /// Obtiene los ejes de transformación de la primitiva.
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

        /// <summary>
        /// Delegado del evento de contacto entre primitivas de colisión
        /// </summary>
        /// <param name="primitive">Primitiva de colisión que ha contactado con la actual</param>
        public delegate void PrimitiveInContactDelegate(CollisionPrimitive primitive);
        /// <summary>
        /// Ocurre cuando una primitva de colisión contacta con la actual
        /// </summary>
        public event PrimitiveInContactDelegate OnPrimitiveContacted;
        /// <summary>
        /// Establece que la primitiva actual ha sido contactada por otra primitiva de colisión
        /// </summary>
        /// <param name="primitive">Primitiva de colisión que ha contactado con la actual</param>
        public virtual void PrimitiveContacted(CollisionPrimitive primitive)
        {
            if (OnPrimitiveContacted != null)
            {
                OnPrimitiveContacted(primitive);
            }
        }
    }
}