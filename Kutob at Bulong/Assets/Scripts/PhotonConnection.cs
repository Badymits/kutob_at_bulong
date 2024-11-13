using UnityEngine;
using Photon;

public class PhotonConnection : UnityEngine.MonoBehaviour // Specify UnityEngine.MonoBehaviour explicitly
{
    // Start is called before the first frame update
    void Start()
    {
        ConnectToPhoton();
    }

    void ConnectToPhoton()
    {
        if (!PhotonNetwork.connected)
        {
            PhotonNetwork.offlineMode = false; // Disable offline mode
            PhotonNetwork.ConnectUsingSettings("1.0"); // Connect to Photon using the settings in PhotonServerSettings
            Debug.Log("Connecting to Photon...");
        }
    }

    void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon!");
        PhotonNetwork.JoinLobby(); // Join the default lobby
    }

    void OnDisconnectedFromPhoton()
    {
        Debug.LogWarning("Disconnected from Photon");
    }

    void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby!");
        // You can start creating/joining rooms here
    }
}
