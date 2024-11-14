using Photon;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class VotingSystem : Photon.MonoBehaviour
{
    public PhotonView photonView;
    private Dictionary<string, int> votes = new Dictionary<string, int>();

    void Start()
    {
        // Make all player cards clickable
        foreach (Transform playerCard in transform)
        {
            Debug.Log(playerCard);
            
            Button button = playerCard.gameObject.AddComponent<Button>();
            string playerName = playerCard.GetComponentInChildren<TMPro.TextMeshProUGUI>().text;

            button.onClick.AddListener(() => CastVote(playerName));
        }
    }

    public void CastVote(string votedPlayerName)
    {
        if (!PhotonNetwork.player.CustomProperties.ContainsKey("hasVoted"))
        {
            // Send the vote across the network
            photonView.RPC("ReceiveVote", PhotonTargets.All, votedPlayerName);

            // Mark this player as having voted
            ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable();
            properties.Add("hasVoted", true);
            PhotonNetwork.player.SetCustomProperties(properties);
        }
    }

    [PunRPC]
    void ReceiveVote(string votedPlayerName)
    {
        // Count the vote
        if (!votes.ContainsKey(votedPlayerName))
            votes[votedPlayerName] = 0;
        votes[votedPlayerName]++;

        // Check if everyone has voted
        if (HasEveryoneVoted())
        {
            EliminatePlayer();
        }
    }

    bool HasEveryoneVoted()
    {
        foreach (PhotonPlayer player in PhotonNetwork.playerList)
        {
            if (!player.CustomProperties.ContainsKey("hasVoted"))
                return false;
        }
        return true;
    }

    void EliminatePlayer()
    {
        if (!PhotonNetwork.isMasterClient) return;

        // Find player with most votes
        string playerToEliminate = "";
        int maxVotes = 0;

        foreach (var vote in votes)
        {
            if (vote.Value > maxVotes)
            {
                maxVotes = vote.Value;
                playerToEliminate = vote.Key;
            }
        }

        // Announce elimination
        photonView.RPC("PlayerEliminated", PhotonTargets.All, playerToEliminate);
    }

    [PunRPC]
    void PlayerEliminated(string playerName)
    {
        Debug.Log($"{playerName} has been eliminated!");
        // Add your elimination logic here (e.g., disable player card, update UI, etc.)
    }
}