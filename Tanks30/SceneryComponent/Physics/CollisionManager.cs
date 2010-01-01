using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Physics;

namespace GameComponents.Physics
{
    public class CollisionManager : GameComponent
    {
        public CollisionManager(Game game)
            : base(game)
        {

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            List<IPhysicObject> list = new List<IPhysicObject>();

            foreach (GameComponent gameComponent in this.Game.Components)
            {
                IPhysicObject obj = gameComponent as IPhysicObject;
                if (obj != null)
                {
                    list.Add(obj);
                }
            }

            for (int a = 0; a < list.Count - 1; a++)
            {
                IPhysicObject objA = list[a];

                for (int b = a + 1; b < list.Count; b++)
                {
                    IPhysicObject objB = list[b];

                    if ((!objA.IsStatic) || (!objB.IsStatic))
                    {
                        CollisionManager.TestCollision(objA, objB);
                    }
                }
            }
        }

        public static void TestCollision(IPhysicObject obj1, IPhysicObject obj2)
        {
            if ((obj1 != null) && (obj2 != null))
            {
                //if (obj1.BSph.Intersects(obj2.BSph))
                {
                    if (obj1.TransformedOBB.Intersects(obj2.TransformedOBB))
                    {
                        obj1.Reaction(obj2);
                        obj2.Reaction(obj1);
                    }
                }
            }
        }
    }
}
