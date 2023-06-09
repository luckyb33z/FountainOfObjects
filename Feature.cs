using System;

namespace Feature
{
    using Utilities;
    
    abstract class Feature
    {
        public string InRoomDescription { get; protected set; } 
        public ConsoleColor DescColor { get; protected set; }
    }

    abstract class LoudFeature: Feature
    {
        public string AdjacentDescription { get; protected set; }
        public int PerceptibleDistance { get; protected set; }
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