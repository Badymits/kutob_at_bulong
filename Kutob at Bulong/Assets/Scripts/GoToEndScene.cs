using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoToEndScene : MonoBehaviour
{

    public GameObject photonViewObject;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if ((string)PhotonNetwork.room.CustomProperties["Game_Winner"] != "")
        {
            Debug.Log("Not damn equal");
            EndGame();
        }
    }

    void EndGame()
    {
        Debug.Log("Called endgame method");
        // Dynamically add a PhotonView when the game ends
        if (photonViewObject != null)
        {
            PhotonView photonView = photonViewObject.GetComponent<PhotonView>();

            // If the object does not already have a PhotonView, add one
            if (photonView == null)
            {
                photonView = photonViewObject.AddComponent<PhotonView>();
            }

            // Optionally, set up the PhotonView (for example, assigning ownership or setting up synchronization)
            photonView.ObservedComponents = new System.Collections.Generic.List<Component> { photonViewObject.GetComponent<Renderer>() };  // Add components to sync, like Renderer or Transform

            // Now the game over object is synchronized across the network
            Debug.Log("PhotonView added to GameObject and synchronized");
            PhotonNetwork.LoadLevel("Announcement_Day");

        }


    }
}
