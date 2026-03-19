using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class StageRuntimeConfigBootstrap
{
    private const string ResourcesFolder = "Assets/Resources";
    private const string AssetPath = ResourcesFolder + "/StageRuntimeConfig.asset";

    static StageRuntimeConfigBootstrap()
    {
        EditorApplication.delayCall += EnsureConfigExists;
    }

    private static void EnsureConfigExists()
    {
        if (EditorApplication.isCompiling || EditorApplication.isUpdating)
        {
            return;
        }

        if (!AssetDatabase.IsValidFolder(ResourcesFolder))
        {
            AssetDatabase.CreateFolder("Assets", "Resources");
        }

        StageRuntimeConfig config = AssetDatabase.LoadAssetAtPath<StageRuntimeConfig>(AssetPath);
        if (config == null)
        {
            config = ScriptableObject.CreateInstance<StageRuntimeConfig>();
            AssetDatabase.CreateAsset(config, AssetPath);
        }

        bool changed = false;
        changed |= AssignIfMissing(ref config.fighterPrefab, "Assets/Humanoid/humanoid2-tpose.fbx");
        changed |= AssignIfMissing(ref config.fighterController, "Assets/Humanoid/Animator Controller.controller");
        changed |= AssignIfMissing(ref config.arenaFloorMaterial, "Assets/Textures/rock_01.mat");
        changed |= AssignIfMissing(ref config.fallbackFighterMaterial, "Assets/Materials/Body_01.mat");
        changed |= Assign(ref config.skyboxMaterial, "Assets/SkyBoxes/DeepSpaceBlue/DSB.mat");

        if (changed)
        {
            EditorUtility.SetDirty(config);
            AssetDatabase.SaveAssets();
        }
    }

    private static bool AssignIfMissing<T>(ref T field, string path) where T : Object
    {
        if (field != null)
        {
            return false;
        }

        T loaded = AssetDatabase.LoadAssetAtPath<T>(path);
        if (loaded == null)
        {
            return false;
        }

        field = loaded;
        return true;
    }

    private static bool Assign<T>(ref T field, string path) where T : Object
    {
        T loaded = AssetDatabase.LoadAssetAtPath<T>(path);
        if (loaded == null || field == loaded)
        {
            return false;
        }

        field = loaded;
        return true;
    }
}
