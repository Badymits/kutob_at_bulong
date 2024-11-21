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
    public PhotonPlayer photonPlayer;

    void Start()
    {
        photonPlayer = PhotonNetwork.player;
        Scene currentScene = SceneManager.GetActiveScene();
        Debug.Log(currentScene.name.ToString());
        StartCoroutine(TimerToNextScene(currentScene.name.ToString()));

        if (name == "NightTransition")
        {
            night_counter++;
            count.text = night_counter.ToString();
        }
        else
        {
            day_counter++;
            count.text = day_counter.ToString();
        }

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
                Increment("Night");

            }
            else
            {
                Increment("Day");
            }
        }
    }

    public void Increment(string time)
    {
        if (time == "Night")
        {
            Debug.Log("Increment count of night");
            switchToNightPhase();
        }
        else
        {
            switchtoDayPhase();
        }

    }



    void switchToNightPhase()
    {

        PhotonNetwork.LoadLevel("NightPhase");
    }

    void switchtoDayPhase()
    {

        if (!(bool)photonPlayer.CustomProperties["isAlive"])
        {
            PhotonNetwork.LoadLevel("EliminationScene");
        }
        else
        {
            PhotonNetwork.LoadLevel("Discussion Phase");
        }
        
    }
}
