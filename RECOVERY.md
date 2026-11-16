# Recovery procedure

This project must survive an unexpected remote-computer disconnect.

## Durable copies
1. GitHub branch: `bodycam-rebuild-2026-11-16` — source scripts and Unity project configuration.
2. Google Drive archive: `My Drive/Unity6Builds/BodycamProjectSource_2026-11-16.zip` — complete project snapshot including imported assets.

## After reconnecting to a new remote machine
1. Restore the Drive ZIP into a fresh workspace.
2. Verify the ZIP with `unzip -t`.
3. Check out the GitHub recovery branch and reconcile the latest source commits.
4. Never overwrite the Drive archive until a new archive has passed ZIP integrity verification.
5. Before each risky scene/asset change, create a dated backup and commit a checkpoint.
6. Keep asset licenses and attribution records with the project.

## Asset policy
Use Unity Asset Store or Sketchfab assets with licenses suitable for the intended use. Inspect models in Blender before import; check scale, orientation, textures, materials, skeleton and animations. Use Mixamo only when the character rig is compatible. No Kenney assets.

## Validation policy
A build is not complete until Unity compilation, project validation, Android build, APK existence/size verification, and real-device checks have all been completed. Clearly report any checks that could not be performed.
