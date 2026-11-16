using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.Build;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.Rendering;

public static class BuildAndroid
{
    public static void ValidateOnly()
    {
        FPSSceneBuilder.CreateScene();
        FPSSceneBuilder.ValidateScene();
        Debug.Log("BODYCAM SCENE VALIDATION COMPLETE");
    }

    public static void SmokeTestOnly()
    {
        ValidateOnly();
        RunSmokeTests();
    }

    private static void RunSmokeTests()
    {
        GameObject playerObject = GameObject.Find("FPS Player");
        FPSPlayerController player = playerObject.GetComponent<FPSPlayerController>();
        FPSWeapon weapon = playerObject.GetComponent<FPSWeapon>();
        GameObject enemy = GameObject.Find("Training Enemy");
        FPSDamageable damage = enemy.GetComponent<FPSDamageable>();
        Vector3 originalEnemyPosition = enemy.transform.position;
        enemy.transform.position = player.transform.position + player.transform.forward * 5f;
        Collider enemyHitbox = enemy.GetComponent<Collider>();
        if (enemyHitbox != null) enemyHitbox.enabled = true;
        GameObject warehouse = GameObject.Find("Abandoned Warehouse Environment");
        Collider[] warehouseColliders = warehouse != null ? warehouse.GetComponentsInChildren<Collider>(true) : new Collider[0];
        foreach (Collider collider in warehouseColliders) collider.enabled = false;

        InvokePrivate(damage, "Start");
        weapon.audioSource = null;
        InvokePrivate(weapon, "Start");
        if (weapon.shotClip == null) throw new System.Exception("Smoke test failed: procedural gunshot audio was not created.");
        if (weapon.pistolModel.GetComponentInChildren<SkinnedMeshRenderer>(true) == null ||
            weapon.akmModel.GetComponentInChildren<SkinnedMeshRenderer>(true) == null)
            throw new System.Exception("Smoke test failed: imported first-person arms are missing.");
        Animator pistolAnimator = weapon.pistolModel.GetComponentInChildren<Animator>(true);
        Animator rifleAnimator = weapon.akmModel.GetComponentInChildren<Animator>(true);
        if (pistolAnimator == null || rifleAnimator == null || pistolAnimator.runtimeAnimatorController == null || rifleAnimator.runtimeAnimatorController == null)
            throw new System.Exception("Smoke test failed: imported weapon animation controllers are missing.");
        bool hasFireTrigger = false, hasReloadTrigger = false;
        foreach (AnimatorControllerParameter parameter in rifleAnimator.parameters)
        {
            if (parameter.name == "oneshoot" && parameter.type == AnimatorControllerParameterType.Trigger) hasFireTrigger = true;
            if (parameter.name == "reload" && parameter.type == AnimatorControllerParameterType.Trigger) hasReloadTrigger = true;
        }
        if (!hasFireTrigger || !hasReloadTrigger)
            throw new System.Exception("Smoke test failed: AK animation fire/reload triggers are missing.");

        int pistolAmmoBefore = weapon.pistol.ammo;
        weapon.FireOnce();
        if (weapon.pistol.ammo != pistolAmmoBefore - 1)
            throw new System.Exception("Smoke test failed: pistol fire did not consume one round.");
        if (damage.Health >= damage.maxHealth)
            throw new System.Exception("Smoke test failed: pistol raycast did not damage the training enemy.");

        weapon.SwitchWeapon();
        if (!weapon.akmModel.activeSelf || weapon.pistolModel.activeSelf)
            throw new System.Exception("Smoke test failed: weapon switching did not activate the AKM model.");
        SetPrivateField(weapon, "nextShot", 0f);
        int akmAmmoBefore = weapon.akm.ammo;
        weapon.FireOnce();
        if (weapon.akm.ammo != akmAmmoBefore - 1)
            throw new System.Exception("Smoke test failed: AKM firing did not consume one round.");

        player.SetAim(true);
        if (!player.IsAiming) throw new System.Exception("Smoke test failed: aim state did not activate.");
        player.ToggleCrouch();
        if (!(bool)GetPrivateField(player, "crouched"))
            throw new System.Exception("Smoke test failed: crouch toggle did not activate.");
        player.ToggleProne();
        if (!(bool)GetPrivateField(player, "prone") || (bool)GetPrivateField(player, "crouched"))
            throw new System.Exception("Smoke test failed: prone stance did not replace crouch.");
        player.ToggleProne();
        player.TogglePeek(-1f);
        if (Mathf.Abs((float)GetPrivateField(player, "peek") + 1f) > .01f)
            throw new System.Exception("Smoke test failed: left lean state incorrect.");
        player.TogglePeek(1f);
        if (Mathf.Abs((float)GetPrivateField(player, "peek") - 1f) > .01f)
            throw new System.Exception("Smoke test failed: right lean state incorrect.");
        player.SetAim(false);

        FPSMobileJoystick stick = player.moveStick;
        EventSystem eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
        PointerEventData pointer = new PointerEventData(eventSystem);
        RectTransform rect = stick.transform as RectTransform;
        pointer.position = RectTransformUtility.WorldToScreenPoint(null, rect.position + Vector3.right * 50f);
        stick.OnPointerDown(pointer);
        if (stick.Value.sqrMagnitude < .01f)
            throw new System.Exception("Smoke test failed: virtual joystick did not produce movement input.");
        stick.OnPointerUp(pointer);

        damage.ApplyDamage(damage.maxHealth, enemy.transform.position + Vector3.up, Vector3.forward);
        if (damage.IsAlive || !damage.ragdollRoot.activeSelf)
            throw new System.Exception("Smoke test failed: lethal damage did not activate enemy ragdoll.");
        foreach (Rigidbody rb in damage.ragdollRoot.GetComponentsInChildren<Rigidbody>(true))
            if (rb.isKinematic) throw new System.Exception("Smoke test failed: ragdoll body remained kinematic.");

        InvokePrivate(damage, "Respawn");
        if (!damage.IsAlive || damage.ragdollRoot.activeSelf)
            throw new System.Exception("Smoke test failed: enemy did not respawn cleanly.");

        enemy.transform.position = originalEnemyPosition;
        foreach (Collider collider in warehouseColliders) collider.enabled = true;
        Debug.Log("FPS SMOKE TEST PASSED: imported pistol/AK arms and animation controllers, firing/ammo, enemy hit detection, weapon switching, aim, crouch/prone/lean, virtual joystick, ragdoll and respawn.");
    }

    private static void InvokePrivate(object target, string method)
    {
        MethodInfo info = target.GetType().GetMethod(method, BindingFlags.Instance | BindingFlags.NonPublic);
        if (info == null) throw new System.Exception("Smoke test helper could not find method: " + method);
        info.Invoke(target, null);
    }

    private static object GetPrivateField(object target, string field)
    {
        FieldInfo info = target.GetType().GetField(field, BindingFlags.Instance | BindingFlags.NonPublic);
        if (info == null) throw new System.Exception("Smoke test helper could not find field: " + field);
        return info.GetValue(target);
    }

    private static void SetPrivateField(object target, string field, object value)
    {
        FieldInfo info = target.GetType().GetField(field, BindingFlags.Instance | BindingFlags.NonPublic);
        if (info == null) throw new System.Exception("Smoke test helper could not find field: " + field);
        info.SetValue(target, value);
    }

    public static void PerformBuild()
    {
        ValidateOnly();
        RunSmokeTests();
        FPSBuildValidation.ValidateFinalAssets();
        PlayerSettings.productName = "Bodycam FPS Mobile";
        PlayerSettings.companyName = "Zahaopang Studio";
        PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, "com.zahaopang.bodycamfps");
        PlayerSettings.bundleVersion = "0.1.0";
        PlayerSettings.Android.bundleVersionCode = 1;
        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel25;
        PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel35;
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
        PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
        PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, false);
        PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new[] { GraphicsDeviceType.OpenGLES3 });
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
        EditorUserBuildSettings.buildAppBundle = false;
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
        Debug.Log("ANDROID TARGET ARCHITECTURES AFTER SWITCH=" + PlayerSettings.Android.targetArchitectures + " numeric=" + (int)PlayerSettings.Android.targetArchitectures);
        Debug.Log("ANDROID ACTIVE BUILD TARGET=" + EditorUserBuildSettings.activeBuildTarget);
        AssetDatabase.SaveAssets();
        QualitySettings.vSyncCount = 0;
        QualitySettings.antiAliasing = 2;

        string output = "build/Android/BodycamFPS-Mobile-ARM64.apk";
        Directory.CreateDirectory(Path.GetDirectoryName(output));
        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = new[] { FPSSceneBuilder.ScenePath },
            locationPathName = output,
            target = BuildTarget.Android,
            options = BuildOptions.StrictMode | BuildOptions.CompressWithLz4
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        if (report.summary.result != BuildResult.Succeeded)
            throw new System.Exception("Android build failed: " + report.summary.result +
                " | Errors: " + report.summary.totalErrors + " | Warnings: " + report.summary.totalWarnings);
        if (!File.Exists(output)) throw new System.Exception("Unity reported success but APK is missing: " + output);
        FileInfo apkInfo = new FileInfo(output);
        if (apkInfo.Length < 5 * 1024 * 1024)
            throw new System.Exception("APK is unexpectedly small; refusing to report it as valid: " + apkInfo.Length + " bytes.");
        File.WriteAllText("build/Android/build-summary.txt",
            "Build: " + report.summary.result + "\n" +
            "Size: " + report.summary.totalSize + "\n" +
            "APK bytes: " + apkInfo.Length + "\n" +
            "Errors: " + report.summary.totalErrors + "\n" +
            "Warnings: " + report.summary.totalWarnings + "\n" +
            "APK: " + output + "\n");
        Debug.Log("BODYCAM APK VERIFIED: " + output + " / " + apkInfo.Length + " bytes.");
    }
}