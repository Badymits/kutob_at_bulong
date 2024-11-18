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
    public NightPhaseManager nightPhaseManager;
    private int photonplayerIDTarget;
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
        nightPhaseManager = FindAnyObjectByType<NightPhaseManager>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // refers to the clicked game object. Doesn't necessarily mean it would refer to self
        GameObject playerCard = gameObject;
     

        // retrieve scene for conidtional modal opening/closing
        string sceneName = SceneManager.GetActiveScene().name;

        // gets photonview component of target, NOT SELF
        PhotonView photonViewTarget = playerCard.GetComponent<PhotonView>();
        PhotonPlayer photonPlayerTarget = photonViewTarget.owner;

        Debug.Log("Photon View component" + photonPlayerTarget.ID);


        // retrieving photon player ID's and username through the PhotonView component attached to prefab
        photonplayerIDTarget = photonPlayerTarget.ID;

        if ( sceneName == "NightPhase")
        {
            //selectTargetScript.OpenSelectTargetModal(photonViewName, photonplayerIDSelf, photonViewIDTarget);
            selectTargetScript.nightSelectTargetModal.SetActive(true);
        }
    }

    public void TestTargetConfirmed()
    {
        Debug.Log("Sending confirm request to night phase script");
        Debug.Log("Photon Player self ID: " + photonplayerIDSelf);
        Debug.Log("Photon Player target ID: " + photonplayerIDTarget);
        nightPhaseManager.ProcessNightAction(photonplayerIDSelf, photonplayerIDTarget);
        selectTargetScript.nightSelectTargetModal.SetActive(false);
        ResetIDs();
    }

    public void ResetIDs()
    {
        photonplayerIDTarget = 1111;
        photonplayerIDSelf = 2222;
    }

    public void EliminatedFromGame(string currentPhase)
    {
        Debug.Log("");
        PhotonView photonView = GetComponent<PhotonView>();
        if (currentPhase == "NightPhase")
        {
            photonView.RPC("LoadEliminationScene", photonView.owner);
        }
    }

    [PunRPC]
    public void LoadEliminationScene()
    {
        // Load the elimination scene just for this player
        SceneManager.LoadScene("EliminationScene");
    }

    // Update is called once per frame
    void Update()
    {
       
        
    }
}
