# Asset and scene audit — 2026-11-16

## Verified findings
- `Assets/Editor/FPSSceneBuilder.cs` currently calls `BuildKenneyBackdrop()` and loads models from `Assets/Art/KenneyIndustrial/Models/FBX format/`. This conflicts with the current project requirement not to use Kenney assets.
- The same scene builder creates placeholder weapons, arms, and enemy body parts from Unity primitives (`CreatePrimitive`, cubes, capsules and spheres). These are not acceptable as final realistic FPS assets and must be replaced rather than shipped.
- `Assets/Art/ThirdParty/RifleFPSHands/Credits.txt` identifies “Animated FPS hands (rifle animation pack” by Cransh, Sketchfab URL `https://skfb.ly/ow86X`, CC BY 4.0.
- `Assets/Art/ThirdParty/RifleFPSHands/Animated FPS hands (rifle animation pack)/Beretta Pistol FPS ANIMATION_Information.txt` identifies the Beretta pistol FPS animation asset by BURNER, Sketchfab URL `https://skfb.ly/oAKR8`, CC BY 4.0.

## Provenance not yet verified
- `Assets/Art/Characters/MaleSoldier03/` GLB metadata explicitly embeds a Microsoft MIT License notice (Copyright (c) 2020 Microsoft). The license permits use subject to retaining the copyright and permission notice, but its source platform has not been verified as Unity Asset Store or Sketchfab; do not use in the final project until the source requirement is resolved.
- `Assets/Art/Weapons/AKM/AKM_Anchors.fbx` identifies `/content/work/AKM_Anchors_Source.blend` as its source project, but the inspected files do not identify the model author, original download page, or license. Do not use it as a final asset until provenance and license are verified.
- The `Assets/Art/THIRD_PARTY_ASSETS.md` file did not provide usable attribution details during this audit.

## Required rebuild approach
- Remove all Kenney model dependencies from the generated scene.
- Replace placeholder weapon/arm/enemy geometry with actual, licensed Sketchfab or Unity Asset Store models.
- Inspect each downloaded model in Blender for orientation, scale, mesh quality, materials, texture maps, skeleton, and animation compatibility before importing into Unity.
- Use Mixamo only where the character rig is compatible and the animation license/source is documented.
- Keep attribution and license records beside every third-party asset.
- Target Android ARM64 and optimize geometry, texture resolution, materials, and draw calls for Realme 8 / Helio G95 / 8 GB RAM.
