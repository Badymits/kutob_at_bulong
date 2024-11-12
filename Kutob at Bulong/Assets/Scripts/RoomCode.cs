using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RoomCode : MonoBehaviour
{
    public InputField roomCodeInputField;

    public void GenerateRandomRoomCode()
    {
        string roomCode = GenerateRoomCode(8);
        roomCodeInputField.text = roomCode;
        PlayerPrefs.SetString("RoomCode", roomCode);
        PlayerPrefs.Save();
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

    public void GoToGameScene()
    {
        string roomCode = roomCodeInputField.text;
        if (string.IsNullOrEmpty(roomCode))
        {
            Debug.LogError("Room code is empty!");
            return;
        }

        PlayerPrefs.SetString("RoomCode", roomCode);
        PlayerPrefs.Save();

        SceneManager.LoadScene("GameScene");
    }
}