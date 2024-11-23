using Photon;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class VotingSystem : Photon.MonoBehaviour
{
    private new PhotonView photonView;
    private Dictionary<string, int> votes = new Dictionary<string, int>();

    HashSet<string> aswangRoles = new HashSet<string>
    {
        "aswang - mandurugo",
        "aswang - manananggal",
        "aswang - berbalang"
    };

    void Start()
    {
        
        photonView = GetComponent<PhotonView>();
    }

    public void CastVote(string playerID)
    {
        // Send the vote across the network
        photonView.RPC("ReceiveVote", PhotonTargets.All, playerID);
    }

    [PunRPC]
    void ReceiveVote(string votedPlayerID)
    {
        Debug.Log("Voted for: " + votedPlayerID);
        // Count the vote
        if (!votes.ContainsKey(votedPlayerID))
        {
            votes[votedPlayerID] = 0;
            votes[votedPlayerID]++;
        } 
        else
        {
            votes[votedPlayerID]++;
        }
        Debug.Log("The votes: " + votes);
    }


    public void EliminatePlayer()
    {

        // Make sure the votes dictionary is not empty
        if (votes.Count == 0)
        {
            Debug.LogWarning("No votes have been cast. Cannot determine elimination.");
            photonView.RPC("TransitionToNextPhase", PhotonTargets.All);
            return;
        }

        // Initialize variables to track the highest vote count and the list of tied players
        string playerToEliminate = "";
        int maxVotes = 0;
        List<string> tiedPlayers = new List<string>();

        // Loop through the votes to find the player(s) with the most votes
        foreach (var vote in votes)
        {
            if (vote.Value > maxVotes)
            {
                // Found a new highest vote, reset the tied players list
                maxVotes = vote.Value;
                tiedPlayers.Clear();
                tiedPlayers.Add(vote.Key);  // Add the player with the highest votes
            }
            else if (vote.Value == maxVotes)
            {
                // Add to the tied players list if the vote count matches the current max
                tiedPlayers.Add(vote.Key);
            }
        }

        // Check if there's a tie (i.e., multiple players with the same highest vote count)
        if (tiedPlayers.Count > 1)
        {
            // No elimination if there's a tie
            Debug.Log("Tie detected! No player will be eliminated.");
            return;  // Exit the function without eliminating anyone
        }
        else
        {
            // If there's no tie, eliminate the player with the most votes
            playerToEliminate = tiedPlayers[0];

            ProcessVoteResults(playerToEliminate);

        }
    }

    public void ProcessVoteResults(string playerID)
    {
        int aswangCount = GetAswangPlayers();

        EliminatePlayer(playerID);
        SetRoomProperty(aswangCount);

        // Announce the elimination to all players via RPC
        photonView.RPC("TransitionToNextPhase", PhotonTargets.All);
    }

    public void SetRoomProperty(int aswangCount)
    {
        // Set the announcement message based on aswangCount
        string announcement = aswangCount switch
        {
            0 => "There are no more aswang left in the game. Taumbayan Wins!",
            1 => "There is one aswang left in the game. The game will continue",
            2 => "There are 2 more aswang left in the game.",
            _ => $"There are {aswangCount} aswang left in the game."
        };

        // Set the room property with the announcement
        ExitGames.Client.Photon.Hashtable roomProperty = new ExitGames.Client.Photon.Hashtable
        {
            { "Announcement_Day", announcement }
        };

        // Additional logic for aswangCount == 0
        if (aswangCount == 0)
        {
            // Set a different room property when there are no aswang left
            roomProperty["Game_Winners"] = "Taumbayan Wins! No more aswang left in the game!";
        }

        // Apply the custom properties
        PhotonNetwork.room.SetCustomProperties(roomProperty);
    }

    public int GetAswangPlayers()
    {
        int aswangCount = 0;
        foreach (PhotonPlayer player in PhotonNetwork.playerList)
        {
            // must only count aswang players that are alive and not voted out
            if (aswangRoles.Contains((string)player.CustomProperties["Role"]) &&
                ((bool)player.CustomProperties["isAlive"] && !(bool)player.CustomProperties["isVotedOut"]))
            {
                aswangCount++;
            }
        }
        return aswangCount;
    }

    [PunRPC]
    void TransitionToNextPhase()
    {
        PhotonNetwork.LoadLevel("Announcement_Day");
    }

    public void EliminatePlayer(string playerID)
    { 
        foreach(PhotonPlayer player in PhotonNetwork.playerList)
        {
            if ((string)player.CustomProperties["playerID"] == playerID)
            {
                ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable
                {
                    { "isVotedOut", true } // Mark the player as voted out
                };
                player.SetCustomProperties(playerProperties);
            }
        }  
    }
}