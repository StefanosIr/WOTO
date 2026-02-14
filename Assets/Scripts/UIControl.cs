using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class UIControl : MonoBehaviour {

	/// <summary>
	/// Changes to a different scene by name
	/// </summary>
	/// <param name="sceneName">Name of the scene to load</param>
	public void ChangeScene(string sceneName)
	{
		SceneManager.LoadScene(sceneName);
	}

	/// <summary>
	/// Quits the application
	/// </summary>
	public void QuitGame()
	{
		Application.Quit();
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#endif
	}

	/// <summary>
	/// Reloads the current scene
	/// </summary>
	public void RestartScene()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
}
