using Photon;
using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class NightPhaseManager : Photon.MonoBehaviour
{
    public GameObject textBox;
    public TMP_Text moderatorLine;

    public enum NightRole
    {
        Mangangaso,         // Hunter
        AswangMandurugo,   // Vampire
        AswangManananggal,  // Flying monster
        AswangBerbalang,    // Shape-shifter
        Babaylan,          // Healer
        Manghuhula         // Seer
    }

    private Dictionary<string, Player> players = new Dictionary<string, Player>();
    private Queue<NightRole> nightTurnOrder;
    private NightRole currentTurn;
    private int nightCount = 0;
    [SerializeField] private UIManager ui_manager;

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
        foreach (PhotonPlayer photonPlayer in PhotonNetwork.playerList)
        {
            Debug.Log("Populating players dictionary");
            string roleProperty = (string)photonPlayer.CustomProperties["Role"];
            Debug.Log("Player's role is: " + roleProperty);

            Player newPlayer = new Player
            {
                username = photonPlayer.NickName,
                role = roleProperty
            };

            string photonPlayerID = photonPlayer.ID.ToString();
            players.Add(photonPlayerID, newPlayer);
        }
        Debug.Log("Calling NIght phase");

        if (ui_manager == null)
        {
            Debug.Log("Empty");
        }
        StartNightPhase();
    }

    private void ProcessNightAction(Player actor, Player target)
    {
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
                        // Find and disable Mangangaso
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
        }

        MoveToNextTurn();
    }

    private void ResetUIState()
    {
        ui_manager.SetFalseSpawnPoints();
        ui_manager.cardContainer.SetActive(false);
    }

    private void MoveToNextTurn()
    {
        if (nightTurnOrder.Count > 0)
        {
            currentTurn = nightTurnOrder.Dequeue(); // Get the next player's turn
            NotifyPlayerTurn(currentTurn);
        }
        else
        {
            EndNightPhase(); // No more turns left, end the night phase
        }
    }

    private void StartNightPhase()
    {
        //nightCount++;
        nightTurnOrder = new Queue<NightRole>();

        Debug.Log("Called Night phase");

        // Set turn order for Mangangaso and Aswang roles
        Player mangangaso = FindPlayerByRole("mangangaso");

        if (mangangaso != null && mangangaso.isAlive && !mangangaso.skipTurn)
        {
            ui_manager.ShowRoleUI("Mangangaso");
            Debug.Log("Mangangaso First Turn");
            nightTurnOrder.Enqueue(NightRole.Mangangaso);
        }

        foreach (var aswangRole in new[] { NightRole.AswangMandurugo, NightRole.AswangManananggal, NightRole.AswangBerbalang })
        {
            if (IsRoleAlive(aswangRole))
            {
                Debug.Log("Aswang Turn");
                ui_manager.ShowRoleUI(aswangRole.ToString());
                nightTurnOrder.Enqueue(aswangRole);
            }
        }

        // Add support roles if alive
        if (IsRoleAlive(NightRole.Babaylan))
        {
            if (ui_manager == null)
            {
                Debug.Log("Empty");
            }
            ui_manager.ShowRoleUI("Babaylan");
            nightTurnOrder.Enqueue(NightRole.Babaylan);
            Debug.Log("Babaylan Turn");
        }

        if (IsRoleAlive(NightRole.Manghuhula))
        {
            ui_manager.ShowRoleUI("Manghuhula");
            nightTurnOrder.Enqueue(NightRole.Manghuhula);
            Debug.Log("Manghuhula Turn");
        }

        if (nightTurnOrder.Count > 0)
        {
            currentTurn = nightTurnOrder.Dequeue();
            NotifyPlayerTurn(currentTurn);
        }
    }

    private void EndNightPhase()
    {
        foreach (var player in players.Values)
        {
            if (player.nightTarget && !player.isProtected)
            {
                player.isAlive = false; // Mark player as dead if targeted and not protected
            }

            // Reset night status for all players at the end of the night phase
            player.nightTarget = false;
            player.isProtected = false;
            player.turnDone = false;
        }

        CheckWinConditions(); // Check for win conditions after the night phase ends
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

    private bool IsRoleAlive(NightRole role)
    {
        foreach (var player in players.Values)
        {
            if (player.role.Equals(role.ToString(), System.StringComparison.OrdinalIgnoreCase) && player.isAlive)
            {
                return true;
            }
        }
        return false;
    }

    private void NotifyPlayerTurn(NightRole role)
    {
        Debug.Log($"It's {role}'s turn");
        ui_manager.ShowRoleUI(role.ToString());
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
                if (IsAswang(player.role)) aswangCount++;
                else villagerCount++;
            }
        }

        if (aswangCount == 0)
        {
            EndGame("Villagers");
        }
        else if (aswangCount >= villagerCount)
        {
            EndGame("Aswangs");
        }
        else
        {
            PhotonNetwork.LoadLevel("Day Transition"); // continue game
        }
    }

    private void EndGame(string winners)
    {
        Debug.Log($"Game Over! {winners} win!");
        // Implement game end logic here.
    }
}