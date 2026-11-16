using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

/// <summary>Deterministic importer/prefab preparation for the approved Sketchfab assets.
/// Run from Bodycam > Prepare Imported Assets after Unity has completed the initial import.</summary>
public static class FPSImportedAssetSetup
{
    private const string Root = "Assets/Art/ThirdParty/Sketchfab/";
    private const string Output = "Assets/Art/Prefabs/Imported/";
    private const string Ak = Root + "FPS_AK74M/FPS_AK74M.fbx";
    private const string Pistol = Root + "FPS_Pistol/FPS_Pistol.fbx";
    private const string Fsb = Root + "FSB_Operator/FSB_Operator.fbx";
    private const string Warehouse = "Assets/Art/Environment/Abandoned_Warehouse/Abandoned_Warehouse.fbx";

    [MenuItem("Bodycam/Prepare Imported Assets")]
    public static void Prepare()
    {
        EnsureFolder("Assets/Art/Prefabs");
        EnsureFolder(Output);
        ConfigureModel(Ak, ModelImporterAnimationType.Generic, true);
        ConfigureModel(Pistol, ModelImporterAnimationType.Generic, true);
        ConfigureModel(Fsb, ModelImporterAnimationType.Generic, true);
        ConfigureModel(Warehouse, ModelImporterAnimationType.None, false);
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

        CreateAnimatedPrefab(Ak, Output + "FPS_AK74M_Arms.prefab", Output + "FPS_AK74M_Arms.controller", "AK_Idle", new[] { "AK_Shot", "AK_Reload", "AK_Reload_full", "AK_Reload_var2", "AK_Draw", "AK_Walk", "AK_Run", "AK_Dpose" });
        CreateAnimatedPrefab(Pistol, Output + "FPS_Pistol_Arms.prefab", Output + "FPS_Pistol_Arms.controller", "FPS_Pistol_Idle", new[] { "FPS_Pistol_Fire", "FPS_Pistol_Reload_easy", "FPS_Pistol_Reload_full", "FPS_Pistol_Walk", "Arms_FPS_Anim_Walk", "Arms_FPS_Anim_OneShot" });
        CreateAnimatedPrefab(Fsb, Output + "FSB_Operator.prefab", Output + "FSB_Operator.controller", null, new[] { "mixamo.com|Layer0" });
        CreateStaticPrefab(Warehouse, Output + "Abandoned_Warehouse.prefab");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Imported asset preparation finished. Review prefab scale/orientation/materials and animation clip names in the Editor before scene integration.");
    }

    private static void ConfigureModel(string path, ModelImporterAnimationType rig, bool animations)
    {
        if (!File.Exists(path)) throw new FileNotFoundException("Required imported model is missing", path);
        var importer = AssetImporter.GetAtPath(path) as ModelImporter;
        if (importer == null) throw new InvalidOperationException("No ModelImporter for " + path);
        importer.animationType = rig;
        importer.importAnimation = animations;
        importer.importCameras = false;
        importer.importLights = false;
        importer.importMaterials = true;
        importer.materialImportMode = ModelImporterMaterialImportMode.ImportStandard;
        importer.meshCompression = ModelImporterMeshCompression.Medium;
        importer.isReadable = false;
        importer.optimizeMesh = true;
        importer.SaveAndReimport();
    }

    private static void CreateAnimatedPrefab(string modelPath, string prefabPath, string controllerPath, string defaultClip, string[] desired)
    {
        GameObject source = AssetDatabase.LoadAssetAtPath<GameObject>(modelPath);
        if (source == null) throw new InvalidOperationException("Model failed to import: " + modelPath);
        var clips = new List<AnimationClip>();
        foreach (var obj in AssetDatabase.LoadAllAssetsAtPath(modelPath))
        {
            var clip = obj as AnimationClip;
            if (clip != null && !clip.name.StartsWith("__preview__", StringComparison.OrdinalIgnoreCase)) clips.Add(clip);
        }
        var controller = AnimatorController.CreateAnimatorControllerAtPath(controllerPath);
        var sm = controller.layers[0].stateMachine;
        AnimatorState idle = null;
        foreach (var clip in clips)
        {
            AnimatorState state = sm.AddState(clip.name);
            state.motion = clip;
            if (idle == null && (string.IsNullOrEmpty(defaultClip) || clip.name.IndexOf(defaultClip, StringComparison.OrdinalIgnoreCase) >= 0)) idle = state;
        }
        if (idle != null) sm.defaultState = idle;
        foreach (var key in desired)
        {
            bool found = false;
            foreach (var clip in clips) if (clip.name.IndexOf(key, StringComparison.OrdinalIgnoreCase) >= 0) { found = true; break; }
            if (!found) Debug.LogWarning("Expected animation clip not found: " + key + " in " + modelPath);
        }
        var instance = PrefabUtility.InstantiatePrefab(source) as GameObject;
        if (instance == null) throw new InvalidOperationException("Could not instantiate " + modelPath);
        instance.name = Path.GetFileNameWithoutExtension(prefabPath);
        var animator = instance.GetComponent<Animator>();
        if (animator == null) animator = instance.AddComponent<Animator>();
        animator.runtimeAnimatorController = controller;
        animator.applyRootMotion = false;
        PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
        UnityEngine.Object.DestroyImmediate(instance);
    }

    private static void CreateStaticPrefab(string modelPath, string prefabPath)
    {
        GameObject source = AssetDatabase.LoadAssetAtPath<GameObject>(modelPath);
        if (source == null) throw new InvalidOperationException("Environment failed to import: " + modelPath);
        var instance = PrefabUtility.InstantiatePrefab(source) as GameObject;
        if (instance == null) throw new InvalidOperationException("Could not instantiate environment " + modelPath);
        instance.name = "Abandoned Warehouse Environment";
        foreach (var renderer in instance.GetComponentsInChildren<Renderer>(true)) renderer.gameObject.isStatic = true;
        PrefabUtility.SaveAsPrefabAsset(instance, prefabPath);
        UnityEngine.Object.DestroyImmediate(instance);
    }

    private static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path)) return;
        string parent = Path.GetDirectoryName(path).Replace('\\', '/');
        string leaf = Path.GetFileName(path);
        if (!AssetDatabase.IsValidFolder(parent)) EnsureFolder(parent);
        AssetDatabase.CreateFolder(parent, leaf);
    }
}
