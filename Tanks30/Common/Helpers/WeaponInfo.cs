﻿using System;
using Microsoft.Xna.Framework;

namespace Common.Helpers
{
    [Serializable]
    public class WeaponInfo
    {
        /// <summary>
        /// Nombre del arma
        /// </summary>
        public string Name;
        /// <summary>
        /// Nombre del nodo desde el que salen los proyectiles
        /// </summary>
        public string BoneName;
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
    }
}
