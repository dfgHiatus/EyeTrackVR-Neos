using HarmonyLib;
using NeosModLoader;
using FrooxEngine;
using BaseX;
using System;

namespace EyeTrackVR_Integration
{
	public class NeosEyeTrackVR : NeosMod
	{
		[AutoRegisterConfigKey]
		public static ModConfigurationKey<float> userPupilSize = new ModConfigurationKey<float>("user_Pupil_Size", "Pupil Size, mm.", () => 0.005f);
		
		public override string Name => "EyeTrackVR-Neos";
		public override string Author => "dfgHiatus";
		public override string Version => "1.0.0";
		public override string Link => "https://github.com/dfgHiatus/EyeTrackVR-Neos";
		public override void OnEngineInit()
		{
			// Harmony.DEBUG = true;
			new Harmony("net.dfgHiatus.Neos-Eye-Face-API").PatchAll();
		}

		[HarmonyPatch(typeof(InputInterface), MethodType.Constructor)]
		[HarmonyPatch(new[] { typeof(Engine)})]
		public class InputInterfaceCtorPatch
		{
			public static void Postfix(InputInterface __instance)
			{
				try
				{
					EyeTrackVRInputDevice gen = new EyeTrackVRInputDevice();
					__instance.RegisterInputDriver(gen);
				}
				catch (Exception e)
				{
					Warn("EyeTrackVR failed to initiallize with the following error:");
					Warn(e.ToString());
				}
			}
		}
	}

	class EyeTrackVRInputDevice : IInputDriver
	{
		public Eyes eyes;
		public GenericDevice.EyeInterface.EyeData eyeInt = new GenericDevice.EyeInterface.EyeData();
		public int UpdateOrder => 100;

		public void CollectDeviceInfos(BaseX.DataTreeList list)
        {
			DataTreeDictionary EyeDataTreeDictionary = new DataTreeDictionary();
			EyeDataTreeDictionary.Add("Name", "EyeTrackVR Eye Tracking");
			EyeDataTreeDictionary.Add("Type", "Eye Tracking");
			EyeDataTreeDictionary.Add("Model", "Model 1");
			list.Add(EyeDataTreeDictionary);
		}

		public void RegisterInputs(InputInterface inputInterface)
		{
			eyes = new Eyes(inputInterface, "EyeTrackVR Tracking");
		}

		public void UpdateInputs(float deltaTime)
        {
			eyes.IsEyeTrackingActive = Engine.Current.InputInterface.VR_Active;

			UpdateEye(gazeData.leftEye, leftStatus, config.GetValue(userPupilSize), 1f, deltaTime, eyes.LeftEye);
			UpdateEye(gazeData.rightEye, rightStatus, config.GetValue(userPupilSize), 1f, deltaTime, eyes.RightEye);

			var combinedGaze = MathX.Average(eyes.LeftEye.Direction, eyes.RightEye.Direction);
			var combinedStatus = eyes.LeftEye.IsTracking || eyes.RightEye.IsTracking;

			UpdateEye(combinedGaze, combinedStatus, config.GetValue(userPupilSize), 1f, deltaTime, eyes.CombinedEye);

			eyes.ComputeCombinedEyeParameters();
			eyes.ConvergenceDistance = 0f;
			eyes.Timestamp += deltaTime;

			eyes.FinishUpdate();
		}

		private void UpdateEye(float2 data, bool status, float pupilSize, float openness, float deltaTime, Eye eye)
		{
			eye.IsDeviceActive = Engine.Current.InputInterface.VR_Active;
			eye.IsTracking = status;

			if (eye.IsTracking)
			{
				eye.UpdateWithDirection((float3)new double3(data.forward.x,
					data.forward.y,
					1f).Normalized);

				eye.RawPosition = float3.Zero;
				eye.PupilDiameter = pupilSize;
			}

			eye.Openness = openness;
			eye.Widen = (float)MathX.Clamp01(data.forward.y);
			eye.Squeeze = 0f;
			eye.Frown = 0f;
		}
	}
}
