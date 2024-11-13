using Photon;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Join : UnityEngine.MonoBehaviour
{
    //public InputField roomCodeInput;
    //public Text chatbox;
    public GameObject playerCardPrefab;
    public Transform playerCardsContainer;
    //public Text playersCountText;
    /*public Dropdown playersDropdown;
    public Dropdown aswangDropdown;*/
    public TMP_Text roomCode;

    private List<PhotonPlayer> playersInRoom = new List<PhotonPlayer>();
    private List<string> playerRoles = new List<string>();

    public GameObject ownerUIElement;

    // This method will be called when the player enters the room list lobby
    public void OnRoomListUpdate(System.Collections.Generic.List<RoomInfo> roomList)
    {
        foreach (RoomInfo room in roomList)
        {
            // Check if the room has the "roomCode" custom property
            if (room.CustomProperties.ContainsKey("RoomCode"))
            {
                string roomCode = room.CustomProperties["RoomCode"].ToString();
                Debug.Log("Room Code from room proeprties" + roomCode);
            }
        }
    }

    void Start()
    {
        RoomInfo room = GetComponent<RoomInfo>();
        Debug.Log(room.CustomProperties.ContainsKey("RoomCode"));
        
        if (PhotonNetwork.connected)
        {
            // Check if the current player is the master client (room owner)
            if (PhotonNetwork.isMasterClient)
            {
                // Show UI element only if the current player is the room owner (master client)
                ShowOwnerUI();
            }
            else
            {
                // Hide the UI element if the current player is not the room owner
                HideOwnerUI();
            }
            PhotonNetwork.player.NickName = "Player" + Random.Range(1000, 9999);
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings("1.0");
        }

        /*playersDropdown.onValueChanged.AddListener(delegate { OnPlayerCountChanged(); });
        aswangDropdown.onValueChanged.AddListener(delegate { OnAswangCountChanged(); });*/


        string savedRoomCode = PlayerPrefs.GetString("RoomCode");
        roomCode.text = savedRoomCode;
    }

    void ShowOwnerUI()
    {
        if (ownerUIElement != null)
        {
            ownerUIElement.SetActive(true);  // Enable the UI element
        }
    }

    // Method to hide the UI element
    void HideOwnerUI()
    {
        if (ownerUIElement != null)
        {
            ownerUIElement.SetActive(false);  // Disable the UI element
        }
    }

    void OnConnectedToMaster()
    {
        NotifyChatbox("Connected to Master Server. Please enter a room code to join.");
    }

    void OnJoinedRoom()
    {
        NotifyChatbox($"{PhotonNetwork.player.name} has joined the room.");
        UpdatePlayersList();
    }

    void OnPlayerEnteredRoom(PhotonPlayer newPlayer)
    {
        NotifyChatbox($"{newPlayer.name} has joined the room.");
        playersInRoom.Add(newPlayer);
        UpdatePlayersList();
    }

    void OnPlayerLeftRoom(PhotonPlayer otherPlayer)
    {
        NotifyChatbox($"{otherPlayer.name} has left the room.");
        playersInRoom.Remove(otherPlayer);
        UpdatePlayersList();
    }

    void OnJoinRoomFailed(string roomName, bool isOffline, string error)
    {
        NotifyChatbox("Failed to join room: " + error);
    }

    private void NotifyChatbox(string message)
    {
        /*if (chatbox != null)
        {
            chatbox.text += message + "\n";
        }*/
        Debug.Log(message);
    }

    private void UpdatePlayersList()
    {
        foreach (Transform child in playerCardsContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (PhotonPlayer player in PhotonNetwork.playerList)
        {
            GameObject playerCard = Instantiate(playerCardPrefab, playerCardsContainer);
            playerCard.GetComponentInChildren<Text>().text = player.NickName;
        }

        //playersCountText.text = $"Players: {PhotonNetwork.playerList.Length}/10";
    }

    private void SetGameRoles(int selectedPlayers, int selectedAswangs)
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
                playerRoles.Add("Aswang");
                NotifyChatbox($"{player.name} has been assigned the Aswang role (Privately).");
                aswangAssigned++;
            }
            else
            {
                playerRoles.Add("Normal");
                NotifyChatbox($"{player.name} is a normal player.");
            }
        }
    }

    private int DetermineAswangCount(int selectedPlayers)
    {
        if (selectedPlayers >= 5 && selectedPlayers <= 7) return 1;
        else if (selectedPlayers >= 8 && selectedPlayers <= 9) return 2;
        else if (selectedPlayers == 10) return 3;
        return 0;
    }

    private void RevealPlayerRole()
    {
        int playerIndex = PhotonNetwork.player.ID - 1;
        if (playerRoles.Count > playerIndex)
        {
            string role = playerRoles[playerIndex];
            NotifyChatbox($"Your role is: {role}");
        }
    }

    public void OnGameStart()
    {
        RevealPlayerRole();
    }
}
