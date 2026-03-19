using UnityEngine;
using UnityEngine.SceneManagement;

public class UIControl : MonoBehaviour
{
    public void ChangeScene(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogWarning("UIControl.ChangeScene called with an empty scene name.", this);
            return;
        }

        SceneManager.LoadScene(sceneName);
    }
}
