using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoteResultsTransition : MonoBehaviour
{
    private PhotonView photonView;
    public float roleSceneTimer = 5f;
    // Start is called before the first frame update
    private bool isTransitioning = false;
    void Start()
    {
        StartCoroutine(TimerToNextScene());
    }

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
            photonView.RPC("LoadNextScene", PhotonTargets.All);
            
        }
    }

    [PunRPC]
    void LoadNextScene()
    {
        if ((bool)PhotonNetwork.player.CustomProperties["isAlive"] || !(bool)PhotonNetwork.player.CustomProperties["isVotedOut"])
        {
            PhotonNetwork.LoadLevel("NightTransition");
        }
        else
        {
            Debug.Log("Conditions not met. Not transitioning");
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
