using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InstantiatePlayers : MonoBehaviour
{

    public GameObject playerCardPrefab;
    public Transform playerCardsContainer;
    public Transform[] spawnPoints;


    void Start()
    {
        CreatePlayerCards();
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void CreatePlayerCards()
    {
        if (PhotonNetwork.playerList == null || playerCardPrefab == null || playerCardsContainer == null) return;

        float cardHeight = 300f;
        float cardSpacing = 10f;
        float startYPosition = 0f;

        for (int i = 0; i < PhotonNetwork.playerList.Length; i++)
        {
            
            PhotonPlayer player = PhotonNetwork.playerList[i];
            Debug.Log("Current player: " + player.NickName);
            Debug.Log("alive property of player: " + player.CustomProperties["isAlive"]);
            if (!(bool)player.CustomProperties["isAlive"] || (bool)player.CustomProperties["isVotedOut"] || player == null)
            {
                Debug.Log("Skipping eliminated player");
                continue;
            }
            else
            {
                Debug.Log("Why....aaaa");
            }

            GameObject playerCard = Instantiate(playerCardPrefab, playerCardsContainer);
            PhotonView playerPhotonView = playerCard.GetComponent<PhotonView>();

            playerPhotonView.TransferOwnership(player);
            if (playerCard != null)
            {
                SetupPlayerCard(playerCard, player, i, cardHeight, cardSpacing, startYPosition);
            }
        }
    }

    private void SetupPlayerCard(GameObject playerCard, PhotonPlayer player, int index, float cardHeight, float cardSpacing, float startYPosition)
    {
        if (playerCard == null || player == null) return;

        TextMeshProUGUI textComponent = playerCard.GetComponentInChildren<TextMeshProUGUI>();
        if (textComponent != null)
        {
            textComponent.text = player.NickName;
            textComponent.enableAutoSizing = false;
        }

        // Check if the player is the local player (self)
        if (player == PhotonNetwork.player)  // Or you can compare NickName: player.NickName == PhotonNetwork.LocalPlayer.NickName
        {
            // Convert the hex color code (#eab308, shade of yellow) to a Color
            Color newColor;
            if (ColorUtility.TryParseHtmlString("#eab308", out newColor))
            {
                // Change text color for the local player to the specified hex color
                textComponent.color = newColor;
            }
            else
            {
                // In case the color parsing fails, you can set a fallback color (e.g., white)
                textComponent.color = Color.green;
            }
        }
        else
        {
            // Default color for other players
            textComponent.color = Color.white;
        }

        RectTransform rectTransform = playerCard.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.localScale = Vector3.one;
            rectTransform.sizeDelta = new Vector2(200, cardHeight);
            rectTransform.anchoredPosition = new Vector2(0, startYPosition - (index * (cardHeight + cardSpacing)));
        }

        SetPlayerPosition(playerCard, index);
    }

    private void SetPlayerPosition(GameObject playerCard, int playerIndex)
    {
        if (playerCard == null || spawnPoints == null || playerIndex >= spawnPoints.Length) return;

        if (spawnPoints[playerIndex] != null)
        {
            RectTransform rectTransform = playerCard.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.position = spawnPoints[playerIndex].position;
                rectTransform.localScale = Vector3.one;
            }
        }
        else
        {
            Debug.LogError($"Spawn point for player index {playerIndex} is missing or out of bounds.");
        }
    }
    // Start is called before the first frame update
    
}
