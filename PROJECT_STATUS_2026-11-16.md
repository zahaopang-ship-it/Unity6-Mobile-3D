# Bodycam FPS — current recovery and asset inspection checkpoint

Date: 2026-11-16
Unity project target: 6000.3.28f1, Android ARM64, Realme 8 / Helio G95
Recovery branch: `bodycam-rebuild-2026-11-16`

## Recovery and host status
- Restored project source from Google Drive `BodycamProjectSource_2026-11-16_Unity63Checkpoint5.zip` into the current remote workspace.
- The original full source ZIP remains untouched.
- Current remote host is `1ed5cf7f39cf`; Blender 4.0.2 is installed and working.
- DNS and HTTPS to Unity, GitHub and Drive work.
- Unity Editor and Unity CLI are not installed on this new host. Therefore Unity authentication/license, Unity compilation, Unity import, and APK build are not currently verified on this host.

## Assets inspected and prepared
- **FPS AK-74m animations — Cransh, CC BY 4.0**: 3 meshes, 84-bone armature, 18 actions, 40,024 triangles.
- **FPS pistol animations — Cransh, CC BY 4.0**: 4 meshes, 80-bone armature, 10 actions, 32,670 triangles.
- **FSB Operator — SpatialNeglect, CC BY 4.0**: 40 meshes, 65-bone armature, one 232-frame action, 114,273 triangles. 17 texture references were relinked to supplied archive maps before FBX export. Some old/unused weapon and patch maps referenced by the source blend are not included in the supplied archive and need Unity visual review.
- **Abandoned Warehouse — Arsen Ismailov, CC BY 4.0**: GLB metadata verified the title, author, Sketchfab source URL and license. 41 meshes; 197,970 triangles before selective decimation, 164,434 after a 0.65 ratio on two window meshes and the concrete stairs.
- The user explicitly rejected Abandoned Factory Hall Scan. It is not used.

## Verified preparation
- Repaired AK-74M archive by substituting byte-identical `armColor.png` and `armNormal.png` from the Cransh pistol archive. Both matched 4096x4096 dimensions, byte sizes and CRC32. Repaired ZIP passed integrity test.
- Exported FBX models for AK-74M, pistol, FSB Operator and Abandoned Warehouse.
- Re-imported all four FBX files into Blender and verified mesh/armature/action counts. This confirms FBX structural round-trip only; it is not a Unity import or in-game test.
- Copied models and source texture files into the Unity project under `Assets/Art/ThirdParty/Sketchfab/` and `Assets/Art/Environment/Abandoned_Warehouse/`.
- Updated final asset validation to look for `Abandoned_Warehouse`, not the rejected FactoryHall folder.
- Added/updated third-party attribution in `Assets/Art/THIRD_PARTY_ASSETS.md`.

## Remaining blockers — do not ship
- Need Unity 6000.3.28f1, Android modules and valid Unity Personal sign-in on an available host. The previous Unity-enabled host is currently offline.
- Unity import/material appearance and FBX importer warnings must be checked in the Editor.
- `FPSSceneBuilder.cs` still creates primitive placeholders for weapons, arms, enemy and environment. Replace and calibrate these in Unity before final validation.
- FSB Operator has some unresolved legacy material references absent from its supplied archive; inspect/replace or hide affected submeshes.
- AKM Asset Store weapon orientation and attachment to the final arms rig remain uncalibrated.
- No new APK has been built or verified. Existing APKs in Drive are old.

## Next steps
1. Restore Unity 6000.3.28f1 and Android modules, then verify Unity Personal licensing.
2. Import assets in Unity and resolve materials/rig settings.
3. Create calibrated AK-74M and pistol viewmodel prefabs and animation controllers while preserving touch-fire/reload/weapon-switch logic.
4. Replace enemy visuals and implement a proper animated/ragdoll setup.
5. Replace placeholder environment with Abandoned Warehouse; add colliders, lighting, occlusion and mobile LOD/texture settings.
6. Remove primitive placeholders, run Unity validation/smoke tests, then build and verify a fresh Android ARM64 APK.
