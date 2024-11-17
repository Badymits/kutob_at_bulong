using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

public class SelectTarget : MonoBehaviour
{

    public GameObject nightSelectTargetModal;
    public GameObject playerCardPrefab;
    public string playerCardName;
    private int playerIDSelf;
    private int playerIDOther;

    public NightPhaseManager nightPhaseManager;

    private void Start()
    {
        nightPhaseManager = FindAnyObjectByType<NightPhaseManager>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Clicked on prefab");
        Debug.Log("Player Name: " + playerCardPrefab.GetComponentInChildren<TMP_Text>().text);

        
        Debug.Log(nightSelectTargetModal);
    }

    public void OpenSelectTargetModal(string username, int selfID, int targetID)
    {
        Debug.Log("Received self ID: " + selfID);
        Debug.Log("Received target ID: " + targetID);
        // assign ID's to public vars to be accessed by other methods.
        GetPhotonPlayer(username);
        playerIDSelf = selfID;
        playerIDOther = targetID;
        playerCardName = username;

       
        if (nightSelectTargetModal != null)
        {
            nightSelectTargetModal.SetActive(true);
        }
        
    }

    public void GetPhotonPlayer(string name)
    {
        foreach (PhotonPlayer player in PhotonNetwork.playerList)
        {
            
            if (player.NickName == name)
            {
                
                playerCardName = player.NickName;
                break;
            }
        }
    }

    public void CloseSelectTargetModal()
    {
        // reset state
        playerCardName = "";
        playerIDSelf = 1111;
        playerIDOther = 2222; 
        nightSelectTargetModal.SetActive(!gameObject.activeSelf);
    }

    public void TargetConfirmed()
    {
        // self and then target
        nightPhaseManager.ProcessNightAction(playerIDSelf, playerIDOther);

        Debug.Log("Target Selected");
        Debug.Log("Player ID: " + playerIDSelf);
        Debug.Log("Player target ID: " + playerIDSelf);
        Debug.Log("Player Name: " + playerCardName);
        //CloseSelectTargetModal();
    }

    public string CheckRole()
    {
        return "";
    }
}
