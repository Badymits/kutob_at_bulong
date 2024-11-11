using Photon;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Join : UnityEngine.MonoBehaviour
{
    public InputField roomCodeInput;
    public Text chatbox;
    public GameObject playerCardPrefab;
    public Transform playerCardsContainer;
    public Text playersCountText;
    public Dropdown playersDropdown;
    public Dropdown aswangDropdown;

    private List<PhotonPlayer> playersInRoom = new List<PhotonPlayer>();

    void Start()
    {
        if (PhotonNetwork.connected)
        {
            PhotonNetwork.player.name = "Player" + Random.Range(1000, 9999);
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings("1.0");
        }

        playersDropdown.onValueChanged.AddListener(delegate { OnPlayerCountChanged(); });
        aswangDropdown.onValueChanged.AddListener(delegate { OnAswangCountChanged(); });

        string savedRoomCode = PlayerPrefs.GetString("RoomCode");
        if (!string.IsNullOrEmpty(savedRoomCode))
        {
            roomCodeInput.text = savedRoomCode;
        }
    }

    public void JoinRoomWithCode()
    {
        string roomCode = roomCodeInput.text;
        if (string.IsNullOrEmpty(roomCode))
        {
            NotifyChatbox("Please enter a valid room code.");
            return;
        }

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.maxPlayers = 10;

        PhotonNetwork.JoinOrCreateRoom(roomCode, roomOptions, TypedLobby.Default);
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
        if (chatbox != null)
        {
            chatbox.text += message + "\n";
        }
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
            playerCard.GetComponentInChildren<Text>().text = player.name;
        }

        playersCountText.text = $"Players: {PhotonNetwork.playerList.Length}/10";
    }

    public void OnPlayerCountChanged()
    {
        int selectedPlayers = playersDropdown.value + 5;
        int selectedAswangs = DetermineAswangCount(selectedPlayers);
        aswangDropdown.value = selectedAswangs - 1;

        SetGameRoles(selectedPlayers, selectedAswangs);
    }

    public void OnAswangCountChanged()
    {
        int selectedPlayers = playersDropdown.value + 5;
        int selectedAswangs = aswangDropdown.value + 1;
        SetGameRoles(selectedPlayers, selectedAswangs);
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
        foreach (PhotonPlayer player in shuffledPlayers)
        {
            if (aswangAssigned < aswangCount)
            {
                NotifyChatbox($"{player.name} has been assigned the Aswang role.");
                aswangAssigned++;
            }
            else
            {
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
}