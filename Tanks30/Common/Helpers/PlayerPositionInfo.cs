using System;
using Microsoft.Xna.Framework;

namespace Common.Helpers
{
    /// <summary>
    /// Posición de jugador
    /// </summary>
    [Serializable]
    public class PlayerPositionInfo
    {
        /// <summary>
        /// Nombre de la posición
        /// </summary>
        public string Name = null;
        /// <summary>
        /// Nombre del nodo
        /// </summary>
        public string BoneName = null;
        /// <summary>
        /// Posición relativa al nodo
        /// </summary>
        public Vector3 Translation = Vector3.Zero;

        /// <summary>
        /// Constructor
        /// </summary>
        public PlayerPositionInfo()
        {

        }
    }
}
