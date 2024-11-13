using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestButtonNextScene : MonoBehaviour
{
    // Start is called before the first frame update
    public void LoadNext()
    {
        // Get the current scene's build index
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Calculate the next scene's index (next in the build settings)
        int nextSceneIndex = currentSceneIndex + 1;

        // Ensure the next scene index is valid (i.e., it doesn't exceed the total number of scenes in the build settings)
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.LogWarning("No next scene found! You've reached the last scene.");
        }
    }

}
