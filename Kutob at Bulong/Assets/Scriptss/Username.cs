using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Username : MonoBehaviour
{
    public TMP_InputField inputField;
    public GameObject UsernamePage;
    // Removed public TMP_Text MyUsername;

    void Start()
    {
        if (string.IsNullOrEmpty(PlayerPrefs.GetString("Username")))
        {
            UsernamePage.SetActive(true);
        }
        else
        {
            UsernamePage.SetActive(true);  // Keep the page visible
        }
    }

    public void SaveUsername()
    {
        if (!string.IsNullOrEmpty(inputField.text))
        {
            PlayerPrefs.SetString("Username", inputField.text);
            
            SceneManager.LoadScene("Loading Screen");
        }
    }
}