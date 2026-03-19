using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class SceneAutoRepair
{
    static SceneAutoRepair()
    {
        EditorSceneManager.sceneOpened += OnSceneOpened;
    }

    private static void OnSceneOpened(Scene scene, OpenSceneMode mode)
    {
        if (!scene.IsValid() || !IsPlayableScene(scene.path))
        {
            return;
        }

        bool changed = EnsureMenuSceneCamera(scene);
        changed |= EnsureEventSystem(scene);

        if (changed)
        {
            EditorSceneManager.MarkSceneDirty(scene);
        }
    }

    private static bool IsPlayableScene(string path)
    {
        return path.EndsWith("mainmenu.unity") || path.EndsWith("stage 1.unity");
    }

    private static bool EnsureMenuSceneCamera(Scene scene)
    {
        Camera[] cameras = scene.GetRootGameObjects()
            .SelectMany(root => root.GetComponentsInChildren<Camera>(true))
            .ToArray();

        Camera camera = cameras.FirstOrDefault();
        if (camera == null)
        {
            GameObject cameraObject = new GameObject("Main Camera");
            SceneManager.MoveGameObjectToScene(cameraObject, scene);
            camera = cameraObject.AddComponent<Camera>();
            cameraObject.AddComponent<AudioListener>();
            cameraObject.tag = "MainCamera";
            camera.transform.position = new Vector3(0f, 1f, -10f);
            camera.clearFlags = CameraClearFlags.Skybox;
            camera.targetDisplay = 0;
            return true;
        }

        bool changed = false;

        if (!camera.gameObject.activeSelf)
        {
            camera.gameObject.SetActive(true);
            changed = true;
        }

        if (!camera.enabled)
        {
            camera.enabled = true;
            changed = true;
        }

        if (camera.targetDisplay != 0)
        {
            camera.targetDisplay = 0;
            changed = true;
        }

        if (!camera.CompareTag("MainCamera"))
        {
            camera.tag = "MainCamera";
            changed = true;
        }

        if (camera.GetComponent<AudioListener>() == null)
        {
            camera.gameObject.AddComponent<AudioListener>();
            changed = true;
        }

        return changed;
    }

    private static bool EnsureEventSystem(Scene scene)
    {
        EventSystem eventSystem = scene.GetRootGameObjects()
            .SelectMany(root => root.GetComponentsInChildren<EventSystem>(true))
            .FirstOrDefault();

        if (eventSystem != null)
        {
            return false;
        }

        GameObject eventSystemObject = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        SceneManager.MoveGameObjectToScene(eventSystemObject, scene);
        return true;
    }
}
