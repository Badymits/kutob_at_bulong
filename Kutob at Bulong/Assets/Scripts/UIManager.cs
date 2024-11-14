using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    // Start is called before the first frame update
    public PhotonView photonView;
    public GameObject cardContainer;
    public Transform[] spawnPoints;

    void Start()
    {
  
    }

    public void ShowRoleUI(string role)
    {
        if (photonView.isMine)
        {
            switch (role)
            {
                case "Mangangaso":
                    cardContainer.SetActive(true);
                    break;

                case "Babaylan":
                    cardContainer.SetActive(true);
                    break;

                case "Manghuhula":
                    cardContainer.SetActive(true);
                    break;

                case "aswang - mandurugo":
                    cardContainer.SetActive(true);
                    break;

                case "aswang - manananggal":
                    cardContainer.SetActive(true);
                    break;

                case "aswang - berbalang":
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
