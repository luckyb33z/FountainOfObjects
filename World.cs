using System;
using System.Collections.Generic;
using Enums;

namespace World
{
    using Room;
    using Monster;
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
                InstantiateLoudRooms();
                if (LoudRooms != null)
                {
                    PlaceLoudRooms();
                    PlaceMonsters();
                }
                else
                {
                    throw new Exception("Failed to instantiate room list.");
                }
            }
        }

        private void PlaceMonsters()
        {
            List<Monster> monsterList = EnumerateMonsters();
            
            foreach (Monster monster in monsterList)
            {
                Room room = GetRandomEmptyRoom();
                room.Monster = monster;
                LoudRooms.Add(room);
            }

        }

        private List<Monster> EnumerateMonsters()
        {
            List<Monster> monsterList = new List<Monster>();

            int numAmaroks = GetAmarokCount();
            int numMaelstroms = GetMaelstromCount();

            for (int i = 0; i < numAmaroks; i++)
            {
                monsterList.Add(new MonsterAmarok());
            }

            for (int i = 0; i < numMaelstroms; i++)
            {
                monsterList.Add(new MonsterMaelstrom());
            }

            return monsterList;
        }

        public Room GetRandomRoom()
        {
            int x = Utilities.rand.Next(0, Size);
            int y = Utilities.rand.Next(0, Size);
            return GetRoom(x, y);
        }

        public Room GetRandomEmptyRoom(bool entryGrace = true, bool withoutMonsters = true)
        {
            int graceRange = entryGrace ? 1 : 0; // Do not put any features this far from the entryway

            int x = 0;
            int y = 0;

            bool rerolling = false;
            Room room = GetRoom(x, y);
            while (!(room is EmptyRoom) && (withoutMonsters ? room.Monster == null : room.Monster != null))
            {
                int errorCount = 0;
                while ((x <= graceRange && y <= graceRange) || rerolling)
                {
                    x = Utilities.rand.Next(graceRange, Size);
                    y = Utilities.rand.Next(graceRange, Size);
                    errorCount++;
                    rerolling = false;
                    if (errorCount >= 10)
                    {
                        const string errorString = "The room generation bug reappeared -- tell Lucky! Crashing the game!";
                        Utilities.WritePromptedColoredLine(ConsoleColor.Red, errorString);
                        throw new OverflowException(errorString);
                    }
                }
                room = GetRoom(x, y);
                if (!(room is EmptyRoom)) {rerolling = true;}
                else {rerolling = false;}      
            }

            return room;
        }

        private int GetAmarokCount()
        {

            const int SMALL_AMAROKS = 1;
            const int MEDIUM_AMAROKS = 2;
            const int LARGE_AMAROKS = 3;

            switch (_worldSize)
            {
                case WorldSize.Small:
                default:
                    return SMALL_AMAROKS;
                case WorldSize.Medium:
                    return MEDIUM_AMAROKS;
                case WorldSize.Large:
                    return LARGE_AMAROKS;
            }
        }

        private int GetMaelstromCount()
        {
            const int SMALL_MAELSTROMS = 1;
            const int MEDIUM_MAELSTROMS = 2;
            const int LARGe_MAELSTROMS = 3;

            switch (_worldSize)
            {
                case WorldSize.Small:
                default:
                    return SMALL_MAELSTROMS;
                case WorldSize.Medium:
                    return MEDIUM_MAELSTROMS;
                case WorldSize.Large:
                    return LARGe_MAELSTROMS;
            }
        }

        private void PlaceLoudRooms()
        {
            const int MIN_PITS = 1;
            const int MAX_PITS = 4;
            const int PIT_LIMITER = 4;

            int pitCount = Math.Clamp(Size - PIT_LIMITER, MIN_PITS, MAX_PITS);

            for (int pitIndex = 0; pitIndex < pitCount; pitIndex++)
            {
                Room room = GetRandomEmptyRoom();
                PitRoom pit = new PitRoom(room.Coordinates.X, room.Coordinates.Y);
                
                room = pit;
                _rooms[room.Coordinates.X][room.Coordinates.Y] = pit;
                LoudRooms.Add(pit);
            }
        }

        private void InstantiateLoudRooms()
        {
            if (LoudRooms == null)
            {
                LoudRooms = new List<Room>();
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
            const int MIN_DISTANCE = 2;

            int x = Utilities.rand.Next(MIN_DISTANCE, Size);
            int y = Utilities.rand.Next(MIN_DISTANCE, Size);

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