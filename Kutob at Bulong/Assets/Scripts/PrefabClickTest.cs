using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PrefabClickTest : MonoBehaviour
{
    // Start is called before the first frame update

    private void OnMouseDown()
    {
        Debug.Log("Clicked on prefab");
    }

    void Start()
    {
        if (GetComponent<BoxCollider2D>() == null)
        {
            Debug.LogError("No BoxCollider2D attached!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left click
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.collider != null)
            {
                Debug.Log("aaaa");
                Debug.Log("Hit: " + hit.collider.gameObject.name);
            }

            // Raycast on UI elements
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            // Create a list to store the raycast results
            List<RaycastResult> raycastResults = new List<RaycastResult>();

            // Perform raycast all and store the results in the list
            EventSystem.current.RaycastAll(pointerEventData, raycastResults);

            // Check if any UI element was hit
            if (raycastResults.Count > 0)
            {
                Debug.Log("");
                Debug.Log("clicke on prefab");
            }
        }
    }
}
