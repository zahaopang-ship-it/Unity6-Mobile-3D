# Asset and scene audit — 2026-11-16

## Verified findings
- `Assets/Editor/FPSSceneBuilder.cs` currently calls `BuildKenneyBackdrop()` and loads models from `Assets/Art/KenneyIndustrial/Models/FBX format/`. This conflicts with the current project requirement not to use Kenney assets.
- The same scene builder creates placeholder weapons, arms, and enemy body parts from Unity primitives (`CreatePrimitive`, cubes, capsules and spheres). These are not acceptable as final realistic FPS assets and must be replaced rather than shipped.
- `Assets/Art/ThirdParty/RifleFPSHands/Credits.txt` identifies “Animated FPS hands (rifle animation pack” by Cransh, Sketchfab URL `https://skfb.ly/ow86X`, CC BY 4.0.
- `Assets/Art/ThirdParty/RifleFPSHands/Animated FPS hands (rifle animation pack)/Beretta Pistol FPS ANIMATION_Information.txt` identifies the Beretta pistol FPS animation asset by BURNER, Sketchfab URL `https://skfb.ly/oAKR8`, CC BY 4.0.

## Provenance not yet verified
- `Assets/Art/Characters/MaleSoldier03/` contains GLB character and animation files, but the local project files inspected so far do not contain a source URL or attribution record. Do not redistribute/use as a final asset until its Sketchfab/Asset Store source and exact license are confirmed.
- `Assets/Art/Weapons/AKM/` contains an FBX and PBR textures, but the inspected project files do not establish its source URL or license. Do not treat it as cleared until verified.
- The `Assets/Art/THIRD_PARTY_ASSETS.md` file did not provide usable attribution details during this audit.

## Required rebuild approach
- Remove all Kenney model dependencies from the generated scene.
- Replace placeholder weapon/arm/enemy geometry with actual, licensed Sketchfab or Unity Asset Store models.
- Inspect each downloaded model in Blender for orientation, scale, mesh quality, materials, texture maps, skeleton, and animation compatibility before importing into Unity.
- Use Mixamo only where the character rig is compatible and the animation license/source is documented.
- Keep attribution and license records beside every third-party asset.
- Target Android ARM64 and optimize geometry, texture resolution, materials, and draw calls for Realme 8 / Helio G95 / 8 GB RAM.
