using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransition : MonoBehaviour
{
    // Start is called before the first frame update
    // Timer delay before transitioning to the introduction scene (e.g., 10 seconds)
    public float roleSceneTimer = 10f;

    // Flag to ensure the scene transition happens only once
    private bool isTransitioning = false;

    void Start()
    {
        StartCoroutine(TimerToNextScene());
    }

    // Coroutine to handle the timer and transition to the next scene
    private IEnumerator TimerToNextScene()
    {
        Debug.Log("Starting timer for scene transition...");

        // Wait for the specified time in seconds
        yield return new WaitForSeconds(roleSceneTimer);

        // After the timer expires, load the introduction scene
        if (!isTransitioning)
        {
            isTransitioning = true;
            Debug.Log("Timer expired. Transitioning to the Introduction Scene.");

            // Transition to the introduction scene for all players
            PhotonNetwork.LoadLevel("Introduction");
        }
    }
}
