using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using System.Data;
using System;

public enum RoleNormal
{
    Mangangaso,
    Aswang,
    Babaylan,
    Manghuhula,
}

public enum RoleAswang
{
    AswangMandurugo,  // Vampire
    AswangManananggal, // Flying monster
    AswangBerbalang,  // Shape-shifter
}
public class RoleManager : MonoBehaviour
{
    public static RoleManager Instance;
    

    // Store the roles for each player

    private RoleNormal[] normalRoles = new RoleNormal[] { RoleNormal.Mangangaso, RoleNormal.Aswang, RoleNormal.Babaylan, RoleNormal.Manghuhula };
    private RoleAswang[] aswangRoles = new RoleAswang[] { RoleAswang.AswangMandurugo, RoleAswang.AswangManananggal, RoleAswang.AswangBerbalang };

    //keep track of players who already got assigned roles to avoid duplicates.
    private List<PhotonPlayer> playersAssignedRoles = new List<PhotonPlayer>();
    public PhotonView photonView;
    private List<String> takenRole = new List<String>();


    /*int roleIndex = Random.Range(0, 3);  // 0 = Villager, 1 = Werewolf, 2 = Seer
    playerRole = (PlayerRole) roleIndex;*/
    

    void Start()
    {
        photonView = GetComponent<PhotonView>();
        if (photonView == null)
        {
            Debug.Log("No photon view");
        }
        else
        {
            Debug.Log("Photon view present");
        }

    }


    public void Something()
    {
        photonView = GetComponent<PhotonView>();
        if (photonView == null)
        {
            Debug.Log("Null");
        }
        else
        {
            Debug.Log("Calling assign roles");
            //AssignRoles();
        }
    }
   

    public void AssignRoles()
    {
        Debug.Log("adawdawd");
        List<PhotonPlayer> shuffledPlayers = new List<PhotonPlayer>(PhotonNetwork.playerList);

        ShuffleList(shuffledPlayers);

        Debug.Log(PhotonNetwork.playerList);

        System.Random random = new System.Random();

        int playerCount = PhotonNetwork.playerList.Length;
        int aswangCount = 1; // Need ko dito yung settings na sinet sa lobby

        foreach (PhotonPlayer player in PhotonNetwork.playerList)
        {

            while (true)
            {
                int roleIndex = random.Next(0, 4);
                RoleNormal role = normalRoles[roleIndex];

                Debug.Log("Role: " + role);

                // populate list first with most important roles
                if (playersAssignedRoles.Count >= (normalRoles.Length + aswangCount - 1))
                {

                    AssignRoleToPlayer(player, "Taumbayan");
                    playersAssignedRoles.Add(player);
                    break;
                }

                else if (!takenRole.Contains(role.ToString()))
                {
                    if (role.ToString() != "Aswang")
                    {
                        AssignRoleToPlayer(player, role.ToString());
                        takenRole.Add(role.ToString());
                        playersAssignedRoles.Add(player);
                        break;
                    }
                    else if (role.ToString() == "Aswang" && aswangCount != 0)
                    {
                        int aswangIndex = random.Next(0, 2);
                        RoleAswang role_aswang = aswangRoles[aswangIndex];

                        string aswang_modified_role = StringModifyAswang(role_aswang.ToString());
                        AssignAswangRoleToPlayer(player, aswang_modified_role);
                        aswangCount--;
                        playersAssignedRoles.Add(player);
                        break;
                    }
                }


                else if (role.ToString() == "Aswang" && aswangCount != 0)
                {
                    int aswangIndex = random.Next(0, 2);
                    RoleAswang role_aswang = aswangRoles[aswangIndex];

                    Debug.Log(role_aswang.ToString());

                    string aswang_modified_role = StringModifyAswang(role_aswang.ToString());
                    AssignAswangRoleToPlayer(player, aswang_modified_role);


                    Debug.Log($"Assigned {aswang_modified_role} role to player: {player.NickName}");
                    aswangCount -= 1;

                    playersAssignedRoles.Add(player);
                    break;

                }
            }
            Debug.Log("Player ID: " + player.UserId);
            Debug.Log("Player Name: " + player.NickName);
        }
        Debug.Log(playersAssignedRoles);

        if (PhotonNetwork.isMasterClient)
        {
            
            photonView.RPC("DistributeScene", PhotonTargets.All);
        }
        
    }

    // Fisher-Yates shuffle for list of PhotonPlayer
    public static void ShuffleList(List<PhotonPlayer> list)
    {
        System.Random random = new System.Random();
        int n = list.Count;

        // Go through the list backwards and swap each element with a randomly selected element
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);  // Generate random index from 0 to n-1
            PhotonPlayer value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public string StringModifyAswang(string str)
    {
        switch (str) 
        {
            case "AswangMandurugo":
                return "aswang - mandurugo";
            case "AswangManananggal":
                return "aswang - manananggal";
            case "AswangBerbalang":
                return "aswang - berbalang";

            default:
                return "";

        }
    }

    public void AssignRoleToPlayer(PhotonPlayer player, string role)
    {
        Debug.Log("Player Role: " + role);
        ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable
        {
            { "Role", role }
        };
        player.SetCustomProperties(playerProperties);
    }

    public void AssignAswangRoleToPlayer(PhotonPlayer player, string role)
    {
        Debug.Log("Aswang Role: " + role);
        ExitGames.Client.Photon.Hashtable playerProperties = new ExitGames.Client.Photon.Hashtable
        {
            { "Role", role }
        };
        player.SetCustomProperties(playerProperties);
    }

    [PunRPC]
    public void DistributeScene()
    {
        Debug.Log("Distribute Scene reaced");
        foreach (PhotonPlayer player in PhotonNetwork.playerList)
        {
            if (player.CustomProperties.ContainsKey("Role"))
            {
                string playerRole = (string)player.CustomProperties["Role"];
                
                Debug.Log(player.NickName + " has role: " + playerRole);

                LoadSceneBasedOnRole(playerRole);
            }
            
        }
    }

    public void LoadSceneBasedOnRole(string role)
    {
        Debug.Log("LoadScenebasedOnRole Reached");
        Debug.Log("Role: " + role);
        switch (role)
        {
            case "Mangangaso":
                PhotonNetwork.LoadLevel("MangangasoReveal");
                break;

            case "Babaylan":
                PhotonNetwork.LoadLevel("BabaylanReveal");
                break;

            case "Manghuhula":
                PhotonNetwork.LoadLevel("ManghuhulaReveal");
                break;

            case "aswang - mandurugo":
                PhotonNetwork.LoadLevel("Mandurugo Reveal");
                break;

            case "aswang - manananggal":
                PhotonNetwork.LoadLevel("ManananggalReveal");
                break;

            case "aswang - berbalang":
                PhotonNetwork.LoadLevel("BerbalangReveal");
                break;

            default:
                PhotonNetwork.LoadLevel("TaumbayanReveal");
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
