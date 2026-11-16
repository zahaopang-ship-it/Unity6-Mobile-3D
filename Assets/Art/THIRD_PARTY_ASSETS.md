# Third-party asset register

All final third-party assets must come from Unity Asset Store or Sketchfab. Retain attribution and original source archives.

## Imported and inspected

- **FPS AK-74m animations** — Cransh, Sketchfab: https://sketchfab.com/3d-models/fps-ak-74m-animations-94be8385c402474cacd39bc096c6ca14. CC BY 4.0. 3 meshes, 84-bone armature, 18 actions, 40,024 triangles. Imported FBX: `Assets/Art/ThirdParty/Sketchfab/FPS_AK74M/FPS_AK74M.fbx`.
- **FPS pistol animations** — Cransh, Sketchfab: https://sketchfab.com/3d-models/fps-pistol-animations-0d7a343dcb6f401197a73c91aee93f6d. CC BY 4.0. 4 meshes, 80-bone armature, 10 actions, 32,670 triangles. Imported FBX: `Assets/Art/ThirdParty/Sketchfab/FPS_Pistol/FPS_Pistol.fbx`.
- **FSB Operator** — SpatialNeglect (Mateusz Woliński), Sketchfab: https://sketchfab.com/3d-models/none-43a561e941704eefb1ab0614be4f0049. CC BY 4.0. 40 meshes, 65-bone armature, one 232-frame action, 114,273 triangles. Imported FBX: `Assets/Art/ThirdParty/Sketchfab/FSB_Operator/FSB_Operator.fbx`. 17 matching supplied texture maps were relinked in Blender. Some legacy weapon/patch texture references are absent from the archive and require visual review in Unity.
- **Abandoned Warehouse** — Arsen Ismailov (Sketchfab user fdgasd7): https://sketchfab.com/3d-models/abandoned-warehouse-698a34300af34095ac6593f348585daa. CC BY 4.0. The original GLB metadata confirms title, author, source URL and license. 41 meshes; 197,970 triangles before selective decimation and 164,434 after 0.65 decimation on two window meshes and concrete stairs. Unity FBX: `Assets/Art/Environment/Abandoned_Warehouse/Abandoned_Warehouse.fbx`.

## Existing approved weapon asset

- **FPS AKM - Model & Textures** — ChamferZone, Unity Asset Store: https://assetstore.unity.com/packages/3d/fps-akm-model-textures-63654. Follow the Standard Unity Asset Store EULA. Existing AKM PBR material and prefab are prepared; orientation and attachment to the final FPS arms rig still require calibration.

## Asset repair and validation notes

- The AK-74M archive's `armColor.png` and `armNormal.png` entries failed CRC. They were replaced with byte-identical copies from the Cransh pistol archive: same 4096x4096 dimensions, file sizes and CRC32 values. The repaired archive passed ZIP integrity testing.
- All four incoming FBX exports were re-imported into Blender and their mesh/armature/action counts checked. This is structural FBX verification, not a Unity import or runtime test.
- The original Abandoned Warehouse GLB is retained in Google Drive `Unity6Builds/IncomingAssets/`. The rejected Abandoned Factory Hall Scan is not used.

## Not approved for final use

- The old Cransh Arm3 rifle pack has a known glitchy rifle mesh; retain for reference only.
- **Beretta Pistol FPS ANIMATION** — BURNER, Sketchfab: https://skfb.ly/oAKR8, CC BY 4.0. Only metadata is present; the model itself is missing.
- `Assets/Kevin Iglesias/Human Animations/` came from itch.io, outside the allowed source list; excluded.
- `Assets/Art/Characters/MaleSoldier03/` has an MIT notice but no verified Asset Store/Sketchfab source; excluded.
- Kenney assets are prohibited.
- **Abandoned Factory Hall Scan** — explicitly rejected by the user; do not download or use it.

## Mobile constraints and pending checks

- Target Android ARM64, Realme 8 / Helio G95.
- Configure imported textures with Android ASTC 6x6 overrides and sensible max sizes.
- Unity import/material appearance, viewmodel orientation, animation controller setup, colliders, LODs and in-game performance still need Editor/device validation.
