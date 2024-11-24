using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnConnect : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (!PhotonNetwork.connected)
        {
            PhotonNetwork.ConnectUsingSettings("1.0");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
