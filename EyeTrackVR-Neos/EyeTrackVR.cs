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
			config = GetConfiguration();
			new Harmony("net.dfgHiatus.EyeTrackVR-Neos").PatchAll();
			Engine.Current.OnShutdown += () => ETVR.Teardown();
		}
		private static ETVR_OSC ETVR;
		private static ModConfiguration config;

		[AutoRegisterConfigKey]
		public static ModConfigurationKey<float> Alpha = new ModConfigurationKey<float>("alpha", "Eye Swing Multiplier X", () => 1.0f);

		[AutoRegisterConfigKey]
		public static ModConfigurationKey<float> Beta = new ModConfigurationKey<float>("beta", "Eye Swing Multiplier Y", () => 1.0f);

		[HarmonyPatch(typeof(InputInterface), MethodType.Constructor)]
		[HarmonyPatch(new[] { typeof(Engine) })]
		public class InputInterfaceCtorPatch
		{
			public static void Postfix(InputInterface __instance)
			{
				try
				{
					ETVR = new ETVR_OSC();
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

		public class EyeTrackVRInterface : IInputDriver
		{
			private Eyes _eyes;
			private const float _defaultPupilSize = 0.0035f;
			public int UpdateOrder => 100;

			public void CollectDeviceInfos(DataTreeList list)
			{
				DataTreeDictionary eyeDataTreeDictionary = new DataTreeDictionary();
				eyeDataTreeDictionary.Add("Name", "EyeTrackVR Eye Tracking");
				eyeDataTreeDictionary.Add("Type", "Eye Tracking");
				eyeDataTreeDictionary.Add("Model", "ETVR Module");
				list.Add(eyeDataTreeDictionary);
			}

			public void RegisterInputs(InputInterface inputInterface)
			{
				_eyes = new Eyes(inputInterface, "EyeTrackVR Eye Tracking");
			}

			public void UpdateInputs(float deltaTime)
			{
				_eyes.IsEyeTrackingActive = Engine.Current.InputInterface.VR_Active || Engine.Current.InputInterface.ScreenActive;

				var fakeWiden = MathX.Remap(MathX.Clamp01(ETVR_OSC.EyesY), 0f, 1f, 0f, 0.33f);

				var leftEyeDirection = Project2DTo3D(ETVR_OSC.LeftEyeX, ETVR_OSC.EyesY);
				UpdateEye(leftEyeDirection, float3.Zero, true, ETVR_OSC.EyeDilation, ETVR_OSC.LeftEyeLidExpandedSqueeze, 
					fakeWiden, 0f, 0f, deltaTime, _eyes.LeftEye);

				var rightEyeDirection = Project2DTo3D(ETVR_OSC.RightEyeX, ETVR_OSC.EyesY);
				UpdateEye(rightEyeDirection, float3.Zero, true, ETVR_OSC.EyeDilation, ETVR_OSC.RightEyeLidExpandedSqueeze, 
					fakeWiden, 0f, 0f, deltaTime, _eyes.RightEye);

				var combinedDirection = MathX.Average(leftEyeDirection, rightEyeDirection);
				var combinedOpeness = MathX.Average(ETVR_OSC.LeftEyeLidExpandedSqueeze, ETVR_OSC.RightEyeLidExpandedSqueeze);
				UpdateEye(combinedDirection, float3.Zero, true, ETVR_OSC.EyeDilation, combinedOpeness, 
					fakeWiden, 0f, 0f, deltaTime, _eyes.CombinedEye);
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
					eye.PupilDiameter = pupilSize != 0f ? pupilSize : _defaultPupilSize;
				}

				eye.Openness = openness;
				eye.Widen = widen;
				eye.Squeeze = squeeze;
				eye.Frown = frown;
			}

			private static float3 Project2DTo3D(float x, float y)
			{
				return new float3(MathX.Tan(config.GetValue(Alpha) * x),
								  MathX.Tan(config.GetValue(Beta) * y),
								  1f).Normalized;
			}
		}
	}
}
