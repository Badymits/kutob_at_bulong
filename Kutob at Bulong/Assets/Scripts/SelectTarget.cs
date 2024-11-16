using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

public class SelectTarget : MonoBehaviour
{

    PhotonView photonView;
    ModalControl modalControl;
    public GameObject nightSelectTargetModal;
    public GameObject playerCardPrefab;


    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Clicked on prefab");
        Debug.Log("Player Name: " + playerCardPrefab.GetComponentInChildren<TMP_Text>().text);

        nightSelectTargetModal.SetActive(true);
        Debug.Log(nightSelectTargetModal);
    }

    public void CloseSelectTargetModal()
    {
        nightSelectTargetModal.SetActive(!gameObject.activeSelf);
    }

    public void Test()
    {

    }

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
