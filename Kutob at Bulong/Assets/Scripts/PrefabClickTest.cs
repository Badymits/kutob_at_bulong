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
    [SerializeField] private SelectVote selectVoteScript;
    public NightPhaseManager nightPhaseManager;
    public VotingSystem votingSystem;

    private string photonplayerIDTarget;
    private string photonplayerIDSelf;

    public float confirmTimer = .3f;

    void Start()
    {
        if (PhotonNetwork.connectedAndReady)
        {
            // If the player is already connected to the room, access the PhotonPlayer ID
            
            photonplayerIDSelf = (string)PhotonNetwork.player.CustomProperties["playerID"];
        }
        photonView = GetComponent<PhotonView>();
        votingSystem = FindAnyObjectByType<VotingSystem>();
        selectTargetScript = FindAnyObjectByType<SelectTarget>();
        selectVoteScript = FindAnyObjectByType<SelectVote>();
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

        string playerID = (string)photonPlayerTarget.CustomProperties["playerID"];


        // retrieving photon player ID's and username through the PhotonView component attached to prefab
        photonplayerIDTarget = playerID;
        Debug.Log(photonplayerIDTarget);

        if ( sceneName == "NightPhase")
        {
            selectTargetScript.OpenSelectTargetModal(photonViewTarget.owner.NickName, photonplayerIDSelf, photonplayerIDTarget);
            //selectTargetScript.nightSelectTargetModal.SetActive(true);
        }
        else
        {
            selectVoteScript.OpenSelectVoteModal(photonViewTarget.owner.NickName, photonplayerIDTarget);
        }
    }

    public void TestTargetConfirmed(string targetID)
    {
        Debug.Log("Sending confirm request to night phase script");
        Debug.Log("Photon Player self ID: " + photonplayerIDSelf);
        Debug.Log("Photon Player target ID: " + photonplayerIDTarget);

        /*StartCoroutine(addDelay(targetID));*/
        selectTargetScript.CloseSelectTargetModal();
        nightPhaseManager.ProcessNightAction((string)PhotonNetwork.player.CustomProperties["playerID"], targetID);
    }

    public void VoteConfirmed(string targetID)
    {
        Debug.Log("Calling cast vote method...");
        votingSystem.CastVote(targetID);
    }

    public void ResetIDs()
    {
        photonplayerIDTarget = "";
        photonplayerIDSelf = "";
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
