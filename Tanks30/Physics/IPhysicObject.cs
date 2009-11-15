using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Physics
{
    public interface IPhysicObject
    {
        OrientedBoundingBox TransformedOBB { get;}
        BoundingSphere TransformedBSph { get;}
        bool IsStatic { get;}

        void Reaction(IPhysicObject other);

        void Update(GameTime gameTime);
    }
}
