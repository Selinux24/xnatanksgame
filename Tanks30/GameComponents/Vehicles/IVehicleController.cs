using Microsoft.Xna.Framework;

namespace GameComponents.Vehicles
{
    /// <summary>
    /// Controlador de vehículo
    /// </summary>
    public interface IVehicleController
    {
        /// <summary>
        /// Posición
        /// </summary>
        Vector3 Position { get; set;}
        /// <summary>
        /// Rotación
        /// </summary>
        Quaternion Orientation { get; set;}
        /// <summary>
        /// Escala
        /// </summary>
        float Scale { get; set;}

        /// <summary>
        /// Transformación actual
        /// </summary>
        Matrix CurrentTransform { get;}

        /// <summary>
        /// Velocidad
        /// </summary>
        float Velocity { get; }
        /// <summary>
        /// Dirección
        /// </summary>
        Vector3 Direction { get;}
        /// <summary>
        /// Sentido
        /// </summary>
        MovingDirections MovingDirection { get;}
        /// <summary>
        /// Indica si el vehículo está avanzando
        /// </summary>
        bool IsAdvancing { get; }
        /// <summary>
        /// Indica si el vehículo está estático
        /// </summary>
        bool IsStatic { get; }

        /// <summary>
        /// Acelerar
        /// </summary>
        void Accelerate();
        /// <summary>
        /// Frenar
        /// </summary>
        void Brake();
        /// <summary>
        /// Acelerar un cantidad determinada
        /// </summary>
        /// <param name="amount">Cantidad</param>
        void Accelerate(float amount);
        /// <summary>
        /// Frenar una cantidad determinada
        /// </summary>
        /// <param name="amount">Cantidad</param>
        void Brake(float amount);
        /// <summary>
        /// Cambiar de sentido
        /// </summary>
        void ChangeDirection();
        /// <summary>
        /// Girar a la izquierda
        /// </summary>
        void TurnLeft();
        /// <summary>
        /// Girar a la derecha
        /// </summary>
        void TurnRight();
        /// <summary>
        /// Girar a la izquierda un ángulo determinado
        /// </summary>
        /// <param name="angle">Angulo</param>
        void TurnLeft(float angle);
        /// <summary>
        /// Girar a la derecha un ángulo determinado
        /// </summary>
        /// <param name="angle">Angulo</param>
        void TurnRight(float angle);
    }
}
