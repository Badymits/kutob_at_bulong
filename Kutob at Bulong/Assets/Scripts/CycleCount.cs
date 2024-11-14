using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class CycleCount : MonoBehaviour
{
    public TMP_Text count;
    int night_counter = 0;
    int day_counter = 0;

    void Start()
    {
        Scene currentScene = SceneManager.GetActiveScene();

        if (currentScene.ToString() == "NightTransition")
        {
            Increment("Night");
        }
        else
        {
            Increment("Day");
        }
    }

    public void Increment(string time)
    {
        if (time == "Night")
        {
            night_counter++;
            count.text = night_counter.ToString();
            switchToNightPhase();
        }
        else
        {
            day_counter++;
            count.text = day_counter.ToString();
            switchtoDayPhase();
        }

    }

    IEnumerator addDelay()
    {
        yield return new WaitForSecondsRealtime(5f);
    }

    void switchToNightPhase()
    {
        StartCoroutine(addDelay());
        PhotonNetwork.LoadLevel("NightPhase");
    }

    void switchtoDayPhase()
    {
        StartCoroutine(addDelay());
        PhotonNetwork.LoadLevel("Discussion Phase");
    }
}
