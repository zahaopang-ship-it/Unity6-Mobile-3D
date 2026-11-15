# Unity 6 Mobile 3D

A small 3D mobile runner prototype built with Unity 6. The scene is generated automatically during the cloud build, so no Unity Editor is needed on your phone.

## Gameplay
- Drag on the left half of the screen to move.
- Tap the upper-right half to jump.
- Collect the eight gold coins and avoid the orange obstacles.

## Cloud APK build
The GitHub Actions workflow builds an Android APK on every push to `main`, and can also be started manually from **Actions → Build Unity 6 Android APK → Run workflow**.

### Required Unity license secrets
Before the first build, add these repository secrets:
- `UNITY_LICENSE`: the contents of your Unity Personal license file (.ulf).
- `UNITY_EMAIL`: your Unity account email.
- `UNITY_PASSWORD`: your Unity account password.

Never put license contents or passwords in repository files or chat messages. GameCI's official guide explains how to create the one-time manual activation file and obtain the .ulf license.

## Download
Open the successful workflow run in **Actions**, scroll to **Artifacts**, and download **Unity6-Mobile-3D-APK**. Extract the ZIP and install the APK on Android.
