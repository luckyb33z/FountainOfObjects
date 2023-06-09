using System;

namespace Room
{
    using Feature;
    using IDescriptive;
    using Monster;
    using Utilities.Exceptions;

    public struct RoomCoords
    {
        public int X {get; private set;}
        public int Y {get; private set;}

        public RoomCoords(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    abstract class Room
    {
        public RoomCoords Coordinates { get; protected set; }
        private Feature _feature;
        public Feature RoomFeature { get {return _feature;} protected set {_feature = value;}}
        public Monster Monster { get; set; }

        public ConsoleColor FeatureColor {
            get
            {
                // TODO: Can RoomFeature/Monster be condensed into IDescriptive?
                if (RoomFeature != null) {return RoomFeature.DescColor;}
                else if (Monster != null) {return Monster.DescColor;}
                else {return ConsoleColor.White;}
            }
        }

        public string RoomDescription
        {
            get
            {
                // TODO: Can RoomFeature/Monster be condensed into IDescriptive?
                if (RoomFeature != null) {return RoomFeature.InRoomDescription;}
                else if (Monster != null) {return Monster.InRoomDescription;}
                else {return string.Empty;}
            }
        }

        public IDescriptiveNoisy InRangeOf(Room room)
        {
            IDescriptiveNoisy noisyThingToCheck = null;
            if (room.RoomFeature is LoudFeature)
            {
                noisyThingToCheck = room.RoomFeature as LoudFeature;
            }
            else if (room.Monster != null)
            {
                noisyThingToCheck = room.Monster;
            }

            IDescriptiveNoisy noisyThing = null;

            if (noisyThingToCheck != null)
            {
                int distanceX = Math.Abs(this.Coordinates.X - room.Coordinates.X);
                int distanceY = Math.Abs(this.Coordinates.Y - room.Coordinates.Y);

                if (distanceX <= noisyThingToCheck.PerceptibleDistance && distanceY <= noisyThingToCheck.PerceptibleDistance)
                {
                    noisyThing = noisyThingToCheck;
                }
            }

            if (noisyThing == null)
            {
                throw new NoNoisyThingException("No noisy thing in range of player");
            }
            else
            {
                return noisyThing;
            }
        }

        public Room (int x, int y)
        {
            Coordinates = new RoomCoords(x, y);
        }
    }

    class PitRoom: Room
    {
        public PitRoom(int x, int y): base(x, y)
        {
            RoomFeature = new LoudFeaturePit();
        }
    }

    class EmptyRoom: Room
    {
        public EmptyRoom(int x, int y): base(x, y)
        {

        }
    }

    class FountainRoom: Room
    {
        public bool IsFountainActive()
        {
            FeatureFountain fountain = RoomFeature as FeatureFountain;

            if (fountain == null)
            {
                return false;
            }
            else
            {
                return fountain.Active;
            }
        }

        public void ActivateFountain()
        {
            FeatureFountain fountain = RoomFeature as FeatureFountain;

            if (fountain != null)
            {
                fountain.Activate();
            }
        }

        public FountainRoom(int x, int y): base(x, y)
        {
            RoomFeature = new FeatureFountain();
        }
    }

    class EntranceRoom: Room
    {
        //public bool IsPlayerPresent();

        public EntranceRoom(int x, int y): base(x, y)
        {
            RoomFeature = new FeatureEntrance();
        }
    }
}