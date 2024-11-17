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
    public int photonViewIDSelf;
    PhotonView photonView;
    [SerializeField] private SelectTarget selectTargetScript;
    private int photonplayerTarget;
    private int photonplayerIDSelf;

    void Start()
    {
        if (PhotonNetwork.connectedAndReady)
        {
            // If the player is already connected to the room, access the PhotonPlayer ID
            Debug.Log(PhotonNetwork.player.ID);
            photonplayerIDSelf = PhotonNetwork.player.ID;
        }
        photonView = GetComponent<PhotonView>();
        selectTargetScript = FindAnyObjectByType<SelectTarget>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // refers to the clicked game object. Doesn't necessarily mean it would refer to self
        GameObject playerCard = gameObject;
        
        Debug.Log("Prefab clicked: " + playerCard.GetComponent<PhotonView>());

        // retrieve scene for conidtional modal opening/closing
        string sceneName = SceneManager.GetActiveScene().name;

        // gets photonview component of target, NOT SELF
        PhotonView photonViewTarget = playerCard.GetComponent<PhotonView>();
        PhotonPlayer photonPlayerTarget = photonView.owner;
        Debug.Log("Photon View component" + photonPlayerTarget.ID);
        

        // retrieving photon player ID's and username through the PhotonView component attached to prefab
        int photonViewIDTarget = photonViewTarget.ownerId;
        Debug.Log("Photon player id target: " + photonViewIDTarget);
        Debug.Log("Photon player id self: " + photonplayerIDSelf);
        
        string photonViewName = playerCard.GetComponentInChildren<TMP_Text>().text;
       

        if ( sceneName == "NightPhase")
        {
            selectTargetScript.OpenSelectTargetModal(photonViewName, photonplayerIDSelf, photonViewIDTarget);
        }
    }
    
    // Update is called once per frame
    void Update()
    {
       
        
    }
}
