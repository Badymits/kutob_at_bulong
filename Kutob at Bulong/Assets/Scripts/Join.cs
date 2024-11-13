using Photon;
using Photon.Realtime;
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
    public TMP_Text roomCodeTMP;

    private List<PhotonPlayer> playersInRoom = new List<PhotonPlayer>();
    private List<string> playerRoles = new List<string>();

    public GameObject ownerUIElement;


    void Start()
    {
        string roomCode = "";

        if (PhotonNetwork.inRoom && PhotonNetwork.room.CustomProperties.ContainsKey("RoomCode")){
            roomCode = PhotonNetwork.room.CustomProperties["RoomCode"].ToString();
            Debug.Log("The room code: " + roomCode);
        }
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

            //string savedRoomCode = PlayerPrefs.GetString("RoomCode");
            roomCodeTMP.text = roomCode;
            PhotonNetwork.player.NickName = PlayerPrefs.GetString("Username");
            UpdatePlayersList();
        }

        /*playersDropdown.onValueChanged.AddListener(delegate { OnPlayerCountChanged(); });
        aswangDropdown.onValueChanged.AddListener(delegate { OnAswangCountChanged(); });*/

        
    
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
    

    public void OnJoinedRoom()
    {
        NotifyChatbox($"{PhotonNetwork.player.name} has joined the room.");
        UpdatePlayersList();
    }

    public void OnPlayerEnteredRoom(PhotonPlayer newPlayer)
    {
        NotifyChatbox($"{newPlayer.name} has joined the room.");
        playersInRoom.Add(newPlayer);
        UpdatePlayersList();
    }

    public void OnPlayerLeftRoom(PhotonPlayer otherPlayer)
    {
        NotifyChatbox($"{otherPlayer.name} has left the room.");
        playersInRoom.Remove(otherPlayer);
        UpdatePlayersList();
    }

    public void OnJoinRoomFailed(string roomName, bool isOffline, string error)
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
            playerCard.GetComponentInChildren<TextMeshProUGUI>().text = player.NickName;

            TextMeshProUGUI textComponent = playerCard.GetComponentInChildren<TextMeshProUGUI>();

            if (textComponent != null)
            {
                textComponent.text = player.NickName;

                // Disable Auto Size (to prevent it from resizing the text)
                textComponent.enableAutoSizing = false;

                // Optionally, set the font size manually
                textComponent.fontSize = 44;  // Example font size
            }
            RectTransform rectTransform = playerCard.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.localScale = Vector3.one; // Reset scale
                rectTransform.sizeDelta = new Vector2(200, 300); // Set a specific size if needed
            }
        }

        //playersCountText.text = $"Players: {PhotonNetwork.playerList.Length}/10";
    }

    /*private void SetGameRoles(int selectedPlayers, int selectedAswangs)
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
    }*/
}
