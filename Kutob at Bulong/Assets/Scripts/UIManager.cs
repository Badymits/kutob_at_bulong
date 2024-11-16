using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static NightPhaseManager;

public class UIManager : MonoBehaviour
{
    // Start is called before the first frame update
    public PhotonView photonView;
    public GameObject cardContainer;
    public Transform[] spawnPoints;
    public TMP_Text tMP;

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

    public void ShowRoleUI(string role)
    {
        // get local player
        PhotonPlayer player = PhotonNetwork.player;

        string playerRole = GetPlayerRole(player, role);
        

        if (photonView.isMine)
        {
            switch (playerRole)
            {
                case "mangangaso":
                    cardContainer.SetActive(true);
                    tMP.text = "Choose who you'll Protect";
                    break;

                case "babaylan":
                    cardContainer.SetActive(true);
                    tMP.text = "Choose who you'll SAVE";
                    break;

                case "manghuhula":
                    cardContainer.SetActive(true);
                    tMP.text = "Choose who you'll Guess";
                    break;

                case "aswang - mandurugo":
                case "aswang - manananggal":
                case "aswang - berbalang":
                    tMP.text = "Choose who you'll KILL";
                    cardContainer.SetActive(true);
                    break;

                default:
                    break;
            }
        }
        
    }

    public string GetPlayerRole(PhotonPlayer photonPlayer, string role)
    {
        
        if (photonPlayer.CustomProperties.ContainsKey("Role"))
        {
            string playerRole = (string)photonPlayer.CustomProperties["Role"];
            Debug.Log("Current turn in night phse" + playerRole);
            return playerRole;
        }
        else
        {
            Debug.Log("Wala tsong");
        }
        return "";
    }

    public void SetFalseSpawnPoints()
    {
        // Loop through each spawn point using a for loop
        foreach (Transform spawnPoint in spawnPoints)
        {
            spawnPoint.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
