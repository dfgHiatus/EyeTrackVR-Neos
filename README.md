# EyeTrackVR-Neos

A [NeosModLoader](https://github.com/zkxs/NeosModLoader) mod for [Neos VR](https://neos.com/) that facilitates the use of [Eye Track VR](https://github.com/RedHawk989/EyeTrackVR) eye tracking.

## Usage
1. Install [NeosModLoader](https://github.com/zkxs/NeosModLoader).
1. Place [EyeTrackVR-Neos.dll](https://github.com/Meister1593/EyeTrackVR-Neos/releases) into your `nml_mods` folder, and extract it. This folder should be at `C:\Program Files (x86)\Steam\steamapps\common\NeosVR` on windows or `$HOME/.steam/steam/steamapps/common/NeosVR` for a default install on linux.
1. Place [Rug.Osc.dll](https://github.com/Meister1593/EyeTrackVR-Neos/releases) into your Neos base folder, one above your 'nml_mods' folder. This folder should be at `C:\Program Files (x86)\Steam\steamapps\common\NeosVR` on windows or `$HOME/.steam/steam/steamapps/common/NeosVR` on linux for a default installation.
1. Start the game. If you want to verify that the mod is working you can check your Neos logs.

If you want to verify that the mod is working you can check your Neos logs, or create an EmptyObject with an AvatarRawEyeData/AvatarRawMouthData Component (Found under Users -> Common Avatar System -> Face -> AvatarRawEyeData/AvatarRawMouthData).