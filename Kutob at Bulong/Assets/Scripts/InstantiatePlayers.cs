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
            if (player == null) continue;

            GameObject playerCard = Instantiate(playerCardPrefab, playerCardsContainer);
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
