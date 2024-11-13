using UnityEngine;

public class HowToPlayManager : MonoBehaviour
{
    public GameObject howToPlayPanel;
    private PanelManager panelManager;

    void Start()
    {
        panelManager = GetComponent<PanelManager>();
    }

    public void ToggleHowToPlay()
    {
        panelManager.CloseAllPanels();
        howToPlayPanel.SetActive(true);
    }
}

