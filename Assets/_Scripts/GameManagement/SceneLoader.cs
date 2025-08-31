using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// A simple script to load a scene by its name.
/// Attach this to a persistent object in your scene (like a Canvas or an empty GameObject).
/// </summary>
public class SceneLoader : MonoBehaviour
{
    /// <summary>
    /// Loads the scene with the specified name.
    /// This method is public so it can be called from a UI Button's OnClick event.
    /// </summary>
    /// <param name="sceneName">The name of the scene to load.</param>
    public void LoadScene(string sceneName)
    {
        // Check if the sceneName is not empty or null
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("Scene name is not specified. Cannot load scene.");
        }
    }
}