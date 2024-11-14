using Photon;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using PhotonBehaviour = Photon.MonoBehaviour;

public class ChatManager : PhotonBehaviour
{
    public TMP_InputField chatInputField;
    public TextMeshProUGUI chatBox;
    public ScrollRect chatScrollRect;

    private void Start()
    {
        chatInputField.onEndEdit.AddListener(SendMessage);
    }

    private void SendMessage(string message)
    {
        if (Input.GetKeyDown(KeyCode.Return) && !string.IsNullOrEmpty(message.Trim()))
        {
            photonView.RPC("ReceiveMessage", PhotonTargets.All, PhotonNetwork.player.NickName, message);
            chatInputField.text = "";
            chatInputField.ActivateInputField();
        }
    }

    [PunRPC]
    private void ReceiveMessage(string playerName, string message)
    {
        chatBox.text += $"\n<color=#00aaff>{playerName}:</color> {message}";
        Canvas.ForceUpdateCanvases();
        chatScrollRect.verticalNormalizedPosition = 0;
    }

    private void OnDestroy()
    {
        chatInputField.onEndEdit.RemoveListener(SendMessage);
    }
}