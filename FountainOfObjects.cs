using System;
using System.Collections.Generic;
using Enums;

namespace FountainOfObjects
{
    using IDescriptive;
    using Utilities;
    using Utilities.Exceptions;
    using World;
    using Room;

    public class GameMaster
    {

        World world;
        Player player;

        public bool Playing { get; private set; } = false;

        public void PlayGame(int worldSize)
        {
            if (BuildWorld())
            {
                Playing = true;
                StartAdventure();
            }
            else
            {
                Console.WriteLine("Failed to build world!");
            }
        }

        private WorldSize GetWorldSize()
        {
            
            WorldSize size = WorldSize.Unknown;
            while (size == WorldSize.Unknown)
            {
                Console.Clear();

                Console.Write("What is the size of the world? (small/medium/large) ");
                string choice = Console.ReadLine();
                size = ProcessWorldSizeChoice(choice);
                
            }
            return size;
        }

        private WorldSize ProcessWorldSizeChoice(string choice)
        {

            switch (choice)
            {
                case "small":
                case "s":
                    return WorldSize.Small;
                case "medium":
                case "m":
                    return WorldSize.Medium;
                case "large":
                case "l":
                    return WorldSize.Large;
                default:
                    Console.WriteLine("Unknown world size.");
                    return WorldSize.Unknown;
            }
        }

        private bool BuildWorld()
        {
            WorldSize size = GetWorldSize();
            world = new World(size);
            player = new Player();

            if (world.Built)
            {
                player.CurrentRoom = world.GetRoom(0, 0);
                return true;
            }
            return false;
        }

        private void StartAdventure()
        {
            Console.Clear();
            string divider = "----------------------------------------";
            
            while (Playing)
            {
                Console.WriteLine(divider);
                PrintLocation();
                PrintCurrentRoomDescription();

                if (CheckForFailure())
                {
                    Playing = false;
                    break;
                }

                PrintAdjacentRoomDescriptions();
                string command = AskForCommand();
                ProcessCommand(command);
                CheckForVictory();
            }
        }

        private bool CheckForFailure()
        {
            if (player.CurrentRoom is PitRoom)
            {
                QuitGame(EndState.Defeat);
                return true;
            }
            
            if (player.CurrentRoom.Monster != null)
            {
                switch (player.CurrentRoom.Monster.Reaction)
                {
                    case Monster.Reaction.Kill:
                        QuitGame(EndState.Defeat);
                        return true;
                    case Monster.Reaction.Throw:
                        return false;
                }
            }

            return false;
        }

        private void CheckForVictory()
        {
            if (player.CurrentRoom == world.EntranceRoom && world.FountainRoom.IsFountainActive())
            {
                QuitGame(EndState.Victory);
            }
        }

        private void ProcessCommand(string command)
        {
            switch (command.ToLower())
            {
                case "activate fountain":
                case "activate":
                {
                    if (player.CurrentRoom == world.FountainRoom)
                    {
                        FountainRoom fountainRoom = player.CurrentRoom as FountainRoom;
                        if (fountainRoom.IsFountainActive())
                        {
                            Console.WriteLine("The Fountain of Objects is already activated!");
                        }
                        else
                        {
                            fountainRoom.ActivateFountain();
                        }
                    }
                    else
                    {
                        Console.WriteLine("The Fountain of Objects is not here.");
                    }
                    break;
                }

                case "move west":
                case "west":
                case "w":
                    TryMove(Direction.West);
                    break;
                case "move east":
                case "east":
                case "e":
                    TryMove(Direction.East);
                    break;
                case "move north":
                case "north":
                case "n":
                    TryMove(Direction.North);
                    break;
                case "move south":
                case "south":
                case "s":
                    TryMove(Direction.South);
                    break;

                case "quit":
                case "qq":
                case "q":
                    if (player.CurrentRoom == world.EntranceRoom)
                    {
                        QuitGame(EndState.Surrender);
                    }
                    else
                    {
                        QuitGame(EndState.Defeat);
                    }
                    break;

                default:
                    Console.WriteLine("Unknown command.");
                    break;
            }
        }

        private void QuitGame(EndState state)
        {
            Playing = false;

            switch (state)
            {
                case EndState.Surrender:
                    Utilities.WriteColoredLine(TermColors.DangerColor, "You flee the dungeon! The Uncoded One will surely know victory if you retreat...");
                    break;

                case EndState.Victory:
                    Utilities.WriteColoredLine(TermColors.VictoryColor, "Success! You reactivated the Fountain of Objects and are prepared for the journey ahead!");
                    break;
                case EndState.Defeat:
                    Utilities.WriteColoredLine(TermColors.DangerColor, "You have been lost in the darkness. Your bones are never found...");
                    break;
            }
        }

        private void TryMove(Direction dir)
        {

            int newX = player.CurrentRoom.Coordinates.X;
            int newY = player.CurrentRoom.Coordinates.Y;

            switch (dir)
            {
                case Direction.West:
                    newX -= 1;
                    break;
                case Direction.East:
                    newX += 1;
                    break;
                case Direction.North:
                    newY += 1;
                    break;
                case Direction.South:
                    newY -= 1;
                    break;
            }

            if (newX < 0 || newY < 0 || newX >= world.Size || newY >= world.Size)
            {
                Utilities.WriteColoredLine(TermColors.BumpColor, "You bump into a wall!");
            }
            else
            {
                player.CurrentRoom = world.GetRoom(newX, newY);
            }
        }

        private void PrintLocation()
        {
            Console.WriteLine($"You are in the room at {player.CurrentRoom.Coordinates.X}/{player.CurrentRoom.Coordinates.Y}.");
        }

        private void PrintCurrentRoomDescription()
        {

            List<IDescriptive> features = player.CurrentRoom.Features;
            
            if (features.Count > 0)
            {
                foreach (IDescriptive feature in features)
                {
                    Utilities.WriteColoredLine(feature.DescColor, feature.InRoomDescription);
                }
            }
            
        }

        private void PrintAdjacentRoomDescriptions()
        {
            foreach (Room room in world.LoudRooms)
            {
                try
                {
                    IDescriptiveNoisy noisyThing = player.CurrentRoom.InRangeOf(room);
                    Utilities.WriteColoredLine(noisyThing.DescColor, noisyThing.AdjacentDescription); 
                }
                catch (NoNoisyThingException)
                {
                    // this is fine, exception is expected if there is no noisy thing nearby
                    continue;
                }
                
            }
        }

        private string AskForCommand()
        {
            Console.Write("What do you want to do? ");
            return Console.ReadLine();
        }
    }

    class Player
    {
        public Room CurrentRoom { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
            GameMaster gm = new GameMaster();
            gm.PlayGame(3);
        }
    }
}
