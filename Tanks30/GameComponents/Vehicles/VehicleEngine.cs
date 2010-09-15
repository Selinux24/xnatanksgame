
namespace GameComponents.Vehicles
{
    /// <summary>
    /// Motor
    /// </summary>
    public class VehicleEngine
    {
        /// <summary>
        /// Indica si el motor está activo
        /// </summary>
        public bool Active = false;
        /// <summary>
        /// Estado del motor
        /// </summary>
        public float Status = 1.0f;
        /// <summary>
        /// Dirección de movimiento del tanque
        /// </summary>
        public MovingDirections MovingDirection = MovingDirections.Forward;

        /// <summary>
        /// Altura mínima base
        /// </summary>
        public float InitialMinFlightHeight = 0f;
        /// <summary>
        /// Altura máxima base
        /// </summary>
        public float InitialMaxFlightHeight = 0f;

        /// <summary>
        /// Velocidad máxima que puede alcanzar el tanque hacia delante
        /// </summary> 
        public float MaxForwardVelocity = 0f;
        /// <summary>
        /// Velocidad máxima que puede alcanzar el tanque marcha atrás
        /// </summary> 
        public float MaxBackwardVelocity = 0f;
        /// <summary>
        /// Modificador de aceleración
        /// </summary>
        public float AccelerationModifier = 0f;
        /// <summary>
        /// Modificador de frenado
        /// </summary>
        public float BrakeModifier = 0f;
        /// <summary>
        /// Velocidad angular
        /// </summary>
        public float AngularVelocityModifier = 0f;
        /// <summary>
        /// Vehículo volador
        /// </summary>
        public bool Skimmer = false;
        /// <summary>
        /// Altura de vuelo máxima
        /// </summary>
        public float MaxFlightHeight = 0f;
        /// <summary>
        /// Altura de vuelo mínima
        /// </summary>
        public float MinFlightHeight = 0f;
        /// <summary>
        /// Angulo de inclinación del morro en el ascenso
        /// </summary>
        public float AscendingAngle = 0f;
        /// <summary>
        /// Angulo de inclinación del morro en el descenso
        /// </summary>
        public float DescendingAngle = 0f;
    }
}
