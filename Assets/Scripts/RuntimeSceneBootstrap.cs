using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public static class RuntimeSceneBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void EnsureSceneIsRunnable()
    {
        EnsureEventSystem();
        EnsureCamera();
    }

    private static void EnsureEventSystem()
    {
        EventSystem existingEventSystem = Object.FindAnyObjectByType<EventSystem>();
        if (existingEventSystem != null)
        {
            return;
        }

        new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
    }

    private static void EnsureCamera()
    {
        Camera[] cameras = Object.FindObjectsByType<Camera>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        Camera chosenCamera = null;

        foreach (Camera camera in cameras)
        {
            if (camera == null)
            {
                continue;
            }

            if (!camera.gameObject.activeSelf)
            {
                camera.gameObject.SetActive(true);
            }

            if (!camera.enabled)
            {
                camera.enabled = true;
            }

            camera.targetDisplay = 0;

            if (chosenCamera == null)
            {
                chosenCamera = camera;
            }
        }

        if (chosenCamera == null)
        {
            GameObject cameraObject = new GameObject("Main Camera");
            chosenCamera = cameraObject.AddComponent<Camera>();
            cameraObject.tag = "MainCamera";
            chosenCamera.clearFlags = CameraClearFlags.Skybox;
            chosenCamera.transform.position = new Vector3(0f, 1f, -10f);
        }

        if (!chosenCamera.CompareTag("MainCamera"))
        {
            chosenCamera.tag = "MainCamera";
        }

        AudioListener[] listeners = Object.FindObjectsByType<AudioListener>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (AudioListener listener in listeners)
        {
            if (listener != null)
            {
                listener.enabled = listener.gameObject == chosenCamera.gameObject;
            }
        }

        if (chosenCamera.GetComponent<AudioListener>() == null)
        {
            chosenCamera.gameObject.AddComponent<AudioListener>();
        }
    }
}
