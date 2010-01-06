
namespace GameComponents.Vehicles
{
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
        /// Destruye el vehículo
        /// </summary>
        protected virtual void Destroy()
        {
            //this.m_Velocity = 0f;
        }
    }
}
