using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeScript : MonoBehaviour
{
    // Start is called before the first frame update
    PhotonPlayer player;
    void Start()
    {
        player.NickName = PlayerPrefs.GetString("Username");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
