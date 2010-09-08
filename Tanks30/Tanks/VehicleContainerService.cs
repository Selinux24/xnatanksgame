using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Tanks.Services
{
    using GameComponents.Vehicles;
    using Vehicles;

    public class VehicleContainerService : GameComponent
    {
        /// <summary>
        /// 
        /// </summary>
        private bool updateList = false;
        /// <summary>
        /// Lista de vehículos
        /// </summary>
        private Vehicle[] m_Vehicles;
        /// <summary>
        /// Obtiene la lista de vehículos
        /// </summary>
        public Vehicle[] Vehicles
        {
            get
            {
                if ((m_Vehicles == null) || (updateList))
                {
                    // La lista está vacía o ha sido modificada, obtener los vehículos de la lista de componentes
                    List<Vehicle> list = new List<Vehicle>();

                    foreach (object obj in this.Game.Components)
                    {
                        Vehicle vehicle = obj as Vehicle;
                        if (vehicle != null)
                        {
                            list.Add(vehicle);
                        }
                    }

                    m_Vehicles = list.ToArray();
                }

                return m_Vehicles;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="game">Juego</param>
        public VehicleContainerService(Game game)
            : base(game)
        {

        }

        /// <summary>
        /// Añade un vehículo del tipo especificado
        /// </summary>
        /// <param name="type">Tipo</param>
        /// <param name="where">Posición inicial</param>
        /// <returns>Devuelve el vehículo añadido</returns>
        public Vehicle AddVehicle(VehicleTypes type, Point where)
        {
            Vehicle newVehicle = null;

            if (type == VehicleTypes.Rhino)
            {
                newVehicle = new Rhino(this.Game, "Content");
            }
            else if (type == VehicleTypes.LandRaider)
            {
                newVehicle = new LandRaider(this.Game, "Content");
            }
            else if (type == VehicleTypes.LandSpeeder)
            {
                newVehicle = new LandSpeeder(this.Game, "Content");
            }
            else if (type == VehicleTypes.LemanRuss)
            {
                newVehicle = new LemanRuss(this.Game, "Content");
            }

            if (newVehicle != null)
            {
                newVehicle.UpdateOrder = this.UpdateOrder;

                this.Game.Components.Add(newVehicle);

                updateList = true;

                newVehicle.Position = new Vector3(where.X, 0f, where.Y);
            }

            return newVehicle;
        }
        /// <summary>
        /// Añade un Rhino
        /// </summary>
        /// <param name="where">Posición</param>
        /// <returns>Devuelve el Rhino añadido</returns>
        public Rhino AddRhino(Point where)
        {
            return AddVehicle(VehicleTypes.Rhino, where) as Rhino;
        }
        /// <summary>
        /// Añade un Land Raider
        /// </summary>
        /// <param name="where">Posición</param>
        /// <returns>Devuelve el Land Raider añadido</returns>
        public LandRaider AddLandRaider(Point where)
        {
            return AddVehicle(VehicleTypes.LandRaider, where) as LandRaider;
        }
        /// <summary>
        /// Añade un Land Speeder
        /// </summary>
        /// <param name="where">Posición</param>
        /// <returns>Devuelve el Land Speeder añadido</returns>
        public LandSpeeder AddLandSpeeder(Point where)
        {
            return AddVehicle(VehicleTypes.LandSpeeder, where) as LandSpeeder;
        }
        /// <summary>
        /// Añade un Leman Russ
        /// </summary>
        /// <param name="where">Posición</param>
        /// <returns>Devuelve el Leman Russ añadido</returns>
        public LemanRuss AddLemanRuss(Point where)
        {
            return AddVehicle(VehicleTypes.LemanRuss, where) as LemanRuss;
        }

        /// <summary>
        /// Obtiene el índice del vehículo especificado
        /// </summary>
        /// <param name="vehicle">Vehículo</param>
        /// <returns>Devuelve el índice que ocupa el vehículo especificado en la colección</returns>
        public int IndexOf(Vehicle vehicle)
        {
            if (this.Vehicles != null)
            {
                for (int i = 0; i < this.Vehicles.Length; i++)
                {
                    if (this.Vehicles[i] == vehicle)
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        /// <summary>
        /// Eliminar el vehículo especificado
        /// </summary>
        /// <param name="vehicle">Vehículo</param>
        public void RemoveVehicle(Vehicle vehicle)
        {
            if (vehicle != null)
            {
                if (this.Game.Components.Contains(vehicle))
                {
                    this.Game.Components.Remove(vehicle);

                    updateList = true;
                }
            }
        }
        /// <summary>
        /// Limpia la lista de vehículos
        /// </summary>
        public void Clear()
        {
            Vehicle[] vehicleList = this.Vehicles;

            foreach (Vehicle vehicle in vehicleList)
            {
                this.Game.Components.Remove(vehicle);
            }

            updateList = true;
        }
    }
}
