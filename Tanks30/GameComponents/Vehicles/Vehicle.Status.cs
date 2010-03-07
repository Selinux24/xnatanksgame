using Microsoft.Xna.Framework;

namespace GameComponents.Vehicles
{
    using GameComponents.Weapons;

    /// <summary>
    /// Estado del vehículo
    /// </summary>
    public partial class Vehicle
    {
        /// <summary>
        /// Valor de integridad
        /// </summary>
        public int Hull = 100;
        /// <summary>
        /// Valor de blindaje
        /// </summary>
        public int Armor = 100;
        /// <summary>
        /// Indica si el vehículo está dañado
        /// </summary>
        public bool Damaged
        {
            get
            {
                return this.Hull < 100;
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

                    this.Destroy();
                }
                else
                {
                    this.Hull = 100;
                    this.Armor = 100;
                }
            }
        }

        /// <summary>
        /// Arma seleccionada
        /// </summary>
        private Weapon m_CurrentWeapon = null;

        /// <summary>
        /// Destruye el vehículo
        /// </summary>
        protected virtual void Destroy()
        {
            //this.m_Velocity = 0f;
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
            if (this.m_CurrentWeapon != null)
            {
                Vector3 direction = this.CurrentPlayerControlTransform.Forward;
                Vector3 position = this.CurrentPlayerControlTransform.Translation + (direction * 3f);

                this.PhysicsController.Fire(
                    this.m_CurrentWeapon.Mass,
                    this.m_CurrentWeapon.Range,
                    position,
                    Vector3.Normalize(direction) * this.m_CurrentWeapon.Velocity,
                    this.m_CurrentWeapon.AppliedGravity,
                    this.m_CurrentWeapon.Radius,
                    this.m_CurrentWeapon.GenerateExplosion);
            }
        }
    }
}
