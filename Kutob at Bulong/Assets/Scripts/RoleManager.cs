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
    Mandurugo,
    Manananggal,
    Berbalang,
}
public class RoleManager : MonoBehaviour
{
    public static RoleManager Instance;
    

    // Store the roles for each player

    private RoleNormal[] normalRoles = new RoleNormal[] { RoleNormal.Mangangaso, RoleNormal.Aswang, RoleNormal.Babaylan, RoleNormal.Manghuhula };
    private RoleAswang[] aswangRoles = new RoleAswang[] { RoleAswang.Mandurugo, RoleAswang.Manananggal, RoleAswang.Berbalang};

    //keep track of players who already got assigned roles to avoid duplicates.
    private List<PhotonPlayer> playersAssignedRoles = new List<PhotonPlayer>();
    PhotonView photonView;
    private List<String> takenRole = new List<String>();


    /*int roleIndex = Random.Range(0, 3);  // 0 = Villager, 1 = Werewolf, 2 = Seer
    playerRole = (PlayerRole) roleIndex;*/
    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

   

    public void AssignRoles()
    {
        
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

                    // Log the assigned role
                    Debug.Log($"Assigned Taumbayan role to player: {player.NickName}");
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

                        Debug.Log($"Assigned {role_aswang.ToString()} role to player: {player.NickName}");
                        AssignAswangRoleToPlayer(player, role_aswang.ToString());
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

                    AssignAswangRoleToPlayer(player, role_aswang.ToString() );


                    Debug.Log($"Assigned {role_aswang.ToString()} role to player: {player.NickName}");
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

            case "Mandurugo":
                PhotonNetwork.LoadLevel("MnadurugoReveal");
                break;

            case "Manananggal":
                PhotonNetwork.LoadLevel("ManananggalReveal");
                break;

            case "Berbalang":
                PhotonNetwork.LoadLevel("BerbalangReveal");
                break;

            default:
                PhotonNetwork.LoadLevel("TaumbayanReveal");
                break;
        }
    }

    


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
