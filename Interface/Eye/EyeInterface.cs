using System;
using System.Collections.Generic;
using System.Threading;
using WebSocketSharp;
using HarmonyLib;
using NeosModLoader;
using FrooxEngine;
using BaseX;

namespace GenericDevice
{
    public class EyeInterface
    {
        // Put your data here :)
        // Feel free to use MathX to make your life easier
        public class EyeData
        {
			// If the device should stream information
			// Useful say if a VR user transitions to Desktop Mode
			public bool LeftIsDeviceActive = false;
			public bool RightIsDeviceActive = false;
			public bool CombinedIsDeviceActive = false;

			// If the tracking is reliable enough to be used
			public bool LeftIsTracking = false;
			public bool RightIsTracking = false;
			public bool CombinedIsTracking = false;

			// Timestamp is long
			public long Timestamp = 0;

			// Normalized 0 to 1
			public float LeftSqueeze = 0f;
			public float RightSqueeze = 0f;
			public float CombinedSqueeze = 0f;

			public float LeftWiden = 0f;
			public float RightWiden = 0f;
			public float CombinedWiden = 0f;

			public float LeftFrown = 0f;
			public float RightFrown = 0f;
			public float CombinedFrown = 0f;

			public float LeftOpenness = 0f;
			public float RightOpenness = 0f;
			public float CombinedOpenness = 0f;

			// PupilDiameter is in mm
			public float LeftPupilDiameter = 0f;
			public float RightPupilDiameter = 0f;
			public float CombinedPupilDiameter = 0f;


        }
    }
}