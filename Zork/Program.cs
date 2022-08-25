using System;

namespace Zork
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Zork!");

            Commands command = Commands.UNKNOWN;
            while (command != Commands.QUIT)
            {
                Console.Write("> ");
                command = ToCommand(Console.ReadLine().Trim());

                string outputString;
                switch (command)
                {
                    case Commands.QUIT:
                        outputString = "Thank you for playing!\nPress any key to continue . . .";
                        break;

                    case Commands.NORTH:
                        outputString = "You moved NORTH.";
                        break;

                    case Commands.SOUTH:
                        outputString = "You moved SOUTH.";
                        break;

                    case Commands.WEST:
                        outputString = "You moved WEST.";
                        break;

                    case Commands.EAST:
                        outputString = "You moved EAST.";
                        break;

                    case Commands.LOOK:
                        outputString = ("This is an open field west of a white house, with a boarded front door.\nA rubber mat saying 'Welcome to Zork!' lies by the door.");
                        break;

                    default:
                        outputString = "Unknown command.";
                        break;
                }

                Console.WriteLine(outputString);
            }
        }

        private static Commands ToCommand(string commandString) => (Enum.TryParse<Commands>(commandString, true, out Commands result) ? result : Commands.UNKNOWN);
    }
}