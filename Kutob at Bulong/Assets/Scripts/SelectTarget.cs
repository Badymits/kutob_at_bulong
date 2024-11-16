using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

public class SelectTarget : MonoBehaviour
{

    PhotonView photonView;
    ModalControl modalControl;
    public GameObject nightSelectTargetModal;
    public GameObject playerCardPrefab;
    public string playerCardName;
    public int playerCardID;


    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Clicked on prefab");
        Debug.Log("Player Name: " + playerCardPrefab.GetComponentInChildren<TMP_Text>().text);

        
        Debug.Log(nightSelectTargetModal);
    }

    public void OpenSelectTargetModal(string username, int id)
    {
        GetPhotonPlayer(username);
        playerCardID = id;
        Debug.Log("playercard name: " + username);
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
        playerCardName = "";
        playerCardID = 1111;
        nightSelectTargetModal.SetActive(!gameObject.activeSelf);
    }


    // Detect mouse click on this object
    void OnMouseDown()
    {
        Debug.Log("Image clicked!");
        modalControl = GetComponent<ModalControl>();
        modalControl.OpenModal();   

        // Additional logic here (e.g., change image, trigger event, etc.)
    }

    public void TargetConfirmed()
    {

        Debug.Log("Target Selected");
        Debug.Log("Player ID: " + playerCardID);
        Debug.Log("Player Name: " + playerCardName);
        //SceneManager.LoadScene("");
    }

    public string CheckRole()
    {
        return "";
    }
}
