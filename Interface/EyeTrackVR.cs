using HarmonyLib;
using NeosModLoader;
using FrooxEngine;
using BaseX;
using System;

namespace EyeTrackVR
{
	public class EyeTrackVR : NeosMod
	{
		public override string Name => "EyeTrackVR-Neos";
		public override string Author => "dfgHiatus";
		public override string Version => "1.0.0";
		public override string Link => "https://github.com/dfgHiatus/EyeTrackVR-Neos";
		public override void OnEngineInit()
		{
			new Harmony("net.dfgHiatus.EyeTrackVR-Neos").PatchAll();
		}

		[HarmonyPatch(typeof(InputInterface), MethodType.Constructor)]
		[HarmonyPatch(new[] { typeof(Engine) })]
		public class InputInterfaceCtorPatch
		{
			public static void Postfix(InputInterface __instance)
			{
				try
				{
					EyeTrackVRInterface gen = new EyeTrackVRInterface();
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


	[HarmonyPatch(typeof(Engine), "Shutdown")]
	public class ShutdownPatch
	{
		public static bool Prefix()
		{
			// Hardware shutdown code goes here
			// ...
			// ...
			return true;
		}
	}

	public class EyeTrackVRInterface : IInputDriver
	{
		private Eyes _eyes;
		public int UpdateOrder => 100;

		public void CollectDeviceInfos(DataTreeList list)
		{
			DataTreeDictionary eyeDataTreeDictionary = new DataTreeDictionary();
			eyeDataTreeDictionary.Add("Name", "EyeTrackVR Eye Tracking");
			eyeDataTreeDictionary.Add("Type", "Eye Tracking");
			eyeDataTreeDictionary.Add("Model", "Single/Dual Cam Model"); // TODO Discern?
			list.Add(eyeDataTreeDictionary);
		}

		public void RegisterInputs(InputInterface inputInterface)
		{
			_eyes = new Eyes(inputInterface, "EyeTrackVR Eye Tracking");
		}

		public void UpdateInputs(float deltaTime)
		{
			_eyes.IsEyeTrackingActive = _eyes.IsEyeTrackingActive;

			UpdateEye(float3.Zero, float3.Zero, true, 0.003f,
				1f, 0f, 0f, 0f, deltaTime, _eyes.LeftEye);
			UpdateEye(float3.Zero, float3.Zero, true, 0.003f,
				1f, 0f, 0f, 0f, deltaTime, _eyes.RightEye);

			UpdateEye(float3.Zero, float3.Zero, true, 0.003f,
				1f, 0f, 0f, 0f, deltaTime, _eyes.CombinedEye);
			_eyes.ComputeCombinedEyeParameters();

			_eyes.ConvergenceDistance = 0f;
			_eyes.Timestamp += deltaTime;
			_eyes.FinishUpdate();
		}
		private void UpdateEye(float3 gazeDirection, float3 gazeOrigin, bool status, float pupilSize, float openness,
			float widen, float squeeze, float frown, float deltaTime, Eye eye)
		{
			eye.IsDeviceActive = Engine.Current.InputInterface.VR_Active;
			eye.IsTracking = status;

			if (eye.IsTracking)
			{
				eye.UpdateWithDirection(gazeDirection);
				eye.RawPosition = gazeOrigin;
				eye.PupilDiameter = pupilSize;
			}

			eye.Openness = openness;
			eye.Widen = widen;
			eye.Squeeze = squeeze;
			eye.Frown = frown;
		}
	}
}
