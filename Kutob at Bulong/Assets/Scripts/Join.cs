using Photon;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static NightPhaseManager;

public class Join : UnityEngine.MonoBehaviour
{

    //public InputField roomCodeInput;
    //public Text chatbox;
    public GameObject playerCardPrefab;
    public Transform playerCardsContainer;
    //public Text playersCountText;
    /*public Dropdown playersDropdown;
    public Dropdown aswangDropdown;*/
    public TMP_Text roomCodeTMP;
    public Transform[] spawnPoints; // Array of spawn points

    private List<PhotonPlayer> playersInRoom = new List<PhotonPlayer>();
    private Dictionary<int, string> playerRoles = new Dictionary<int, string>();
    //private List<string> playerRoles = new List<string>();
    private PhotonView photonView;

    public GameObject ownerUIElement;


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
            SetPlayerPosition();
        }

        /*playersDropdown.onValueChanged.AddListener(delegate { OnPlayerCountChanged(); });
        aswangDropdown.onValueChanged.AddListener(delegate { OnAswangCountChanged(); });*/

    }


    IEnumerator addDelay()
    {
        yield return new WaitForSecondsRealtime(.5f);
        UpdatePlayerList();
    }

    void ShowOwnerUI()
    {
        if (ownerUIElement != null)
        {
            ownerUIElement.SetActive(true);
        }
    }

    void HideOwnerUI()
    {
        if (ownerUIElement != null)
        {
            ownerUIElement.SetActive(false);
        }
    }

    public void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        
        Debug.Log(newPlayer.NickName);
        Debug.Log($"{newPlayer.NickName} has entered the room.");
        playersInRoom.Add(newPlayer);
        StartCoroutine(addDelay());
        UpdatePlayerList();
        SetPlayerPosition(); // Update positions after a new player joins
    }


    public void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        Debug.Log($"{otherPlayer.NickName} has left the room.");
        playersInRoom.Remove(otherPlayer);
        UpdatePlayerList();
        SetPlayerPosition(); // Update positions after a new player joins
    }


    private void NotifyChatbox(string message)
    {
        /*if (chatbox != null)
        {
            chatbox.text += message + "\n";
        }*/
        Debug.Log(message);
    }



    private void UpdatePlayerList()
    {


        foreach (Transform child in playerCardsContainer)
        {
            Destroy(child.gameObject);
        }

        float cardHeight = 300f;
        float cardSpacing = 10f;
        float startYPosition = 0f;

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

            RectTransform rectTransform = playerCard.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.localScale = Vector3.one;
                rectTransform.sizeDelta = new Vector2(200, cardHeight);
                rectTransform.anchoredPosition = new Vector2(0, startYPosition - (i * (cardHeight + cardSpacing)));
            }

            playerCard.transform.SetParent(playerCardsContainer);
        }
    }

    private void SetPlayerPosition()
    {
        int playerIndex = PhotonNetwork.playerList.Length - 1;
        if (playerIndex < spawnPoints.Length)
        {
            Vector3 spawnPosition = spawnPoints[playerIndex].position;
            photonView.RPC("SetPositionRPC", PhotonNetwork.player, spawnPosition);
        }
    }

    [PunRPC]
    private void SetPositionRPC(Vector3 position)
    {
        transform.position = position;
    }
    public void SetGameRoles(int selectedPlayers, int selectedAswangs)
    {
        if (selectedAswangs > selectedPlayers)
        {
            NotifyChatbox("Invalid Aswang count. Aswang count cannot exceed total players.");
            return;
        }

        NotifyChatbox($"Total Players: {selectedPlayers}, Aswangs: {selectedAswangs}");
        AssignRolesToPlayers(selectedAswangs);
    }

    private void AssignRolesToPlayers(int aswangCount)
    {
        List<PhotonPlayer> shuffledPlayers = new List<PhotonPlayer>(PhotonNetwork.playerList);
        System.Random rand = new System.Random();
        shuffledPlayers.Sort((x, y) => rand.Next(-1, 2));

        int aswangAssigned = 0;
        playerRoles.Clear();

        foreach (PhotonPlayer player in shuffledPlayers)
        {
            if (aswangAssigned < aswangCount)
            {
                playerRoles[player.ID] = "Aswang";
                photonView.RPC("AssignRoleToPlayer", player, "Aswang");
                aswangAssigned++;
            }
            else
            {
                playerRoles[player.ID] = "Normal";
                photonView.RPC("AssignRoleToPlayer", player, "Normal");
            }
        }
    }

    [PunRPC]
    private void AssignRoleToPlayer(string role)
    {
        NotifyChatbox($"Your role is: {role}");
    }

    private int DetermineAswangCount(int selectedPlayers)
    {
        if (selectedPlayers >= 5 && selectedPlayers <= 7) return 1;
        else if (selectedPlayers >= 8 && selectedPlayers <= 9) return 2;
        else if (selectedPlayers == 10) return 3;
        return 0;
    }

    public void OnGameStart()
    {
        if (playerRoles.ContainsKey(PhotonNetwork.player.ID))
        {
            string role = playerRoles[PhotonNetwork.player.ID];
            NotifyChatbox($"Your role is: {role}");
        }
    }
}



