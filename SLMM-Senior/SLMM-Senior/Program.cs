using System;
using System.Net.Http;
using System.Text;

using Newtonsoft.Json;
using System.Linq;

namespace SLMM_Senior
{
    class Program
    {
        private static MowerSession session;
        private static readonly string baseUrl = "http://localhost:52513/SLMM/";

        private static bool exit = false;
        private static bool isDataValidLawn = false;
        private static bool isDataValidMower = false;

        static void Main()
        {
            while(isDataValidLawn == false) {
                LawnDimensions();
            }

            while (isDataValidMower == false) {
                MowerPosition();
            }

            while (exit == false ) {
                CommandInput();
            }
        }

        private static void LawnDimensions()
        {

            Console.WriteLine("Please enter the diemensions of the lawn (width, height), type exit to finish");
            string input = Console.ReadLine().Trim();

            if(input.Trim().ToLower() == "exit") {
                isDataValidLawn = true;
                exit = true;
                return;
            }

            int width = 0;
            int height = 0;

            string validationResult = Validation(input, ref width, ref height);

            if (validationResult == string.Empty)
            {
                ClientCall("lawndimensions", width, height);
                isDataValidLawn = true;
            }
            else {
                Console.WriteLine(validationResult);
            }
        }

        private static void MowerPosition()
        {
            if(exit == true) {
                isDataValidMower = true;
                return;
            }

            Console.WriteLine("Please enter the position of the mower (X, Y), type exit to finish");
            string input = Console.ReadLine().Trim();

            isDataValidMower = false;

            if (input.Trim().ToLower() == "exit") {
                isDataValidMower = true;
                exit = true;
                return;
            }

            int positionX = 0;
            int positionY = 0;

            string validationResult = Validation(input, ref positionX, ref positionY);

            if (validationResult == string.Empty)
            {
                ClientCall("mowerposition", positionX, positionY);
                isDataValidMower = true;
            }
            else {
                Console.WriteLine(validationResult);
            }
        }

        private static void CommandInput()
        {
            Console.WriteLine("Please enter a command.");
            string command = Console.ReadLine().Trim();

            switch(command.ToUpper())
            {
                case "ML": // Move Left
                    ClientCall("moveleft");
                    break;

                case "MR": // Move Right
                    ClientCall("moveright");
                    break;

                case "MU": // Move Up
                    ClientCall("moveup");
                    break;

                case "MD": // Move Down
                    ClientCall("movedown");
                    break;

                case "EXIT": // Exit the program
                    exit = true;
                    break;

                default: // deals with unknown commands
                    Console.WriteLine("Command not found.");
                    break;
            }
        }

        private static void ClientCall(string actionName, int x = 0, int y = 0)
        {
            var content = new StringContent(JsonConvert.SerializeObject(session), Encoding.UTF8, "application/json");
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = new HttpResponseMessage();

                if(x > 0 && y > 0) {
                    response = client.PostAsync(baseUrl + actionName + "/" + x + "/" + y, content).Result;
                }
                else {
                    response = client.PostAsync(baseUrl + actionName + "/", content).Result;
                }

                session = JsonConvert.DeserializeObject<MowerSession>(response.Content.ReadAsStringAsync().Result);
                Console.WriteLine(session.Status);
            }
        }

        private static string Validation(string input, ref int value1, ref int value2)
        {
            if (input.Count(i => i == ',') == 1)
            {
                string[] values = input.Split(',');

                int checkvalue1;
                int checkvalue2;

                if (int.TryParse(values[0], out checkvalue1) == false) {
                    return "Please enter numbers for the width.";
                }

                if (int.TryParse(values[1], out checkvalue2) == false) {
                    return "Please enter numbers for the height.";
                }

                if(checkvalue1 <= 0 || checkvalue2 <= 0) {
                    return "Please enter positive numbers";
                }

                value1 = checkvalue1;
                value2 = checkvalue2;

                return string.Empty;
            }
            else {
                return "Data isn't in correct format.";
            }
        }
    }
}
