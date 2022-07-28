using System;
using System.Threading;
using Rug.Osc;

namespace EyeTrackVR
{
    public class OSCClient
    {
        public float LeftEyeX = 0f;
        public float RightEyeX = 0f;

        public float EyeY = 0f;

        public float EyeDilation = 0f;

        private static OscReceiver _receiver = null;
        private static Thread _thread = null;
        private const int DEFAULT_PORT = 9001;

        public OSCClient()
        {
            _receiver = new OscReceiver(DEFAULT_PORT);
            _thread = new Thread(new ThreadStart(ListenLoop));
            _receiver.Connect();
            _thread.Start();
        }

        public OSCClient(int port)
        {
            _receiver = new OscReceiver(port);
            _thread = new Thread(new ThreadStart(ListenLoop));
            _receiver.Connect();
            _thread.Start();
        }

        private static void ListenLoop()
        {
            try
            {
                while (_receiver.State != OscSocketState.Closed)
                {
                    if (_receiver.State == OscSocketState.Connected)
                    {
                        var packet = _receiver.Receive();
                        if (OscMessage.TryParse(packet.ToString(), out var message) && message.Address == "/1/fader1")
                        {
                            var x = (float)message[0];
                            Console.WriteLine($"pos: {x}");
                        }
                        else
                        {
                            Console.WriteLine("message.Address " + message.Address);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (_receiver.State == OscSocketState.Connected) { Console.Error.WriteLine(e.Message); }
            }
        }

        public void Teardown()
        {
            _receiver.Close();
            _thread.Join();
        }
    }
}
