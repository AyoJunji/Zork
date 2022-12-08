using System;
using System.Linq;
using Newtonsoft.Json;

namespace Zork.Common
{
    public class Game
    {
        public World World { get; }

        [JsonIgnore]
        public Player Player { get; }
        [JsonIgnore]
        public IInputService Input { get; private set; }
        [JsonIgnore]
        public IOutputService Output { get; private set; }
        [JsonIgnore]
        public bool IsRunning { get; private set; }

        public Game(World world, string startingLocation)
        {
            World = world;
            Player = new Player(World, startingLocation, 100);
        }

        public void Run(IInputService input, IOutputService output)
        {
            Input = input ?? throw new ArgumentNullException(nameof(input));
            Output = output ?? throw new ArgumentNullException(nameof(output));

            IsRunning = true;
            Input.InputReceived += OnInputReceived;
            Output.WriteLine("Welcome to Zork!");
            Look();
            Output.WriteLine($"\n{Player.CurrentRoom}");
        }

        public void OnInputReceived(object sender, string inputString)
        {
            char separator = ' ';
            string[] commandTokens = inputString.Split(separator);

            string verb;
            string subject = null;
            string preposition = null;
            string noun = null;
            if (commandTokens.Length == 0)
            {
                return;
            }
            else if (commandTokens.Length == 1)
            {
                verb = commandTokens[0];
            }
            else if (commandTokens.Length == 2)
            {
                verb = commandTokens[0];
                subject = commandTokens[1];
            }
            else if (commandTokens.Length == 3)
            {
                verb = commandTokens[0];
                subject = commandTokens[1];
                preposition = commandTokens[2];
            }
            else
            {
                verb = commandTokens[0];
                subject = commandTokens[1];
                preposition = commandTokens[2];
                noun = commandTokens[3];
            }

            Room previousRoom = Player.CurrentRoom;
            Commands command = ToCommand(verb);
            switch (command)
            {
                case Commands.Quit:
                    IsRunning = false;
                    Output.WriteLine("Thank you for playing!");
                    break;

                case Commands.Look:
                    Look();
                    break;

                case Commands.North:
                case Commands.South:
                case Commands.East:
                case Commands.West:
                    Directions direction = (Directions)command;
                    if (Player.Move(direction))
                    {
                        Output.WriteLine($"You moved {direction}.");
                        Player.Moves++;
                    }
                    else
                    {
                        Output.WriteLine("The way is shut!");
                    }
                    break;

                case Commands.Take:
                    if (string.IsNullOrEmpty(subject))
                    {
                        Output.WriteLine("This command requires a subject.");
                    }
                    else
                    {
                        Take(subject);
                        Player.Moves++;
                    }
                    break;

                case Commands.Drop:
                    if (string.IsNullOrEmpty(subject))
                    {
                        Output.WriteLine("This command requires a subject.");
                    }
                    else
                    {
                        Drop(subject);
                        Player.Moves++;
                    }
                    break;

                case Commands.Inventory:
                    Player.Moves++;
                    if (Player.Inventory.Count() == 0)
                    {
                        Output.WriteLine("You are empty handed.");
                    }
                    else
                    {
                        Output.WriteLine("You are carrying:");
                        foreach (Item item in Player.Inventory)
                        {
                            Output.WriteLine(item.InventoryDescription);
                        }
                    }
                    break;
                case Commands.Reward:
                    Player.Score++;
                    break;

                case Commands.Score:
                    Output.WriteLine($"Your score is {Player.Score} in {Player.Moves} turns.");
                    break;

                case Commands.Attack:

                    if (string.IsNullOrEmpty(subject))
                    {
                        Output.WriteLine("This command requires a subject.");
                    }
                    else if (string.IsNullOrEmpty(noun))
                    {
                        Output.WriteLine("Unknown command");
                    }
                    else
                    {
                        Attack(subject, noun);
                        Player.Moves++;
                    }
                    break;

                default:
                    Output.WriteLine("Unknown command.");
                    break;
            }

            if (ReferenceEquals(previousRoom, Player.CurrentRoom) == false)
            {
                Look();
            }

            Output.WriteLine($"\n{Player.CurrentRoom}");
        }

        private void Look()
        {
            Output.WriteLine(Player.CurrentRoom.Description);
            foreach (Item item in Player.CurrentRoom.Inventory)
            {
                Output.WriteLine(item.LookDescription);
            }

            foreach (Enemy enemy in Player.CurrentRoom.Enemy)
            {
                Output.WriteLine(enemy.LookDescription);
            }
        }

        private void Take(string itemName)
        {
            Item itemToTake = Player.CurrentRoom.Inventory.FirstOrDefault(item => string.Compare(item.Name, itemName, ignoreCase: true) == 0);
            if (itemToTake == null)
            {
                Output.WriteLine("You can't see any such thing.");
            }
            else
            {
                Player.AddItemToInventory(itemToTake);
                Player.CurrentRoom.RemoveItemFromInventory(itemToTake);
                Output.WriteLine("Taken.");
            }
        }

        private void Drop(string itemName)
        {
            Item itemToDrop = Player.Inventory.FirstOrDefault(item => string.Compare(item.Name, itemName, ignoreCase: true) == 0);
            if (itemToDrop == null)
            {
                Output.WriteLine("You can't see any such thing.");
            }
            else
            {
                Player.CurrentRoom.AddItemToInventory(itemToDrop);
                Player.RemoveItemFromInventory(itemToDrop);
                Output.WriteLine("Dropped.");
            }
        }

        private void Attack(string enemyName, string weaponName)
        {
            Item weaponItem = Player.Inventory.FirstOrDefault(item => string.Compare(item.Name, weaponName, ignoreCase: true) == 0);
            Enemy whichEnemyToAttack = Player.CurrentRoom.Enemy.FirstOrDefault(enemy => string.Compare(enemy.Name, enemyName, ignoreCase: true) == 0);

            if (whichEnemyToAttack == null)
            {
                Output.WriteLine("No such thing is in view.");
            }

            else if (weaponItem == null)
            {
                Output.WriteLine($"You don't have a {weaponName}.");
            }

            else if (!weaponItem.IsWeapon)
            {
                Output.WriteLine($"You can't attack a {enemyName} with a {weaponName}");
            }
            else
            {
                Random randomAttackChance = new Random();

                Type type = typeof(Attacks);
                Array values = type.GetEnumValues();
                int attackChanceIndex = randomAttackChance.Next(values.Length);

                switch (attackChanceIndex)
                {
                    case (int)Attacks.Missed:
                        Output.WriteLine($"You swing your sword aimlessly and completely missed the {enemyName}");
                        break;

                    case (int)Attacks.InstantKill:
                        Output.WriteLine($"You swing your sword viciously and impales the {enemyName}! It dies and falls to the ground");
                        Player.CurrentRoom.RemoveEnemyFromRoom(whichEnemyToAttack);
                        break;

                    case (int)Attacks.Damaged:
                        Output.WriteLine("You swing your sword and it's a solid hit");
                        whichEnemyToAttack.TakeDamage(5);
                        if (whichEnemyToAttack.Health <= 0)
                        {
                            Player.CurrentRoom.RemoveEnemyFromRoom(whichEnemyToAttack);
                            Output.WriteLine($"The {enemyName} dies and falls to the ground");
                        }
                        else
                        {
                            Output.WriteLine($"{enemyName} Health: {whichEnemyToAttack.Health}");
                        }
                        break;

                    default:
                        break;

                }
            }
        }
        private static Commands ToCommand(string commandString) => Enum.TryParse(commandString, true, out Commands result) ? result : Commands.Unknown;
    }
}