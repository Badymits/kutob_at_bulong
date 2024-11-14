using UnityEngine;

public class Quit : MonoBehaviour
{
    public void QuitGame()
    {
        // This will quit the game when running in a build
        Debug.Log("Game is quitting...");
        Application.Quit();

        // If in the Unity editor, it will stop the play mode
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
