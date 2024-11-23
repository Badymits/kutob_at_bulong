using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AnnouncementDay : MonoBehaviour
{
    public TMP_Text announcementText;
    // Start is called before the first frame update
    void Start()
    {
        announcementText.text = (string)PhotonNetwork.room.CustomProperties["Announcement_Day"];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
