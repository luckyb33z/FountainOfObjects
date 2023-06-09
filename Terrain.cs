using System;

namespace Terrain
{
    using Utilities;
    using IDescriptive;

    abstract class Terrain: IDescriptive
    {
        virtual public string InRoomDescription { get; set; } 
        virtual public ConsoleColor DescColor { get; set; }
    }

    abstract class TerrainNoisy: Terrain, IDescriptiveNoisy
    {
        virtual public string AdjacentDescription { get; set; }
        virtual public int PerceptibleDistance { get; set; } = 1;
    }

    class TerrainNoisyPit: TerrainNoisy
    {
        public TerrainNoisyPit()
        {
            InRoomDescription = "Oh no! There's no floor below you!";
            AdjacentDescription = "You feel a draft. There is a pit in a nearby room.";
            DescColor = TermColors.DangerColor;
        }
    }

    class TerrainEntrance: Terrain
    {
        public TerrainEntrance()
        {
            InRoomDescription = "You see light coming from the cavern entrance.";
            DescColor = TermColors.LightColor;
        }
    }

    class TerrainFountain: Terrain
    {
        public bool Active { get; private set; }

        public void Activate()
        {
            InRoomDescription = "You hear the rushing waters from the Fountain of Objects. It has been reactivated!";
            Active = true;
        }

        public TerrainFountain()
        {
            InRoomDescription = "You hear water dripping in this room. The Fountain of Objects is here!";
            DescColor = TermColors.WaterColor;
            Active = false;
        }
    }
}