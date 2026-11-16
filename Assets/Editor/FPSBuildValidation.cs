using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class FPSBuildValidation
{
    private const string AkmPrefabPath = "Assets/Art/Weapons/AKM/Prefabs/AKM_AssetStore_PBR.prefab";

    [MenuItem("Bodycam/Report Missing Final Assets")]
    public static void ReportMissingFinalAssets()
    {
        var missing = GetMissingFinalAssets();
        if (missing.Count == 0)
        {
            Debug.Log("FINAL ASSET CHECK PASSED: all required imported models are present.");
            return;
        }
        Debug.LogWarning("FINAL APK BUILD IS BLOCKED. Missing or unfinished items:\n - " + string.Join("\n - ", missing));
    }

    public static void ValidateFinalAssets()
    {
        var missing = GetMissingFinalAssets();
        if (missing.Count > 0)
            throw new Exception("Final APK build blocked until real approved assets replace the placeholders:\n - " + string.Join("\n - ", missing));
    }

    private static List<string> GetMissingFinalAssets()
    {
        var missing = new List<string>();
        var akm = AssetDatabase.LoadAssetAtPath<GameObject>(AkmPrefabPath);
        if (!akm)
            missing.Add("Prepared Asset Store AKM prefab is missing: " + AkmPrefabPath);
        else if (akm.GetComponentsInChildren<Renderer>(true).Length < 1)
            missing.Add("Prepared AKM prefab has no renderers.");

        RequireModelFolder("Assets/Art/ThirdParty/Sketchfab/FPS_AK74M", "FPS rifle arms/animation model", missing);
        RequireModelFolder("Assets/Art/ThirdParty/Sketchfab/FPS_Pistol", "FPS pistol arms/animation model", missing);
        RequireModelFolder("Assets/Art/ThirdParty/Sketchfab/FSB_Operator", "full-body enemy character", missing);
        RequireModelFolder("Assets/Art/Environment/Abandoned_Warehouse", "abandoned warehouse environment", missing);

        var sceneBuilder = "Assets/Editor/FPSSceneBuilder.cs";
        if (File.Exists(sceneBuilder))
        {
            var source = File.ReadAllText(sceneBuilder);
            if (source.IndexOf("Kenney", StringComparison.OrdinalIgnoreCase) >= 0)
                missing.Add("FPSSceneBuilder still references prohibited Kenney assets.");
            if (source.Contains("CreatePrimitive"))
                missing.Add("FPSSceneBuilder still creates primitive placeholder geometry; replace weapons, arms, enemy, and environment with approved imported models.");
        }
        return missing;
    }

    private static void RequireModelFolder(string folder, string label, List<string> missing)
    {
        if (!Directory.Exists(folder))
        {
            missing.Add("Missing " + label + " folder: " + folder);
            return;
        }
        var models = Directory.GetFiles(folder, "*.*", SearchOption.AllDirectories);
        int modelCount = 0;
        foreach (var model in models)
        {
            var ext = Path.GetExtension(model).ToLowerInvariant();
            if (ext == ".fbx" || ext == ".glb" || ext == ".gltf") modelCount++;
        }
        if (modelCount == 0)
            missing.Add("No FBX/GLB/GLTF model found for " + label + " under " + folder);
    }
}
