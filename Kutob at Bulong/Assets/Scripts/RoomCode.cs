using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class RoomCode : MonoBehaviour
{
    //public InputField roomCodeInputField;
    public TMP_Text playerStatusText;

    private void Start()
    {
        //something to prevent the warning/error
    }

    public void OnConnectedToMaster()
    {
        Debug.Log("Connected to master server");
        //GoToGameScene(GenerateRoomCode(8));

        if (playerStatusText != null)
        {
            playerStatusText.text = "Player Connected";
        }
    }

    public void GenerateRandomRoomCode()
    {
        string roomCode = GenerateRoomCode(8);
        //roomCodeInputField.text = roomCode;
        PlayerPrefs.SetString("RoomCode", roomCode);
        PlayerPrefs.Save();
        GoToGameScene(roomCode);
    }

    private string GenerateRoomCode(int length)
    {
        string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        string randomRoomCode = "";
        System.Random random = new System.Random();

        for (int i = 0; i < length; i++)
        {
            randomRoomCode += chars[random.Next(chars.Length)];
        }

        return randomRoomCode;
    }

    public void GoToGameScene(string roomCode)
    {
        /*string roomCode = roomCodeInputField.text;
        if (string.IsNullOrEmpty(roomCode))
        {
            Debug.LogError("Room code is empty!");
            return;
        }*/

        PlayerPrefs.SetString("RoomCode", roomCode);
        PlayerPrefs.Save();

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 10;

        roomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable
        {
            { "RoomCode", roomCode },  // Store the room code in custom properties
            { "Scene", "CreateLobby" },
            { "Announcement_Night", "There were 0 victims during the night" }, // if there are any players eliminated during the night
            { "Announcement_Day", "The vote is a tie. The game will continue" },
            { "Game_Winner", "" },
            { "Night_Count", 0 },
            { "Day_Count", 0 },
            { "End_Phase", "" } // to synchronize the ending phase for other users that were eliminated beforehand
        };

        roomOptions.IsVisible = false;

        roomOptions.CustomRoomPropertiesForLobby = new string[] { roomCode };

        PhotonNetwork.CreateRoom(roomCode, roomOptions, null);

        //SceneManager.LoadScene("CreateLobby");
    }

    public void CopyRoomCode()
    {
        // Retrieve the room code from PlayerPrefs
        string roomCode = PlayerPrefs.GetString("RoomCode", string.Empty);

        // Check if the room code exists
        if (!string.IsNullOrEmpty(roomCode))
        {
            GUIUtility.systemCopyBuffer = roomCode; // Copy to clipboard
            Debug.Log("Room code copied to clipboard: " + roomCode);
        }
        else
        {
            Debug.LogWarning("No room code found to copy!");
        }
    }

    public void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("CreateLobby");
    }
}