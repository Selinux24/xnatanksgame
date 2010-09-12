
namespace Physics
{
    public delegate void VehicleStateHandler(IVehicle vehicle);

    /// <summary>
    /// Vehículo
    /// </summary>
    public interface IVehicle : IPhysicObject
    {
        /// <summary>
        /// Actualiza la altura sobre el terreno actual del vehículo
        /// </summary>
        /// <param name="height">Nueva altura</param>
        void UpdateHeight(float? height);

        /// <summary>
        /// Evento que se produce cuando se daña ligeramente el vehículo
        /// </summary>
        event VehicleStateHandler OnVehicleSlightlyDamaged;
        /// <summary>
        /// Evento que se produce cuando se daña el vehículo
        /// </summary>
        event VehicleStateHandler OnVehicleDamaged;
        /// <summary>
        /// Evento que se produce cuando se daña severamente el vehículo
        /// </summary>
        event VehicleStateHandler OnVehicleHeavyDamaged;
        /// <summary>
        /// Evento que se produce cuando el vehículo es destruído
        /// </summary>
        event VehicleStateHandler OnVehicleDestroyed;
    }
}
