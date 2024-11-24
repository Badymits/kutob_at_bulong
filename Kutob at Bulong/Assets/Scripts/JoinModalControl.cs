using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class JoinModalControl : MonoBehaviour
{
    public TMP_InputField inputField;
    public GameObject message;
    public GameObject modal;
    private PopupMessage popupMessage;

    void Start()
    {
        popupMessage = FindAnyObjectByType<PopupMessage>();
    }
    // Start is called before the first frame update
    public void OpenModal()
    {
        if (modal != null) { modal.SetActive(true); }
    }

    public void CloseModal()
    {
        if (modal != null) { modal.SetActive(false); }
    }

    public void EnterRoomCode()
    {
        string userInput = inputField.text;

        // Remove all whitespaces
        userInput = userInput.Replace(" ", "");
        if (!string.IsNullOrEmpty(inputField.text))
        {
            PhotonNetwork.JoinRoom(userInput.ToUpper());
        }
        else
        {
            ShowErrorMessage();
        }
    }

    private void ShowErrorMessage()
    {
        message.SetActive(true);
        CloseMessage();
    }

    // added delay to not close immediately
    public void CloseMessage()
    {
        if (message != null && message.activeInHierarchy)
        {
            StartCoroutine(CloseMessageObject());
        }
        return;
    }

    IEnumerator CloseMessageObject()
    {
        yield return new WaitForSeconds(5f);

        message.SetActive(false);
    }

    // This is called when the room join fails in Photon 1.x
    public void OnPhotonJoinRoomFailed(object[] codeAndMsg)
    {
        // codeAndMsg contains the error code and the error message
        short errorCode = (short)codeAndMsg[0];
        string errorMsg = (string)codeAndMsg[1];

        
    }
}
