using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using System.Data;


public enum RoleNormal
{
    Mangangaso,
    Aswang,
    Babaylan,
    Manghuhula,
    Taumbayan
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
    public RoleNormal[] playerRoles;
    public RoleAswang[] playerAswang;
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
       
        var playerArray = PhotonNetwork.playerList;
        Debug.Log(playerArray);

        System.Random random = new System.Random();

        int playerCount = PhotonNetwork.playerList.Length;
        int aswangCount = 1; // Need ko dito yung settings na sinet sa lobby

        /*foreach(var player in players)
        {
            Debug.Log("Player ID: " + player.UserId);
            Debug.Log("Player Name: " + player.NickName);
        } */


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
