using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SelectVote : MonoBehaviour
{
    public GameObject selectVoteModal;
    public TMP_Text playerName;
    public TMP_Text playerID;

    public PrefabClickTest prefabScript;


    // Start is called before the first frame update
    void Start()
    {
        prefabScript = FindAnyObjectByType<PrefabClickTest>();
    }

    public void OpenSelectVoteModal(string username,  string targetID)
    {

        if (selectVoteModal != null)
        {
            playerName.text = username;
            playerID.text = targetID;
            selectVoteModal.SetActive(true);
        }
        Debug.Log("Set text: " + targetID);
    }

    public void VoteConfirmed()
    {
        selectVoteModal.SetActive(false);
        prefabScript.VoteConfirmed(playerID.text);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
