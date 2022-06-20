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
    // Put your data here :)
    // Feel free to use MathX to make your life easier
    public class MouthInterface
    {
        public class MouthData
        {
			// If the device should stream information
			// Useful say if a VR user transitions to Desktop Mode
			public bool IsDeviceActive = false;

			// If the tracking is reliable enough to be used
			public bool IsTracking = false;

			public float3 Jaw = new float3();
			public float3 Tongue = new float3();

			// Normalized 0 to 1
			public float JawOpen = 0f;
			public float MouthPout = 0f;
			public float TongueRoll = 0f;

			public float LipBottomOverUnder = 0f;
			public float LipBottomOverturn = 0f;
			public float LipTopOverUnder = 0f;
			public float LipTopOverturn = 0f;

			public float LipLowerHorizontal = 0f;
			public float LipUpperHorizontal = 0f;

			public float LipLowerLeftRaise = 0f;
			public float LipLowerRightRaise = 0f;
			public float LipUpperRightRaise = 0f;
			public float LipUpperLeftRaise = 0f;

			// Normalized -1 to 1
			public float MouthRightSmileFrown = 0f;
			public float MouthLeftSmileFrown = 0f;
			public float CheekLeftPuffSuck = 0f;
			public float CheekRightPuffSuck = 0f;
        }
    }
}