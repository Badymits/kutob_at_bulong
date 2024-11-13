using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    public GameObject settingsPanel;
    private PanelManager panelManager;

    void Start()
    {
        panelManager = GetComponent<PanelManager>();
        settingsPanel.SetActive(false);  // Make sure panel starts hidden
    }

    public void ToggleSettingsPanel()
    {
        panelManager.CloseAllPanels();
        settingsPanel.SetActive(true);
    }
}