using System;
using System.Threading;
using VRCFaceTracking;

namespace EyeTrack_VRCFT
{
    public class ETVR : ExtTrackingModule
    {
        private UDPClient _client;

        public override (bool eyeSuccess, bool lipSuccess) Initialize(bool eye, bool lip)
        {
            _client = new UDPClient();
            return (true, false);
        }
        
        public override Action GetUpdateThreadFunc()
        {
            return () =>
            {
                while (true)
                {
                    Update();
                    Thread.Sleep(10);
                }
            };
        }

        private void Update()
        {
            UnifiedTrackingData.LatestEyeData.Left.Look.x = _client.Data.LeftEyeX;
            UnifiedTrackingData.LatestEyeData.Left.Look.y = _client.Data.EyesY;
            UnifiedTrackingData.LatestEyeData.Left.Openness = _client.Data.LeftEyeLid;
            
            UnifiedTrackingData.LatestEyeData.Right.Look.x = _client.Data.RightEyeX;
            UnifiedTrackingData.LatestEyeData.Right.Look.y = _client.Data.EyesY;
            UnifiedTrackingData.LatestEyeData.Right.Openness = _client.Data.RightEyeLid;

            UnifiedTrackingData.LatestEyeData.EyesDilation = _client.Data.EyeDilation;
        }

        public override void Teardown()
        {
            _client.Teardown();
        }
    }
}
