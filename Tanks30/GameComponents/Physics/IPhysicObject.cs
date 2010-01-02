using Microsoft.Xna.Framework;
using Physics;

namespace GameComponents.Physics
{
    /// <summary>
    /// Objeto con físicas
    /// </summary>
    public interface IPhysicObject
    {
        /// <summary>
        /// OBB que contiene al objeto
        /// </summary>
        OrientedBoundingBox TransformedOBB { get;}
        /// <summary>
        /// Esfera que contiene al objeto
        /// </summary>
        BoundingSphere TransformedBSph { get;}
        /// <summary>
        /// Indica si el objeto está estático y no deben comprobrarse físicas
        /// </summary>
        bool IsStatic { get;}

        /// <summary>
        /// Racción a la colisión
        /// </summary>
        /// <param name="other">Otro objeto</param>
        void Reaction(IPhysicObject other);
        /// <summary>
        /// Actualiza el objeto
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
        void Update(GameTime gameTime);
    }
}
