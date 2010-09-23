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
        /// Cambiar de sentido
        /// </summary>
        void ChangeDirection();
        /// <summary>
        /// Acelerar
        /// </summary>
        void Accelerate(GameTime gameTime);
        /// <summary>
        /// Frenar
        /// </summary>
        void Brake(GameTime gameTime);
        /// <summary>
        /// Girar a la izquierda
        /// </summary>
        void TurnLeft(GameTime gameTime);
        /// <summary>
        /// Girar a la derecha
        /// </summary>
        void TurnRight(GameTime gameTime);
    }
}
