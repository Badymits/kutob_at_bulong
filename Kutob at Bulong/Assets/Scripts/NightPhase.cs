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

    string player_role = "";
    string winner_type = "";

    HashSet<string> aswangRoles = new HashSet<string>
    {
        "aswang - mandurugo",
        "aswang - manananggal",
        "aswang - berbalang"
    };

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
        public string playerID; // seems redundant since we're applying PhotonPlayer id to int key to dictionary but still necessary
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
            if ((bool)photonPlayer.CustomProperties["isAlive"] && !(bool)photonPlayer.CustomProperties["isVotedOut"]) // only add players that are still in the game
            {
                Debug.Log("Populating players dictionary");
                string roleProperty = (string)photonPlayer.CustomProperties["Role"];

                Debug.Log("Player's role is: " + roleProperty);

                // instantiate player class obj
                Player newPlayer = new()
                {
                    playerID = (string)photonPlayer.CustomProperties["playerID"],
                    username = photonPlayer.NickName,
                    role = roleProperty
                };

                Debug.Log("The PhotonPlayer ID: " + photonPlayer.ID);
                players.Add(photonPlayer.ID, newPlayer);
            }
            
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
              
                break;

            case "aswang - mandurugo":
                ResetUIState();
                if (!IsAswang(target.CustomProperties["Role"].ToString()))
                {
                    target.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "nightTarget", true } });
                }
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
                break;

            case "aswang - berbalang":
                ResetUIState();
                if (!IsAswang(target.CustomProperties["Role"].ToString()) && !(bool)target.CustomProperties["isProtected"])
                {
                    target.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "nightTarget", true } });
                }
                break;

            case "babaylan":
                ResetUIState();
                if ((bool)target.CustomProperties["nightTarget"])
                {
                    target.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "nightTarget", false } }); // Cancel the target's night action
                }
                break;

            case "manghuhula":
                ResetUIState();
                RevealRole(actor, target); // Seer gets to know target's role
                break;

            default:
                break;
        }
        
        actor.SetCustomProperties(new ExitGames.Client.Photon.Hashtable() { { "turnDone", true } });
        Debug.Log("On to the next role...");
        photonView.RPC("MoveToNextTurn", PhotonTargets.All);
    }

    private void ResetUIState()
    {
        ui_manager.SetFalseSpawnPoints();
        ui_manager.cardContainer.SetActive(false);
        ui_manager.tMP.text = "Wait for your turn";
    }

    [PunRPC]
    private void MoveToNextTurn()
    {

        if (nightTurnOrder.Count > 0)
        {
            currentTurn = nightTurnOrder.Dequeue();
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
            List<Player> aliveAswangs = GetAliveAswangs(aswangRole);

            // If there are any alive Aswangs for this role, add them to the turn order
            if (aliveAswangs.Count > 0)
            {
                foreach (var aswang in aliveAswangs)
                {
                    Debug.Log($"{aswangRole} Turn Added");
                    nightTurnOrder.Enqueue(aswangRole);  // Add the role to the queue
                }
            }
            else
            {
                Debug.Log($"Role: {aswangRole} Not found for some reason. ");
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

        Debug.Log("The night turn order: " + nightTurnOrder);
        foreach (var roles in nightTurnOrder)
        {
            Debug.Log("role: " + roles);
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

    // manages the UI
    private void NotifyPlayerTurn(NightRole role)
    {
        Debug.Log($"It's {role}'s turn");
        Debug.Log("Showing roles....");
        foreach (var roles in nightTurnOrder)
        {
            Debug.Log(roles);  // Assuming NightRole has a meaningful ToString implementation
        }
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
        Debug.Log("Ending night phase...");
        int victimCount = 0;
        foreach (PhotonPlayer player in PhotonNetwork.playerList)
        {
            Debug.Log("Currently removing eliminated players....");
            if ((bool)player.CustomProperties["nightTarget"] && !(bool)player.CustomProperties["isProtected"])
            {
                Debug.Log("Setting isAlive value to player: ");
                // Mark player as dead by setting "isAlive" to false in custom properties
                ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable
                {
                    { "isAlive", false } // Mark the player as dead
                };

                Debug.Log("Setting custom property");

                player.SetCustomProperties(playerProperties);
                victimCount++;
                
                // call method from script that is attached to the prefab itself
                //prefabScript.EliminatedFromGame("NightPhase");
            }

            Debug.Log("Resetting player values for next night");
            // Reset night status for all players at the end of the night phase
            ExitGames.Client.Photon.Hashtable resetProperties = new ExitGames.Client.Photon.Hashtable
            {
                { "nightTarget", false },
                { "isProtected", false },
                { "turnDone", false }
            };

            Debug.Log("setting reset custom properties");
            player.SetCustomProperties(resetProperties);
        }

        string announcement = victimCount == 1
        ? $"There was {victimCount} victim during the night"
        : $"There were {victimCount} victims during the night";

        // Set the room property
        ExitGames.Client.Photon.Hashtable roomProperty = new ExitGames.Client.Photon.Hashtable
        {
            { "Announcement_Night", announcement }
        };
        PhotonNetwork.room.SetCustomProperties(roomProperty);

        Debug.Log("Calling CheckWinCon method...");
        CheckWinConditions(); // Check for win conditions after the night phase ends


    }

    [PunRPC]
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

    // Modified method to return a list of alive players for the given Aswang role
    private List<Player> GetAliveAswangs(NightRole role)
    {
        List<Player> aliveAswangs = new List<Player>();

        foreach (var player in players.Values)
        {
            Debug.Log("Finding role: " + player.role);
            string modifiedString = StringModifyAswang(player.role);
            Debug.Log("Modified String: " + modifiedString);
            Debug.Log("Player role: " + role);

            // If role matches and the player is alive, add them to the list
            if (modifiedString.Equals(role.ToString(), System.StringComparison.OrdinalIgnoreCase) && player.isAlive)
            {
                aliveAswangs.Add(player);
            }
        }

        return aliveAswangs;
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
            EndGame("Aswang");
            
            return;
        }
        else
        {
            // Automatically transition to discussion phase
            photonView.RPC("TransitionToDiscussionPhase", PhotonTargets.All);
        }
    }

    private void EndGame(string winners)
    {
        Debug.Log($"Game Over! {winners} win!");
        ExitGames.Client.Photon.Hashtable roomProperty = new ExitGames.Client.Photon.Hashtable
        {
            { "Game_Winner", winners }
        };

        PhotonNetwork.room.SetCustomProperties(roomProperty);

        photonView.RPC("DistributeGameResultsScene", PhotonTargets.All);
        return;
    }

    [PunRPC]
    void DistributeGameResultsScene()
    {

        string gameWinner = (string)PhotonNetwork.room.CustomProperties["Game_Winner"];

        bool isAswang = aswangRoles.Contains(player_role);
        bool isVillagers = gameWinner == "Villagers";
        bool isAswangWinner = gameWinner == "Aswang";


        if (isVillagers)
        {
            winner_type = isAswang ? "d" : "a";  // "d" if Aswang, "a" if Villagers
        }
        else if (isAswangWinner)
        {
            winner_type = isAswang ? "c" : "b";  // "c" if Aswang, "b" if Villagers
        }


        GoToGameResults(winner_type);
    }

    void GoToGameResults(string type)
    {
        switch (type)
        {
            case "a":
                PhotonNetwork.LoadLevel("VictoryTaumbayan");
                break;
            case "b":
                PhotonNetwork.LoadLevel("DefeatTaumbayan");
                break;
            case "c":
                PhotonNetwork.LoadLevel("VictoryAswang");
                break;
            case "d":
                PhotonNetwork.LoadLevel("DefeatAswang");
                break;
            default:
                break;
        }
    }
}
