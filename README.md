# EyeTrackVR-Neos

A [NeosModLoader](https://github.com/zkxs/NeosModLoader) mod for [Neos VR](https://neos.com/) that facilitates the use of [Eye Track VR](https://github.com/RedHawk989/EyeTrackVR) eye tracking.

## Usage
1. Install [NeosModLoader](https://github.com/zkxs/NeosModLoader).
1. Place [EyeTrackVR-Neos](https://github.com/dfgHiatus/EyeTrackVR-Neos/releases/download/v0.0.1/ETVR-Neos-v0.0.1.rar) into your `nml_mods` folder, and extract it. This folder should be at `/home/plyshka/Games/SteamLibrary/steamapps/common/NeosVR/nml_mods` for a default install. You can create it if it's missing, or if you launch the game once with NeosModLoader installed it will create the folder for you.
1. Place `Rug.Osc.dll` into your Neos install directory, one folder above `nml_mods`. This folder should be at `C:\Program Files (x86)\Steam\steamapps\common\NeosVR` for a default install, containing `neos.exe`.
1. Start the game. If you want to verify that the mod is working you can check your Neos logs.

If you want to verify that the mod is working you can check your Neos logs, or create an EmptyObject with an AvatarRawEyeData/AvatarRawMouthData Component (Found under Users -> Common Avatar System -> Face -> AvatarRawEyeData/AvatarRawMouthData).
