using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Use TextMeshPro namespace
using ExitGames.Client.Photon; // Photon PUN 1 specific namespace
using Photon;

public class DiscussionChat : PunBehaviour // Inherit from PunBehaviour for Photon PUN 1
{
    [SerializeField] private TMP_InputField inputField; // Use TMP_InputField for TextMeshPro
    [SerializeField] private GameObject messagePrefab; // Renamed for clarity
    [SerializeField] private GameObject content;

    public void SendMessage()
    {
        if (string.IsNullOrEmpty(inputField.text))
        {
            return; // Do not send an empty message
        }

        string messageToSend = PhotonNetwork.playerName + " : " + inputField.text; // Use playerName instead of player.Nickname
        photonView.RPC("GetMessage", PhotonTargets.All, messageToSend); // PhotonTargets.All for Photon PUN 1

        inputField.text = ""; // Clear the input field after sending
    }

    [PunRPC]
    public void GetMessage(string receiveMessage)
    {
        GameObject messageInstance = Instantiate(messagePrefab, Vector3.zero, Quaternion.identity, content.transform);

        // Ensure that you have a reference to the TMP_Text component correctly
        TMP_Text messageText = messageInstance.GetComponentInChildren<TMP_Text>(); // Adjust based on your prefab structure

        if (messageText != null)
        {
            messageText.text = receiveMessage; // Set the text of the message
        }
        else
        {
            Debug.LogError("TMP_Text component not found in Message prefab.");
        }
    }
}