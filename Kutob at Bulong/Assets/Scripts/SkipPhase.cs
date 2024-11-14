using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkipPhase : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Skip()
    {
        PhotonNetwork.LoadLevel("Discussion Phase");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
