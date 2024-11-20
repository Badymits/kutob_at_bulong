using Photon;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;
using System;
using System.Data;

public class NightPhaseManager : Photon.MonoBehaviour
{
    public GameObject textBox;
    public TMP_Text moderatorLine;
    public TMP_Text temp_role;
    public TMP_Text temp_name;
    public TMP_Text target_text;

    public enum NightRole
    {
        Mangangaso, // Hunter
        AswangMandurugo, // Vampire
        AswangManananggal, // Flying monster
        AswangBerbalang, // Shape-shifter
        Babaylan, // Healer
        Manghuhula // Seer
    }

    private Dictionary<int, Player> players = new Dictionary<int, Player>();
    private Queue<NightRole> nightTurnOrder;
    private NightRole currentTurn;
    private int nightCount = 0;

    [SerializeField] private UIManager ui_manager;
    private PrefabClickTest prefabScript;
    private PhotonView photonView;


    public class Player
    {
        public string username;
        public string role;
        public bool isAlive = true;
        public bool isProtected;
        public bool skipTurn;
        public int nightSkip;
        public bool canExecute;
        public bool nightTarget;
        public bool turnDone;
    }

    private void Start()
    {
        // manually instantiate at start
        ui_manager = FindObjectOfType<UIManager>();
        prefabScript = FindAnyObjectByType<PrefabClickTest>();
        photonView = GetComponent<PhotonView>();

        if (photonView == null)
        {
            Debug.Log("PhotonView not found");
        }
        else
        {
            Debug.Log("PhotonView found");
        }

        foreach (PhotonPlayer photonPlayer in PhotonNetwork.playerList)
        {
            Debug.Log("Populating players dictionary");
            string roleProperty = (string)photonPlayer.CustomProperties["Role"];

            Debug.Log("Player's role is: " + roleProperty);

            // instantiate player class obj
            Player newPlayer = new Player
            {
                username = photonPlayer.NickName,
                role = roleProperty
            };

            Debug.Log("The PhotonPlayer ID: " + photonPlayer.ID);
            players.Add(photonPlayer.ID, newPlayer);
        }
        PhotonPlayer photonPlayerTest = PhotonNetwork.player;
        string photonPlayer1 = PhotonNetwork.player.NickName;
        string photonPlayerRole = (string)photonPlayerTest.CustomProperties["Role"];

        temp_name.text = photonPlayer1;
        temp_role.text = photonPlayerRole;

        Debug.Log("Calling Night phase");

        if (ui_manager == null)
        {
            Debug.Log("Empty");
        }

        StartNightPhase();
    }

    public void ProcessNightAction(string selfID, string targetID) // receieves photon player id for self and for target
    {
        PhotonPlayer actor = GetActor(selfID);
        PhotonPlayer target = GetActor(targetID);

        Debug.Log("The actor: " + actor + " Actor role: " + actor.CustomProperties["Role"].ToString());
        Debug.Log("The target: " + target + " target role: " + target.CustomProperties["Role"].ToString());

        target_text.text = targetID.ToString();


        switch (actor.CustomProperties["Role"].ToString().ToLower())
        {
            case "mangangaso":
                ResetUIState();
                if (!(bool)actor.CustomProperties["canExecute"])
                {
                    target.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "isProtected", true } });
                }
                else if ((bool)actor.CustomProperties["skipTurn"] || (int)actor.CustomProperties["nightSkip"] == nightCount)
                {
                    // Skip turn if disabled by Manananggal
                    return;
                }
                else
                {
                    target.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "nightTarget", true } });
                }
                target.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "turnDone", true } });
                break;

            case "aswang - mandurugo":
                ResetUIState();
                if (!IsAswang(target.CustomProperties["Role"].ToString()))
                {
                    target.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "nightTarget", true } });
                }
                target.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "turnDone", true } });
                break;

            case "aswang - manananggal":
                ResetUIState();
                if (!IsAswang(target.CustomProperties["Role"].ToString()))
                {
                    if (!(bool)actor.CustomProperties["isProtected"])
                    {
                        target.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "nightTarget", true } });
                    }
                    else
                    {
                        // Find and disable Mangangaso Player
                        Player mangangaso = FindPlayerByRole("mangangaso");
                        if (mangangaso != null)
                        {
                            mangangaso.skipTurn = true;
                            mangangaso.nightSkip = nightCount + 2; // Skip next two nights
                        }
                    }
                }
                target.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "turnDone", true } });
                break;

            case "aswang - berbalang":
                ResetUIState();
                if (!IsAswang(target.CustomProperties["Role"].ToString()) && !(bool)target.CustomProperties["isProtected"])
                {
                    target.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "nightTarget", true } });
                }
                target.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "turnDone", true } });
                break;

            case "babaylan":
                ResetUIState();
                if ((bool)actor.CustomProperties["nightTarget"])
                {
                    target.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "nightTarget", false } }); // Cancel the target's night action
                }
                target.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "turnDone", true } });
                break;

            case "manghuhula":
                ResetUIState();
                RevealRole(actor, target); // Seer gets to know target's role
                target.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "turnDone", true } });
                break;

            default:
                break;
        }
        Debug.Log("On to the next role...");
        MoveToNextTurn();
    }

    private void ResetUIState()
    {
        ui_manager.SetFalseSpawnPoints();
        ui_manager.cardContainer.SetActive(false);
        ui_manager.tMP.text = "Wait for your turn";
    }

    private void MoveToNextTurn()
    {
        if (nightTurnOrder.Count > 0)
        {
            currentTurn = nightTurnOrder.Dequeue(); // Get the next player's turn
            Debug.Log("Current night turn order value: " + nightTurnOrder.Count);

            NotifyPlayerTurn(currentTurn);
        }
        else
        {
            EndNightPhase(); // No more turns left, end the night phase
        }
    }

    private void StartNightPhase()
    {
        nightCount++;
        nightTurnOrder = new Queue<NightRole>();

        Debug.Log("Called Night phase");
        Debug.Log("Current night turn order value: " + nightTurnOrder.Count);

        // Set turn order for Mangangaso and Aswang roles
        Player mangangaso = FindPlayerByRole("mangangaso");

        if (mangangaso != null && mangangaso.isAlive && !mangangaso.skipTurn)
        {

            Debug.Log("Mangangaso First Turn Added");
            nightTurnOrder.Enqueue(NightRole.Mangangaso);
        }
        else
        {
            Debug.Log("Mangangaso not found or not alive.");
        }

        foreach (var aswangRole in new[] { NightRole.AswangMandurugo, NightRole.AswangManananggal, NightRole.AswangBerbalang })
        {
            if (isAswangAlive(aswangRole))
            {
                Debug.Log($"{aswangRole} Turn Added");
                nightTurnOrder.Enqueue(aswangRole);
            }
            else
            {
                Debug.Log("Role: " + aswangRole + " Not found for some reason. ");
            }
        }

        // Add support roles if alive
        if (IsRoleAlive(NightRole.Babaylan))
        {

            nightTurnOrder.Enqueue(NightRole.Babaylan);
            Debug.Log("Babaylan Turn Added");
        }

        if (IsRoleAlive(NightRole.Manghuhula))
        {
            nightTurnOrder.Enqueue(NightRole.Manghuhula);
            Debug.Log("Manghuhula Turn Added");
        }

        

        // Only notify turn if there are players in the queue
        if (nightTurnOrder.Count > 0)
        {
            currentTurn = nightTurnOrder.Dequeue();
            NotifyPlayerTurn(currentTurn);
        }
        else
        {
            Debug.Log("No players available for night phase.");
        }
    }

    

    // this method ensures the current turn to be the same with other clients within the game
    [PunRPC]
    public void SyncTurnOrder(string currentTurnRole)
    {
        string[] aswangRoles = { "aswang - mandurugo", "aswang - manananggal", "aswang - berbalang" };
        // You can now use the `currentTurnRole` to display the current player's turn in the UI
        Debug.Log("Syncing turn order: " + currentTurnRole);
        if (aswangRoles.Contains(currentTurnRole))
        {
            

            // You might also want to update other game logic that depends on the current player's turn
            currentTurn = (NightRole)Enum.Parse(typeof(NightRole), currentTurnRole); // If you need to store the current turn role

        }
        else
        {
            // You might also want to update other game logic that depends on the current player's turn
            currentTurn = (NightRole)Enum.Parse(typeof(NightRole), currentTurnRole); // If you need to store the current turn role
        }
    }

    // manages the UI
    private void NotifyPlayerTurn(NightRole role)
    {
        Debug.Log($"It's {role}'s turn");
        ui_manager.photonView.RPC("UpdatePlayerTurnUI", PhotonTargets.All, role.ToString().ToLower());
    }

    public PhotonPlayer GetActor(string actorID)
    {
        Debug.Log("Called Get actor method");
        foreach (PhotonPlayer player in PhotonNetwork.playerList)
        {

            string photonPlayerID = (string)player.CustomProperties["playerID"];
            Debug.Log("The player ID: " + player.ID);
            Debug.Log("The actor ID: " + actorID);
            PhotonPlayer classPlayer = player;

            Debug.Log("The class player: " + classPlayer);
            if (actorID == photonPlayerID)
            {
                Debug.Log("The returned value: " + classPlayer);
                return classPlayer;
            }
            else
            {
                Debug.Log("No player found");
            }
        }
        return null;
    }

    private void EndNightPhase()
    {
        foreach (var player in players.Values)
        {
            if (player.nightTarget && !player.isProtected)
            {
                player.isAlive = false; // Mark player as dead if targeted and not protected
                // call method from script that is attached to the prefab itself
                prefabScript.EliminatedFromGame("NightPhase");
            }

            // Reset night status for all players at the end of the night phase
            player.nightTarget = false;
            player.isProtected = false;
            player.turnDone = false;
        }

        CheckWinConditions(); // Check for win conditions after the night phase ends


    }

    private void TransitionToDiscussionPhase()
    {
        Debug.Log("Transitioning to Discussion Phase...");
        // Implement your logic here to move to discussion phase
        PhotonNetwork.LoadLevel("DayTransition"); // continue game 
    }

    private bool IsAswang(string role)
    {
        Debug.Log("Aswang detected");
        return role.StartsWith("aswang", System.StringComparison.OrdinalIgnoreCase);
    }

    private Player FindPlayerByRole(string role)
    {
        foreach (var player in players.Values)
        {
            if (player.role.Equals(role, System.StringComparison.OrdinalIgnoreCase) && player.isAlive)
            {
                return player;
            }
        }

        return null;
    }

    // I know, its a few extra steps...
    private string StringModifyAswang(string role)
    {
        switch (role)
        {

            case "aswang - mandurugo":
                return "AswangMandurugo";

            case "aswang - manananggal":
                return "AswangManananggal";

            case "aswang - berbalang":
                return "AswangBerbalang";

            default:
                return "";

        }
    }

    private bool isAswangAlive(NightRole role)
    {
        foreach (var player in players.Values)
        {
            Debug.Log("Finding role: " + player.role);
            string modifiedString = StringModifyAswang(player.role);
            Debug.Log("Modified String: " + modifiedString);
            Debug.Log("Player role: " + role);
            if (modifiedString.Equals(role.ToString(), System.StringComparison.OrdinalIgnoreCase) && player.isAlive)
            {
                return true;
            }
        }
        return false;

    }

    private bool IsRoleAlive(NightRole role)
    {
        foreach (var player in players.Values)
        {
            Debug.Log("Finding role: " + player.role);
            if (player.role.Equals(role.ToString(), System.StringComparison.OrdinalIgnoreCase) && player.isAlive)
            {
                return true;
            }
        }

        return false;
    }

    private void RevealRole(PhotonPlayer seer, PhotonPlayer target)
    {
        Debug.Log($"Revealed {(string)target.CustomProperties["Role"]} to {(string)seer.CustomProperties["Role"]}");
        // Implement UI to show target's role to the seer here.
    }

    private void CheckWinConditions()
    {
        int aswangCount = 0;
        int villagerCount = 0;

        foreach (var player in players.Values)
        {
            if (player.isAlive)
            {
                if (IsAswang(player.role))
                    aswangCount++;
                else
                    villagerCount++;
            }
        }

        // modify this later to send user to appropriate end game screen depending on their role
        if (aswangCount == 0)
        {
            EndGame("Villagers");
            return;
        }
        else if (aswangCount >= villagerCount && villagerCount == 0)
        {
            EndGame("Aswangs");
            
            return;
        }
        else
        {
            // Automatically transition to discussion phase
            TransitionToDiscussionPhase();
        }
    }

    private void EndGame(string winners)
    {
        Debug.Log($"Game Over! {winners} win!");
        if (winners == "Villagers")
        {
            PhotonNetwork.LoadLevel("VictoryTaumbayan");
        }
        else
        {
            PhotonNetwork.LoadLevel("VictoryAswang");
        }
        return;
        // Implement game end logic here. 
    }
}
