using UnityEngine;
public class PanelManager : MonoBehaviour
{
    public GameObject howToPlayPanel;
    public GameObject rolesPanel;
    public GameObject settingsPanel;

    public void CloseAllPanels()
    {
        howToPlayPanel.SetActive(false);
        rolesPanel.SetActive(false);
        settingsPanel.SetActive(false);
    }
}