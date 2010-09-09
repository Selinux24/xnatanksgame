using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameComponents.Weapons
{
    using GameComponents.Animation;

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
        /// Nombre del nodo desde el que salen los proyectiles
        /// </summary>
        public ModelBone Bone;
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
        /// <param name="model">Modelo</param>
        /// <param name="weaponInfo">Lista de armas</param>
        /// <returns>Devuelve una colección de armas</returns>
        public static Weapon[] CreateWeaponList(Model model, WeaponInfo[] weaponInfo)
        {
            List<Weapon> list = new List<Weapon>();

            if (weaponInfo != null && weaponInfo.Length > 0)
            {
                foreach (WeaponInfo wInfo in weaponInfo)
                {
                    Weapon newWeapon = new Weapon()
                    {
                        Name = wInfo.Name,
                        Bone = (string.IsNullOrEmpty(wInfo.BoneName)) ? null : model.Bones[wInfo.BoneName],
                        Mass = wInfo.Mass,
                        Range = wInfo.Range,
                        Velocity = wInfo.Velocity,
                        AppliedGravity = wInfo.AppliedGravity,
                        Radius = wInfo.Radius,
                        GenerateExplosion = wInfo.GenerateExplosion,
                        Damage = wInfo.Damage,
                        Penetration = wInfo.Penetration,
                    };

                    list.Add(newWeapon);
                }
            }

            return list.ToArray();
        }

        /// <summary>
        /// Obtiene la transformación absoluta de la posición
        /// </summary>
        /// <param name="controller">Controlador de animación</param>
        /// <param name="modelTransform">Transformación</param>
        /// <returns>Devuelve la transformación absoluta del modelo</returns>
        public Matrix GetModelMatrix(AnimationController controller, Matrix modelTransform)
        {
            // Calcular la transformación global compuesta por la transformación adicional, la transformación del bone y la transformación del modelo
            if (this.Bone != null)
            {
                return controller.GetAbsoluteTransform(this.Bone) * modelTransform;
            }
            else
            {
                return modelTransform;
            }
        }
    }
}
