using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class CycleCount : MonoBehaviour
{
    // Start is called before the first frame update

    public TMP_Text count;
    int night_counter = 0;
    int day_counter = 0;

    void Start()
    {
       Scene currentScene = SceneManager.GetActiveScene();
        Debug.Log(currentScene.name.ToString());
        if (currentScene.name.ToString() == "NightTransition") 
        {
            StartCoroutine(addDelay());
            Increment("Night");
        }
        else
        {
            StartCoroutine(addDelay());
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
        
        PhotonNetwork.LoadLevel("NightPhase");
    }

    void switchtoDayPhase()
    {
        PhotonNetwork.LoadLevel("Discussion Phase");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
