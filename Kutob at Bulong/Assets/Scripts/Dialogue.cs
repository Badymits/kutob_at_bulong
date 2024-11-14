using UnityEngine;
using TMPro;
using System.Collections;

public class Dialogue : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textComponent;
    [SerializeField] private string[] lines;
    [SerializeField] private float textSpeed = 0.05f;
    [SerializeField] private float delayBetweenLines = 1f; // Delay before next line starts

    private int currentLineIndex;
    private bool isTyping;
    private Coroutine typeCoroutine;

    private void Start()
    {
        InitializeDialogue();
    }

    private void InitializeDialogue()
    {
        currentLineIndex = 0;
        textComponent.text = string.Empty;
        StartTyping();
    }

    private void StartTyping()
    {
        isTyping = true;
        typeCoroutine = StartCoroutine(TypeLine());
    }

    private IEnumerator TypeLine()
    {
        textComponent.text = string.Empty;

        foreach (char c in lines[currentLineIndex])
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

        isTyping = false;

        // Wait for delay then proceed to next line
        yield return new WaitForSeconds(delayBetweenLines);

        if (currentLineIndex < lines.Length - 1)
        {
            NextLine();
        }
        else
        {
            EndDialogue();
        }
    }

    private void NextLine()
    {
        currentLineIndex++;
        StartTyping();
    }

    private void EndDialogue()
    {
        textComponent.text = string.Empty;
        gameObject.SetActive(false);
        PhotonNetwork.LoadLevel("NightTransition");
    }
}