using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PrefabClickTest : MonoBehaviour, IPointerClickHandler
{
    // Start is called before the first frame update

    public GameObject modal;
    public Scene currentScene;

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Prefab clicked");
        GameObject playerCard = gameObject;
        Debug.Log("gameObject: " + playerCard.name);
        Debug.Log("Player name: " + playerCard.GetComponentInChildren<TMP_Text>().text);
        Debug.Log(playerCard.GetPhotonView().viewID);
        Debug.Log(gameObject.name);

        Debug.Log(SceneManager.GetActiveScene().name);

        if (modal != null)
        {
            modal.SetActive(true);
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
        
    }
}
