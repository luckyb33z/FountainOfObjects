using System;

namespace Feature
{
    using Utilities;
    using IDescriptive;

    abstract class Feature: IDescriptive
    {
        public string InRoomDescription { get; set; } 
        public ConsoleColor DescColor { get; set; }
    }

    abstract class LoudFeature: Feature, IDescriptiveNoisy
    {
        public string AdjacentDescription { get; set; }
        public int PerceptibleDistance { get; set; }
    }

    class LoudFeaturePit: LoudFeature
    {
        public LoudFeaturePit()
        {
            InRoomDescription = "Oh no! There's no floor below you!";
            AdjacentDescription = "You feel a draft. There is a pit in a nearby room.";
            PerceptibleDistance = 1;
            DescColor = TermColors.DangerColor;
        }
    }

    class FeatureEntrance: Feature
    {
        public FeatureEntrance()
        {
            InRoomDescription = "You see light coming from the cavern entrance.";
            DescColor = TermColors.LightColor;
        }
    }

    class FeatureFountain: Feature
    {
        public bool Active { get; private set; }

        public void Activate()
        {
            InRoomDescription = "You hear the rushing waters from the Fountain of Objects. It has been reactivated!";
            Active = true;
        }

        public FeatureFountain()
        {
            InRoomDescription = "You hear water dripping in this room. The Fountain of Objects is here!";
            DescColor = TermColors.WaterColor;
            Active = false;
        }
    }
}