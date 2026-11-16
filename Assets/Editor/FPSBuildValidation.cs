using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
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
        if (!akm || akm.GetComponentsInChildren<Renderer>(true).Length < 1)
            missing.Add("Prepared Asset Store AKM prefab is missing or has no renderers: " + AkmPrefabPath);

        RequireModelFolder("Assets/Art/ThirdParty/Sketchfab/FPS_AK74M", "FPS rifle arms/animation model", missing);
        RequireModelFolder("Assets/Art/ThirdParty/Sketchfab/FPS_Pistol", "FPS pistol arms/animation model", missing);
        RequireModelFolder("Assets/Art/ThirdParty/Sketchfab/FSB_Operator", "full-body enemy character", missing);
        RequireModelFolder("Assets/Art/Environment/Abandoned_Warehouse", "abandoned warehouse environment", missing);

        const string scenePath = "Assets/Scenes/BodycamTraining.unity";
        if (!File.Exists(scenePath))
        {
            missing.Add("Training scene is missing: " + scenePath);
            return missing;
        }
        EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        GameObject player = GameObject.Find("FPS Player");
        if (!player) missing.Add("FPS Player is missing from the training scene.");
        else
        {
            FPSWeapon weapon = player.GetComponent<FPSWeapon>();
            if (!weapon || !weapon.pistolModel || !weapon.akmModel) missing.Add("Imported pistol/AK rifle viewmodels are not connected to FPSWeapon.");
            else
            {
                if (weapon.pistolModel.GetComponentInChildren<SkinnedMeshRenderer>(true) == null) missing.Add("Pistol viewmodel has no skinned arms mesh.");
                if (weapon.akmModel.GetComponentInChildren<SkinnedMeshRenderer>(true) == null) missing.Add("AK viewmodel has no skinned arms mesh.");
            }
        }
        GameObject enemy = GameObject.Find("Training Enemy");
        if (!enemy || enemy.transform.Find("FSB Operator Character") == null) missing.Add("Imported FSB Operator is not integrated as the training enemy.");
        else if (enemy.GetComponentInChildren<SkinnedMeshRenderer>(true) == null || enemy.GetComponentInChildren<Animator>(true) == null)
            missing.Add("FSB Operator model is missing skinned renderers or Animator.");

        GameObject warehouse = GameObject.Find("Abandoned Warehouse Environment");
        if (!warehouse || warehouse.GetComponentsInChildren<MeshCollider>(true).Length < 10)
            missing.Add("Imported warehouse environment or mesh collision setup is missing.");
        if (!Camera.main || !Camera.main.GetComponent<FPSBodycamEffects>()) missing.Add("Bodycam camera effects are not connected.");
        if (!GameObject.Find("EventSystem") || !GameObject.Find("FIRE Button") || !GameObject.Find("AIM Button") || !GameObject.Find("RELOAD Button") || !GameObject.Find("SWAP Button"))
            missing.Add("Required mobile touch controls are missing.");
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