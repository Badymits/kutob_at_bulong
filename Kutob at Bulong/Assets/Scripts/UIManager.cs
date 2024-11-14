using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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

        if (photonView.isMine)
        {
            switch (role)
            {
                case "Mangangaso":
                    cardContainer.SetActive(true);
                    tMP.text = "Choose who you'll Protect";
                    break;

                case "Babaylan":
                    cardContainer.SetActive(true);
                    tMP.text = "Choose who you'll SAVE";
                    break;

                case "Manghuhula":
                    cardContainer.SetActive(true);
                    tMP.text = "Choose who you'll Guess";
                    break;

                case "aswang - mandurugo":
                    tMP.text = "Choose who you'll KILL";
                    cardContainer.SetActive(true);
                    break;

                case "aswang - manananggal":
                    tMP.text = "Choose who you'll KILL";
                    cardContainer.SetActive(true);
                    break;

                case "aswang - berbalang":
                    tMP.text = "Choose who you'll KILL";
                    cardContainer.SetActive(true);
                    break;

                default:
                    break;
            }
        }
        
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
