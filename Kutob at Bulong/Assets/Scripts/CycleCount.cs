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
<<<<<<< HEAD
        Scene currentScene = SceneManager.GetActiveScene();

        if (currentScene.ToString() == "NightTransition")
=======
       Scene currentScene = SceneManager.GetActiveScene();
        Debug.Log(currentScene.name.ToString());
        if (currentScene.name.ToString() == "NightTransition") 
>>>>>>> e1c62be521cb3c3649b956f38f8331c1631bd6b9
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
<<<<<<< HEAD
=======
    }

    // Update is called once per frame
    void Update()
    {
        
>>>>>>> e1c62be521cb3c3649b956f38f8331c1631bd6b9
    }
}
