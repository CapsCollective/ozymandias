using UnityEngine;
using static GameState.GameManager;

namespace UI
{
    public class WelcomeSign : MonoBehaviour
    {
        private TextMesh _textMesh;
        private const string SignContent = "Welcome to <town name here> !\nPopulation: ";

        private void Start()
        {
            void UpdatePopulation()
            {
                _textMesh.text = SignContent + Manager.Adventurers.Count.ToString("00");
            }

            _textMesh = GetComponent<TextMesh>();
        
            OnUpdateUI += UpdatePopulation;
            UpdatePopulation();
        }
    }
}
