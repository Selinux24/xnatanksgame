using Microsoft.Xna.Framework;

namespace GameComponents.Vehicles
{
    /// <summary>
    /// Controlador de veh�culo
    /// </summary>
    public interface IVehicleController
    {
        /// <summary>
        /// Posici�n
        /// </summary>
        Vector3 Position { get; set;}
        /// <summary>
        /// Rotaci�n
        /// </summary>
        Quaternion Orientation { get; set;}
        /// <summary>
        /// Escala
        /// </summary>
        float Scale { get; set;}

        /// <summary>
        /// Transformaci�n actual
        /// </summary>
        Matrix CurrentTransform { get;}

        /// <summary>
        /// Velocidad
        /// </summary>
        float Velocity { get; }
        /// <summary>
        /// Direcci�n
        /// </summary>
        Vector3 Direction { get;}
        /// <summary>
        /// Sentido
        /// </summary>
        MovingDirections MovingDirection { get;}
        /// <summary>
        /// Indica si el veh�culo est� avanzando
        /// </summary>
        bool IsAdvancing { get; }
        /// <summary>
        /// Indica si el veh�culo est� est�tico
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
        /// Girar a la izquierda un �ngulo determinado
        /// </summary>
        /// <param name="angle">Angulo</param>
        void TurnLeft(float angle);
        /// <summary>
        /// Girar a la derecha un �ngulo determinado
        /// </summary>
        /// <param name="angle">Angulo</param>
        void TurnRight(float angle);
    }
}
