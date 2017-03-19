using System;
using System.Net.Http;
using System.Text;

using Newtonsoft.Json;
using System.Linq;

namespace SLMM_Senior
{
    class Program
    {
        // This the mower session object that is passed back over to the server
        private static MowerSession session;

        // This is the base url accessing the REST API 
        private static readonly string baseUrl = "http://localhost:52513/SLMM/";

        // This boolean indicates if the user wants to exit the program 
        private static bool exit = false;

        // This bool indicates if the lawn dimensions are valid 
        // and depending on the value it decides if the user can move on
        private static bool isDataValidLawn = false;

        // This bool indicates if the mower position are valid 
        // and depending on the value it decides if the user can move on
        private static bool isDataValidMower = false;

        // Entry point of the program
        static void Main()
        {
            // This makes a looped call to the LawnDimensions method till the data is valid
            while(isDataValidLawn == false) {
                LawnDimensions();
            }

            // This makes a looped call to the MowerPosition method till the data is valid
            while (isDataValidMower == false) {
                MowerPosition();
            }
            // This makes a looped call to the CommandInput method till the user wants to exit
            while (exit == false ) {
                CommandInput();
            }
        }

        private static void LawnDimensions()
        {
            // Asks the user to enter in the lawn dimensiosn
            Console.WriteLine("Please enter the diemensions of the lawn (width, height), type exit to finish");

            // The input is taken in and all white space is removed 
            string input = Console.ReadLine().Trim();

            // if the user types in exit then the program will close at this point
            if(input.Trim().ToLower() == "exit") {
                isDataValidLawn = true;
                exit = true;
                return;
            }

            // initalizes the width and height variables that will be referenced 
            // if the call to Validation is passed then the values passed into the 
            // console will end up here
            int width = 0;
            int height = 0;

            // This makes the call to the validation method to make sure the input is correct
            // the results are then passed to the initalized string
            string validationResult = Validation(input, ref width, ref height);

            // If the string is empty then the call is made server using the ServerCall method
            // and the method call ends here
            if (validationResult == string.Empty)
            {
                ServerCall("lawndimensions", width, height);
                isDataValidLawn = true;
            }
            else { // Else the error message is written out and the user is asked to enter the details again
                Console.WriteLine(validationResult);
            }
        }

        private static void MowerPosition()
        {
            // if the user has already typed exit in the previous method call 
            // then the program leaves this method
            if (exit == true) {
                isDataValidMower = true;
                return;
            }

            // Asks the user to enter in the mower position
            Console.WriteLine("Please enter the position of the mower (X, Y), type exit to finish");

            // The input is taken in and all white space is removed 
            string input = Console.ReadLine().Trim();

            // this is set to false intially and changed if wrong
            isDataValidMower = false;

            // if the user types in exit then the program will close at this point
            if (input.Trim().ToLower() == "exit") {
                isDataValidMower = true;
                exit = true;
                return;
            }

            // initalizes the position X and position Y variables that will be referenced 
            // if the call to Validation is passed then the values passed into the 
            // console will end up here
            int positionX = 0;
            int positionY = 0;

            // This makes the call to the validation method to make sure the input is correct
            // the results are then passed to the initalized string
            string validationResult = Validation(input, ref positionX, ref positionY);

            // If the string is empty then the call is made server using the ServerCall method 
            // and the method call ends
            if (validationResult == string.Empty)
            {
                ServerCall("mowerposition", positionX, positionY);
                isDataValidMower = true;
            }
            else { // Else the error message is written out and the user is asked to enter the details again
                Console.WriteLine(validationResult);
            }
        }

        private static void CommandInput()
        {
            // This asks the user to enter the command they want the mower to carry out
            Console.WriteLine("Please enter a command.");
            string command = Console.ReadLine().Trim();

            switch(command.ToUpper())
            {
                case "ML": // Move Left
                    ServerCall("moveleft");
                    break;

                case "MR": // Move Right
                    ServerCall("moveright");
                    break;

                case "MU": // Move Up
                    ServerCall("moveup");
                    break;

                case "MD": // Move Down
                    ServerCall("movedown");
                    break;

                case "EXIT": // Exit the program
                    exit = true;
                    break;

                default: // deals with unknown commands
                    Console.WriteLine("Command not found.");
                    break;
            }
        }

        // This method makes the call to the server side of the application 
        // and sends over the current session information,
        private static void ServerCall(string actionName, int x = 0, int y = 0)
        {
            // This formats the json object 
            var content = new StringContent(JsonConvert.SerializeObject(session), Encoding.UTF8, "application/json");

            // This setups up a http client so that the program can connect to the server side application
            using (HttpClient client = new HttpClient())
            {
                // This is used receive the response and parse it back into the program
                HttpResponseMessage response = new HttpResponseMessage();

                if(x > 0 && y > 0) { // This is the call made if the user needs to pass the x and y parameter
                    response = client.PostAsync(baseUrl + actionName + "/" + x + "/" + y, content).Result;
                }
                else { // This is the call made if it's just a url call
                    response = client.PostAsync(baseUrl + actionName + "/", content).Result;
                }

                // this deserializes the response and passes the new details it received from the server as 
                // the mower session object
                session = JsonConvert.DeserializeObject<MowerSession>(response.Content.ReadAsStringAsync().Result);

                // This writes the status passed back from the server application
                Console.WriteLine(session.Status);
            }
        }

        // This method validates the input the user puts in for the lawn dimensions and 
        // the mower position
        private static string Validation(string input, ref int value1, ref int value2)
        {
            // Checks to see if the comma is there only comma in the string, I used count as opposed to contains 
            // as contains only checks to see if there is a comma meaning multiple commas can be in there and still be correct
            // while the count specifys only one comma is neeeded   
            if (input.Count(i => i == ',') == 1)
            {
                // Splits the string in two, the first value is supposed to be the width 
                // and the second supposed to be the right
                string[] values = input.Split(',');

                // These are the ints that will get the values back if they are correct
                int checkvalue1;
                int checkvalue2;

                // This checks to see if it is a number and if it isn't then it 
                // sends back an approrpiate message and if it is then assigns it as the 
                // max width of the lawn
                if (int.TryParse(values[0], out checkvalue1) == false) {
                    return "Please enter numbers for the width.";
                }

                // This checks to see if it is a number and if it isn't then it 
                // sends back an approrpiate message and if it is then assigns it as the 
                // max height of the lawn
                if (int.TryParse(values[1], out checkvalue2) == false) {
                    return "Please enter numbers for the height.";
                }


                // This makes sure that the numbers entered in by the user are 
                // positive numbers
                if(checkvalue1 <= 0 || checkvalue2 <= 0) {
                    return "Please enter positive numbers";
                }

                // If everthing is fine then the checked values 
                // are passed onto value 1 and 2 to be outputted to the 
                // ints outside the method
                value1 = checkvalue1;
                value2 = checkvalue2;


                // If nothing is wrong then I simply send back an empty string
                return string.Empty;
            }
            else { //if the string doesn't have a comma then it invalid and a message is sent
                return "Data isn't in correct format.";
            }
        }
    }
}
