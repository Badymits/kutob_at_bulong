using Photon;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        photonView = GetComponent<PhotonView>();
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
            PhotonNetwork.player.NickName = PlayerPrefs.GetString("Username");
            UpdatePlayerList();
        }
    }

    IEnumerator addDelay()
    {
        yield return new WaitForSecondsRealtime(.5f);
        UpdatePlayerList();
    }

    void ShowOwnerUI() { }
    void HideOwnerUI() { }

    public void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        Debug.Log($"{newPlayer.NickName} has entered the room.");
        playersInRoom.Add(newPlayer);

        StartCoroutine(addDelay());
    }

    public void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        Debug.Log($"{otherPlayer.NickName} has left the room.");
        playersInRoom.Remove(otherPlayer);

        StartCoroutine(addDelay());
    }

    private void NotifyChatbox(string message)
    {
        Debug.Log(message);
    }

    private void UpdatePlayerList()
    {
        foreach (Transform child in playerCardsContainer)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < PhotonNetwork.playerList.Length; i++)
        {
            PhotonPlayer player = PhotonNetwork.playerList[i];
            GameObject playerCard = Instantiate(playerCardPrefab, playerCardsContainer);

            TextMeshProUGUI textComponent = playerCard.GetComponentInChildren<TextMeshProUGUI>();
            if (textComponent != null)
            {
                textComponent.text = player.NickName;
                textComponent.enableAutoSizing = false;
                textComponent.fontSize = 44;
            }

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
                rectTransform.position = spawnPosition;
                rectTransform.localScale = Vector3.one;
            }
        }
        else
        {
            Debug.LogError($"Spawn point for player index {playerIndex} is missing or out of bounds.");
        }
    }
}