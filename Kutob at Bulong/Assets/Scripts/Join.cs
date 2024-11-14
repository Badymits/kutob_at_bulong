using Photon;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Join : Photon.MonoBehaviour
{
    public GameObject playerCardPrefab; // Prefab for player cards
    public Transform playerCardsContainer; // Parent container for player cards
    public TMP_Text roomCodeTMP; // Room code display
    public Transform[] spawnPoints; // Array of spawn points for player cards

    private List<PhotonPlayer> playersInRoom = new List<PhotonPlayer>(); // List of players in the room
    private PhotonView photonView;



    void Start()
    {
        photonView = GetComponent<PhotonView>();

        // Display room code if available
        string roomCode = "";
        if (PhotonNetwork.inRoom && PhotonNetwork.room.CustomProperties.ContainsKey("RoomCode"))
        {
            roomCode = PhotonNetwork.room.CustomProperties["RoomCode"].ToString();
            Debug.Log("The room code: " + roomCode);
        }

        if (PhotonNetwork.connected)
        {
            if (PhotonNetwork.isMasterClient)
            {
                ShowOwnerUI();
            }
            else
            {
                HideOwnerUI();
            }

            roomCodeTMP.text = roomCode;

            // Set player's nickname from PlayerPrefs
            PhotonNetwork.player.NickName = PlayerPrefs.GetString("Username");

            UpdatePlayerList();
        }
    }

    IEnumerator addDelay()
    {
        yield return new WaitForSecondsRealtime(.5f);
        UpdatePlayerList();
    }

    void ShowOwnerUI()
    {
        // Show UI elements for the lobby owner (if applicable)
    }

    void HideOwnerUI()
    {
        // Hide UI elements for non-owners (if applicable)
    }

    public void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        Debug.Log(newPlayer.NickName);
        Debug.Log($"{newPlayer.NickName} has entered the room.");
        playersInRoom.Add(newPlayer);

        GameObject playerCard = Instantiate(playerCardPrefab, playerCardsContainer);
        int playerIndex = playersInRoom.Count - 1; // Get the index of the new player
        SetPlayerPosition(playerCard, playerIndex); // Set position based on index

        StartCoroutine(addDelay());
        //UpdatePlayerList();
    }

    public void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        Debug.Log($"{otherPlayer.NickName} has left the room.");
        playersInRoom.Remove(otherPlayer);

        if (playerCardPrefab != null)
        {
            Destroy(playerCardPrefab);
        }

        // Clear all existing cards and re-instantiate remaining players' cards
        UpdatePlayerList();
    }

    private void UpdatePlayerList()
    {
        // Clear existing cards before updating
        foreach (Transform child in playerCardsContainer)
        {
            Destroy(child.gameObject);
        }

        float cardHeight = 300f;
        float cardSpacing = 10f;
        float startYPosition = 0f;

        // Loop through all players and instantiate a card for each one
        for (int i = 0; i < PhotonNetwork.playerList.Length; i++)
        {
            PhotonPlayer player = PhotonNetwork.playerList[i];

            // Instantiate player card at the spawn point
            GameObject playerCard = Instantiate(playerCardPrefab, playerCardsContainer);



            //// Set player's nickname on the card
            //TextMeshProUGUI textComponent = playerCard.GetComponentInChildren<TextMeshProUGUI>();
            //if (textComponent != null)
            //{
            //    textComponent.text = player.NickName; // Set player's nickname on the card
            //    textComponent.enableAutoSizing = false;
            //    textComponent.fontSize = 100;


            //}
            //else
            //{
            //    Debug.LogError("TextMeshProUGUI component not found in playerCardPrefab.");
            //}

            // Set position and size of the player's card
            RectTransform rectTransform = playerCard.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.localScale = Vector3.one;
                rectTransform.sizeDelta = new Vector2(200, cardHeight);
                rectTransform.anchoredPosition = new Vector2(0, startYPosition - (i * (cardHeight + cardSpacing)));
            }

            // Set parent container for proper hierarchy management
            SetPlayerPosition(playerCard, i);
        }
    }

    private void SetPlayerPosition(GameObject playerCard, int playerIndex)
    {
        if (playerIndex < spawnPoints.Length && spawnPoints[playerIndex] != null)
        {
            Vector3 spawnPosition = spawnPoints[playerIndex].position;

            RectTransform rectTransform = playerCard.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.position = spawnPosition;  // Set position based on predefined spawn point
                rectTransform.localScale = Vector3.one;  // Ensure correct scaling
            }
        }
        else
        {
            Debug.LogError($"Spawn point for player index {playerIndex} is missing or out of bounds.");
        }
    }



}