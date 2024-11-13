using UnityEngine;
using System.Collections.Generic;

public class NightPhaseManager : MonoBehaviour
{
    public enum NightRole
    {
        Mangangaso,    // Hunter
        AswangMandurugo,  // Vampire
        AswangManananggal, // Flying monster
        AswangBerbalang,  // Shape-shifter
        Babaylan,      // Healer
        Manghuhula     // Seer
    }

    private Dictionary<string, Player> players = new Dictionary<string, Player>();
    private Queue<NightRole> nightTurnOrder;
    private NightRole currentTurn;
    private int nightCount = 0;

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

    void ProcessNightAction(Player actor, Player target)
    {
        switch (actor.role)
        {
            case "mangangaso":
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
                if (!IsAswang(target.role))
                {
                    target.nightTarget = true;
                }
                actor.turnDone = true;
                break;

            case "aswang - manananggal":
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
                            mangangaso.nightSkip = nightCount + 2;
                        }
                    }
                }
                actor.turnDone = true;
                break;

            case "aswang - berbalang":
                if (!IsAswang(target.role) && !target.isProtected)
                {
                    target.nightTarget = true;
                }
                actor.turnDone = true;
                break;

            case "babaylan":
                if (target.nightTarget)
                {
                    target.nightTarget = false;
                }
                break;

            case "manghuhula":
                // Seer just gets to know target's role
                RevealRole(actor, target);
                break;
        }

        MoveToNextTurn();
    }

    private void MoveToNextTurn()
    {
        if (nightTurnOrder.Count > 0)
        {
            currentTurn = nightTurnOrder.Dequeue();
            // Notify the current player it's their turn
            NotifyPlayerTurn(currentTurn);
        }
        else
        {
            EndNightPhase();
        }
    }

    private void StartNightPhase()
    {
        nightCount++;
        nightTurnOrder = new Queue<NightRole>();

        // Set turn order
        Player mangangaso = FindPlayerByRole("mangangaso");
        if (mangangaso != null && mangangaso.isAlive && !mangangaso.skipTurn)
        {
            nightTurnOrder.Enqueue(NightRole.Mangangaso);
        }

        // Add alive Aswang roles
        foreach (var aswangRole in new[] { NightRole.AswangMandurugo, NightRole.AswangManananggal, NightRole.AswangBerbalang })
        {
            if (IsRoleAlive(aswangRole))
            {
                nightTurnOrder.Enqueue(aswangRole);
            }
        }

        // Add support roles if alive
        if (IsRoleAlive(NightRole.Babaylan)) nightTurnOrder.Enqueue(NightRole.Babaylan);
        if (IsRoleAlive(NightRole.Manghuhula)) nightTurnOrder.Enqueue(NightRole.Manghuhula);

        if (nightTurnOrder.Count > 0)
        {
            currentTurn = nightTurnOrder.Dequeue();
            NotifyPlayerTurn(currentTurn);
        }
    }

    private void EndNightPhase()
    {
        // Process night actions
        foreach (var player in players.Values)
        {
            if (player.nightTarget && !player.isProtected)
            {
                player.isAlive = false;
            }
            // Reset night status
            player.nightTarget = false;
            player.isProtected = false;
            player.turnDone = false;
        }

        // Check win conditions
        CheckWinConditions();
    }

    private bool IsAswang(string role)
    {
        return role.StartsWith("aswang");
    }

    private Player FindPlayerByRole(string role)
    {
        foreach (var player in players.Values)
        {
            if (player.role == role && player.isAlive)
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
            if (player.role == role.ToString().ToLower() && player.isAlive)
            {
                return true;
            }
        }
        return false;
    }

    private void NotifyPlayerTurn(NightRole role)
    {
        // Implement UI notification for current player's turn
        Debug.Log($"It's {role}'s turn");
    }

    private void RevealRole(Player seer, Player target)
    {
        // Implement UI to show target's role to the seer
        Debug.Log($"Revealed {target.role} to {seer.username}");
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

        if (aswangCount == 0)
        {
            EndGame("Villagers");
        }
        else if (aswangCount >= villagerCount)
        {
            EndGame("Aswangs");
        }
    }

    private void EndGame(string winners)
    {
        Debug.Log($"Game Over! {winners} win!");
        // Implement game end logic
    }
}