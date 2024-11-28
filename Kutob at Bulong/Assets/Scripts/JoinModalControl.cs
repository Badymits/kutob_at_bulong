using System.Collections;
using TMPro;
using UnityEngine;

public class JoinModalControl : MonoBehaviour
{
    public TMP_InputField inputField;  // For user input
    public GameObject modal;          // Join room modal
    public TMP_Text errorMessageText; // Reference to the text for displaying errors
    private PopupMessage popupMessage;

    void Start()
    {
        popupMessage = FindAnyObjectByType<PopupMessage>();

        // Ensure the error message text is hidden at the start
        if (errorMessageText != null)
        {
            errorMessageText.text = ""; // Clear the text
            errorMessageText.gameObject.SetActive(false); // Hide initially
        }
    }

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
        if (!string.IsNullOrEmpty(userInput))
        {
            PhotonNetwork.JoinRoom(userInput.ToUpper());
        }
        else
        {
            ShowErrorMessage("Room code cannot be empty!");
        }
    }

    private void ShowErrorMessage(string message)
    {
        if (errorMessageText != null)
        {
            errorMessageText.text = message;           // Set the error message text
            errorMessageText.gameObject.SetActive(true); // Show the text
            StartCoroutine(HideErrorMessageAfterDelay(5f)); // Hide after 5 seconds
        }
    }

    IEnumerator HideErrorMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (errorMessageText != null)
        {
            errorMessageText.text = ""; // Clear the text
            errorMessageText.gameObject.SetActive(false); // Hide the text
        }
    }

    public void OnPhotonJoinRoomFailed(object[] codeAndMsg)
    {
        // Extract the error code and message from the parameters
        short errorCode = (short)codeAndMsg[0];
        string errorMsg = (string)codeAndMsg[1];

        Debug.LogError($"Failed to join room: Error {errorCode} - {errorMsg}");

        // Show the error message on the screen
        ShowErrorMessage("Invalid Room Code! Please try again.");
    }
}
