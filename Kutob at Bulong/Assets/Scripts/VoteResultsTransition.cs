using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoteResultsTransition : MonoBehaviour
{
    private PhotonView photonView;
    public float roleSceneTimer = 5f;
    // Start is called before the first frame update
    private bool isTransitioning = false;
    string player_role = "";
    string winner_type = "";

    HashSet<string> aswangRoles = new HashSet<string>
    {
        "aswang - mandurugo",
        "aswang - manananggal",
        "aswang - berbalang"
    };


    void Start()
    {
        photonView = FindAnyObjectByType<PhotonView>(); 
        player_role = (string)PhotonNetwork.player.CustomProperties["Role"];
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
            if ((string)PhotonNetwork.room.CustomProperties["Game_Winner"] != "")
            {
                photonView.RPC("DistributeGameResultsScene", PhotonTargets.All);
            }
            else
            {
                photonView.RPC("LoadNextScene", PhotonTargets.All);
            }
            
            
        }
    }
    [PunRPC]
    void DistributeGameResultsScene()
    {

        string gameWinner = (string)PhotonNetwork.room.CustomProperties["Game_Winner"];

        bool isAswang = aswangRoles.Contains(player_role);
        bool isVillagers = gameWinner == "Villagers";
        bool isAswangWinner = gameWinner == "Aswang";


        if (isVillagers)
        {
            winner_type = isAswang ? "d" : "a";  // "d" if Aswang, "a" if Villagers
        }
        else if (isAswangWinner)
        {
            winner_type = isAswang ? "c" : "b";  // "c" if Aswang, "b" if Villagers
        }


        GoToGameResults(winner_type);
    }

    [PunRPC]
    void LoadNextScene()
    {
        if ((bool)PhotonNetwork.player.CustomProperties["isAlive"] && !(bool)PhotonNetwork.player.CustomProperties["isVotedOut"])
        {
            PhotonNetwork.LoadLevel("NightTransition");
        }
        else
        {
            Debug.Log("Conditions not met. Not transitioning");
        }
        
    }

    void GoToGameResults(string type)
    {
        switch (type)
        {
            case "a":
                PhotonNetwork.LoadLevel("VictoryTaumbayan");
                break;
            case "b":
                PhotonNetwork.LoadLevel("DefeatTaumbayan");
                break;
            case "c":
                PhotonNetwork.LoadLevel("VictoryAswang");
                break;
            case "d":
                PhotonNetwork.LoadLevel("DefeatAswang");
                break;
            default:
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
