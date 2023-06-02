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
        public override string Author => "dfgHiatus + PLYSHKA";
        public override string Version => "1.0.1";
        public override string Link => "https://github.com/dfgHiatus/EyeTrackVR-Neos";
        public override void OnEngineInit()
        {
            Config = GetConfiguration();
            new Harmony("net.dfgHiatus.plyshka.EyeTrackVR-Neos").PatchAll();
            Engine.Current.OnShutdown += () => ETVR.Teardown();
        }
        private static ETVROSC ETVR;
        private static ModConfiguration Config;
        [AutoRegisterConfigKey]
        public static ModConfigurationKey<bool> ModEnabled = new ModConfigurationKey<bool>("enabled", "Mod Enabled", () => true);

        [AutoRegisterConfigKey]
        public static ModConfigurationKey<float> Alpha = new ModConfigurationKey<float>("alpha", "Eye Swing Multiplier X", () => 1.0f);

        [AutoRegisterConfigKey]
        public static ModConfigurationKey<float> Beta = new ModConfigurationKey<float>("beta", "Eye Swing Multiplier Y", () => 1.0f);
        [AutoRegisterConfigKey]
        private static ModConfigurationKey<int> OscPort = new ModConfigurationKey<int>("osc_port", "EyeTrackVR OSC port", () => 9000);

        [HarmonyPatch(typeof(InputInterface), MethodType.Constructor)]
        [HarmonyPatch(new[] { typeof(Engine) })]
        public class InputInterfaceCtorPatch
        {
            public static void Postfix(InputInterface __instance)
            {
                try
                {
                    ETVR = new ETVROSC(Config.GetValue(OscPort));
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
                if (!Config.GetValue(ModEnabled))
                {
                    _eyes.IsEyeTrackingActive = false;
                    return;
                }
                else
                {
                    _eyes.IsEyeTrackingActive = Engine.Current.InputInterface.VR_Active || Engine.Current.InputInterface.ScreenActive;
                }

                var fakeWiden = MathX.Remap(MathX.Clamp01(ETVROSC.EyeDataWithAddress["/avatar/parameters/EyesY"]), 0f, 1f, 0f, 0.33f);

                var leftEyeDirection = Project2DTo3D(ETVROSC.EyeDataWithAddress["/avatar/parameters/LeftEyeX"], ETVROSC.EyeDataWithAddress["/avatar/parameters/EyesY"]);
                UpdateEye(leftEyeDirection, float3.Zero, true, ETVROSC.EyeDataWithAddress["/avatar/parameters/EyesDilation"], ETVROSC.EyeDataWithAddress["/avatar/parameters/LeftEyeLidExpandedSqueeze"],
                    fakeWiden, 0f, 0f, deltaTime, _eyes.LeftEye);

                var rightEyeDirection = Project2DTo3D(ETVROSC.EyeDataWithAddress["/avatar/parameters/RightEyeX"], ETVROSC.EyeDataWithAddress["/avatar/parameters/EyesY"]);
                UpdateEye(rightEyeDirection, float3.Zero, true, ETVROSC.EyeDataWithAddress["/avatar/parameters/EyesDilation"], ETVROSC.EyeDataWithAddress["/avatar/parameters/RightEyeLidExpandedSqueeze"],
                    fakeWiden, 0f, 0f, deltaTime, _eyes.RightEye);

                var combinedDirection = MathX.Average(leftEyeDirection, rightEyeDirection);
                var combinedOpeness = MathX.Average(ETVROSC.EyeDataWithAddress["/avatar/parameters/LeftEyeLidExpandedSqueeze"], ETVROSC.EyeDataWithAddress["/avatar/parameters/RightEyeLidExpandedSqueeze"]);
                UpdateEye(combinedDirection, float3.Zero, true, ETVROSC.EyeDataWithAddress["/avatar/parameters/EyesDilation"], combinedOpeness,
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
                return new float3(MathX.Tan(Config.GetValue(Alpha) * x),
                                  MathX.Tan(Config.GetValue(Beta) * y),
                                  1f).Normalized;
            }
        }
    }
}
