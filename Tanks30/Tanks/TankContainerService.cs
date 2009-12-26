using System.Collections.Generic;
using GameComponents.Vehicles;
using Microsoft.Xna.Framework;
using Tanks.Vehicles;

namespace Tanks.Services
{
    public class TankContainerService : GameComponent
    {
        private bool updateList = false;
        private TankGameComponent[] m_Tanks;

        public TankGameComponent[] Tanks
        {
            get
            {
                if ((m_Tanks == null) || (updateList))
                {
                    List<TankGameComponent> list = new List<TankGameComponent>();

                    foreach (object obj in this.Game.Components)
                    {
                        TankGameComponent tank = obj as TankGameComponent;
                        if (tank != null)
                        {
                            list.Add(tank);
                        }
                    }

                    m_Tanks = list.ToArray();
                }

                return m_Tanks;
            }
        }

        public TankContainerService(Game game)
            : base(game)
        {

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public Rhino AddRhino(Point where)
        {
            Rhino newRhino = new Rhino(this.Game)
            {
                UpdateOrder = this.UpdateOrder,
            };

            this.Game.Components.Add(newRhino);

            updateList = true;

            newRhino.Position = new Vector3(where.X, 0f, where.Y);

            return newRhino;
        }

        public LandRaider AddLandRaider(Point where)
        {
            LandRaider newLandRaider = new LandRaider(this.Game)
            {
                UpdateOrder = this.UpdateOrder,
            };

            this.Game.Components.Add(newLandRaider);

            updateList = true;

            newLandRaider.Position = new Vector3(where.X, 0f, where.Y);

            return newLandRaider;
        }

        public LandSpeeder AddLandSpeeder(Point where)
        {
            LandSpeeder newLandSpeeder = new LandSpeeder(this.Game)
            {
                UpdateOrder = this.UpdateOrder,
            };

            this.Game.Components.Add(newLandSpeeder);

            updateList = true;

            newLandSpeeder.Position = new Vector3(where.X, 0f, where.Y);

            return newLandSpeeder;
        }

        public LemanRuss AddLemanRuss(Point where)
        {
            LemanRuss newLemanRuss = new LemanRuss(this.Game)
            {
                UpdateOrder = this.UpdateOrder,
            };

            this.Game.Components.Add(newLemanRuss);

            updateList = true;

            newLemanRuss.Position = new Vector3(where.X, 0f, where.Y);

            return newLemanRuss;
        }

        public void RemoveTank(TankGameComponent tank)
        {
            if (tank != null)
            {
                if (this.Game.Components.Contains(tank))
                {
                    this.Game.Components.Remove(tank);

                    updateList = true;
                }
            }
        }

        public void ClearTanks()
        {
            foreach (TankGameComponent tank in this.Tanks)
            {
                this.Game.Components.Remove(tank);
            }

            updateList = true;
        }
    }
}
