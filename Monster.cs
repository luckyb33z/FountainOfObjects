using System;

namespace Monster
{
    using IDescriptive;
    using Room;
    using Utilities;
    using Enums;

    abstract class Monster: IDescriptiveNoisy
    {
        virtual public string InRoomDescription { get; set; } 
        virtual public string AdjacentDescription { get; set; }
        virtual public ConsoleColor DescColor { get; set; } = TermColors.DangerColor;
        virtual public int PerceptibleDistance { get; set; } = 1;

        virtual public Room Room { get; set; }

        virtual public MonsterReaction Reaction { get; set; }
    }

    class MonsterAmarok: Monster
    {
        public MonsterAmarok()
        {
            InRoomDescription = "You are beset by an amarok! It tears you to shreds!";
            AdjacentDescription = "You smell the rotten stench of an amarok in a nearby room.";
            Reaction = MonsterReaction.Kill;
        }
    }

    class MonsterMaelstrom: Monster
    {
        public MonsterMaelstrom()
        {
            InRoomDescription = "You are caught in the whirlwind of a maelstrom and sent flying!";
            AdjacentDescription = "You hear the growling and groaning of a maelstrom nearby.";
            Reaction = MonsterReaction.MaelstromThrow;
        }
    }
}