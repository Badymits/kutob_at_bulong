using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VotingSystem : MonoBehaviour
{
    // Start is called before the first frame update
    public GameManager gameManager;
    public Button voteButton;
    public GameObject votingUI;

    public PhotonView photonView;

    private List<PhotonPlayer> votedPlayer = new List<PhotonPlayer>();  

    private void Start()
    {
        voteButton.onClick.AddListener(VoteToEliminate);
    }

    void VoteToEliminate()
    {
        
        if (photonView.isMine)
        {
            // Voting Logic: Each player votes to eliminate someone
            // This example assumes all votes go to the same player
            Debug.Log("Voting to eliminate a player");

            
        }
    }

    public void ShowVotingUI()
    {
        votingUI.SetActive(true); // Show voting UI
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
