using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModalControl : MonoBehaviour
{

    public GameObject modal;
    // Start is called before the first frame update
    public void OpenModal()
    {
        if (modal != null) { modal.SetActive(true); }
    }

    public void CloseModal()
    {
        if (modal != null) { modal.SetActive (false);  }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
