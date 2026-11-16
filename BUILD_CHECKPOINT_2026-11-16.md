# Build checkpoint — 2026-11-16

## Verified durable recovery points
- Recovery branch: `bodycam-rebuild-2026-11-16`.
- Full project archive: Google Drive `My Drive/Unity6Builds/BodycamProjectSource_2026-11-16.zip` (53 MB); `unzip -t` passed on 2026-11-16.
- Recovery instructions: `RECOVERY.md`.

## Remote environment findings
- Remote Desktop Commander device `7816f5ac7b89` was online at the time of inspection.
- Unity account sign-in is active.
- Unity Editor `6000.0.85f2` and Android SDK/NDK/OpenJDK modules are installed.
- Unity CLI `1.0.0` expects licensing IPC protocol `1.19.0`; the installed Editor's bundled Licensing Client is `1.18.3`. The Licensing Client log reports `Unsupported protocol version '1.19.0'`.
- DNS resolves Unity and GitHub hosts at the time of inspection.
- Attempt to install `6000.0.86f1` was rejected with `EDITOR_NOT_ENTITLED` (Extended LTS entitlement required).
- Installation of Unity `6000.3.28f1` LTS with Android modules was started as the compatible route because its official release notes state that it includes Licensing Client `1.19.0`. Installation was still running when this checkpoint was written; do not assume completion until verified.

## Project workspace
- Restored project path: `/content/work/Unity6-Mobile-3D`.
- The restored workspace does not currently contain a `.git` directory. Do not treat it as locally committed or pushed until a Git repository is initialized and synchronized.
- Project's original Editor version in `ProjectSettings/ProjectVersion.txt`: `6000.0.40f1`.

## Build status
- No new APK has been built or verified.
- Do not use older APKs as the new build.
- Before project migration, preserve the current Drive ZIP and make a separate copy. Validate Unity compilation, scene checks, Android ARM64 build output, APK existence and size, and device behavior separately.

## Asset constraints
- Use Unity Asset Store or Sketchfab assets only; inspect/prepare models in Blender; use Mixamo when needed for compatible character animation.
- Do not use Kenney assets.
- Target: Android ARM64; optimize for Realme 8 / Helio G95 / 8 GB RAM.
