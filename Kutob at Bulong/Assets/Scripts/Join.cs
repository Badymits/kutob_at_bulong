using Photon;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Join : Photon.MonoBehaviour
{
    public GameObject playerCardPrefab;
    public Transform playerCardsContainer;
    public TMP_Text roomCodeTMP;
    public Transform[] spawnPoints;

    private List<PhotonPlayer> playersInRoom = new List<PhotonPlayer>();
    private PhotonView photonView;

    void Start()
    {
        // Add null checks for required components
        if (playerCardPrefab == null || playerCardsContainer == null || roomCodeTMP == null)
        {
            Debug.LogError("Required components are not assigned in the Inspector!");
            return;
        }

        InitializeRoom();
    }

    private void InitializeRoom()
    {
        photonView = GetComponent<PhotonView>();
        if (!PhotonNetwork.connected) return;

        SetupRoomCode();
        SetupPlayerRole();
        UpdatePlayerList();
    }

    private void SetupRoomCode()
    {
        if (roomCodeTMP == null) return;

        string roomCode = "";
        if (PhotonNetwork.inRoom && PhotonNetwork.room.CustomProperties.ContainsKey("RoomCode"))
        {
            roomCode = PhotonNetwork.room.CustomProperties["RoomCode"].ToString();
            Debug.Log("The room code: " + roomCode);
        }
        roomCodeTMP.text = roomCode;
    }

    private void SetupPlayerRole()
    {
        if (PhotonNetwork.isMasterClient)
        {
            ShowOwnerUI();
        }
        else
        {
            HideOwnerUI();
        }
        PhotonNetwork.player.NickName = PlayerPrefs.GetString("Username", "Player");
    }

    public void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        if (newPlayer == null || playerCardsContainer == null || playerCardPrefab == null) return;

        Debug.Log($"{newPlayer.NickName} has entered the room.");
        playersInRoom.Add(newPlayer);

        GameObject playerCard = Instantiate(playerCardPrefab, playerCardsContainer);
        if (playerCard != null)
        {
            SetPlayerPosition(playerCard, playersInRoom.Count - 1);
            StartCoroutine(addDelay());
        }
    }

    public void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        if (otherPlayer == null) return;

        Debug.Log($"{otherPlayer.NickName} has left the room.");
        playersInRoom.Remove(otherPlayer);

        if (playerCardPrefab != null)
        {
            Destroy(playerCardPrefab);
        }
        UpdatePlayerList();
    }

    private void UpdatePlayerList()
    {
        if (playerCardsContainer == null) return;

        ClearExistingCards();
        CreatePlayerCards();
    }

    private void ClearExistingCards()
    {
        if (playerCardsContainer == null) return;

        foreach (Transform child in playerCardsContainer)
        {
            if (child != null)
            {
                Destroy(child.gameObject);
            }
        }
    }

    private void CreatePlayerCards()
    {
        if (PhotonNetwork.playerList == null || playerCardPrefab == null || playerCardsContainer == null) return;

        float cardHeight = 300f;
        float cardSpacing = 10f;
        float startYPosition = 0f;

        for (int i = 0; i < PhotonNetwork.playerList.Length; i++)
        {
            PhotonPlayer player = PhotonNetwork.playerList[i];
            if (player == null) continue;

            GameObject playerCard = Instantiate(playerCardPrefab, playerCardsContainer);

            BoxCollider2D box = playerCard.GetComponent<BoxCollider2D>();
            if (box != null)
            {
                Debug.Log("Has box collider");
            }
            if (playerCard != null)
            {
                SetupPlayerCard(playerCard, player, i, cardHeight, cardSpacing, startYPosition);
            }
        }
    }

    private void SetupPlayerCard(GameObject playerCard, PhotonPlayer player, int index, float cardHeight, float cardSpacing, float startYPosition)
    {
        if (playerCard == null || player == null) return;

        TextMeshProUGUI textComponent = playerCard.GetComponentInChildren<TextMeshProUGUI>();
        if (textComponent != null)
        {
            textComponent.text = player.NickName;
            textComponent.enableAutoSizing = false;
        }

        RectTransform rectTransform = playerCard.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.localScale = Vector3.one;
            rectTransform.sizeDelta = new Vector2(200, cardHeight);
            rectTransform.anchoredPosition = new Vector2(0, startYPosition - (index * (cardHeight + cardSpacing)));
        }

        SetPlayerPosition(playerCard, index);
    }

    private void SetPlayerPosition(GameObject playerCard, int playerIndex)
    {
        if (playerCard == null || spawnPoints == null || playerIndex >= spawnPoints.Length) return;

        if (spawnPoints[playerIndex] != null)
        {
            RectTransform rectTransform = playerCard.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.position = spawnPoints[playerIndex].position;
                rectTransform.localScale = Vector3.one;
            }
        }
        else
        {
            Debug.LogError($"Spawn point for player index {playerIndex} is missing or out of bounds.");
        }
    }

    IEnumerator addDelay()
    {
        yield return new WaitForSecondsRealtime(.5f);
        UpdatePlayerList();
    }

    void ShowOwnerUI()
    {
        // Show UI elements for the lobby owner
    }

    void HideOwnerUI()
    {
        // Hide UI elements for non-owners
    }
}