# Bodycam FPS — Unity 6.3 recovery and Android ARM64 build checkpoint

Date: 2026-11-16
Project: Unity 6000.3.28f1
Target: Android ARM64, Landscape, OpenGLES3, Realme 8 / Helio G95
Recovery branch: `bodycam-rebuild-2026-11-16`
Working project: `/content/work/Unity6-Mobile-3D-Unity63`

## Environment and licensing
- Unity 6000.3.28f1 Editor, Android Build Support, SDK, NDK 27.2 and OpenJDK 17 are installed on remote host `1ed5cf7f39cf`.
- Unity account sign-in is successful and Unity Personal is assigned.
- Unity batchmode compilation has succeeded; C# compilation and editor scene generation are verified.

## Imported assets integrated in the scene
- FPS AK-74M arms/viewmodel: Cransh, CC BY 4.0. Source archive repaired using byte-identical `armColor.png` and `armNormal.png` from the Cransh FPS pistol archive; CRC and ZIP integrity passed.
- FPS pistol arms/viewmodel: Cransh, CC BY 4.0.
- FSB Operator full-body enemy: SpatialNeglect / Mateusz Woliński, CC BY 4.0. The imported skinned meshes and Animator are integrated as the training enemy.
- Abandoned Warehouse: Arsen Ismailov / fdgasd7, CC BY 4.0. Integrated as the scene environment with 41 MeshColliders.
- Rejected Abandoned Factory Hall Scan is not used.
- No Kenney assets are used.

## Scene and gameplay verification
- FPSSceneBuilder creates the Bodycam camera and FPSBodycamEffects, with walk/sprint bob, breathing micro-motion, inertial sway/roll, landing impulse, recoil and FOV transitions.
- Imported pistol and AK-74M viewmodels are connected to FPSWeapon. Imported Animator controllers are configured with fire/reload/draw triggers and walk speed parameter.
- Mobile joystick and FIRE/AIM/RELOAD/SWAP/CROUCH/PRONE/LEAN/SPRINT controls are present.
- Imported warehouse meshes are assigned MeshColliders; scene validation passes.
- Automated Unity smoke test passed: pistol/AK ammo consumption, raycast damage, weapon switching, aim, crouch/prone/lean, virtual joystick, enemy death/ragdoll activation and respawn.

## Known limitations — not a final release yet
- The enemy's visible character is the imported FSB Operator, but its death ragdoll currently uses separate capsule physics proxies; a bone-mapped ragdoll matching the visible character is still required.
- Imported weapon and enemy materials, model orientation, hand alignment and camera framing still need visual review in the Unity Game view and on a physical phone. Batchmode smoke tests cannot prove visual quality.
- The FSB archive omits some legacy weapon/patch texture references; affected materials still need visual review.
- No APK has yet passed final packaging and artifact verification. Do not deliver older APKs from Drive as the current build.

## Android ARM64 build
- Build settings: ARM64, IL2CPP, Android API 35 target, API 25 minimum, LandscapeLeft, OpenGLES3.
- The Android ARM64 IL2CPP build is currently running on the remote host. Unity has passed scene validation and smoke tests and is compiling native Android code. APK packaging and artifact checks are pending.
- Mono is not selected for the ARM64 target.

## Backup
- Google Drive checkpoint: `BodycamProjectSource_2026-11-16_Unity63Checkpoint9.zip`
- Size: 379 MB
- ZIP integrity: passed
- SHA-256: `50b764148197dbacf52074bde86f59bc7ca94f3199c8436425f9a7dc90fcac08`

## Next steps
1. Complete the Android ARM64 IL2CPP build.
2. Verify APK with ZIP integrity, apksigner, aapt badging, ABI inspection and BuildReport.
3. Complete visual inspection of weapon/arms/enemy materials and model alignment.
4. Replace capsule proxy ragdoll with a bone-mapped FSB Operator ragdoll.
5. Test on the Realme 8 or a suitable Android ARM64 device before calling the build release-ready.