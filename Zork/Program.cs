using System;

namespace Zork
{
    internal class Program
    {
        private static string CurrentRoom
        {
            get
            {
                return Rooms[location.Row, location.Column];
            }
        }

        private static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Zork!");

            Commands command = Commands.UNKNOWN;
            while (command != Commands.QUIT)
            {

                Console.Write($"{CurrentRoom}\n> ");
                command = ToCommand(Console.ReadLine().Trim());

                string outputString;
                switch (command)
                {
                    case Commands.QUIT:
                        outputString = "Thank you for playing!";
                        break;

                    case Commands.LOOK:
                        outputString = "A rubber mat saying 'Welcome to Zork!' lies by the door.";
                        break;

                    case Commands.NORTH:
                    case Commands.SOUTH:
                    case Commands.EAST:
                    case Commands.WEST:
                        if (Move(command))
                        {
                            outputString = $"You moved {command}.";
                        }
                        else
                        {
                            outputString = "The way is shut!";
                        }
                        break;

                    default:
                        outputString = "Unknown command.";
                        break;
                }

                Console.WriteLine(outputString);
            }
        }

        private static string[,] Rooms =
        {
            {"Rocky Trail", "South of House", "Canyon View" },
            {"Forest", "West of House", "Behind House"},
            {"Dense Woods", "North of House", "Clearing" }
        };

        private static bool Move(Commands command)
        {
            bool didMove = false;

            switch (command)
            {
                case Commands.NORTH when location.Row < Rooms.GetLength(0) - 1:
                    location.Row++;
                    didMove = true;
                    break;

                case Commands.SOUTH when location.Row > 0:
                    location.Row--;
                    didMove = true;
                    break;

                case Commands.EAST when location.Column < Rooms.GetLength(1) - 1:
                    location.Column++;
                    didMove = true;
                    break;

                case Commands.WEST when location.Column > 0:
                    location.Column--;
                    didMove = true;
                    break;
            }

            return didMove;
        }

        private static Commands ToCommand(string commandString) => (Enum.TryParse<Commands>(commandString, true, out Commands result) ? result : Commands.UNKNOWN);

        private static (int Row, int Column) location = (1, 1);

    }
}