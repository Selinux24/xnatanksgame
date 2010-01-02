using System;
using System.Collections.Generic;
using System.Text;

namespace Physics
{
    public abstract class ForceGenerator
    {
        /// <summary>
        /// Overload this in implementations of the interface to calculate and update the force applied to the given rigid body
        /// </summary>
        /// <param name="body">Cuerpo</param>
        /// <param name="duration">Cantidad de tiempo</param>
        public abstract void UpdateForce(ref RigidBody body, float duration);
    }
}
