using System;

namespace Zork.Common
{
    public class Game
    {
        public World World { get; }

        public Player Player { get; }

        public Room Room { get; }

        public Item Item { get; }

        public IOutputService Output { get; private set; }

        public IInputService Input { get; private set; }

        public bool IsRunning { get; private set; }

        public Game(World world, string startingLocation)
        {
            World = world;
            Player = new Player(World, startingLocation);
        }

        public void Run(IOutputService output, IInputService input)
        {
            Output = output ?? throw new ArgumentNullException(nameof(input));
            Input = input ?? throw new ArgumentNullException(nameof(output));

            Input.InputReceived += Input_InputReceived;

            Room previousRoom = null;
            bool isRunning = true;
            while (isRunning)
            {
                Output.WriteLine(Player.CurrentRoom);
                if (previousRoom != Player.CurrentRoom)
                {
                    Output.WriteLine(Player.CurrentRoom.Description);
                    previousRoom = Player.CurrentRoom;
                }

                Output.Write("\n> ");

                string inputString = Console.ReadLine().Trim();
                char  separator = ' ';
                string[] commandTokens = inputString.Split(separator);
                
                string verb = null;
                string subject = null;

                if (commandTokens.Length == 0)
                {
                    continue;
                }
                
                else if (commandTokens.Length == 1)
                {
                    verb = commandTokens[0];
                }
                
                else
                {
                    verb = commandTokens[0];
                    subject = commandTokens[1];
                }

                Commands command = ToCommand(verb);
                switch (command)
                {
                    case Commands.Quit:
                        isRunning = false;
                        output.WriteLine("Thank you for playing!");
                        break;

                    case Commands.Look:

                        output.WriteLine(Player.CurrentRoom.Description);
                        foreach(Item item in Player.CurrentRoom.Inventory)
                        {
                            output.WriteLine(item.Description);
                        }
                        break;

                    case Commands.North:
                    case Commands.South:
                    case Commands.East:
                    case Commands.West:
                        Directions direction = (Directions)command;
                        if (Player.Move(direction))
                        {
                            output.WriteLine($"You moved {direction}.\n");
                        }
                        else
                        {
                            output.WriteLine("The way is shut!\n");
                        }
                        break;

                    case Commands.Take:
                        Item itemToTake = null;
                        foreach (Item item in World.Items)
                        {
                            if (string.Compare(subject, item.Name, ignoreCase: true) == 0)
                            {
                                itemToTake = item;
                                break;
                            }
                        }

                        bool itemIsInRoomInventory = false;

                        foreach (Item item in Player.CurrentRoom.Inventory)
                        {
                            if (item == itemToTake)
                            {
                                itemIsInRoomInventory = true;
                                break;
                            }
                        }

                        if (itemIsInRoomInventory == false)
                        {
                            output.WriteLine("You can't see any such thing.\n");
                        }

                        else
                        {
                            Player.AddToInventory(itemToTake);
                            Player.CurrentRoom.RemoveFromInventory(itemToTake);
                            output.WriteLine("Taken.\n");
                        }
                        break;

                    case Commands.Drop:
                        Item itemToDrop = null;
                        foreach (Item item in World.Items)
                        {
                            if (string.Compare(subject, item.Name, ignoreCase: true) == 0)
                            {
                                itemToDrop = item;
                                break;
                            }
                        }

                        bool itemIsInPlayerInventory = false;
                        foreach (Item item in Player.Inventory)
                        {
                            if (item == itemToDrop)
                            {
                                itemIsInPlayerInventory = true;
                                break;
                            }
                        }

                        if (itemIsInPlayerInventory == false)
                        {
                            output.WriteLine("You don't have that item.\n");
                        }

                        else
                        {
                            Player.RemoveFromInventory(itemToDrop);
                            Player.CurrentRoom.AddToInventory(itemToDrop);
                            output.WriteLine("Dropped.\n");
                        }
                        break;

                    case Commands.Inventory:
                        if (Player.Inventory.Count == 0)
                        {
                            output.WriteLine("You are empty-handed.\n");
                            break;
                        }
                        
                        else
                        {
                            foreach (Item item in Player.Inventory)
                            {
                                output.WriteLine(item.Description);
                            }
                        }
                        break;

                    default:
                        output.WriteLine("Unknown command.\n");
                        break;
                }
            }
        }
        public void Input_InputReceived(object sender, string inputString)
        {

        }
        private static Commands ToCommand(string commandString) => Enum.TryParse(commandString, true, out Commands result) ? result : Commands.Unknown;
    }
}
