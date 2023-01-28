﻿using Rug.Osc;
using System;
using System.Net;
using System.Threading;

namespace EyeTrackVR
{
    // Credit to yewnyx on the VRC OSC Discord for this
    
    public class OSCClient
    {
        public static float LeftEyeX {get; set;}
        public static float RightEyeX { get; set; }
        public static float EyesY { get; set; }
        public static float LeftEyeLidExpandedSqueeze { get; set; }
        public static float RightEyeLidExpandedSqueeze { get; set; }
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
            IPAddress candidate;
            IPAddress.TryParse("127.0.0.1", out candidate);

            _receiver = new OscReceiver(candidate, DEFAULT_PORT);
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
            float candidate = 0;

            while (_receiver.State != OscSocketState.Closed) {
                try {
                    if (_receiver.State == OscSocketState.Connected) {
                        packet = _receiver.Receive();
                        if (OscMessage.TryParse(packet.ToString(), out message)) {
                            switch (message.Address) {
                                case "/avatar/parameters/LeftEye":
                                    float.TryParse(message[0].ToString(), out candidate);
                                    LeftEyeX = candidate;
                                    break;
                                case "/avatar/parameters/RightEye":
                                    float.TryParse(message[0].ToString(), out candidate);
                                    RightEyeX = candidate;
                                    break;
                                case "/avatar/parameters/EyesY":
                                    float.TryParse(message[0].ToString(), out candidate);
                                    EyesY = candidate;
                                    break;
                                case "/avatar/parameters/LeftEyeLidExpandedSqueeze":
                                    float.TryParse(message[0].ToString(), out candidate);
                                    LeftEyeLidExpandedSqueeze = candidate;
                                    break;
                                case "/avatar/parameters/RightEyeLidExpandedSqueeze":
                                    float.TryParse(message[0].ToString(), out candidate);
                                    RightEyeLidExpandedSqueeze = candidate;
                                    break;
                                case "/avatar/parameters/EyesDilation":
                                    float.TryParse(message[0].ToString(), out candidate);
                                    EyeDilation = candidate;
                                    break;
                                default:
                                    break;
                            }
                            PrintDebugString();
                        }
                    }
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
            Console.WriteLine("LeftEyeX: {0}, RightEyeX: {1}, EyesY: {2}, LeftEyeLidExpandedSqueeze: {3}, RightEyeLidExpandedSqueeze: {4}, EyeDilation: {5}", LeftEyeX, RightEyeX, EyesY, LeftEyeLidExpandedSqueeze, RightEyeLidExpandedSqueeze, EyeDilation);
        }
    }
}
