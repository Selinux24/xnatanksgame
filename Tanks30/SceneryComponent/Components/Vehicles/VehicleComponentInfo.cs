using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework.Graphics;

namespace GameComponents.Vehicles
{
    using GameComponents.Vehicles.Animation;

    [Serializable]
    public partial class VehicleComponentInfo
    {
        public string Model = null;
        public AnimationInfo[] AnimationControlers;
        public PlayerPositionInfo[] PlayerPositions;
        public float MaxForwardVelocity;
        public float MaxBackwardVelocity;
        public float AccelerationModifier;
        public float BrakeModifier;
        public float AngularVelocityModifier;
        public float Height;
        public bool Skimmer = false;

        public static VehicleComponentInfo Load(string xml)
        {
            StreamReader rd = new StreamReader(xml);
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(VehicleComponentInfo));

                VehicleComponentInfo result = serializer.Deserialize(rd) as VehicleComponentInfo;

                return result;
            }
            finally
            {
                rd.Close();
                rd.Dispose();
                rd = null;
            }
        }

        public VehicleComponentInfo()
        {

        }

        public AnimationBase[] CreateAnimationList(Model model)
        {
            List<AnimationBase> animationList = new List<AnimationBase>();

            foreach (AnimationInfo animationInfo in this.AnimationControlers)
            {
                if (animationInfo.Type == typeof(AnimationBase).ToString())
                {
                    AnimationBase animation = new AnimationBase(animationInfo.Name, model.Bones[animationInfo.BoneName]);
                    animation.Initialize(animationInfo.Axis);

                    animationList.Add(animation);
                }
                else if (animationInfo.Type == typeof(AnimationClamped).ToString())
                {
                    AnimationClamped animation = new AnimationClamped(animationInfo.Name, model.Bones[animationInfo.BoneName]);
                    animation.Initialize(animationInfo.Axis, animationInfo.AngleFrom, animationInfo.AngleTo, animationInfo.Velocity, animationInfo.Inverse);

                    animationList.Add(animation);
                }
            }

            return animationList.ToArray();
        }

        public PlayerPosition[] CreatePlayerPositionList(Model model)
        {
            List<PlayerPosition> m_PlayerControlList = new List<PlayerPosition>();

            foreach (PlayerPositionInfo positionInfo in this.PlayerPositions)
            {
                PlayerPosition position = new PlayerPosition(positionInfo.Name, model.Bones[positionInfo.BoneName], positionInfo.Translation);

                m_PlayerControlList.Add(position);
            }

            return m_PlayerControlList.ToArray();
        }
    }
}
