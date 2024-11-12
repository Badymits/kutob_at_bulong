using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SelectTarget : MonoBehaviour
{

    PhotonView photonView;
    ModalControl modalControl;
    // Detect mouse click on this object
    void OnMouseDown()
    {
        Debug.Log("Image clicked!");
        modalControl = GetComponent<ModalControl>();
        modalControl.OpenModal();   

        // Additional logic here (e.g., change image, trigger event, etc.)
    }

    public void TargetConfirmed()
    {

        Debug.Log("Target Selected");
        //SceneManager.LoadScene("");
    }

    public string CheckRole()
    {
        return "";
    }
}
