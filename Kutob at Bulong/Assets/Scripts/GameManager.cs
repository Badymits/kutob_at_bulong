using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Chat;
using Photon.Realtime;
using Unity.Mathematics;

public enum GamePhase { RoleReveal, Introduction, Night, Day, Discussion, Voting, EndGame }

public class GameManager : MonoBehaviour
{
    
    public GamePhase currentPhase;
    CycleCount cycleCount;

    private float phaseDuration = 10f; // Set the phase duration (in seconds)
    private float timer;

    PhotonView photonView;

    // keep track of eliminated players
    private Dictionary<int, bool> playerEliminated = new Dictionary<int, bool>();

    // fixed flow of the turns 
    public string[] turnOrder = new string[] { "Mangangaso", "Aswang", "Babaylan", "Manghuhula" };
    public int currentTurnIndex = 0;

    public List<int> werewolves = new List<int>(); // List to hold the IDs of werewolves
    public int werewolfTurnIndex = 0;  // Keep track of which werewolf's turn it is



    // Start is called before the first frame update
    void Start()
    {
        //PhotonNetwork.ConnectUsingSettings();
        StartCoroutine(PhaseTimer());
    }

    


    // Coroutine that acts as the phase timer
    IEnumerator PhaseTimer()
    {
        while (currentPhase != GamePhase.EndGame)
        {
            timer = phaseDuration; // Reset the timer at the start of each phase

            // Notify all players of the phase change
            photonView.RPC("UpdatePhase", PhotonTargets.All, currentPhase.ToString());

            while (timer > 0)
            {
                timer -= Time.deltaTime;
                yield return null; // Wait for the next frame

                if (timer <= 0)
                {
                    // Phase is over, move to the next one
                    TransitionToNextPhase();
                }
            }
        }
    }

    // Call this to transition to the next phase
    void TransitionToNextPhase()
    {
        // Transition to next phase (independently for each room)
        currentPhase = currentPhase switch
        {
            GamePhase.Night => GamePhase.Day,
            GamePhase.Day => GamePhase.Voting,
            GamePhase.Voting => GamePhase.EndGame,
            _ => currentPhase
        };

        // Example of loading a new scene based on phase
        switch (currentPhase)
        {
            case GamePhase.Night:
                cycleCount.Increment("Night");
                PhotonNetwork.LoadLevel("NightTransition"); // Scene specific to current phase
                break;
            case GamePhase.Day:
                cycleCount.Increment("Day");
                PhotonNetwork.LoadLevel("DayTransition"); // Scene specific to current phase
                break;
            case GamePhase.Voting:
                PhotonNetwork.LoadLevel("VotingScene"); // Scene specific to current phase
                break;
            case GamePhase.EndGame:
                PhotonNetwork.LoadLevel("EndGameScene");
                break;
        }
    }

    [PunRPC]
    void UpdatePhase(string phase)
    {
        currentPhase = (GamePhase)System.Enum.Parse(typeof(GamePhase), phase);
        Debug.Log("Current Phase: " + currentPhase);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Called to start the game after enough players have joined
    public void StartGame()
    {
        PhotonNetwork.LoadLevel("GameScene");  // Switch string 'GameScene' to name of the first scene in Kutob
    }

    public void SwitchPhase()
    {
        Debug.Log("Switch");
    }
}

