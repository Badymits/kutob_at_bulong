using UnityEngine;
using TMPro;

public class ChatManager : MonoBehaviour
{
    public TMP_InputField messageInput; // Input field for typing messages
    public TextMeshProUGUI chatDisplay; // Text display for chat messages

    private PhotonView photonView;

    void Start()
    {
        photonView = GetComponent<PhotonView>();

        if (photonView == null)
        {
            Debug.LogError("PhotonView is missing on this GameObject!");
            return;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SendMessage();
        }
    }

    public void SendMessage()
    {
        if (messageInput == null || chatDisplay == null)
        {
            Debug.LogError("UI elements are not assigned in the Inspector!");
            return;
        }

        if (photonView == null)
        {
            Debug.LogError("PhotonView is null!");
            return;
        }

        if (!string.IsNullOrWhiteSpace(messageInput.text))
        {
            photonView.RPC("BroadcastMessage", PhotonTargets.All, PhotonNetwork.playerName, messageInput.text.Trim());
            messageInput.text = string.Empty; // Clear the input field
        }
    }

    [PunRPC]
    void BroadcastMessage(string sender, string message)
    {
        if (chatDisplay != null)
        {
            chatDisplay.text += $"<b>{sender}:</b> {message}\n";
        }
    }
}
