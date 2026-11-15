using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class BuildAndroid
{
    public static void PerformBuild()
    {
        FPSSceneBuilder.CreateScene();
        PlayerSettings.productName = "Bodycam FPS Mobile";
        PlayerSettings.companyName = "Zahaopang Studio";
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.zahaopang.bodycamfps");
        PlayerSettings.bundleVersion = "0.1.0";
        PlayerSettings.Android.bundleVersionCode = 1;
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel23;
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
        EditorUserBuildSettings.buildAppBundle = false;
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
        QualitySettings.vSyncCount = 0;
        QualitySettings.antiAliasing = 2;

        string output = "build/Android/BodycamFPS-Mobile-ARM64.apk";
        Directory.CreateDirectory(Path.GetDirectoryName(output));
        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = new[] { FPSSceneBuilder.ScenePath },
            locationPathName = output,
            target = BuildTarget.Android,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        if (report.summary.result != BuildResult.Succeeded)
            throw new System.Exception("Android build failed: " + report.summary.result +
                " | Errors: " + report.summary.totalErrors + " | Warnings: " + report.summary.totalWarnings);
        File.WriteAllText("build/Android/build-summary.txt",
            "Build: " + report.summary.result + "\n" +
            "Size: " + report.summary.totalSize + "\n" +
            "Errors: " + report.summary.totalErrors + "\n" +
            "Warnings: " + report.summary.totalWarnings + "\n" +
            "APK: " + output + "\n");
        Debug.Log("BODYCAM APK CREATED: " + output);
    }
}