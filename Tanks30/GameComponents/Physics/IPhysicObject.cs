using Microsoft.Xna.Framework;
using Physics;

namespace GameComponents.Physics
{
    /// <summary>
    /// Objeto con f�sicas
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
        /// Indica si el objeto est� est�tico y no deben comprobrarse f�sicas
        /// </summary>
        bool IsStatic { get;}

        /// <summary>
        /// Racci�n a la colisi�n
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
