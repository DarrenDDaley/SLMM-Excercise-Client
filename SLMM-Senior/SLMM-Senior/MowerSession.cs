namespace SLMM_Senior
{
    // This is the class object that is sent across to the client and received on the server with the updated information
    // Since REST is supposed to be a stateless API the use of sessions didn't seem like a good fit so instead, I took the 
    // approach of passing around an object from the server to the client and back again, I feel this works pretty effectively 
    public class MowerSession
    {
        // This is to store the width
        public int Width { get; set; }

        // This is to store the height
        public int Height { get; set; }

        // This is to store the X Position 
        public int PositionX { get; set; }

        // This is to store the Y Position
        public int PositionY { get; set; }

        // This is to store the last done action so that it can be displayed on the client side 
        public string Status { get; set; }

        // This is to store error messages
        public string Error { get; set; }
    }
}
