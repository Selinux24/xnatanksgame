using System;
using Physics;
using Microsoft.Xna.Framework;

namespace BigBallisticDemo
{
    /// <summary>
    /// Una caja
    /// </summary>
    class Box : CollisionBox
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="halfSize">Longitud del centro hasta las caras en los tres ejes</param>
        public Box(Vector3 halfSize)
        {
            this.HalfSize = halfSize;

            if (this.Body != null)
            {
                this.Body.Mass = this.HalfSize.X * this.HalfSize.Y * this.HalfSize.Z * 8.0f;
                this.Body.SetDamping(1f, 0.8f);
            }
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="halfSize">Longitud del centro hasta las caras en los tres ejes</param>
        /// <param name="mass">Masa</param>
        public Box(Vector3 halfSize, float mass)
        {
            this.HalfSize = halfSize;

            if (this.Body != null)
            {
                this.Body.Mass = mass;
                this.Body.SetDamping(0.99f, 0.8f);
            }
        }

        /// <summary>
        /// Establece el estado inicial de la caja en la posición indicada
        /// </summary>
        /// <param name="position">Posición inicial</param>
        public void SetState(Vector3 position)
        {
            this.SetState(position, Quaternion.Identity);
        }
        /// <summary>
        /// Establece el estado inicial de la caja en la posición y orientación indicadas
        /// </summary>
        /// <param name="position">Posición inicial</param>
        /// <param name="orientation">Orientación inicial</param>
        public void SetState(Vector3 position, Quaternion orientation)
        {
            if (this.Body != null)
            {
                this.Body.Position = position;
                this.Body.Orientation = orientation;

                this.Body.Velocity = Vector3.Zero;
                this.Body.Rotation = Vector3.Zero;
                this.Body.Acceleration = Physics.Constants.GravityForce;
                this.Body.InertiaTensor = Core.SetInertiaTensorToBox(this.HalfSize, this.Body.Mass);

                this.Body.ClearAccumulators();

                this.Body.CanSleep = true;
                this.Body.IsAwake = true;

                this.Body.CalculateDerivedData();
            }

            this.CalculateInternals();
        }
        /// <summary>
        /// Establece la masa de la caja
        /// </summary>
        /// <param name="mass">Masa</param>
        public void SetMass(float mass)
        {
            this.Body.Mass = mass;
        }
    }
}