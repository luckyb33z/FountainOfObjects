using System;
using System.Collections.Generic;
using Enums;

namespace World
{
    using Room;
    using Utilities;

    class World
    {
        List<List<Room>> _rooms;

        public EntranceRoom EntranceRoom { get; private set; }
        public FountainRoom FountainRoom { get; private set; }
        public List<Room> LoudRooms {get; private set; }

        readonly WorldSize _worldSize;

        public bool Built { get; private set; }
        public int Size { 
            get 
            { 
                return EnumerateWorldSize();
            }
        }

        RoomCoords entranceCoords = new RoomCoords(0, 0);
        RoomCoords fountainCoords;

        public World(WorldSize size)
        {
            _worldSize = size;
            fountainCoords = RandomizeFountainCoords();
            Built = BuildWorld();
            if (Built)
            {
                PlaceLoudRooms();
            }
        }

        private void PlaceLoudRooms()
        {
            const int MIN_PITS = 1;
            const int MAX_PITS = 4;
            const int PIT_LIMITER = 4;

            int x = 0;
            int y = 0;

            int pitCount = Math.Clamp(Size - PIT_LIMITER, MIN_PITS, MAX_PITS);

            LoudRooms = new List<Room>();

            for (int pitIndex = 0; pitIndex < pitCount; pitIndex++)
            {
                while (!((GetRoom(x, y) is EmptyRoom)))
                {
                    x = Utilities.rand.Next(0, Size);
                    y = Utilities.rand.Next(0, Size);
                }

                PitRoom pit = new PitRoom(x, y);

                _rooms[x][y] = pit;
                LoudRooms.Add(pit);
            }

            
        }

        private int EnumerateWorldSize()
        {
            const int small = 4;
            const int medium = 6;
            const int large = 8;

            switch (_worldSize)
            {
                case WorldSize.Small:
                    return small;
                case WorldSize.Medium:
                    return medium;
                case WorldSize.Large:
                    return large;
                default:    // WorldSize.Unknown somehow provided
                    return small;
            }
        }

        private RoomCoords RandomizeFountainCoords()
        {

            int x = 0;
            int y = 0;

            while (x == 0 && y == 0)
            {
                x = Utilities.rand.Next(0, Size);
                y = Utilities.rand.Next(0, Size);
            }

            return new RoomCoords(x, y);

        }

        public bool BuildWorld()
        {

            _rooms = new List<List<Room>>();
            for (int x = 0; x < Size; x++)
            {
                List<Room> colRooms = new List<Room>();
                for (int y = 0; y < Size; y++)
                {
                    if (x == entranceCoords.X && y == entranceCoords.Y)
                    {
                        EntranceRoom entrance = new EntranceRoom(x, y);
                        colRooms.Add(entrance);
                        EntranceRoom = entrance;
                    }
                    else if (x == fountainCoords.X && y == fountainCoords.Y)
                    {
                        FountainRoom fountain = new FountainRoom(x, y);
                        colRooms.Add(fountain);
                        FountainRoom = fountain;
                    }
                    else
                    {
                        colRooms.Add(new EmptyRoom(x, y));
                    }
                }
                _rooms.Add(colRooms);
            }

            int expectedRoomNum = Size * Size;

            if (_rooms.Count == expectedRoomNum)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public Room GetRoom(int x, int y)
        {
            if (x < 0 || x >= Size || y < 0 || y >= Size)
            {
                throw new IndexOutOfRangeException($"Requested Room index is out of range: {x}/{y}");
            }
            else
            {
                return _rooms[x][y];
            }
        } 

    }
}