using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabClickTest : MonoBehaviour
{
    // Start is called before the first frame update

    /*private void OnMouseDown()
    {
        Debug.Log("Clicked on prefab");        
    }*/

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))  // 0 for left-click
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Raycast to detect click on this GameObject
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                Debug.Log("Prefab clicked via raycast!");
            }
        }
    }
}
