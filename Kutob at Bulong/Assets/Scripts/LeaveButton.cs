using UnityEngine;
using UnityEngine.SceneManagement;

public class LeaveButton : MonoBehaviour
{
    public void OnLeaveButtonPressed()
    {
        SceneManager.LoadScene("Loading Screen");
    }
}