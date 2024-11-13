using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RoomCode : MonoBehaviour
{
    //public InputField roomCodeInputField;


    private void Start()
    {
        //something to prevent the warning/error
    }

    public void OnConnectedToMaster()
    {
        Debug.Log("Connected to master server");
        //GoToGameScene(GenerateRoomCode(8));
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
            { "Scene", "CreateLobby" }
        };

        roomOptions.IsVisible = false;

        roomOptions.CustomRoomPropertiesForLobby = new string[] { roomCode };

        PhotonNetwork.CreateRoom(roomCode, roomOptions, null);

        //SceneManager.LoadScene("CreateLobby");
    }


    public void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("CreateLobby");
    }
}