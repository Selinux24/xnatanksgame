using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Physics
{
    public interface IVehicleController : IPhysicObject
    {
        Vector3 Position { get; set;}
        Quaternion Rotation { get; set;}
        float Scale { get; set;}

        Matrix CurrentTransform { get;}

        float Velocity { get; }
        Vector3 Direction { get;}
        MovingDirections MovingDirection { get;}
        bool IsAdvancing { get; }

        void Accelerate();
        void Brake();
        void Accelerate(float amount);
        void Brake(float amount);
        void ChangeDirection();
        void TurnLeft();
        void TurnRight();
        void TurnLeft(float angle);
        void TurnRight(float angle);
    }
}
