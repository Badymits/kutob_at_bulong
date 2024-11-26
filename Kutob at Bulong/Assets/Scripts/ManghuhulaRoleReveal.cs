using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static NightPhaseManager;

public class ManghuhulaRoleReveal : MonoBehaviour
{

    public GameObject rolePanel;
    public TMP_Text roleReveal;
    // Start is called before the first frame update
    void Start()
    {
        if ((string)PhotonNetwork.player.CustomProperties["Role"] != "manghuhula")
        {
            rolePanel.SetActive(false);
        }
        else
        {
            roleReveal.text = (string)PhotonNetwork.player.CustomProperties["roleRevealed"];
            rolePanel.SetActive(true);
        }
        StartCoroutine(HidePanel());
    }

    IEnumerator HidePanel()
    {
        yield return new WaitForSecondsRealtime(10f);

        PhotonNetwork.player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "roleRevealed", "" } });
        rolePanel.SetActive(false); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
