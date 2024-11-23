using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupMessage : MonoBehaviour
{

    public GameObject message;
    public float messageTimer = 5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OpenMessage()
    {
        message.SetActive(true);
        CloseMessage();
    }
     // added delay to not close immediately
    public void CloseMessage()
    {
        if (message != null && message.activeInHierarchy)
        {
            StartCoroutine(CloseMessageObject());
        }
        return;
    }

    IEnumerator CloseMessageObject()
    {
        yield return new WaitForSeconds(messageTimer);

        message.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
