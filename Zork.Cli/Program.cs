using System;
using System.IO;
using Newtonsoft.Json;
using Zork.Common;

namespace Zork.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            const string defaultRoomsFilename = @"Content\Game.json";
            string gameFilename = (args.Length > 0 ? args[(int)CommandLineArguments.GameFilename] : defaultRoomsFilename);
            Game game = JsonConvert.DeserializeObject<Game>(File.ReadAllText(gameFilename));

            var output = new ConsoleOutputService();
            var input = new ConsoleInputService();

            input.InputReceived += Input_InputReceived;


            Console.WriteLine("Welcome to Zork!");
            game.Run(output, input);
            Console.WriteLine("Finished.");
        }

        public static void Input_InputReceived(object sender, string inputString)
        {
            throw new NotImplementedException();
        }
        private enum CommandLineArguments
        {
            GameFilename = 0
        }
    }
}