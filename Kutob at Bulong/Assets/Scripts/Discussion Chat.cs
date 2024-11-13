using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Use TextMeshPro namespace
using ExitGames.Client.Photon; // Photon PUN 1 specific namespace
using Photon;

public class DiscussionChat : PunBehaviour // Inherit from PunBehaviour for Photon PUN 1
{
    public TMP_InputField inputField; // Use TMP_InputField for TextMeshPro
    public GameObject Message;
    public GameObject Content;

    public void SendMessage()
    {
        photonView.RPC("GetMessage", PhotonTargets.All, inputField.text); // PhotonTargets.All for Photon PUN 1
    }

    [PunRPC]
    public void GetMessage(string ReceiveMessage)
    {
        GameObject M = Instantiate(Message, Vector3.zero, Quaternion.identity, Content.transform);
        M.GetComponent<Message>().MyMessage.text = ReceiveMessage; // Assuming MyMessage is a TMP_Text component
    }
}
