using UnityEngine;

public class RolesManager : MonoBehaviour
{
    public GameObject[] rolePanels;  // Array to hold all 7 role panels
    private int currentPanel = 0;
    private PanelManager panelManager;

    void Start()
    {
        panelManager = GetComponent<PanelManager>();
        // Hide all panels except first one
        for (int i = 0; i < rolePanels.Length; i++)
        {
            rolePanels[i].SetActive(i == 0);
        }
    }

    public void ToggleRolesPanel()
    {
        if (rolePanels != null && rolePanels.Length > 0)
        {
            panelManager.CloseAllPanels();
            rolePanels[currentPanel].SetActive(true);
        }
    }

    public void NextPanel()
    {
        if (rolePanels != null && rolePanels.Length > 0)
        {
            rolePanels[currentPanel].SetActive(false);
            currentPanel = (currentPanel + 1) % rolePanels.Length;
            rolePanels[currentPanel].SetActive(true);
        }
    }

    public void PreviousPanel()
    {
        if (rolePanels != null && rolePanels.Length > 0)
        {
            rolePanels[currentPanel].SetActive(false);
            currentPanel = (currentPanel + rolePanels.Length - 1) % rolePanels.Length;
            rolePanels[currentPanel].SetActive(true);
        }
    }
}