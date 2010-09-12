﻿using System;
using Microsoft.Xna.Framework;

namespace GameComponents.Vehicles
{
    using GameComponents.Weapons;
    using Physics;

    /// <summary>
    /// Estado del vehículo
    /// </summary>
    public partial class Vehicle
    {
        /// <summary>
        /// Integridad original
        /// </summary>
        public float BaseHull = 0;
        /// <summary>
        /// Blindaje original
        /// </summary>
        public float BaseArmor = 0;
        /// <summary>
        /// Valor de integridad
        /// </summary>
        public float Hull = 0;
        /// <summary>
        /// Valor de blindaje
        /// </summary>
        public float Armor = 0;
        /// <summary>
        /// Indica si el vehículo está ligeramente dañado
        /// </summary>
        public bool SlightlyDamaged
        {
            get
            {
                return this.Hull < this.BaseHull;
            }
        }
        /// <summary>
        /// Indica si el vehículo está dañado
        /// </summary>
        public bool Damaged
        {
            get
            {
                return this.Hull < (this.BaseHull * 0.50f);
            }
        }
        /// <summary>
        /// Indica si el vehículo está fuertemente dañado
        /// </summary>
        public bool HeavyDamaged
        {
            get
            {
                return this.Hull < (this.BaseHull * 0.25f);
            }
        }
        /// <summary>
        /// Indica si el vehículo está destruido
        /// </summary>
        public bool Destroyed
        {
            get
            {
                if (this.Hull <= 0)
                {
                    return true;
                }

                return false;
            }
            set
            {
                if (value == true)
                {
                    this.Hull = 0;
                    this.Armor = 0;
                }
                else
                {
                    this.Hull = this.BaseHull;
                    this.Armor = this.BaseArmor;
                }
            }
        }

        /// <summary>
        /// Arma seleccionada
        /// </summary>
        private Weapon m_CurrentWeapon = null;
        /// <summary>
        /// Lista de armas
        /// </summary>
        private WeaponList m_WeapontList = new WeaponList();

        /// <summary>
        /// Evento que se produce cuando se daña ligeramente el vehículo
        /// </summary>
        public event VehicleDamagedHandler OnVehicleSlightlyDamaged;
        /// <summary>
        /// Evento que se produce cuando se daña el vehículo
        /// </summary>
        public event VehicleDamagedHandler OnVehicleDamaged;
        /// <summary>
        /// Evento que se produce cuando se daña severamente el vehículo
        /// </summary>
        public event VehicleDamagedHandler OnVehicleHeavyDamaged;
        /// <summary>
        /// Evento que se produce cuando el vehículo es destruído
        /// </summary>
        public event VehicleDamagedHandler OnVehicleDestroyed;

        /// <summary>
        /// Disparador del evento de vehículo ligeramente dañado
        /// </summary>
        protected virtual void FireOnVehicleSlightlyDamaged()
        {
            if (this.OnVehicleSlightlyDamaged != null)
            {
                this.OnVehicleSlightlyDamaged(this);
            }
        }
        /// <summary>
        /// Disparador del evento de vehículo dañado
        /// </summary>
        protected virtual void FireOnVehicleDamaged()
        {
            if (this.OnVehicleDamaged != null)
            {
                this.OnVehicleDamaged(this);
            }
        }
        /// <summary>
        /// Disparador del evento de vehículo fuertemente dañado
        /// </summary>
        protected virtual void FireOnVehicleHeavyDamaged()
        {
            if (this.OnVehicleHeavyDamaged != null)
            {
                this.OnVehicleHeavyDamaged(this);
            }
        }
        /// <summary>
        /// Disparador del evento de vehículo destruído
        /// </summary>
        protected virtual void FireOnVehicleDestroyed()
        {
            if (this.OnVehicleDestroyed != null)
            {
                this.OnVehicleDestroyed(this);
            }
        }

        /// <summary>
        /// Recibir daño
        /// </summary>
        /// <param name="damage">Daño</param>
        /// <param name="penetration">Penetración</param>
        public virtual void TakeDamage(float damage, float penetration)
        {
            if (this.Hull > 0f)
            {
                if (this.Armor > 0f)
                {
                    if (penetration > this.Armor)
                    {
                        //Impacto interno
                        this.Hull -= damage;
                        this.Armor -= penetration * 0.25f;
                    }
                    else
                    {
                        //Impacto externo
                        this.Hull -= damage * 0.5f;
                        this.Armor -= penetration * 0.05f;
                    }

                    if (this.Armor < 0f)
                    {
                        //Como mínimo blindaje 0
                        this.Armor = 0f;
                    }

                    if (this.Hull < 0f)
                    {
                        //Como mínimo integridad 0
                        this.Hull = 0f;
                    }
                }
                else
                {
                    //Sin blindaje cualquier impacto destruye el vehículo
                    this.Hull = 0f;
                }

                if (this.Destroyed)
                {
                    this.FireOnVehicleDestroyed();
                }
                else if (this.HeavyDamaged)
                {
                    this.FireOnVehicleHeavyDamaged();
                }
                else if (this.Damaged)
                {
                    this.FireOnVehicleDamaged();
                }
                else if (this.SlightlyDamaged)
                {
                    this.FireOnVehicleSlightlyDamaged();
                }
            }
        }
        /// <summary>
        /// Recibir daño
        /// </summary>
        /// <param name="round">Munición</param>
        public virtual void TakeDamage(AmmoRound round)
        {
            if (round != null)
            {
                float penetration = round.Penetration;
                float damage = round.Damage;

                this.TakeDamage(damage, penetration);
            }
        }

        /// <summary>
        /// Obtiene una posición de jugador por nombre
        /// </summary>
        /// <param name="name">Nombre de la posición del jugador</param>
        /// <returns>Devuelve la posición del jugador</returns>
        public Weapon GetWeapon(string name)
        {
            foreach (Weapon weapon in m_WeapontList)
            {
                if (string.Compare(weapon.Name, name, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return weapon;
                }
            }

            return null;
        }
        /// <summary>
        /// Selecciona el arma especificada
        /// </summary>
        /// <param name="weapon"></param>
        public void SelectWeapon(Weapon weapon)
        {
            this.m_CurrentWeapon = weapon;
        }
        /// <summary>
        /// Dispara
        /// </summary>
        public void Fire()
        {
            //Obtener el arma actual
            if (this.m_CurrentWeapon != null)
            {
                // Calcular la transformación global compuesta por la transformación adicional, la transformación del bone y la transformación del modelo
                Matrix transform = this.m_CurrentWeapon.GetModelMatrix(this.m_AnimationController, this.CurrentTransform);

                Vector3 direction = transform.Forward;
                Vector3 position = transform.Translation + (direction);

                this.Fire(position, direction);
            }
            else
            {
                Vector3 direction = this.CurrentPlayerControlTransform.Forward;
                Vector3 position = this.CurrentPlayerControlTransform.Translation + (direction * 3f);

                this.Fire(position, direction);
            }
        }
        /// <summary>
        /// Dispara
        /// </summary>
        /// <param name="position">Posición</param>
        /// <param name="direction">Dirección</param>
        public void Fire(Vector3 position, Vector3 direction)
        {
            if (this.m_CurrentWeapon != null)
            {
                this.PhysicsController.Fire(
                    this.m_CurrentWeapon.Mass,
                    this.m_CurrentWeapon.Range,
                    this.m_CurrentWeapon.Damage,
                    this.m_CurrentWeapon.Penetration,
                    position,
                    Vector3.Normalize(direction) * this.m_CurrentWeapon.Velocity,
                    this.m_CurrentWeapon.AppliedGravity,
                    this.m_CurrentWeapon.Radius,
                    this.m_CurrentWeapon.GenerateExplosion);
            }
        }
    }
}
