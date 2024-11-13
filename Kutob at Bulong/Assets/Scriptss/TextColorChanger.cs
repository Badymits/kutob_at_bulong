using UnityEngine;
using TMPro;

public class TextColorChanger : MonoBehaviour
{
    private TextMeshProUGUI text;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    public void OnHoverEnter()
    {
        // Change to highlighted color when hovered
        text.color = new Color32(255, 180, 0, 255);  // Yellow
    }

    public void OnHoverExit()
    {
        // Revert to normal color when hover ends
        text.color = new Color32(255, 255, 255, 255);  // White
    }

    public void OnClick()
    {
        // Change to pressed color when clicked
        text.color = new Color32(200, 200, 200, 255);  // Grey
    }
}