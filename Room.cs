using System;

namespace Room
{
    using Feature;

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

        public ConsoleColor FeatureColor {
            get
            {
                if (RoomFeature == null)
                {
                    return ConsoleColor.White;
                }
                else
                {
                    return RoomFeature.DescColor;
                }
            }
        }

        public string RoomDescription
        {
            get
            {
                if (RoomFeature == null)
                {
                    return string.Empty;
                }
                else
                {
                    return RoomFeature.InRoomDescription;
                }
            }
        }

        public bool InRangeOf(Room room)
        {
            if (room.RoomFeature is LoudFeature)
            {
                LoudFeature feature = room.RoomFeature as LoudFeature;

                int distanceX = Math.Abs(this.Coordinates.X - room.Coordinates.X);
                int distanceY = Math.Abs(this.Coordinates.Y - room.Coordinates.Y);

                if (distanceX <= feature.PerceptibleDistance && distanceY <= feature.PerceptibleDistance)
                {
                    return true;
                }
            }
            return false;
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