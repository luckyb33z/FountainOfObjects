using System;

namespace Monster
{
    using IDescriptive;
    using Room;
    using Utilities;

    enum Reaction
    {
        Kill,
        Throw
    }

    abstract class Monster: IDescriptiveNoisy
    {
        virtual public string InRoomDescription { get; set; } 
        virtual public string AdjacentDescription { get; set; }
        virtual public ConsoleColor DescColor { get; set; }
        virtual public int PerceptibleDistance { get; set; } = 1;

        virtual public Room Room { get; set; }

        virtual public Reaction Reaction { get; set; }
    }

    class MonsterAmarok: Monster
    {
        public MonsterAmarok()
        {
            InRoomDescription = "The amarok attacks! It tears you to shreds!";
            AdjacentDescription = "You smell the rotten stench of an amarok in a nearby room.";
            DescColor = TermColors.DangerColor;
            Reaction = Reaction.Kill;
        }
    }
}