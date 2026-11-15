# Bodycam FPS Mobile (Unity 6)

A landscape Android first-person shooter prototype built for touch screens. The project is configured for Unity 6000.0.40f1, Android ARM64 and IL2CPP.

## Current prototype systems
- Mobile virtual movement joystick and right-side touch look.
- Bodycam-style first-person camera motion, weapon sway, recoil and aim FOV.
- Pistol and AKM, semi/automatic fire, ammo, reload and weapon switching.
- Aim, crouch, prone, left/right lean and sprint touch controls.
- Training enemy with health, physical ragdoll and timed respawn.
- Procedural two-bone arm IK targets for first-person weapon holding.
- Industrial training yard / warehouse test map.
- Optional Kenney City Kit Industrial models (CC0); the scene builder falls back to generated geometry when the pack is absent.

## Touch controls
- Left joystick: movement.
- Drag the empty right side: look.
- FIRE: shoot (hold for AKM automatic fire).
- AIM: hold to aim.
- SWAP / RELOAD: change weapon / reload.
- CROUCH / PRONE: change stance.
- LEAN L / LEAN R: toggle camera lean.
- SPRINT: hold to run.

## Free environment assets
The scene can use Kenney City Kit (Industrial), released under CC0. The pack's License.txt is included with the downloaded files. The pack is available at:
https://kenney.nl/assets/city-kit-industrial

For this build, the ZIP is also saved in Google Drive at:
My Drive/Unity6Builds/KenneyIndustrialPack.zip

Extract the Models/FBX format folder and License.txt to:
Assets/Art/KenneyIndustrial/

## Build Android APK
1. Open the project with Unity 6000.0.40f1.
2. Ensure Android Build Support, Android SDK & NDK Tools, and OpenJDK are installed.
3. Activate a valid Unity Personal license on the build machine.
4. Run the editor in batch mode:
   bash
   Unity -batchmode -nographics -quit -projectPath /path/to/Unity6-Mobile-3D -executeMethod BuildAndroid.PerformBuild -logFile /path/to/unity-build.log
5. Output: build/Android/BodycamFPS-Mobile-ARM64.apk

## Notes
This is a first playable combat prototype, not a finished commercial Bodycam clone. The scene is designed for mobile performance; visual assets, full humanoid animation/IK, footstep/audio variety, enemy combat AI, and optimization will be iterated after device testing.
