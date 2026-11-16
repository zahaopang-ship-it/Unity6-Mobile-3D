[Reading 104 lines from start (total: 104 lines, 0 remaining)]

using System.IO;
using UnityEditor;
using UnityEngine;

public static class FPSAssetSetup
{
    private const string Root = "Assets/Art/Weapons/AKM/";
    private const string ModelPath = Root + "AKM_Anchors.fbx";
    private const string MaterialPath = Root + "Materials/AKM_PBR.mat";
    private const string PrefabPath = Root + "Prefabs/AKM_AssetStore_PBR.prefab";

    [MenuItem("Bodycam/Prepare AKM Asset")]
    public static void PrepareAKMAsset()
    {
        ConfigureTexture(Root + "Textures/AKM_BaseColor.jpg", TextureImporterType.Default, true, 2048, false);
        ConfigureTexture(Root + "Textures/AKM_Normal.png", TextureImporterType.NormalMap, false, 2048, false);
        ConfigureTexture(Root + "Textures/AKM_MetallicSmoothness.png", TextureImporterType.Default, false, 1024, true);
        ConfigureTexture(Root + "Textures/AKM_Occlusion.png", TextureImporterType.Default, false, 1024, false);
        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

        var albedo = AssetDatabase.LoadAssetAtPath<Texture2D>(Root + "Textures/AKM_BaseColor.jpg");
        var normal = AssetDatabase.LoadAssetAtPath<Texture2D>(Root + "Textures/AKM_Normal.png");
        var metallic = AssetDatabase.LoadAssetAtPath<Texture2D>(Root + "Textures/AKM_MetallicSmoothness.png");
        var occlusion = AssetDatabase.LoadAssetAtPath<Texture2D>(Root + "Textures/AKM_Occlusion.png");
        if (!albedo || !normal || !metallic || !occlusion)
            throw new System.Exception("AKM PBR texture set is incomplete. Check the four source texture files.");

        Directory.CreateDirectory(Root + "Materials");
        Directory.CreateDirectory(Root + "Prefabs");
        var material = AssetDatabase.LoadAssetAtPath<Material>(MaterialPath);
        if (!material)
        {
            var shader = Shader.Find("Standard");
            if (!shader) throw new System.Exception("Built-in Standard shader not found.");
            material = new Material(shader) { name = "AKM_PBR" };
            AssetDatabase.CreateAsset(material, MaterialPath);
        }
        material.shader = Shader.Find("Standard");
        material.SetTexture("_MainTex", albedo);
        material.SetTexture("_BumpMap", normal);
        material.SetTexture("_MetallicGlossMap", metallic);
        material.SetTexture("_OcclusionMap", occlusion);
        material.SetFloat("_Metallic", 1f);
        material.SetFloat("_Glossiness", 0.58f);
        material.SetFloat("_OcclusionStrength", 1f);
        material.SetFloat("_BumpScale", 1f);
        material.EnableKeyword("_NORMALMAP");
        material.EnableKeyword("_METALLICGLOSSMAP");
        material.EnableKeyword("_OCCLUSIONMAP");
        EditorUtility.SetDirty(material);

        var model = AssetDatabase.LoadAssetAtPath<GameObject>(ModelPath);
        if (!model) throw new System.Exception("AKM FBX not found at " + ModelPath);
        var root = new GameObject("AKM_AssetStore_PBR");
        var instance = (GameObject)PrefabUtility.InstantiatePrefab(model);
        instance.transform.SetParent(root.transform, false);
        instance.name = "AKM_Anchors";
        int renderers = 0;
        foreach (var renderer in instance.GetComponentsInChildren<Renderer>(true))
        {
            var materials = renderer.sharedMaterials;
            for (int i = 0; i < materials.Length; i++) materials[i] = material;
            renderer.sharedMaterials = materials;
            renderers++;
        }
        if (renderers == 0)
        {
            Object.DestroyImmediate(root);
            throw new System.Exception("AKM FBX imported without any renderers.");
        }
        PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
        Object.DestroyImmediate(root);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("AKM ASSET PREPARED: PBR material and raw prefab created. Renderers=" + renderers + ". Viewmodel orientation/attachment still requires calibration with the incoming FPS rig.");
    }

    private static void ConfigureTexture(string path, TextureImporterType type, bool srgb, int maxSize, bool preserveAlpha)
    {
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (!importer) throw new System.Exception("Texture importer missing: " + path);
        bool changed = false;
        if (importer.textureType != type) { importer.textureType = type; changed = true; }
        if (type != TextureImporterType.NormalMap && importer.sRGBTexture != srgb) { importer.sRGBTexture = srgb; changed = true; }
        if (importer.maxTextureSize != maxSize) { importer.maxTextureSize = maxSize; changed = true; }
        if (importer.textureCompression != TextureImporterCompression.CompressedHQ) { importer.textureCompression = TextureImporterCompression.CompressedHQ; changed = true; }
        if (importer.mipmapEnabled != true) { importer.mipmapEnabled = true; changed = true; }
        if (preserveAlpha && importer.alphaSource != TextureImporterAlphaSource.FromInput) { importer.alphaSource = TextureImporterAlphaSource.FromInput; changed = true; }
        if (preserveAlpha && importer.alphaIsTransparency) { importer.alphaIsTransparency = false; changed = true; }
        var android = importer.GetPlatformTextureSettings("Android");
        if (!android.overridden || android.format != TextureImporterFormat.ASTC_6x6 || android.maxTextureSize != maxSize)
        {
            android.name = "Android";
            android.overridden = true;
            android.format = TextureImporterFormat.ASTC_6x6;
            android.maxTextureSize = maxSize;
            android.textureCompression = TextureImporterCompression.CompressedHQ;
            android.compressionQuality = 60;
            importer.SetPlatformTextureSettings(android);
            changed = true;
        }
        if (changed) importer.SaveAndReimport();
    }
}

[executed on device: 7816f5ac7b89 (e2f42bfa-5503-4545-8f10-916f2d001bfd)]