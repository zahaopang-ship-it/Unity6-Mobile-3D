# Bodycam FPS — Unity 6.3 working checkpoint

Date: 2026-11-16
Working editor: Unity 6000.3.28f1 (Android modules installed)
Target: Android ARM64, Landscape, Realme 8 / Helio G95

## Verified in this checkpoint
- Unity Personal entitlement is assigned and Unity CLI account is signed in.
- Unity 6.3 batch import/compile completed without C# compiler errors.
- `BuildAndroid.ValidateOnly` passed twice.
- `BuildAndroid.SmokeTestOnly` passed: Editor-side pistol/AKM firing and ammo, raycast damage, weapon switching, aim, crouch/prone/lean, virtual joystick, ragdoll and respawn.
- Added `com.unity.modules.animation` to the package manifest.
- Fixed Android mouse/touch behavior in `FPSWeapon.cs`: mouse input is editor/desktop only; mobile fire must come from the explicit fire UI. Added optional Animator parameter hooks.
- Removed the Kenney backdrop code from `FPSSceneBuilder.cs`.
- Removed the unapproved itch.io Kevin Iglesias asset folder and the source-unverified MaleSoldier03 model from this isolated working copy.
- Created `Assets/Art/Weapons/AKM/Materials/AKM_PBR.mat` and `Prefabs/AKM_AssetStore_PBR.prefab` from the verified ChamferZone Unity Asset Store AKM model and its PBR maps. The prefab has two renderers. Texture import is configured for Android ASTC 6x6.
- Added `Assets/Editor/FPSAssetSetup.cs` to reproduce the AKM material/prefab setup.
- Added `Assets/Editor/FPSBuildValidation.cs` and connected it to `BuildAndroid.PerformBuild`. It blocks the final APK while approved model folders are missing or primitive placeholders remain.
- Build settings now use Android API 25 minimum, ARM64, IL2CPP, StrictMode and LZ4. The build script verifies APK existence and size before reporting success.
- Created Google Drive `Unity6Builds/IncomingAssets/` with a README listing four required Sketchfab archives.

## Still incomplete — do not ship as final
- The scene builder still contains primitive placeholder geometry for weapons, arms, enemy, and environment. The final scene must replace these with approved Asset Store/Sketchfab models.
- The currently present Cransh Arm3 first-person pack has a known rifle mesh issue; it is not approved for the final viewmodel.
- Incoming Sketchfab assets required: FPS AK-74m animations, FPS pistol animations, FSB Operator, and Abandoned Factory Hall Scan. See `Assets/Art/THIRD_PARTY_ASSETS.md` and Drive `Unity6Builds/IncomingAssets/README.txt`.
- The AKM PBR material and raw prefab are prepared, but its viewmodel orientation/attachment is not yet calibrated against the final FPS arms rig.
- No final APK has been built or verified. The final build is intentionally blocked until the above assets are imported and placeholders removed.

## Recovery
- Original untouched backup: Google Drive `Unity6Builds/BodycamProjectSource_2026-11-16.zip`.
- Latest Unity 6.3 source archive: `BodycamProjectSource_2026-11-16_Unity63Checkpoint4.zip`; its ZIP integrity test passed.
- Extract the latest checkpoint and open it with Unity 6000.3.28f1. Keep the original backup untouched.
