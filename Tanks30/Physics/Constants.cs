using System;
using Microsoft.Xna.Framework;

namespace Physics
{
    public abstract class Constants
    {
        /// <summary>
        /// Fuerza de la gravedad
        /// </summary>
        public static Vector3 GravityForce = new Vector3(0f, -20f, 0f);
        /// <summary>
        /// Fuerza de gravedad para simular proyectiles rápidos
        /// </summary>
        public static Vector3 FastProyectileGravityForce
        {
            get
            {
                return GravityForce * 0.05f;
            }
        }
        /// <summary>
        /// Fuerza de la gravedad para simular partículas sin masa
        /// </summary>
        public static Vector3 ZeroMassGravityForce
        {
            get
            {
                return Vector3.Zero;
            }
        }
    }
}
