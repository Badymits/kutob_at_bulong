using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manghuhula : MonoBehaviour
{
    // Start is called before the first frame update
    PhotonView photonView;
    public GameObject player;

    // The Seer can inspect a player's role
    public static class EventCodes
    {
        public const byte RevealRole = 100;  // Event code for revealing the role
    }
    public void RevealRole()
    {
        

        // Only the Seer should see the role
        if (photonView.isMine)
        {
            // Print the role that the Seer is inspecting
            Debug.Log("Seer inspects: " + player);
        }
    }

    string GetPlayerRole(string playerName)
    {
        // Placeholder for getting the role of a player
        return "Werewolf";  // For testing, always return "Werewolf"
    }
}
