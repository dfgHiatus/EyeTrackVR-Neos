using HarmonyLib;
using NeosModLoader;
using FrooxEngine;
using BaseX;
using System;

namespace Neos_OpenSeeFace_Integration
{
	public class Neos_OpenSeeFace_Integration : NeosMod
	{
		public override string Name => "Neos-WCFace-Integration";
		public override string Author => "dfgHiatus";
		public override string Version => "1.0.0";
		public override string Link => "https://github.com/dfgHiatus/Neos-Eye-Face-API/";
		public override void OnEngineInit()
		{
			// Harmony.DEBUG = true;
			Harmony harmony = new Harmony("net.dfgHiatus.Neos-Eye-Face-API");
			harmony.PatchAll();
		}

		[HarmonyPatch(typeof(InputInterface), MethodType.Constructor)]
		[HarmonyPatch(new[] { typeof(Engine)})]
		public class InputInterfaceCtorPatch
		{
			public static void Postfix(InputInterface __instance)
			{
				try
				{
					GenericInputDevice gen = new GenericInputDevice();
					Debug("Module Name: " + gen.ToString());
					__instance.RegisterInputDriver(gen);
				}
				catch (Exception e)
				{
					Warn("Module failed to initiallize.");
					Warn(e.ToString());
				}
			}
		}
	}

	class GenericInputDevice : IInputDriver
	{
		public Eyes eyes;
		public Mouth mouth;
		public GenericDevice.EyeInterface.EyeData eyeInt = new GenericDevice.EyeInterface.EyeData();
		public GenericDevice.MouthInterface.MouthData mouthInt = new GenericDevice.MouthInterface.MouthData();
		public int UpdateOrder => 100;

		public void CollectDeviceInfos(BaseX.DataTreeList list)
        {
			DataTreeDictionary EyeDataTreeDictionary = new DataTreeDictionary();
			EyeDataTreeDictionary.Add("Name", "Generic Eye Tracking");
			EyeDataTreeDictionary.Add("Type", "Eye Tracking");
			EyeDataTreeDictionary.Add("Model", "Generic Eye Model");
			list.Add(EyeDataTreeDictionary);

			DataTreeDictionary MouthDataTreeDictionary = new DataTreeDictionary();
			MouthDataTreeDictionary.Add("Name", "Generic Face Tracking");
			MouthDataTreeDictionary.Add("Type", "Face Tracking");
			MouthDataTreeDictionary.Add("Model", "Generic Face Model");
			list.Add(MouthDataTreeDictionary);
		}

		public void RegisterInputs(InputInterface inputInterface)
		{
			eyes = new Eyes(inputInterface, "Generic Eye Tracking");
			mouth = new Mouth(inputInterface, "Generic Mouth Tracking");
		}

		public void UpdateInputs(float deltaTime)
        {
			UpdateEyes();
			UpdateMouth();
		}

		// See EyeInterface.cs for how to update these values
		public void UpdateEyes()
        {
			eyes.LeftEye.IsDeviceActive = eyeInt.LeftIsDeviceActive;
			eyes.RightEye.IsDeviceActive = eyeInt.RightIsDeviceActive;
			eyes.CombinedEye.IsDeviceActive = eyeInt.CombinedIsDeviceActive;

			eyes.LeftEye.IsTracking = eyeInt.LeftIsTracking;
			eyes.RightEye.IsTracking = eyeInt.RightIsDeviceActive;
			eyes.CombinedEye.IsTracking = eyeInt.CombinedIsDeviceActive;

			eyes.Timestamp = eyeInt.Timestamp;

			eyes.LeftEye.Squeeze = eyeInt.LeftSqueeze;
			eyes.RightEye.Squeeze = eyeInt.RightSqueeze;
			eyes.RightEye.Squeeze = eyeInt.CombinedSqueeze;

			eyes.LeftEye.Widen = eyeInt.LeftWiden;
			eyes.RightEye.Widen = eyeInt.RightWiden;
			eyes.CombinedEye.Widen = eyeInt.CombinedWiden;

			eyes.LeftEye.Frown = eyeInt.LeftFrown;
			eyes.RightEye.Frown = eyeInt.RightFrown;
			eyes.CombinedEye.Frown = eyeInt.CombinedFrown;

			eyes.LeftEye.Openness = eyeInt.LeftOpenness;
			eyes.RightEye.Openness = eyeInt.RightOpenness;
			eyes.CombinedEye.Openness = eyeInt.CombinedOpenness;

			eyes.LeftEye.PupilDiameter = eyeInt.LeftPupilDiameter;
			eyes.RightEye.PupilDiameter = eyeInt.RightPupilDiameter;
			eyes.CombinedEye.PupilDiameter = eyeInt.CombinedPupilDiameter;
		}

		// See MouthInterface.cs for how to update these values
		public void UpdateMouth()
        {
			mouth.IsDeviceActive = mouthInt.IsDeviceActive;

			mouth.IsTracking = mouthInt.IsTracking;

			mouth.Jaw = mouthInt.Jaw;
			mouth.Tongue = mouthInt.Tongue;

			mouth.JawOpen = mouthInt.JawOpen;
			mouth.MouthPout = mouthInt.MouthPout;
			mouth.TongueRoll = mouthInt.TongueRoll;

			mouth.LipBottomOverUnder = mouthInt.LipBottomOverUnder;
			mouth.LipBottomOverturn = mouthInt.LipBottomOverturn;
			mouth.LipTopOverUnder = mouthInt.LipTopOverUnder;
			mouth.LipTopOverturn = mouthInt.LipTopOverturn;

			mouth.LipLowerHorizontal = mouthInt.LipLowerHorizontal;
			mouth.LipUpperHorizontal = mouthInt.LipUpperHorizontal;

			mouth.LipLowerLeftRaise = mouthInt.LipLowerLeftRaise;
			mouth.LipLowerRightRaise = mouthInt.LipLowerRightRaise;
			mouth.LipUpperRightRaise = mouthInt.LipUpperRightRaise;
			mouth.LipUpperLeftRaise = mouthInt.LipUpperLeftRaise;

			mouth.MouthRightSmileFrown = mouthInt.MouthRightSmileFrown;
			mouth.MouthLeftSmileFrown = mouthInt.MouthLeftSmileFrown;
			mouth.CheekLeftPuffSuck = mouthInt.CheekLeftPuffSuck;
			mouth.CheekRightPuffSuck = mouthInt.CheekRightPuffSuck;

		}
	}
}
