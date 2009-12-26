using System;
using Microsoft.Xna.Framework;

namespace GameComponents.Vehicles.Animation
{
    [Serializable]
    public class AnimationInfo
    {
        public string Name = null;
        public string Type = null;
        public string BoneName = null;
        public Vector3 Axis = Vector3.Up;
        public float AngleFrom;
        public float AngleTo;
        public float Velocity;

        public AnimationInfo()
        {

        }
    }
}
