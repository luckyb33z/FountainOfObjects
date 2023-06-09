using System;

namespace Feature
{
    using Utilities;
    using IDescriptive;

    abstract class Feature: IDescriptive
    {
        virtual public string InRoomDescription { get; set; } 
        virtual public ConsoleColor DescColor { get; set; }
    }

    abstract class LoudFeature: Feature, IDescriptiveNoisy
    {
        virtual public string AdjacentDescription { get; set; }
        virtual public int PerceptibleDistance { get; set; } = 1;
    }

    class LoudFeaturePit: LoudFeature
    {
        public LoudFeaturePit()
        {
            InRoomDescription = "Oh no! There's no floor below you!";
            AdjacentDescription = "You feel a draft. There is a pit in a nearby room.";
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