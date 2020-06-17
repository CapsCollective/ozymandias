using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static GameManager;

public class AdventurerList : MonoBehaviour
{
    public GameObject rowPrefab;

    public void Display()
    {
        foreach (Transform child in transform) Destroy(child.gameObject);

        foreach (Adventurer adventurer in Manager.adventurers.OrderByDescending(x => x.assignedQuest ? x.assignedQuest.QuestTitle : "").ThenByDescending(x => x.turnJoined))
        {
            GameObject row = Instantiate(rowPrefab, transform);
            row.GetComponent<AdventurerRow>().Display(adventurer);
        }
    }
}
