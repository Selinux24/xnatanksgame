using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GameComponents.Physics
{
    /// <summary>
    /// Gestor de colisión de componentes
    /// </summary>
    public class CollisionManager : GameComponent
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game">Juego</param>
        public CollisionManager(Game game)
            : base(game)
        {

        }

        /// <summary>
        /// Actualiza el gestor de colisión
        /// </summary>
        /// <param name="gameTime">Tiempo de juego</param>
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

        /// <summary>
        /// Realiza los tests de colisión
        /// </summary>
        /// <param name="obj1">Objeto 1</param>
        /// <param name="obj2">Objeto 2</param>
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
