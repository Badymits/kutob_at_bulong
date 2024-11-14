using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyManager : MonoBehaviour
{
    [Header("Player Count Buttons")]
    public Button[] playerCountButtons;  // Buttons for 5-10 players

    [Header("Aswang Count Buttons")]
    public Button[] aswangButtons;  // Buttons for 1-3 aswangs

    private int selectedPlayerCount = 5;  // Default minimum

    void Start()
    {
        // Initialize all buttons
        InitializeButtons();
        UpdateAswangButtonsState(5);  // Start with minimum players
    }

    void InitializeButtons()
    {
        // Setup player count buttons (5-10)
        for (int i = 0; i < playerCountButtons.Length; i++)
        {
            int playerCount = i + 5;  // Convert index to player count (5-10)
            playerCountButtons[i].onClick.AddListener(() => OnPlayerCountSelected(playerCount));

            // Set the text of each button
            TextMeshProUGUI buttonText = playerCountButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
                buttonText.text = playerCount.ToString();
        }

        // Setup aswang buttons (1-3)
        for (int i = 0; i < aswangButtons.Length; i++)
        {
            int aswangCount = i + 1;  // Convert index to aswang count (1-3)
            aswangButtons[i].onClick.AddListener(() => OnAswangCountSelected(aswangCount));

            // Set the text of each button
            TextMeshProUGUI buttonText = aswangButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
                buttonText.text = aswangCount.ToString();
        }
    }

    void OnPlayerCountSelected(int count)
    {
        selectedPlayerCount = count;
        UpdateAswangButtonsState(count);

        // Highlight selected button (optional)
        HighlightSelectedPlayerButton(count);
    }

    void UpdateAswangButtonsState(int playerCount)
    {
        // Enable/disable aswang buttons based on player count
        aswangButtons[1].interactable = (playerCount >= 8);  // 2nd aswang button (index 1)
        aswangButtons[2].interactable = (playerCount == 10); // 3rd aswang button (index 2)

        // Optional: Make them invisible instead of just disabled
        aswangButtons[1].gameObject.SetActive(playerCount >= 8);
        aswangButtons[2].gameObject.SetActive(playerCount == 10);
    }

    void OnAswangCountSelected(int count)
    {
        // Store the selected aswang count
        PlayerPrefs.SetInt("SelectedAswangCount", count);

        // Highlight selected button (optional)
        HighlightSelectedAswangButton(count);
    }

    // Optional: Visual feedback for selected buttons
    void HighlightSelectedPlayerButton(int count)
    {
        for (int i = 0; i < playerCountButtons.Length; i++)
        {
            bool isSelected = (i + 5) == count;
            playerCountButtons[i].GetComponent<Image>().color = isSelected ? Color.yellow : Color.white;
        }
    }

    void HighlightSelectedAswangButton(int count)
    {
        for (int i = 0; i < aswangButtons.Length; i++)
        {
            if (aswangButtons[i].interactable)
            {
                bool isSelected = (i + 1) == count;
                aswangButtons[i].GetComponent<Image>().color = isSelected ? Color.yellow : Color.white;
            }
        }
    }
}