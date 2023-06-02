using BaseX;
using OscCore;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace EyeTrackVR
{
    // Credit to yewnyx on the VRC OSC Discord for this

    public class ETVROSC
    {
        public static bool OscSocketState;
        public static Dictionary<string, float> EyeDataWithAddress = new Dictionary<string, float>();

        private static UdpClient? _receiver;
        private static Task? _task;

        private const int DEFAULT_PORT = 9000;

        public ETVROSC(int? port = null)
        {
            if (_receiver != null)
            {
                return;
            }

            IPAddress candidate;
            IPAddress.TryParse("127.0.0.1", out candidate);

            if (port.HasValue)
                _receiver = new UdpClient(new IPEndPoint(candidate, port.Value));
            else
                _receiver = new UdpClient(new IPEndPoint(candidate, DEFAULT_PORT));

            foreach (var shape in ETVRExpressions.EyeDataWithAddress)
                EyeDataWithAddress.Add(shape, 0f);

            OscSocketState = true;
            _task = Task.Run(() => ListenLoop());
        }

        private static async void ListenLoop()
        {
            UniLog.Log("Started EyeTrackVR loop");
            while (OscSocketState)
            {
                var result = await _receiver.ReceiveAsync();
                OscMessage message = OscMessage.Read(result.Buffer, 0, result.Buffer.Length);
                if (!EyeDataWithAddress.ContainsKey(message.Address))
                {
                    continue;
                }
                if (float.TryParse(message[0].ToString(), out float candidate))
                {
                    EyeDataWithAddress[message.Address] = candidate;
                }

            }
        }

        public void Teardown()
        {
            UniLog.Log("EyeTrackVR teardown called");
            OscSocketState = false;
            _receiver.Close();
            _task.Wait();
            UniLog.Log("EyeTrackVR teardown completed");
        }
    }
}
