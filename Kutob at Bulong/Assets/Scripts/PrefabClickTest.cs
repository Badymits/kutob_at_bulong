using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PrefabClickTest : MonoBehaviour, IPointerClickHandler
{
    
    public Scene currentScene;
    [SerializeField] private SelectTarget selectTargetScript;


    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Prefab clicked");
        GameObject playerCard = gameObject;
        Debug.Log("gameObject: " + playerCard.name);
        Debug.Log("Player name: " + playerCard.GetComponentInChildren<TMP_Text>().text);
        Debug.Log(playerCard.GetPhotonView().viewID);
        Debug.Log(gameObject.name);

        string sceneName = SceneManager.GetActiveScene().name;

        if ( sceneName == "NightPhase")
        {
            selectTargetScript.OpenSelectTargetModal(playerCard.GetComponentInChildren<TMP_Text>().text, playerCard.GetPhotonView().viewID);
        }
    }

    public void ConfirmTarget(string name)
    {
        
    }
    

    void Start()
    {
        selectTargetScript = FindAnyObjectByType<SelectTarget>();
    }

    // Update is called once per frame
    void Update()
    {
       
        
    }
}
