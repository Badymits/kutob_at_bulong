using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CycleCount : MonoBehaviour
{
    // Start is called before the first frame update

    public TMP_Text count;
    int night_counter = 0;
    int day_counter = 0;

    void Start()
    {
            
    }

    public void Increment(string time)
    {
        if (time == "Night")
        {
            night_counter++;
            count.text = night_counter.ToString();
            StartCoroutine(addDelay());
            switchToNightPhase();
        }
        else
        {
            day_counter++;
            count.text = day_counter.ToString();
            StartCoroutine(addDelay());
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
        PhotonNetwork.LoadLevel("DiscussionPhase");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
