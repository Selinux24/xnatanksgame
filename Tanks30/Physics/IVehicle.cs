using Microsoft.Xna.Framework;

namespace Physics
{
    public delegate void VehicleStateHandler(IVehicle vehicle);

    public delegate void VehicleMovingHandler(IVehicle vehicle, Vector3 velocity, Vector3 acceleration);

    /// <summary>
    /// Vehículo
    /// </summary>
    public interface IVehicle : IPhysicObject
    {
        /// <summary>
        /// Evento que se produce cuando el vehículo recibe daños
        /// </summary>
        event VehicleStateHandler TakingDamage;
        /// <summary>
        /// Evento que se produce cuando se daña ligeramente el vehículo
        /// </summary>
        event VehicleStateHandler SlightlyDamaged;
        /// <summary>
        /// Evento que se produce cuando se daña el vehículo
        /// </summary>
        event VehicleStateHandler Damaged;
        /// <summary>
        /// Evento que se produce cuando se daña severamente el vehículo
        /// </summary>
        event VehicleStateHandler HeavyDamaged;
        /// <summary>
        /// Evento que se produce cuando el vehículo es destruído
        /// </summary>
        event VehicleStateHandler Destroyed;
        /// <summary>
        /// Evento que se produce cuando el cuerpo empieza a moverse
        /// </summary>
        event VehicleMovingHandler StartMoving;
        /// <summary>
        /// Evento que se produce cuando el cuerpo acelera
        /// </summary>
        event VehicleMovingHandler Accelerating;
        /// <summary>
        /// Evento que se produce cuando el cuerpo decelera
        /// </summary>
        event VehicleMovingHandler Braking;
        /// <summary>
        /// Evento que se produce cuando el cuerpo deja de moverse
        /// </summary>
        event VehicleMovingHandler StopMoving;
    }
}
