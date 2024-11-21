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
    //public GameObject playerCardPrefab;
    public string playerCardName;

    public TMP_Text playerName;
    public TMP_Text playerID;
    private string testText;
    private string username1;

    public NightPhaseManager nightPhaseManager;
    public PrefabClickTest prefabScript;

    private void Start()
    {
        prefabScript = FindAnyObjectByType<PrefabClickTest>();
    }

  /*  public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Clicked on prefab");
        Debug.Log("Player Name: " + playerCardPrefab.GetComponentInChildren<TMP_Text>().text);

        
        Debug.Log(nightSelectTargetModal);
    }*/

    public void OpenSelectTargetModal(string username, string selfID, string targetID)
    {

        if (nightSelectTargetModal != null)
        {
            playerName.text = username;
            playerID.text = targetID;
            testText = targetID;
            username1 = username;
            nightSelectTargetModal.SetActive(true);
        }
        Debug.Log("testText: " + testText);
        Debug.Log("Set text: " + targetID);
        
    }

    public void CloseSelectTargetModal()
    {
        // reset state
        playerCardName = "";
        nightSelectTargetModal.SetActive(!gameObject.activeSelf);

    }

    public void TargetConfirmed()
    {
        /*Debug.Log(testText);
        Debug.Log(playerID.text);
        prefabScript.TestTargetConfirmed(playerID.text);*/
        Debug.Log("clicked on confirm btn");
        Debug.Log("Test Text: " + this.testText);
        Debug.Log("username: " + username1);

        //Debug.Log("Player ID Text: " + this.playerID.text);
        Debug.Log("trying children of modal: " + nightSelectTargetModal);

        prefabScript.TestTargetConfirmed(playerID.text);
    }

    public string CheckRole()
    {
        return "";
    }
}
