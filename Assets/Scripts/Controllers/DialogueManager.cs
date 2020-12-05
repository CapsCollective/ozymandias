#pragma warning disable 0649
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Controllers
{
    public class DialogueManager : MonoBehaviour
    {
        [SerializeField] private Text dialogueWindowText;
        private string[] _dialogueEvents;

        public void StartDialogue(string dialogueId)
        {
            SetupDialogueEvents(dialogueId);
            gameObject.SetActive(true);
            Continue();
        }
    
        public void Continue()
        {
            if (_dialogueEvents.Length > 0)
            {
                dialogueWindowText.text = _dialogueEvents[0];
                _dialogueEvents = _dialogueEvents.Skip(1).ToArray();
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
            _dialogueEvents = System.IO.File.ReadAllText(Application.dataPath + additionalPath + "/StreamingAssets/Dialogue/" + dialogueId + ".dialogue")
                .Split(new [] { "~~" }, StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < _dialogueEvents.Length; i++)
            {
                _dialogueEvents[i] = _dialogueEvents[i].Trim('\n');
            }
        }
    }
}
