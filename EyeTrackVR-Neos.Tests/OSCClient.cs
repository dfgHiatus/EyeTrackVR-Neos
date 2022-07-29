using Rug.Osc;

namespace EyeTrackVR
{
    public class OSCClient
    {
        public static float LeftEyeX {get; set;}
        public static float RightEyeX { get; set; }
        public static float EyesY { get; set; }
        public static float LeftEyeLid { get; set; }
        public static float RightEyeLid { get; set; }
        public static float EyeDilation { get; set; }

        private static OscReceiver _receiver;
        private static Thread _thread;
        private const int DEFAULT_PORT = 9000;

        public OSCClient()
        {
            if (_receiver != null)
            {
                return;
            }

            _receiver = new OscReceiver(DEFAULT_PORT);
            _thread = new Thread(new ThreadStart(ListenLoop));
            _receiver.Connect();
            _thread.Start();
        }

        public OSCClient(int port)
        {
            if (_receiver != null)
            {
                return;
            }

            _receiver = new OscReceiver(port);
            _thread = new Thread(new ThreadStart(ListenLoop));
            _receiver.Connect();
            _thread.Start();
        }

        private static void ListenLoop()
        {
            OscPacket packet;
            OscMessage message;
            float canidate = 0;

            while (_receiver.State != OscSocketState.Closed) {
                try {
                    if (_receiver.State == OscSocketState.Connected) {
                        packet = _receiver.Receive();
                        if (OscMessage.TryParse(packet.ToString(), out message)) {
                            switch (message.Address) {
                                case "/avatar/parameters/LeftEye":
                                    float.TryParse(message[0].ToString(), out canidate);
                                    LeftEyeX = canidate;
                                    break;
                                case "/avatar/parameters/RightEye":
                                    float.TryParse(message[0].ToString(), out canidate);
                                    RightEyeX = canidate;
                                    break;
                                case "/avatar/parameters/EyesY":
                                    float.TryParse(message[0].ToString(), out canidate);
                                    EyesY = canidate;
                                    break;
                                case "/avatar/parameters/LeftEyeLid":
                                    float.TryParse(message[0].ToString(), out canidate);
                                    LeftEyeLid = canidate;
                                    break;
                                case "/avatar/parameters/RightEyeLid":
                                    float.TryParse(message[0].ToString(), out canidate);
                                    RightEyeLid = canidate;
                                    break;
                                case "/avatar/parameters/EyesDilation":
                                    float.TryParse(message[0].ToString(), out canidate);
                                    EyeDilation = canidate;
                                    break;
                                default:
                                    break;
                            }
                            PrintDebugString();
                        }
                    }
                    Thread.Sleep(10);
                }
                catch (Exception e)
                {
                    if (_receiver.State == OscSocketState.Connected) 
                        Console.Error.WriteLine(e.Message);
                }
            }
        }

        public void Teardown()
        {
            _receiver.Close();
            _thread.Join();
        }

        private static void PrintDebugString()
        {
            Console.WriteLine("LeftEyeX: {0}, RightEyeX: {1}, EyesY: {2}, LeftEyeLid: {3}, RightEyeLid: {4}, EyeDilation: {5}", LeftEyeX, RightEyeX, EyesY, LeftEyeLid, RightEyeLid, EyeDilation);
        }
    }
}
