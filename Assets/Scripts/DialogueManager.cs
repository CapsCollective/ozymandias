using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    #pragma warning disable 0649
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
        var additionalPath = Application.platform == RuntimePlatform.OSXPlayer ? "/Resources/Data" : "";
        dialogueEvents = System.IO.File.ReadAllText(Application.dataPath + additionalPath + "/StreamingAssets/Dialogue/" + dialogueId + ".dialogue")
            .Split(new [] { "~~" }, StringSplitOptions.RemoveEmptyEntries);
        for (var i = 0; i < dialogueEvents.Length; i++)
        {
            dialogueEvents[i] = dialogueEvents[i].Trim('\n');
        }
    }
}
