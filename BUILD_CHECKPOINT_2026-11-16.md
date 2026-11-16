# Build checkpoint — 2026-11-16

## Recovery points
- Recovery branch: `bodycam-rebuild-2026-11-16`.
- Full project archive: Google Drive `My Drive/Unity6Builds/BodycamProjectSource_2026-11-16.zip` (about 53 MB); `unzip -t` passed.
- Recovery instructions: `RECOVERY.md`.
- Current isolated Unity 6.3 working copy: `/content/work/Unity6-Mobile-3D-Unity63`.
- Original workspace remains at `/content/work/Unity6-Mobile-3D`; do not overwrite it during migration.

## Unity environment
- Remote Desktop Commander device `7816f5ac7b89` was online during this run.
- DNS resolved Unity and GitHub hosts.
- Unity CLI account sign-in is active.
- Unity Personal entitlement was activated successfully inside a corrected cgroup namespace; the Editor subsequently launched and resolved entitlement details.
- Unity Editor `6000.3.28f1` is installed and verified.
- Android Build Support, Android SDK/NDK Tools, and OpenJDK modules are present.
- The earlier `6000.0.86f1` install was rejected because the account does not have Extended LTS entitlement; do not retry that version.
- Run Unity CLI/Editor processes inside `unshare --cgroup --fork` in this Colab container. Without a fresh cgroup namespace, Licensing Client 1.19.0 can crash while traversing the container's unusual cgroup path.

## Project migration and compilation
- Working copy was created from the original project while excluding generated Library/Temp/Obj/Logs/Build folders.
- Project originally targeted Unity `6000.0.40f1`; the isolated working copy is being migrated with Unity `6000.3.28f1`.
- Added built-in dependency `com.unity.modules.animation: 1.0.0` to the manifest.
- Removed the non-approved itch.io Kevin Iglesias asset folder and the source-unverified Microsoft MIT MaleSoldier03 model from the isolated working copy.
- Batch import/compile completed in Unity 6.3 after the manifest fix; no C# compiler errors were present in the final import log.
- `FPSWeapon.cs` now gates mouse-click firing to Editor/Standalone only. Android touch taps outside the explicit fire UI will no longer trigger shots. Animator hooks were added for firing, reload, draw, and movement speed.
- `FPSSceneBuilder.cs` no longer calls or contains the Kenney industrial backdrop loader.

## Known unfinished scene work
- The current scene builder still contains primitive placeholder weapons, arms, and enemy geometry. These are not acceptable for the final game and must be replaced with verified Asset Store/Sketchfab models.
- The existing Cransh `Arm3.fbx` pack is CC BY 4.0, but its author notes glitchy rifle vertices. Do not use it as the final first-person viewmodel.
- The Asset Store AKM source is identified as **FPS AKM - Model & Textures** by ChamferZone. Its FBX has main/magazine anchors; its current Unity materials need rebuilding from the supplied base-colour, normal, metallic/smoothness, and occlusion textures.
- Required incoming Sketchfab models are listed in `Assets/Art/THIRD_PARTY_ASSETS.md`: a better FPS AK-74m viewmodel/animation pack, a pistol FPS animation pack, and a full-body FSB Operator character.
- Do not build or deliver a final APK until real weapon/viewmodel and enemy assets are integrated and validated.

## Build status
- No new APK has been built or verified.
- Older APKs in Drive are not new builds and must not be delivered as the result of this checkpoint.
- Preserve the original Drive ZIP and make a fresh checkpoint before any destructive migration or asset replacement.

## Target and constraints
- Android ARM64, Landscape.
- Optimize for Realme 8 / Helio G95 / 8 GB RAM: reduce texture sizes, compress textures appropriately, keep viewmodel/enemy triangle counts and draw calls controlled.
- Use Unity Asset Store or Sketchfab assets only.
- Inspect and prepare imported models in Blender; use Mixamo where a full-body character needs a compatible rig/animations.
- Never use Kenney assets.
- Do not claim any step succeeded without checking the actual logs/files.
