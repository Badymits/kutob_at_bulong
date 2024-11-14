using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConnectOnline : MonoBehaviour
{

    void Start()
    {
        Debug.Log("Running");
        Invoke("LoadScene", 3);
    }

    private void LoadScene()
    {
        Debug.Log("Invoked");
        PhotonNetwork.ConnectUsingSettings("1.0");
        SceneManager.LoadScene("HomeScene");
    }
}
