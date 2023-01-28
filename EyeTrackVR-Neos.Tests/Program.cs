using System;

namespace EyeTrackVR.Tests
{
    public class Program
    {
        public static void Main(string[] args)
        {
            OSCClient client = new OSCClient();
            Console.WriteLine("Press any key to exit");
            Console.ReadKey(true);
            client.Teardown();
        }
    }
}