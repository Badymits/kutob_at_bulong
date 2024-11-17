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

    public void ShowRoleUI(string role) // for cases such as aswang, the value returned here is AswangMandurugo, AswangManananggal, AswangBerbalang
    {
        // get local player
        PhotonPlayer localPlayer = PhotonNetwork.player;

        string playerRole = GetPlayerRole(localPlayer, role);
        Debug.Log("Parameter role: " + role);
        Debug.Log("Player role in UI manager: " + playerRole);
        

        if (photonView.isMine)
        {
            switch (playerRole.ToLower())
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

                case "aswangmandurugo":
                case "aswangmanananggal":
                case "aswangberbalang":
                    tMP.text = "Choose who you'll KILL";
                    cardContainer.SetActive(true);
                    break;

                default:
                    break;
            }
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

    public string GetPlayerRole(PhotonPlayer localPlayer, string role)
    {
        
        if (localPlayer.CustomProperties.ContainsKey("Role"))
        {
            string asawngRole = StringModifyAswang(role);

            // the aswang roles that were set in RoleManager are: aswang - mandurugo, aswang - manananggal, and aswang - berbalang
            string localPlayerRole = (string)localPlayer.CustomProperties["Role"]; 

            if (localPlayerRole == asawngRole)
            {
                return role; // just return the role
            }

            else if (localPlayerRole != asawngRole && localPlayerRole == role)
            {
                Debug.Log("Current turn in night phse" + localPlayerRole);
                return localPlayerRole;
            }

            else
            {
                return "";
            }

        }
        else
        {
            Debug.Log("No Role");
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
