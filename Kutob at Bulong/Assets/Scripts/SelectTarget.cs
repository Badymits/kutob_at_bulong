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
    public PrefabClickTest prefabScript;


    private void Start()
    {
        prefabScript = FindAnyObjectByType<PrefabClickTest>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Clicked on prefab");
        Debug.Log("Player Name: " + playerCardPrefab.GetComponentInChildren<TMP_Text>().text);

        
        Debug.Log(nightSelectTargetModal);
    }

    public void OpenSelectTargetModal(string username, int selfID, int targetID)
    {

        if (nightSelectTargetModal != null)
        {
            nightSelectTargetModal.SetActive(true);
        }
        
    }

    public void CloseSelectTargetModal()
    {
        // reset state
        playerCardName = "";
        nightSelectTargetModal.SetActive(!gameObject.activeSelf);
    }

    public void TargetConfirmed()
    {
        prefabScript.TestTargetConfirmed();
    }

    public string CheckRole()
    {
        return "";
    }
}
