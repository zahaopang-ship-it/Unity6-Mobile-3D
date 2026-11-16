# Asset and scene audit — 2026-11-16

## Verified approved assets already in the project

- **FPS AKM - Model & Textures** — ChamferZone, Unity Asset Store: https://assetstore.unity.com/packages/3d/fps-akm-model-textures-63654. This is the source of the imported AKM model in `Assets/Art/Weapons/AKM/`. The imported FBX has separate `AKM_Main_Anchor` and `AKM_Magazine_Anchor` transforms. Its current Unity material assignment is broken/white because the FBX's embedded material references do not match the actual texture filenames. Rebuild a PBR material using `AKM_BaseColor.jpg`, `AKM_Normal.png`, `AKM_MetallicSmoothness.png`, and `AKM_Occlusion.png`. Preserve the Asset Store attribution/license terms.

- **Animated FPS hands (rifle animation pack)** — Cransh, Sketchfab: https://skfb.ly/ow86X. License: CC BY 4.0. The project contains `Arm3.fbx`, its controller, and textures. The model author notes that the rifle has glitchy vertices; do not use this pack as the final viewmodel. It may be retained only for inspection/reference.

## Recommended incoming Sketchfab assets (all downloadable, CC BY 4.0)

- **FPS AK-74m animations** — Cransh: https://sketchfab.com/3d-models/fps-ak-74m-animations-94be8385c402474cacd39bc096c6ca14. 40,024 faces / 20,823 vertices. Includes first-person arms, AK-74m weapon model, rig, and animations. Use as a candidate first-person rifle rig; if the project keeps the Asset Store AKM, inspect its weapon attachment bones and fit the AKM mesh in Blender.
- **FPS pistol animations** — Cransh: https://sketchfab.com/3d-models/fps-pistol-animations-0d7a343dcb6f401197a73c91aee93f6d. 32,670 faces / 16,607 vertices. Includes FP arms, Springfield Armory XD Mod.2 Sub-Compact pistol, rig, and animations.
- **FSB Operator** — SpatialNeglect: https://sketchfab.com/3d-models/none-43a561e941704eefb1ab0614be4f0049. 114,273 faces / 60,095 vertices. CC BY 4.0. Verify rig state after download; if unrigged, use Mixamo, then reduce mesh complexity and create LODs for mobile.

These incoming assets are not yet present in the project. Do not claim they are imported or usable until their actual downloaded archives are placed in Google Drive `My Drive/Unity6Builds/IncomingAssets/`, integrity-tested, imported, and inspected in Blender/Unity.

## Identified but not present as a usable model

- **Beretta Pistol FPS ANIMATION** — BURNER, Sketchfab: https://skfb.ly/oAKR8, CC BY 4.0. The current project contains only its `_Information.txt` file; no corresponding pistol FBX/model was found. The Cransh FPS pistol pack above is a better documented replacement candidate.

## Excluded from final use

- `Assets/Kevin Iglesias/Human Animations/` originated from an itch.io distribution, outside the user's allowed source list. Removed from the isolated working copy; do not use.
- `Assets/Art/Characters/MaleSoldier03/` has a Microsoft MIT notice, but no verified Unity Asset Store/Sketchfab source. Removed from the isolated working copy; do not use.
- Kenney assets are prohibited. The Kenney backdrop call and helper methods have been removed from `FPSSceneBuilder.cs`; do not reintroduce Kenney assets.

## Scene audit findings

- The earlier `FPSSceneBuilder.cs` used primitive placeholder geometry for guns, arms, and the enemy. These are not final assets.
- The environment still needs real Asset Store/Sketchfab props and modular cover geometry.
- Do not build or deliver the final APK until the first-person viewmodels, enemy character, and environment assets have been replaced and validated.
