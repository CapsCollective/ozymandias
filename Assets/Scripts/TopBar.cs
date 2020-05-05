using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopBar : MonoBehaviour
{
    public Text adventurers, satisfaction, effectiveness, threat, defense;
    // Update is called once per frame
    public void UpdateUI()
    {
        adventurers.text = "Adventurers: " + 
                           GameManager.Instance.AvailableAdventurers +
                           " / " +
                           GameManager.Instance.Accommodation;
        
        satisfaction.text = "Satisfaction: " + GameManager.Instance.Satisfaction + "%";

        effectiveness.text = "Effectiveness: " + GameManager.Instance.Effectiveness + "%";

        threat.text = "Threat: " + GameManager.Instance.Threat;
        defense.text = "Defense: " + GameManager.Instance.Defense;
    }
}
