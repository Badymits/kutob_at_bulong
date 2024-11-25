using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using static NightPhaseManager;

public class PrefabClickTest : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    
    public Scene currentScene;
    public int photonViewIDSelf;
    PhotonView photonView;
    [SerializeField] private SelectTarget selectTargetScript;
    [SerializeField] private SelectVote selectVoteScript;
    public NightPhaseManager nightPhaseManager;
    public VotingSystem votingSystem;
    private PopupMessage popupMessage;
    private SpriteRenderer spriteRenderer;

    private string photonplayerIDTarget;
    private string photonplayerIDSelf;

    public ExitGames.Client.Photon.Hashtable playerProperty = new ExitGames.Client.Photon.Hashtable();

    public float confirmTimer = .3f;
    public float opacityReductionAmount = 0.1f;

    HashSet<string> aswangRoles = new HashSet<string>
    {
        "aswang - mandurugo",
        "aswang - manananggal",
        "aswang - berbalang"
    };

    void Start()
    {
        if (PhotonNetwork.connectedAndReady)
        {
            // If the player is already connected to the room, access the PhotonPlayer ID
            
            photonplayerIDSelf = (string)PhotonNetwork.player.CustomProperties["playerID"];
        }
        InitializeGameObjects();
    }


    public void InitializeGameObjects()
    {
        photonView = GetComponent<PhotonView>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        votingSystem = FindAnyObjectByType<VotingSystem>();
        selectTargetScript = FindAnyObjectByType<SelectTarget>();
        selectVoteScript = FindAnyObjectByType<SelectVote>();
        nightPhaseManager = FindAnyObjectByType<NightPhaseManager>();
        popupMessage = FindAnyObjectByType<PopupMessage>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (PhotonNetwork.player.CustomProperties == null)
        {
            return;
        }
        // refers to the clicked game object. Doesn't necessarily mean it would refer to self
        GameObject playerCard = gameObject;

        // retrieve scene for conidtional modal opening/closing
        string sceneName = SceneManager.GetActiveScene().name;

        // gets photonview component of target, NOT SELF
        PhotonView photonViewTarget = playerCard.GetComponent<PhotonView>();
        PhotonPlayer photonPlayerTarget = photonViewTarget.owner;


        string playerID = (string)photonPlayerTarget.CustomProperties["playerID"];

        // retrieving photon player ID's and username through the PhotonView component attached to prefab
        photonplayerIDTarget = playerID;


        if ( sceneName == "NightPhase" && CheckValidTarget(photonPlayerTarget))
        {
            selectTargetScript.OpenSelectTargetModal(photonViewTarget.owner.NickName, photonplayerIDSelf, photonplayerIDTarget);
            return;
        }
        else
        {
            if ((bool)PhotonNetwork.player.CustomProperties["hasVoted"] && PhotonNetwork.player.CustomProperties != null)
            {
                // will add popup message later
                Debug.Log("Cannot vote twice");
                return;
            }
            selectVoteScript.OpenSelectVoteModal(photonViewTarget.owner.NickName, photonplayerIDTarget);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Hover");
        // Change cursor to pointer (hand icon)
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);  // You can also use a custom cursor texture if needed
        Cursor.SetCursor(Resources.Load<Texture2D>("PointerCursor"), Vector2.zero, CursorMode.Auto);

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Hover not");

        // Reset cursor to default
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    // to avoid player voting twice
    public void UpdatePlayerProperty(string playerID)
    {
        if (PhotonNetwork.player.CustomProperties.TryGetValue("playerID", out var playerIDValue) && playerIDValue.ToString() == playerID)
        {
            PhotonNetwork.player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "hasVoted", true } });
        }
        return;
    }

    // disallow aswang players to select themselves or other fellow aswangs as target
    public bool CheckValidTarget(PhotonPlayer player)
    {
        string targetRole = (string)player.CustomProperties["Role"];
        string selfRole = (string)PhotonNetwork.player.CustomProperties["Role"];

        if (aswangRoles.Contains(targetRole) && aswangRoles.Contains(selfRole))
        {
            popupMessage.OpenMessage();
            return false;
        }

        return true;
    }

    public void TestTargetConfirmed(string targetID)
    {
        Debug.Log("Sending confirm request to night phase script");
        Debug.Log("Photon Player self ID: " + photonplayerIDSelf);
        Debug.Log("Photon Player target ID: " + photonplayerIDTarget);

        selectTargetScript.CloseSelectTargetModal();
        nightPhaseManager.ProcessNightAction((string)PhotonNetwork.player.CustomProperties["playerID"], targetID);
        return;
    }

    public void VoteConfirmed(string targetID)
    {
        Debug.Log("Calling cast vote method...");
        UpdatePlayerProperty((string)PhotonNetwork.player.CustomProperties["playerID"]);
        votingSystem.CastVote(targetID);
        return;
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
