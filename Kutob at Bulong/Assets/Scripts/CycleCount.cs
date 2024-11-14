using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class CycleCount : MonoBehaviour
{
    public TMP_Text count;
    private bool isTransitioning = false;
    public float roleSceneTimer = 5f;
    int night_counter = 0;
    int day_counter = 0;

    void Start()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        Debug.Log(currentScene.name.ToString());
        StartCoroutine(TimerToNextScene(currentScene.name.ToString()));
<<<<<<< HEAD

=======
        if (name == "NightTransition")
        {
            night_counter++;
        }
        else
        {
            day_counter++;
        }
>>>>>>> 0fc2becbcb8b05442ea6c6360191c9bfcc4e8dc5
    }

    private IEnumerator TimerToNextScene(string name)
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
            if (name == "NightTransition")
            {
                night_counter++;
                Increment("Night");

            }
            else
            {
                day_counter++;
                Increment("Day");
            }
        }
    }

    public void Increment(string time)
    {
        if (time == "Night")
        {
            Debug.Log("Increment count of night");
            count.text = night_counter.ToString();
            switchToNightPhase();
        }
        else
        {

            count.text = day_counter.ToString();
            switchtoDayPhase();
        }

    }



    void switchToNightPhase()
    {

        PhotonNetwork.LoadLevel("NightPhase");
    }

    void switchtoDayPhase()
    {
        PhotonNetwork.LoadLevel("Discussion Phase");
    }
}
