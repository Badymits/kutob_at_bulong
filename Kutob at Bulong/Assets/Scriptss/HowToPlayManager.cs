using UnityEngine;

public class HowToPlayManager : MonoBehaviour
{
    public GameObject howToPlayPanel;
    private bool isPanelActive = false;

    public void ToggleHowToPlay()
    {
        isPanelActive = !isPanelActive;
        howToPlayPanel.SetActive(isPanelActive);
    }
}