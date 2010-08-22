
namespace Physics
{
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
    }
}
