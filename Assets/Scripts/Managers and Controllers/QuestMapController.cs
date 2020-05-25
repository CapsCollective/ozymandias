using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestMapController : MonoBehaviour
{
    // Fields
    #pragma warning disable 0649
    [SerializeField] private GameObject[] flyerList;
    private Dictionary<string, GameObject> flyerMappings = new Dictionary<string, GameObject>();
    private HighlightOnHover displayingFlyerComponent;

    public static Action OnNewQuest;
    public static List<Quest> QuestList = new List<Quest>(8);

    private void Start()
    {
        foreach (var flyer in flyerList)
        {
            flyer.GetComponent<HighlightOnHover>().callbackMethod = OnFlyerClick;
        }

        OnNewQuest += UpdateDisplay;
    }

    private void Update()
    {
        if (!Input.GetMouseButtonDown(0) || !displayingFlyerComponent) return;
        if (displayingFlyerComponent.mouseOver) return;
        displayingFlyerComponent.ResetDisplay();
        displayingFlyerComponent = null;
    }

    private void OnFlyerClick(GameObject flyer)
    {
        if (displayingFlyerComponent) return;
        displayingFlyerComponent = flyer.GetComponent<HighlightOnHover>();
        displayingFlyerComponent.DisplaySelected();
    }

    public void UpdateDisplay()
    {
        // Commenting out since it's unused and has a lot of deprecated stuff
        // Fetch the currently active quests
        var quests = GetQuests();

        // Create a new mapping for previously posted quests
        var newMappings = quests.Where(q => flyerMappings.ContainsKey(q.QuestTitle))
            .ToDictionary(q => q.QuestTitle, q => flyerMappings[q.QuestTitle]);
        
        // Remove quests that have already been mapped
        quests = quests.Where(q => !newMappings.Keys.Contains(q.QuestTitle)).ToArray();
        
        // Create a shuffled array of unused flyers
        var unusedFlyers = flyerList.Where(f => !flyerMappings.Values.Contains(f)).ToArray()
            .OrderBy(x => new System.Random().Next(1, 8)).ToArray();
        
        // Assign the remaining quests to the unused flyers, setting their states and recording mappings
        for (var i = 0; i < unusedFlyers.Length; i++)
        {
            if (i < quests.Length)
            {
                unusedFlyers[i].SetActive(true);
                unusedFlyers[i].GetComponent<QuestDisplayManager>().SetQuest(quests[i]);
                newMappings.Add(quests[i].QuestTitle, unusedFlyers[i]);
            }
            else
                unusedFlyers[i].SetActive(false);
        }
        
        // Set the new flyer mappings
        flyerMappings = newMappings;
    }

    private Quest[] GetQuests()
    {
        return QuestList.ToArray();
    }

    public static void AddQuest(Quest q)
    {
        QuestList.Add(q);
        OnNewQuest?.Invoke();
    }

    private void OnDestroy()
    {
        OnNewQuest -= UpdateDisplay;
    }
}
