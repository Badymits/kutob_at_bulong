using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NightPhase : MonoBehaviour
{

    public enum PlayerRole
    {
        Villager,
        Werewolf,
        Seer
    }
    private PhotonView photonView;
    private Text player_name; 

    private List<PhotonPlayer> playersInRoom = new List<PhotonPlayer>();

    public GameObject playerPrefab;  // The prefab of the 2D player (assigned in the Inspector)
    public Transform spawnArea;      // The location where players will spawn (assigned in the Inspector)
    public float horizontalSpacing = 100f;  // Space between the cards in the row (in pixels)
    public int maxCardsPerRow = 5;


    void Start()
    {
        // Connect to Photon and join a room
        SimulateAdditionalPlayers();
        photonView.RPC("NewRole", PhotonTargets.All, ""); // last arg are the roles of each user
    }

    private void SimulateAdditionalPlayers()
    {
        int numberOfSimulatedPlayers = 5; // Simulating 5 players for testing
        for (int i = 1; i <= numberOfSimulatedPlayers; i++)
        {
            // Simulate player join by setting a unique nickname for each
            PhotonNetwork.player.NickName = "Player" + i;
            Debug.Log("Simulating player join: Player" + i);
            Debug.Log(PhotonNetwork.player.NickName);
            playersInRoom.Add(PhotonNetwork.player);
       
            // Instantiate the 2D player prefab for this simulated player
            InstantiatePlayerPrefab();
        }
        
    }

    private void InstantiatePlayerPrefab()
    {
        if (playerPrefab != null && spawnArea != null)
        {
            foreach(PhotonPlayer p in PhotonNetwork.playerList)
            {
                // Instantiate the prefab at the calculated position inside the container
                GameObject playerObject = Instantiate(playerPrefab, spawnArea);

                playerObject.name = "Player" + (p.NickName);
                Debug.Log("Instantiated 2D player prefab: " + playerObject.name);
            }
            DisplayPlayerList();
        }
        else
        {
            Debug.LogError("Player Prefab or Spawn Area is not assigned!");
        }
    }

    

    private void DisplayPlayerList()
    {
        PhotonPlayer[] players = PhotonNetwork.playerList;
        //Debug.Log("Players in the room:", playersInRoom);
        foreach (PhotonPlayer player in players)
        {
            Debug.Log(player.NickName);
        }
    }

}
