using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{

    public TMP_Text announcementNight;
    // Start is called before the first frame update
    void Start()
    {
        announcementNight.text = (string)PhotonNetwork.room.CustomProperties["Announcement_Night"];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
