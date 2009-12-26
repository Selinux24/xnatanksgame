using System;
using Microsoft.Xna.Framework;

namespace GameComponents.Vehicles.Animation
{
    [Serializable]
    public class PlayerPositionInfo
    {
        public string Name = null;
        public string BoneName = null;
        public Vector3 Translation = Vector3.Zero;

        public PlayerPositionInfo()
        {

        }
    }
}
