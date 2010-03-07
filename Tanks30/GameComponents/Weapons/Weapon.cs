using Microsoft.Xna.Framework;

namespace GameComponents.Weapons
{
    /// <summary>
    /// Arma
    /// </summary>
    public class Weapon
    {
        /// <summary>
        /// Nombre del arma
        /// </summary>
        public string Name;
        /// <summary>
        /// Masa del proyectil
        /// </summary>
        public float Mass;
        /// <summary>
        /// Rango
        /// </summary>
        public float Range;
        /// <summary>
        /// Velocidad
        /// </summary>
        public float Velocity;
        /// <summary>
        /// Gravedad a aplicar
        /// </summary>
        public Vector3 AppliedGravity;
        /// <summary>
        /// Radio del proyectil
        /// </summary>
        public float Radius;
        /// <summary>
        /// Indica si el impacto genera explosión
        /// </summary>
        public bool GenerateExplosion;
    }
}
