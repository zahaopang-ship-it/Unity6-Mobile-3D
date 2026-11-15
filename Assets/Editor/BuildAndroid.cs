using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine.UI;

public static class BuildAndroid
{
    private const string ScenePath = "Assets/Scenes/Starter.unity";

    public static void PerformBuild()
    {
        Directory.CreateDirectory("Assets/Scenes");
        CreateStarterScene();

        PlayerSettings.productName = "Unity 6 Mobile 3D";
        PlayerSettings.companyName = "Zahaopang Studio";
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.zahaopang.unity6mobile3d");
        PlayerSettings.bundleVersion = "1.0";
        PlayerSettings.Android.bundleVersionCode = 1;
        PlayerSettings.Android.minSdkVersion = Android最低Sdk();
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
        EditorUserBuildSettings.buildAppBundle = false;

        string output = "build/Android/Unity6Mobile3D.apk";
        Directory.CreateDirectory(Path.GetDirectoryName(output));

        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = new[] { ScenePath },
            locationPathName = output,
            target = BuildTarget.Android,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        if (report.summary.result != BuildResult.Succeeded)
            throw new System.Exception("Android build failed: " + report.summary.result +
                                       " | Errors: " + report.summary.totalErrors);
        Debug.Log("APK created: " + output);
    }

    private static AndroidSdkVersions Android最低Sdk()
    {
        return AndroidSdkVersions.AndroidApiLevel23;
    }

    private static void CreateStarterScene()
    {
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.55f, 0.62f, 0.72f);
        RenderSettings.fog = true;
        RenderSettings.fogColor = new Color(0.62f, 0.75f, 0.86f);
        RenderSettings.fogDensity = 0.008f;

        Material groundMat = MakeMaterial("Ground", new Color(0.16f, 0.34f, 0.28f));
        Material playerMat = MakeMaterial("Player", new Color(0.12f, 0.55f, 0.95f));
        Material obstacleMat = MakeMaterial("Obstacle", new Color(0.95f, 0.34f, 0.16f));
        Material coinMat = MakeMaterial("Coin", new Color(1f, 0.72f, 0.08f));
        Material trimMat = MakeMaterial("Trim", new Color(0.12f, 0.2f, 0.3f));

        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ground.name = "Ground";
        ground.transform.position = new Vector3(0f, -0.5f, 12f);
        ground.transform.localScale = new Vector3(18f, 1f, 48f);
        ground.GetComponent<Renderer>().sharedMaterial = groundMat;

        for (int side = -1; side <= 1; side += 2)
        {
            GameObject rail = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rail.name = "Track Rail";
            rail.transform.position = new Vector3(side * 8.6f, 0.45f, 12f);
            rail.transform.localScale = new Vector3(0.35f, 0.9f, 48f);
            rail.GetComponent<Renderer>().sharedMaterial = trimMat;
        }

        GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        player.name = "Player";
        player.tag = "Player";
        player.transform.position = new Vector3(0f, 1.05f, -7f);
        player.GetComponent<Renderer>().sharedMaterial = playerMat;
        Collider oldCollider = player.GetComponent<Collider>();
        Object.DestroyImmediate(oldCollider);
        CharacterController controller = player.AddComponent<CharacterController>();
        controller.height = 2f;
        controller.radius = 0.48f;
        controller.center = Vector3.zero;
        player.AddComponent<PlayerController>();

        int[] coinZ = { 0, 5, 10, 15, 20, 25, 30, 35 };
        for (int i = 0; i < coinZ.Length; i++)
        {
            GameObject coin = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            coin.name = "Coin " + (i + 1);
            coin.transform.position = new Vector3((i % 2 == 0) ? -2.5f : 2.5f, 1.4f, coinZ[i]);
            coin.transform.localScale = Vector3.one * 0.65f;
            coin.GetComponent<Renderer>().sharedMaterial = coinMat;
            SphereCollider trigger = coin.GetComponent<SphereCollider>();
            trigger.isTrigger = true;
            coin.AddComponent<CoinSpin>();
        }

        for (int i = 0; i < 6; i++)
        {
            GameObject block = GameObject.CreatePrimitive(PrimitiveType.Cube);
            block.name = "Training Obstacle " + (i + 1);
            block.transform.position = new Vector3(i % 2 == 0 ? 3.5f : -3.5f, 0.65f, 3f + i * 5f);
            block.transform.localScale = new Vector3(1.5f, 1.3f, 1.5f);
            block.GetComponent<Renderer>().sharedMaterial = obstacleMat;
        }

        GameObject lightObject = new GameObject("Sun");
        Light sun = lightObject.AddComponent<Light>();
        sun.type = LightType.Directional;
        sun.intensity = 1.2f;
        lightObject.transform.rotation = Quaternion.Euler(48f, -32f, 0f);

        GameObject cameraObject = new GameObject("Main Camera");
        cameraObject.tag = "MainCamera";
        Camera camera = cameraObject.AddComponent<Camera>();
        camera.fieldOfView = 65f;
        camera.nearClipPlane = 0.1f;
        camera.farClipPlane = 150f;
        cameraObject.AddComponent<AudioListener>();
        FollowCamera follow = cameraObject.AddComponent<FollowCamera>();
        follow.SetTarget(player.transform);
        cameraObject.transform.position = new Vector3(0f, 6f, -14f);
        cameraObject.transform.LookAt(player.transform.position);

        GameObject canvasObject = new GameObject("HUD");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        canvasObject.AddComponent<GraphicRaycaster>();

        Text title = MakeText(canvasObject.transform, "TITLE",
            "3D RUNNER", 52, new Vector2(0.5f, 0.94f), new Vector2(0.5f, 0.94f),
            new Vector2(0, 0), new Vector2(700, 100), TextAnchor.MiddleCenter);
        title.color = Color.white;
        Text score = MakeText(canvasObject.transform, "SCORE",
            "COINS  0 / 8", 34, new Vector2(0.5f, 0.87f), new Vector2(0.5f, 0.87f),
            new Vector2(0, 0), new Vector2(500, 70), TextAnchor.MiddleCenter);
        GameSession.ResetSession();
        GameSession.RegisterScoreText(score);

        Text controls = MakeText(canvasObject.transform, "CONTROLS",
            "DRAG LEFT SIDE TO MOVE     TAP RIGHT SIDE TO JUMP",
            23, new Vector2(0.5f, 0.04f), new Vector2(0.5f, 0.04f),
            new Vector2(0, 0), new Vector2(1000, 70), TextAnchor.MiddleCenter);
        controls.color = new Color(1f, 1f, 1f, 0.9f);

        EditorSceneManager.SaveScene(scene, ScenePath);
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
    }

    private static Material MakeMaterial(string name, Color color)
    {
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) shader = Shader.Find("Standard");
        Material material = new Material(shader);
        material.name = name;
        material.color = color;
        return material;
    }

    private static Text MakeText(Transform parent, string objectName, string value, int size,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, Vector2 dimensions,
        TextAnchor alignment)
    {
        GameObject go = new GameObject(objectName);
        go.transform.SetParent(parent, false);
        RectTransform rect = go.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = dimensions;
        Text text = go.AddComponent<Text>();
        text.text = value;
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = size;
        text.alignment = alignment;
        text.color = Color.white;
        return text;
    }
}
