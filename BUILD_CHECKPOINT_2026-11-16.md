# Build checkpoint — 2026-11-16

## Recovery points
- Recovery branch: `bodycam-rebuild-2026-11-16`.
- Original full project archive: Google Drive `My Drive/Unity6Builds/BodycamProjectSource_2026-11-16.zip` (about 53 MB); `unzip -t` passed.
- Unity 6.3 checkpoint archive 1: `My Drive/Unity6Builds/BodycamProjectSource_2026-11-16_Unity63Checkpoint1.zip` (32 MB, 170 files), integrity test passed.
- Latest checkpoint archive 2: `My Drive/Unity6Builds/BodycamProjectSource_2026-11-16_Unity63Checkpoint2.zip` (32 MB, 173 files), integrity test passed.
- Incoming model folder: Google Drive `My Drive/Unity6Builds/IncomingAssets/` with a README listing the four required Sketchfab downloads.
- Recovery instructions: `RECOVERY.md`.
- Current isolated Unity 6.3 working copy: `/content/work/Unity6-Mobile-3D-Unity63`.
- Original workspace remains at `/content/work/Unity6-Mobile-3D`; do not overwrite it.

## Unity environment
- Remote Desktop Commander device `7816f5ac7b89` was online during this run.
- DNS resolves Unity and GitHub hosts.
- Unity CLI account sign-in is active.
- `unity license status --verbose` inside a fresh cgroup namespace reports **Unity Personal — Assigned** and **Asset Store — Assigned**.
- Unity Editor `6000.3.28f1` is installed and verified.
- Android Build Support, Android SDK/NDK Tools, and OpenJDK modules are installed.
- Use `unshare --cgroup --fork` for Unity CLI/Editor processes in this container; it avoids the Licensing Client crash caused by the container's unusual cgroup path.
- The earlier `6000.0.86f1` install was rejected because the account does not have Extended LTS entitlement; do not retry it.

## Project migration and compilation
- Working copy created from the original project, excluding generated Library/Temp/Obj/Logs/Build folders.
- `ProjectSettings/ProjectVersion.txt` now identifies Unity `6000.3.28f1`.
- Added `com.unity.modules.animation: 1.0.0` to `Packages/manifest.json`.
- Removed the non-approved itch.io Kevin Iglesias asset folder and the source-unverified MaleSoldier03 model from this isolated working copy.
- Unity 6.3 batch import/compile completed without C# compiler errors.
- `BuildAndroid.ValidateOnly` passed twice, including after AKM asset preparation. Latest log reports `FPS SCENE VALIDATION PASSED` and `BODYCAM SCENE VALIDATION COMPLETE`. This is an Editor-side structural check, not proof of Android device behavior.
- `FPSWeapon.cs` now gates mouse-click firing to Editor/Standalone only; Android touch taps outside the explicit fire UI will not trigger shots. Optional Animator hooks were added for shooting, reload, draw, and movement speed.
- `FPSSceneBuilder.cs` no longer contains or calls the Kenney industrial backdrop loader.

## AKM asset preparation
- The imported AKM is from **FPS AKM - Model & Textures** by ChamferZone, Unity Asset Store.
- Added `Assets/Editor/FPSAssetSetup.cs`, which configures the AKM textures, builds a Standard PBR material, and creates a reusable prefab.
- Verified outputs: `Assets/Art/Weapons/AKM/Materials/AKM_PBR.mat` and `Assets/Art/Weapons/AKM/Prefabs/AKM_AssetStore_PBR.prefab`; the prefab contains two renderers.
- Base colour/normal maps are capped at 2048; metallic/smoothness and occlusion maps at 1024; Android overrides use ASTC 6x6.
- AKM material and raw prefab are prepared, but viewmodel orientation and attachment must still be calibrated against the final incoming FPS rig.

## Known unfinished scene work
- The current scene builder still contains primitive placeholder weapons, arms, enemy geometry, and environment pieces. These are not acceptable for the final game.
- The older Cransh `Arm3.fbx` pack is CC BY 4.0, but its author notes glitchy rifle vertices; do not use it as the final viewmodel.
- Incoming approved Sketchfab assets are listed in `Assets/Art/THIRD_PARTY_ASSETS.md`: FPS AK-74m animations, FPS pistol animations, FSB Operator, and Abandoned Factory Hall Scan.
- Do not build or deliver a final APK until the real weapon/viewmodel, enemy character, and environment assets are integrated and validated.

## Build status
- No new APK has been built or verified.
- Older APKs in Drive are not new builds and must not be delivered as the result of this checkpoint.
- Preserve the original ZIP and latest Unity 6.3 checkpoint ZIP. Create a new checkpoint before destructive migration or asset replacement.

## Target and constraints
- Android ARM64, Landscape.
- Optimize for Realme 8 / Helio G95 / 8 GB RAM.
- Use Unity Asset Store or Sketchfab assets only.
- Inspect and prepare imported models in Blender; use Mixamo where a full-body character needs a compatible rig/animations.
- Never use Kenney assets.
- Never claim a step succeeded without checking the actual logs/files.
