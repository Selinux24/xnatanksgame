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
        void Accelerate(GameTime gameTime);
        /// <summary>
        /// Frenar
        /// </summary>
        void Brake(GameTime gameTime);
        /// <summary>
        /// Acelerar un cantidad determinada
        /// </summary>
        /// <param name="amount">Cantidad</param>
        void Accelerate(GameTime gameTime, float amount);
        /// <summary>
        /// Frenar una cantidad determinada
        /// </summary>
        /// <param name="amount">Cantidad</param>
        void Brake(GameTime gameTime, float amount);
        /// <summary>
        /// Cambiar de sentido
        /// </summary>
        void ChangeDirection();
        /// <summary>
        /// Girar a la izquierda
        /// </summary>
        void TurnLeft(GameTime gameTime);
        /// <summary>
        /// Girar a la derecha
        /// </summary>
        void TurnRight(GameTime gameTime);
        /// <summary>
        /// Girar a la izquierda un ángulo determinado
        /// </summary>
        /// <param name="angle">Angulo</param>
        void TurnLeft(GameTime gameTime, float angle);
        /// <summary>
        /// Girar a la derecha un ángulo determinado
        /// </summary>
        /// <param name="angle">Angulo</param>
        void TurnRight(GameTime gameTime, float angle);
    }
}
