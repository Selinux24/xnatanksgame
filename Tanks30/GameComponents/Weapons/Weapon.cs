using System.Collections.Generic;
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
        /// <summary>
        /// Daño del arma
        /// </summary>
        public float Damage;
        /// <summary>
        /// Penetración del blindaje
        /// </summary>
        public float Penetration;

        /// <summary>
        /// Crea una lista de armas a partir de la definición XML
        /// </summary>
        /// <param name="weaponInfo">Lista de armas</param>
        /// <returns>Devuelve una colección de armas</returns>
        public static Weapon[] CreateWeaponList(WeaponInfo[] weaponInfo)
        {
            List<Weapon> list = new List<Weapon>();

            if (weaponInfo != null && weaponInfo.Length > 0)
            {
                foreach (WeaponInfo wInfo in weaponInfo)
                {
                    Weapon newWeapon = new Weapon()
                    {
                        Mass = wInfo.Mass,
                        AppliedGravity = wInfo.AppliedGravity,
                        Damage = wInfo.Damage,
                        Penetration = wInfo.Penetration,
                        GenerateExplosion = wInfo.GenerateExplosion,
                        Name = wInfo.Name,
                        Radius = wInfo.Radius,
                        Range = wInfo.Range,
                        Velocity = wInfo.Velocity,
                    };

                    list.Add(newWeapon);
                }
            }

            return list.ToArray();
        }
    }
}
