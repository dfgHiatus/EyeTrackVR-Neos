using BaseX;
using Rug.Osc;
using System;
using System.Threading;

namespace EyeTrackVR
{
    public class ETVR_OSC
    {
        public static float LeftEyeX { get; set; }
        public static float RightEyeX { get; set; }
        public static float EyesY { get; set; }
        public static float LeftEyeLid { get; set; }
        public static float RightEyeLid { get; set; }
        public static float EyeDilation { get; set; }

        private static OscReceiver _receiver;
        private static Thread _thread;
        private const int DEFAULT_PORT = 9000;

        public ETVR_OSC()
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

        public ETVR_OSC(int port)
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

            while (_receiver.State != OscSocketState.Closed)
            {
                try
                {
                    if (_receiver.State == OscSocketState.Connected)
                    {
                        packet = _receiver.Receive();
                        if (OscMessage.TryParse(packet.ToString(), out message))
                        {
                            switch (message.Address)
                            {
                                case "/avatar/parameters/LeftEye":
                                    LeftEyeX = (float)message[0];
                                    break;
                                case "/avatar/parameters/RightEye":
                                    RightEyeX = (float)message[0];
                                    break;
                                case "/avatar/parameters/EyesY":
                                    EyesY = (float)message[0];
                                    break;
                                case "/avatar/parameters/LeftEyeLid":
                                    LeftEyeLid = (float)message[0];
                                    break;
                                case "/avatar/parameters/RightEyeLid":
                                    RightEyeLid = (float)message[0];
                                    break;
                                case "/avatar/parameters/EyesDilation":
                                    EyeDilation = (float)message[0];
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    Thread.Sleep(10);
                }
                catch (Exception e)
                {
                    if (_receiver.State == OscSocketState.Connected)
                        UniLog.Error(e.Message);
                }
            }
        }

        public void Teardown()
        {
            _receiver.Close();
            _thread.Join();
        }
    }
}
