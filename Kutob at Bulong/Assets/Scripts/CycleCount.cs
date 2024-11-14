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
<<<<<<< HEAD

=======
       Scene currentScene = SceneManager.GetActiveScene();

        if (currentScene.ToString() == "NightTransition") 
        {
            Increment("Night");
        }
        else
        {
            Increment("Day");
        }
>>>>>>> 4d72014af0a54baf27dc881dd8a59990b932804a
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
        PhotonNetwork.LoadLevel("NightPhase");
    }

    void switchToNightPhase()
    {
<<<<<<< HEAD
        Debug.Log("Switching to Night");
=======
>>>>>>> 4d72014af0a54baf27dc881dd8a59990b932804a
        StartCoroutine(addDelay());
        PhotonNetwork.LoadLevel("NightPhase");
    }

    void switchtoDayPhase()
    {
        StartCoroutine(addDelay());
        PhotonNetwork.LoadLevel("DiscussionPhase");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
