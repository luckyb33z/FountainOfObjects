using System;
using System.Collections.Generic;
using Enums;

namespace FountainOfObjects
{
    using IDescriptive;
    using Player;
    using Utilities;
    using Utilities.Exceptions;
    using World;
    using Room;

    public class GameMaster
    {

        World world;
        Player player;

        private bool RoomReacting { get; set; } = false;
        public bool Playing { get; private set; } = false;

        public void PlayGame(int worldSize)
        {
            if (BuildWorld())
            {
                Playing = true;
                ShowIntroductoryText();
                StartAdventure();
            }
            else
            {
                Console.WriteLine("Failed to build world!");
            }
        }

        private void ShowIntroductoryText()
        {
            Console.Clear();

            // Line 1: "You enter the Cavern of Objects, a maze of rooms filled with dangerous pits in search of the Fountain of Objects."
            Console.Write("You enter the ");
            Utilities.WriteColored(TermColors.LightColor, "Cavern of Objects");
            Console.Write(", a maze of rooms filled with ");
            Utilities.WriteColored(TermColors.DangerColor, "dangerous pits");
            Console.Write(" in search of the ");
            Utilities.WriteColored(TermColors.WaterColor, "Fountain of Objects");
            Console.WriteLine(".");

            // Line 2: "Light is visible only in the entrance, and no other light is seen anywhere in the Caverns."
            Console.WriteLine("Light is visible only in the entrance, and no other light is seen anywhere in the Caverns.");

            // Line 3: "You must navigate the Caverns with your other senses."
            Console.WriteLine("You must navigate the Caverns with your other senses.");

            // Line 4: "Find the Fountain of Objects, ACTIVATE it, and return to the entrance."
            Console.Write("Find the ");
            Utilities.WriteColored(TermColors.WaterColor, "Fountain of Objects");
            Console.Write(", ");
            Utilities.WriteColored(TermColors.VictoryColor, "ACTIVATE");
            Console.Write(" it, and return to the ");
            Utilities.WriteColored(TermColors.LightColor, "entrance");
            Console.WriteLine(".");

            // Line 5: "Look out for pits. You will feel a breeze if a pit is in a nearby room. If you enter a room with a pit, you will die."
            Console.Write("Look out for ");
            Utilities.WriteColored(TermColors.DangerColor, "pits");
            Console.Write(". You will feel a breeze if a ");
            Utilities.WriteColored(TermColors.DangerColor, "pit");
            Console.Write(" is in a nearby room. ");
            Utilities.WriteColoredLine(TermColors.DangerColor, "If you enter a room with a pit, you will die.");

            // Line 6: "Maelstroms are violent forces of sentient wind. Entering a room with one could send you flying to any of the rooms in the cavern!"
            Utilities.WriteColored(TermColors.DangerColor, "Maelstroms");
            Console.Write(" are violent forces of sentient wind. ");
            Utilities.WriteColoredLine(TermColors.BumpColor, "Entering a room with one could send you flying to any of the rooms in the cavern!");

            // Line 7: "Amaroks roam the caverns. Encountering one is certain death, but they smell strongly of rot."
            Utilities.WriteColored(TermColors.DangerColor, "Amaroks");
            Console.Write(" roam the caverns. ");
            Utilities.WriteColored(TermColors.DangerColor, "Encountering one is certain death");
            Console.Write(", but ");
            Utilities.WriteColoredLine(TermColors.BumpColor, "they smell strongly of rot.");

            // Line 8: "You carry with you a bow and arrow. You can use them to SHOOT monsters in adjacent rooms, but you have a limited supply of arrows."
            Console.Write("You carry with you a ");
            Utilities.WriteColored(TermColors.VictoryColor, "bow and arrow");
            Console.Write(". You can use them to ");
            Utilities.WriteColored(TermColors.VictoryColor, "SHOOT");
            Console.Write(" monsters in adjacent rooms, ");
            Utilities.WriteColoredLine(TermColors.BumpColor, "but you have a limited supply of arrows.");

            Utilities.WriteColoredLine(TermColors.BumpColor, "\nPress enter to begin...");
            Console.Read();
        }

        private WorldSize GetWorldSize()
        {
            
            WorldSize size = WorldSize.Unknown;
            while (size == WorldSize.Unknown)
            {
                Console.Clear();

                Console.Write("What is the difficulty of the adventure? (Easy/Medium/Hard) ");
                string choice = Console.ReadLine();
                size = ProcessWorldSizeChoice(choice);
                
            }
            return size;
        }

        private WorldSize ProcessWorldSizeChoice(string choice)
        {

            switch (choice)
            {
                case "easy":
                case "e":
                case "ez": // haha
                    return WorldSize.Small;
                case "medium":
                case "med":
                case "m":
                    return WorldSize.Medium;
                case "hard":
                case "h":
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
            
            while (Playing)
            {   
                RoomReacting = true;
                while (RoomReacting)
                {
                    ReportStatus();
                    RoomReacting = DoRoomReactions();  // This while loop will allow for repeated messages in the event of multi-maelstrom; it's almost recursive
                }

                if (player.Thrown)
                {
                    ReportStatus();
                    player.Thrown = false;
                }

                if (!IsGameOver())
                {
                    PrintAdjacentRoomDescriptions();
                    string command = AskForCommand();
                    ProcessCommand(command);
                    CheckForVictory();
                }
            }
        }

        private void ReportStatus()
        {
            const string divider = "----------------------------------------";
            Console.WriteLine(divider);
            Console.WriteLine($"You have {player.Arrows} arrows left.");
            PrintLocation();
            PrintCurrentRoomDescription();
        }

        private bool IsGameOver()
        {
            if (player.Dead || !Playing)
            {
                return true;
            }
            return false;
        }

        private bool DoRoomReactions()
        {
            if (player.CurrentRoom is PitRoom)
            {
                QuitGame(EndState.Defeat);
            }
            else if (player.CurrentRoom.Monster != null)
            {
                switch (player.CurrentRoom.Monster.Reaction)
                {
                    case MonsterReaction.Kill:

                        QuitGame(EndState.Defeat);
                        break;

                    case MonsterReaction.MaelstromThrow:

                        DoMaelstromThrow();
                        break;
                }
            }

            return false;
        }

        private void DoMaelstromThrow()
        {
            Room newPlayerRoom = world.GetRandomRoom();
            Room newMaelstromRoom = world.GetRandomEmptyRoom();

            // Guarantee the maelstrom and player do not land into the same room
            while (newMaelstromRoom == newPlayerRoom)
            {
                newMaelstromRoom = world.GetRandomEmptyRoom();
            }

            newMaelstromRoom.Monster = player.CurrentRoom.Monster;
            player.CurrentRoom.Monster = null;
            player.CurrentRoom = newPlayerRoom;

            player.Thrown = true;
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
            // This is a big function. How can I break it down?

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
                            Utilities.WritePromptedColoredLine(TermColors.LightColor, "You did it! Now you must escape the Cavern with your life!");
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

                // Arrow shooting
                case "shoot west":
                case "shoot w":
                    TryShoot(Direction.West);
                    break;
                case "shoot east":
                case "shoot e":
                    TryShoot(Direction.East);
                    break;
                case "shoot north":
                case "shoot n":
                    TryShoot(Direction.North);
                    break;
                case "shoot south":
                case "shoot s":
                    TryShoot(Direction.South);
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
                    Utilities.WritePromptedColoredLine(TermColors.DangerColor, "You flee the dungeon! The Uncoded One will surely know victory if you retreat...");
                    break;
                case EndState.Victory:
                    Utilities.WritePromptedColoredLine(TermColors.VictoryColor, "Success! You reactivated the Fountain of Objects and are prepared for the journey ahead!");
                    break;
                case EndState.Defeat:
                    const string defeatString = "You have been lost in the darkness. Your bones are never found...";
                    
                    if (world.FountainRoom.IsFountainActive())
                    {
                        Utilities.WriteColoredLine(TermColors.DangerColor, defeatString);
                        Utilities.WritePromptedColoredLine(TermColors.VictoryColor, "But despite your death, the Fountain has been reactivated! There's hope for the world!");
                    }
                    else
                    {
                        Utilities.WritePromptedColoredLine(TermColors.DangerColor, defeatString);
                    }

                    player.Dead = true;
                    break;
            }
        }

        private void TryShoot(Direction dir)
        {
            if (player.CanShoot())
            {
                RoomCoords destination = GetDirectionalOffset(dir);

                if (destination.X < 0 || destination.Y < 0 || destination.X >= world.Size || destination.Y >= world.Size)
                {
                    Utilities.WriteColoredLine(TermColors.BumpColor, "You can't shoot that direction; you'd hit a wall!");
                }
                else
                {
                    Room targetRoom = world.GetRoom(destination.X, destination.Y);
                    if (targetRoom.Monster != null)
                    {
                        Utilities.WritePromptedColoredLine(TermColors.VictoryColor, "You hear the death cry of a monster!");
                        targetRoom.Monster = null;
                        world.LoudRooms.Remove(targetRoom);
                    }
                    else
                    {
                        Utilities.WritePromptedColoredLine(TermColors.LightColor, $"You loose an arrow to the {dir}... but you hit nothing!");
                    }
                }
                player.ShootArrow();
            }
            else
            {
                Utilities.WritePromptedColoredLine(TermColors.LightColor, "Alas, you are out of arrows!");
            }
        }

        private void TryMove(Direction dir)
        {

            RoomCoords destination = GetDirectionalOffset(dir);

            if (destination.X < 0 || destination.Y < 0 || destination.X >= world.Size || destination.Y >= world.Size)
            {
                Utilities.WriteColoredLine(TermColors.BumpColor, "You bump into a wall!");
            }
            else
            {
                player.CurrentRoom = world.GetRoom(destination.X, destination.Y);
            }
        }

        private RoomCoords GetDirectionalOffset(Direction dir)
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

            return new RoomCoords(newX, newY);
        }

        private void PrintLocation()
        {
            Console.WriteLine($"You are in the room at {player.CurrentRoom.Coordinates.X + 1}/{player.CurrentRoom.Coordinates.Y + 1}.");
        }

        private void PrintCurrentRoomDescription()
        {

            List<IDescriptive> features = player.CurrentRoom.Features;
            
            if (features.Count > 0)
            {
                foreach (IDescriptive feature in features)
                {
                    if (feature is Monster.MonsterMaelstrom)
                    {
                        Utilities.WritePromptedColoredLine(feature.DescColor, feature.InRoomDescription);
                    }
                    else
                    {
                        Utilities.WriteColoredLine(feature.DescColor, feature.InRoomDescription);
                    }
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
