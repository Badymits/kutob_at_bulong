using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mangangaso : MonoBehaviour
{

    PhotonView photonView;
    PhotonPlayer photonPlayer;
    // check if player can perform action
    public bool CheckIfSkipTurn()
    {

        if (photonView.isMine)
        {
            if (photonPlayer.CustomProperties.ContainsKey("SkipTurn")) return true;
        }
        return false;
    }

    public bool CanExecute()
    {
        return true;
    }

    public void ProtectPlayer(string playerName)
    {
        
        if (!CheckIfSkipTurn())
        {
            PhotonPlayer protectedPlayer = GetPlayer(playerName);

            ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable
            {
                { "IsProtected", true }
            };

            protectedPlayer.SetCustomProperties(hash);
        }
    }

    PhotonPlayer GetPlayer(string playerName)
    {
        
        PhotonPlayer[] players = PhotonNetwork.playerList;

        foreach (PhotonPlayer player in players)
        {
            if (player.NickName == playerName)
            {
                return player;
            }
        }

        return null; 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
