using UnityEngine;
using TMPro;

public class CopyToClipboard : MonoBehaviour
{
    public TMP_Text roomCodeText;

    public void CopyRoomCode()
    {
        GUIUtility.systemCopyBuffer = roomCodeText.text;
        Debug.Log("Room code copied to clipboard: " + roomCodeText.text);
    }
}
