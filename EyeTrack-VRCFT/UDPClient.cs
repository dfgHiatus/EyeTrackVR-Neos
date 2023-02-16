using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace EyeTrack_VRCFT
{
    public class UDPClient
    {
        public ETVRData Data;
        
        private UdpClient _client;
        private Thread _thread;
        private CancellationTokenSource _token;

        public UDPClient(int? port = 8000)
        {
            if (port.HasValue)
                _client = new UdpClient(port.Value);
            else
                _client = new UdpClient(8000);
            
            _thread = new Thread(new ThreadStart(ListenLoop));
            _thread.Start();
        }

        private void ListenLoop()
        {
            while (!_token.IsCancellationRequested)
            {
                var result = _client.ReceiveAsync();
                result.Wait();
                var message = Encoding.ASCII.GetString(result.Result.Buffer);
                
                float candidate;
                switch (message)
                {
                    case "/avatar/parameters/LeftEye":
                        float.TryParse(message[0].ToString(), out candidate);
                        Data.LeftEyeX = candidate;
                        break;
                    case "/avatar/parameters/RightEye":
                        float.TryParse(message[0].ToString(), out candidate);
                        Data.RightEyeX = candidate;
                        break;
                    case "/avatar/parameters/EyesY":
                        float.TryParse(message[0].ToString(), out candidate);
                        Data.EyesY = candidate;
                        break;
                    case "/avatar/parameters/LeftEyeLid":
                        float.TryParse(message[0].ToString(), out candidate);
                        Data.LeftEyeLid = candidate;
                        break;
                    case "/avatar/parameters/RightEyeLid":
                        float.TryParse(message[0].ToString(), out candidate);
                        Data.RightEyeLid = candidate;
                        break;
                    case "/avatar/parameters/EyesDilation":
                        float.TryParse(message[0].ToString(), out candidate);
                        Data.EyeDilation = candidate;
                        break;
                    default:
                        break;
                }
            }
        }

        public void Teardown()
        {
            _token.Cancel();
            _token.Dispose();
            _thread.Abort();
            _client.Close();
            _client.Dispose();
        }
    }
}
