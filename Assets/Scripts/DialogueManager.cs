using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private Text dialogueWindowText;
    private string[] dialogueEvents;
    
    public void StartDialogue()
    {
        dialogueEvents = GetDialogueEvents();
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
            gameObject.SetActive(false);
        }
    }

    private static string[] GetDialogueEvents()
    {
        return new[] {
            "Hello, this is the dialogue speaking...", 
            "This is the game dialogue window. It displays important information about the game and the size of " +
            "the window will adapt to the content of the window accordingly.",
            "Ok then. Goodbye for now!", 
        };
    }
}
