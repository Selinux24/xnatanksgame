using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Tanks
{
    using Buildings;
    using GameComponents.Buildings;
    
    public class BuildingContainerService : GameComponent
    {
        /// <summary>
        /// 
        /// </summary>
        private bool updateList = false;
        /// <summary>
        /// Lista de edificios
        /// </summary>
        private Building[] m_Buildings;
        /// <summary>
        /// Obtiene la lista de edificios
        /// </summary>
        public Building[] Buildings
        {
            get
            {
                if ((m_Buildings == null) || (updateList))
                {
                    // La lista está vacía o ha sido modificada, obtener los vehículos de la lista de componentes
                    List<Building> list = new List<Building>();

                    foreach (object obj in this.Game.Components)
                    {
                        Building vehicle = obj as Building;
                        if (vehicle != null)
                        {
                            list.Add(vehicle);
                        }
                    }

                    m_Buildings = list.ToArray();
                }

                return m_Buildings;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game">Juego</param>
        public BuildingContainerService(Game game)
            : base(game)
        {

        }

        /// <summary>
        /// Añade un edificio del tipo especificado
        /// </summary>
        /// <param name="type">Tipo</param>
        /// <param name="where">Posición inicial</param>
        /// <returns>Devuelve el edificio añadido</returns>
        public Building AddBuilding(BuildingTypes type, Point where)
        {
            Building newBuilding = null;

            if (type == BuildingTypes.Type0)
            {
                newBuilding = new BuildingType0(this.Game, @"Content/Buildings/", @"WHBuilding01.xml");
            }

            if (newBuilding != null)
            {
                newBuilding.UpdateOrder = this.UpdateOrder;

                this.Game.Components.Add(newBuilding);

                updateList = true;

                newBuilding.Position = new Vector3(where.X, 0f, where.Y);
            }

            return newBuilding;
        }
        /// <summary>
        /// Añade un edificio de tipo 0
        /// </summary>
        /// <param name="where">Posición</param>
        /// <returns>Devuelve el edificio añadido</returns>
        public BuildingType0 AddType0(Point where)
        {
            return AddBuilding(BuildingTypes.Type0, where) as BuildingType0;
        }

        /// <summary>
        /// Obtiene el índice del edificio especificado
        /// </summary>
        /// <param name="building">Edificio</param>
        /// <returns>Devuelve el índice que ocupa el edificio especificado en la colección</returns>
        public int IndexOf(Building building)
        {
            if (this.Buildings != null)
            {
                for (int i = 0; i < this.Buildings.Length; i++)
                {
                    if (this.Buildings[i] == building)
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        /// <summary>
        /// Eliminar el edificio especificado
        /// </summary>
        /// <param name="building">Edificio</param>
        public void RemoveBuilding(Building building)
        {
            if (building != null)
            {
                if (this.Game.Components.Contains(building))
                {
                    this.Game.Components.Remove(building);

                    updateList = true;
                }
            }
        }
        /// <summary>
        /// Limpia la lista de edificios
        /// </summary>
        public void Clear()
        {
            Building[] buildingList = this.Buildings;

            foreach (Building building in buildingList)
            {
                this.Game.Components.Remove(building);
            }

            updateList = true;
        }
    }
}
