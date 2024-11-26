using Photon;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class VotingSystem : Photon.MonoBehaviour
{
    private new PhotonView photonView;
    private Dictionary<string, int> votes = new Dictionary<string, int>();
    public TMP_Text announcementText;

    HashSet<string> aswangRoles = new HashSet<string>
    {
        "aswang - mandurugo",
        "aswang - manananggal",
        "aswang - berbalang"
    };

    void Start()
    {
        photonView = GetComponent<PhotonView>();
        announcementText.text = (string)PhotonNetwork.room.CustomProperties["Announcement_Night"];
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
        foreach(var vote in votes)
        {
            Debug.Log("vote: " + vote);
        }
    }


    public void EliminatePlayer()
    {

        // Make sure the votes dictionary is not empty
        if (votes.Count == 0)
        {
            Debug.LogWarning("No votes have been cast. Cannot determine elimination.");
            ResetPhotonPlayerProperty();
            photonView.RPC("TransitionToNextPhase", PhotonTargets.All);
            return;
        }

        // Initialize variables to track the highest vote count and the list of tied players
        //string playerToEliminate = "";
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
        ResetPhotonPlayerProperty();
        // Check if there's a tie (i.e., multiple players with the same highest vote count)
        if (tiedPlayers.Count > 1)
        {
            // No elimination if there's a tie
            Debug.Log("Tie detected! No player will be eliminated.");

            ExitGames.Client.Photon.Hashtable roomProperty = new ExitGames.Client.Photon.Hashtable
            {
                { "Announcement_Day", "The vote is a tie. The game will continue" } // Mark the player as voted out
            };

            PhotonNetwork.room.SetCustomProperties(roomProperty);
            
            photonView.RPC("TransitionToNextPhase", PhotonTargets.All);
            return;  // Exit the function without eliminating anyone
        }
        else
        {
            // If there's no tie, eliminate the player with the most votes
            string playerToEliminate = tiedPlayers[0];
            ProcessVoteResults(playerToEliminate);
            return;
        }
    }

    public void ProcessVoteResults(string playerID)
    {
        // eliminate first before getting aswang count
        PhotonPlayer eliminatedPlayer = EliminatePhotonPlayer(playerID);

        int aswangCount = GetAswangPlayers();
        SetRoomProperty(aswangCount, CheckEliminatedPlayerRole((string)eliminatedPlayer.CustomProperties["playerID"]));
       
        // Announce the elimination to all players via RPC
        photonView.RPC("TransitionToNextPhase", PhotonTargets.All);
        return;
    }


    // checking role of eliminated player to notify users if they have eliminated the aswang
    public bool CheckEliminatedPlayerRole(string playerID)
    {
        foreach (PhotonPlayer player in PhotonNetwork.playerList)
        {
            Debug.Log("Currently eliminating player");
            if ((string)player.CustomProperties["playerID"] == playerID)
            {
                return aswangRoles.Contains((string)player.CustomProperties["Role"]);
            }
        }
        return false;
    }


    // setting the custom property of the eliminated player
    public PhotonPlayer EliminatePhotonPlayer(string playerID)
    {
        foreach (PhotonPlayer player in PhotonNetwork.playerList)
        {
            Debug.Log("Currently eliminating player");
            if ((string)player.CustomProperties["playerID"] == playerID)
            {
                Debug.Log("Eliminate player: " + player.NickName);
                ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable
                {
                    { "isVotedOut", true } // Mark the player as voted out
                };
                player.SetCustomProperties(playerProperties);
                return player;
            }
        }
        return null;
    }

    public void SetRoomProperty(int aswangCount, bool isAswangEliminated)
    {
        bool aliveVillagers = CheckAliveForVillagers();
        // Set the announcement message based on aswangCount and eliminated player role
        string announcement = aswangCount switch
        {
            0 => "There are no more aswang left in the game. Taumbayan Wins!",

            1 => isAswangEliminated
                ? "The eliminated player is the aswang. There is one aswang left in the game. The game will continue."
                : "The eliminated player is not the aswang. There is one aswang left in the game. The game will continue.",

            2 => isAswangEliminated
                ? "The eliminated player is the aswang. There are 2 more aswang left in the game. The game will continue."
                : "The eliminated player is not the aswang. There are 2 more aswang left in the game. The game will continue.",

            _ => $"There are {aswangCount} aswang left in the game."
        };

        // Set the room property with the announcement
        ExitGames.Client.Photon.Hashtable roomProperty = new ExitGames.Client.Photon.Hashtable
        {
            { "Announcement_Day", announcement }
        };

        // Additional logic for aswangCount == 0
        if (aswangCount == 0 && aliveVillagers)
        {
            // Set a different room property when there are no aswang left
            roomProperty["Game_Winner"] = "Villagers";
        }
        else if (aswangCount >= 1 && !aliveVillagers)
        {
            roomProperty["Game_Winner"] = "Aswang";
        }

        // Apply the custom properties
        PhotonNetwork.room.SetCustomProperties(roomProperty);
    }

    public bool CheckAliveForVillagers()
    {
        int villagerCount = 0; 

        foreach (PhotonPlayer player in PhotonNetwork.playerList)
        {
            if ((((bool)player.CustomProperties["isAlive"] &&
                !(bool)player.CustomProperties["isVotedOut"])))
            {
                villagerCount++;
                
            }
        }

        return villagerCount > 0;
    }

    public int GetAswangPlayers()
    {
        int aswangCount = 0;
        foreach (PhotonPlayer player in PhotonNetwork.playerList)
        {
            // must only count aswang players that are alive and not voted out
            if (aswangRoles.Contains((string)player.CustomProperties["Role"]) &&
                ((bool)player.CustomProperties["isAlive"] && 
                !(bool)player.CustomProperties["isVotedOut"]))
            {
                Debug.Log("Aswang found");
                aswangCount++;
            }
        }
        return aswangCount;
    }

    [PunRPC]
    void TransitionToNextPhase()
    {
        if ((bool)PhotonNetwork.player.CustomProperties["isVotedOut"])
        {
            PhotonNetwork.LoadLevel("VotedOutScene");
            return;
        }

        if ((bool)PhotonNetwork.player.CustomProperties["isAlive"] || !(bool)PhotonNetwork.player.CustomProperties["isVotedOut"])
        {
            PhotonNetwork.LoadLevel("Announcement_Day");
            return;
        }
    }

    public void ResetPhotonPlayerProperty()
    {
        foreach (PhotonPlayer player in PhotonNetwork.playerList)
        {
            player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "hasVoted", false } });
        }
        return;
    }
    
}