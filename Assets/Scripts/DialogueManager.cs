using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private Text dialogueWindowText;
    private string[] dialogueEvents;

    public void StartDialogue(string dialogueId)
    {
        SetupDialogueEvents(dialogueId);
        gameObject.SetActive(true);
        Continue();
    }
    
    public void Continue()
    {
        if (dialogueEvents.Length > 0)
        {
            dialogueWindowText.text = dialogueEvents[0];
            dialogueEvents = dialogueEvents.Skip(1).ToArray();
        }
        else
        {
            End();
        }
    }
    
    public void End()
    {
        gameObject.SetActive(false);
    }

    private void SetupDialogueEvents(string dialogueId)
    {
        dialogueEvents = System.IO.File.ReadAllText("./Assets/Dialogue/" + dialogueId + ".dialogue")
            .Split(new [] { "~~" }, StringSplitOptions.RemoveEmptyEntries);
        for (var i = 0; i < dialogueEvents.Length; i++)
        {
            dialogueEvents[i] = dialogueEvents[i].Trim('\n');
        }
    }
}
