using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class JoinModalControl : MonoBehaviour
{
    public TMP_InputField inputField;
    public GameObject modal;
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
        if (!string.IsNullOrEmpty(inputField.text))
        {
            PhotonNetwork.JoinRoom(inputField.text.ToUpper());
        }
    }
}
