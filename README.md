# Bodycam FPS Mobile — Unity 6

A mobile-first, landscape Android FPS prototype. The current project workspace is built with Unity 6 and targets Android ARM64 / IL2CPP.

## Current project scope

- Touchscreen movement joystick and right-side look input.
- Bodycam-inspired camera sway, recoil and aiming.
- Pistol and AKM weapon systems, firing, ammunition, reload and weapon switching.
- Crouch, prone, lean and sprint controls.
- Training target, damage and ragdoll prototype.
- Procedural first-person arm IK prototype.
- Industrial training environment.

## Important development status

This is an in-progress prototype, **not a finished Bodycam clone**. The scene builder and older prototype visuals still require replacement with properly rigged character and weapon assets, animation integration, and real-device testing. Do not treat a successful editor-side validation as proof that the APK works correctly on a phone.

**Asset rule for the rebuild:** use assets from Unity Asset Store or Sketchfab only, check each asset's license and attribution requirements, and use Blender for inspection/cleanup. Mixamo may be used for compatible character animations. Do not add Kenney assets.

## Recovery and backups

- The current source code and Unity project configuration are checkpointed in the `bodycam-rebuild-2026-11-16` branch.
- The full project archive, including imported art assets, is stored separately in Google Drive at `My Drive/Unity6Builds/BodycamProjectSource_2026-11-16.zip`.
- Keep Unity's generated folders (`Library/`, `Temp/`, `Obj/`, `Logs/`, and build output) out of Git.
- After each meaningful change, commit source/configuration to this branch and create a fresh dated archive in Drive. Never rely on the remote computer's temporary disk as the only copy.

## Project version and Android target

- Unity editor version recorded in the project: `6000.0.40f1`.
- Android target: ARM64, IL2CPP.
- Test target: Android phone with touch controls; final verification must be performed on the actual device.

## Build

The editor build entry point is `BuildAndroid.PerformBuild` in `Assets/Editor/BuildAndroid.cs`. Before creating a release APK:
1. Open/import the project and resolve all compiler/import errors.
2. Run `BuildAndroid.ValidateOnly`.
3. Run the full editor smoke tests.
4. Build the Android ARM64 APK.
5. Install and test it on a real Android device, checking movement, multitouch, firing, reload, weapon switching, aim, enemy hit detection, ragdoll and respawn.
