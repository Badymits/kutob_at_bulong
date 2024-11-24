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
    int night_counter;
    int day_counter;
    public PhotonPlayer photonPlayer;
    public PhotonView photonView;

    void Start()
    {
        // Get initial counters from room custom properties
        night_counter = (int)PhotonNetwork.room.CustomProperties["Night_Count"];
        day_counter = (int)PhotonNetwork.room.CustomProperties["Day_Count"];

        photonView = FindObjectOfType<PhotonView>();

        // Update the UI with current values
        Scene currentScene = SceneManager.GetActiveScene();
        Debug.Log(currentScene.name);

        // You can trigger the initial scene count setting from any client, not just Master Client
        photonView.RPC("SetNightOrDayCount", PhotonTargets.All, currentScene.name);
    }

    [PunRPC]
    void SetNightOrDayCount(string sceneName)
    {
        // Increment the counters based on the scene
        if (sceneName == "NightTransition")
        {
            night_counter++;
            // Update room custom properties
            ExitGames.Client.Photon.Hashtable newProperties = new ExitGames.Client.Photon.Hashtable();
            newProperties["Night_Count"] = night_counter;
            PhotonNetwork.room.SetCustomProperties(newProperties);
        }
        else
        {
            day_counter++;
            ExitGames.Client.Photon.Hashtable newProperties = new ExitGames.Client.Photon.Hashtable();
            newProperties["Day_Count"] = day_counter;
            PhotonNetwork.room.SetCustomProperties(newProperties);
        }

        // Update the UI locally
        count.text = (sceneName == "NightTransition") ? night_counter.ToString() : day_counter.ToString();

        // Start timer for next scene transition
        StartCoroutine(TimerToNextScene(sceneName));
    }

    private IEnumerator TimerToNextScene(string name)
    {
        Debug.Log("Starting timer for scene transition...");

        // Wait for the specified time in seconds
        yield return new WaitForSeconds(roleSceneTimer);

        // After the timer expires, load the appropriate scene
        if (!isTransitioning)
        {
            isTransitioning = true;
            Debug.Log("Timer expired. Transitioning to the next scene.");

            // Transition to the appropriate phase (Night/Day)
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
        // Ensure the scene transition happens for all players
        photonView.RPC("TransitionToNight", PhotonTargets.All);
    }

    void switchtoDayPhase()
    {
        // Ensure the scene transition happens for all players
        photonView.RPC("TransitionToDay", PhotonTargets.All);
    }

    [PunRPC]
    void TransitionToNight()
    {
        PhotonNetwork.LoadLevel("NightPhase");
    }

    [PunRPC]
    void TransitionToDay()
    {
        Debug.Log("local player: " + PhotonNetwork.player);
        PhotonPlayer photonPlayer = PhotonNetwork.player;  // Using PhotonPlayer in PUN 1
        if (!(bool)photonPlayer.CustomProperties["isAlive"])
        {
            // For players who are eliminated, load the eliminated scene
            PhotonNetwork.LoadLevel("EliminatedScene");
        }
        else
        {
            // For alive players, load the discussion phase scene
            PhotonNetwork.LoadLevel("Discussion Phase");
        }
    }
}
