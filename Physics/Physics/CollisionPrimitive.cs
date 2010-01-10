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
        /// Cuerpo rígido representado por esta primitiva
        /// </summary>
        public RigidBody Body = new RigidBody();

        /// <summary>
        /// Obtiene la transformación resultante del cuerpo rígido y la transformación de la primitiva con respecto al cuerpo rígido.
        /// </summary>
        public Matrix Transform
        {
            get
            {
                if (this.Body != null)
                {
                    return this.Body.Transform;
                }

                return Matrix.Identity;
            }
        }
        /// <summary>
        /// Obtiene el eje X de la transformación
        /// </summary>
        public Vector3 XAxis
        {
            get
            {
                return this.Transform.Right;
            }
        }
        /// <summary>
        /// Obtiene el eje Y de la transformación
        /// </summary>
        public Vector3 YAxis
        {
            get
            {
                return this.Transform.Up;
            }
        }
        /// <summary>
        /// Obtiene el eje Z de la transformación
        /// </summary>
        public Vector3 ZAxis
        {
            get
            {
                return this.Transform.Backward;
            }
        }
        /// <summary>
        /// Obtiene la posición de la primitiva
        /// </summary>
        public Vector3 Position
        {
            get
            {
                return this.Transform.Translation;
            }
        }
              
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="halfSize">Longitud del centro hasta las caras en los tres ejes</param>
        /// <param name="mass">Masa</param>
        public CollisionPrimitive(float mass)
        {
            if (this.Body != null)
            {
                this.Body.Mass = mass;
                this.Body.SetDamping(0.99f, 0.8f);
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
        /// Establece el estado inicial de la caja en la posición indicada
        /// </summary>
        /// <param name="position">Posición inicial</param>
        public virtual void SetState(Vector3 position)
        {
            this.SetState(position, Quaternion.Identity);
        }
        /// <summary>
        /// Establece el estado inicial de la caja en la posición y orientación indicadas
        /// </summary>
        /// <param name="position">Posición inicial</param>
        /// <param name="orientation">Orientación inicial</param>
        public virtual void SetState(Vector3 position, Quaternion orientation)
        {
            if (this.Body != null)
            {
                this.Body.Position = position;
                this.Body.Orientation = orientation;

                this.Body.Velocity = Vector3.Zero;
                this.Body.Rotation = Vector3.Zero;
                this.Body.Acceleration = Physics.Constants.GravityForce;

                this.Body.ClearAccumulators();

                this.Body.CanSleep = true;
                this.Body.IsAwake = true;

                this.Body.CalculateDerivedData();
            }
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