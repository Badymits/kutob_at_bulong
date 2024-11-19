using Photon;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;
using System;

public class NightPhaseManager : Photon.MonoBehaviour
{
    public GameObject textBox;
    public TMP_Text moderatorLine;
    public TMP_Text temp_role;
    public TMP_Text temp_name;

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
    private new PhotonView photonView;


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

    public void ProcessNightAction(int selfID, int targetID) // receieves photon player id for self and for target
    {
        Player actor = GetActor(selfID);
        Player target = GetActor(targetID);

        Debug.Log("The actor: " + actor + " Actor role: " + actor.role);
        Debug.Log("The target: " + target + " target role: " + target.role);


        switch (actor.role.ToLower())
        {
            case "mangangaso":
                ResetUIState();
                if (!actor.canExecute)
                {
                    target.isProtected = true;
                }
                else if (actor.skipTurn || actor.nightSkip == nightCount)
                {
                    // Skip turn if disabled by Manananggal
                    return;
                }
                else
                {
                    target.nightTarget = true;
                }
                actor.turnDone = true;
                break;

            case "aswang - mandurugo":
                ResetUIState();
                if (!IsAswang(target.role))
                {
                    target.nightTarget = true;
                }
                actor.turnDone = true;
                break;

            case "aswang - manananggal":
                ResetUIState();
                if (!IsAswang(target.role))
                {
                    if (!target.isProtected)
                    {
                        target.nightTarget = true;
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
                actor.turnDone = true;
                break;

            case "aswang - berbalang":
                ResetUIState();
                if (!IsAswang(target.role) && !target.isProtected)
                {
                    target.nightTarget = true;
                }
                actor.turnDone = true;
                break;

            case "babaylan":
                ResetUIState();
                if (target.nightTarget)
                {
                    target.nightTarget = false; // Cancel the target's night action
                }
                break;

            case "manghuhula":
                ResetUIState();
                RevealRole(actor, target); // Seer gets to know target's role
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

            // Sync turn for all players
            ui_manager.photonView.RPC("SyncTurnOrder", PhotonTargets.All, currentTurn.ToString());
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


        ui_manager.photonView.RPC("SyncNightTurnOrder", PhotonTargets.All, GetTurnOrderArray(), nightTurnOrder.Count);

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

    private string[] GetTurnOrderArray()
    {
        List<string> turnOrderList = new List<string>();
        foreach (NightRole role in nightTurnOrder)
        {
            turnOrderList.Add(role.ToString());
        }
        return turnOrderList.ToArray();
    }

    [PunRPC]
    public void SyncNightTurnOrder(string[] turnOrderArray, int queueCount)
    {
        nightTurnOrder.Clear(); // Reset the queue on all clients

        // Rebuild the nightTurnOrder based on the array received
        foreach (var role in turnOrderArray)
        {
            nightTurnOrder.Enqueue((NightRole)Enum.Parse(typeof(NightRole), role));
        }

        int nightTurnOrderCount = queueCount; // Sync the queue count

        Debug.Log("Turn order synchronized, current turn is: " + currentTurn);
    }

    [PunRPC]
    public void SyncTurnOrder(string currentTurnRole, int queueCount, string[] roleQueue)
    {
        // Sync the current turn and the queue state across all players
        currentTurn = (NightRole)Enum.Parse(typeof(NightRole), currentTurnRole);
        nightTurnOrder.Clear(); // Reset the local queue
        foreach (var role in roleQueue)
        {
            nightTurnOrder.Enqueue((NightRole)Enum.Parse(typeof(NightRole), role));
        }

        Debug.Log("Turn synchronized to: " + currentTurn);
        int nightTurnOrderCount = queueCount;

        // Notify the current player (who can now perform actions) and disable others
        NotifyPlayerTurn(currentTurn);
    }

    private void NotifyPlayerTurn(NightRole role)
    {
        Debug.Log($"It's {role}'s turn");
        //ui_manager.ShowRoleUI(role.ToString().ToLower());
        ui_manager.photonView.RPC("UpdatePlayerTurnUI", PhotonTargets.All, role.ToString().ToLower());
    }

    public Player GetActor(int actorID)
    {
        Debug.Log("Called Get actor method");
        foreach (KeyValuePair<int, Player> player in players)
        {
            int photonPlayerID = player.Key;
            Debug.Log("The player key: " + player.Key);
            Debug.Log("The actor ID: " + actorID);
            Player classPlayer = player.Value;

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

    private void RevealRole(Player seer, Player target)
    {
        Debug.Log($"Revealed {target.role} to {seer.username}");
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
