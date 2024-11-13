using UnityEngine;

public class PanelManager : MonoBehaviour
{
    public GameObject howToPlayPanel;
    public GameObject rolesPanel;

    public void CloseAllPanels()
    {
        howToPlayPanel.SetActive(false);
        rolesPanel.SetActive(false);
    }
}