using System;
using System.Collections.Generic;
using System.Linq;
using Managers_and_Controllers;
using UnityEngine;
using Random = UnityEngine.Random;

public class QuestMapController : MonoBehaviour
{
    // Fields
    #pragma warning disable 0649
    [SerializeField] private List<QuestDisplayManager> availableFlyers = new List<QuestDisplayManager>();
    [SerializeField] private List<QuestDisplayManager> usedFlyers = new List<QuestDisplayManager>();
    [SerializeField] private QuestCounter counter;
    //private Dictionary<string, GameObject> flyerMappings = new Dictionary<string, GameObject>();
    private HighlightOnHover displayingFlyerComponent;

    private static QuestMapController instance;
    public static QuestMapController QuestMap
    {
        get
        {
            if (!instance)
                instance = FindObjectsOfType<QuestMapController>()[0];
            return instance;
        }
    }

    public int ActiveQuests => usedFlyers.Count;
    
    public void OnOpened()
    {
        if (PlayerPrefs.GetInt("tutorial_video_quests", 0) > 0) return;
        PlayerPrefs.SetInt("tutorial_video_quests", 1);
        TutorialPlayerController.Instance.PlayClip(2);
    }

    private void Start()
    {
        foreach (var flyer in availableFlyers)
        {
            flyer.GetComponent<HighlightOnHover>().callbackMethod = OnFlyerClick;
        }
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
    
    public bool AddQuest(Quest q)
    {
        if (availableFlyers.Count == 0 || usedFlyers.Any(x => x.flyerQuest == q)) return false;
        QuestDisplayManager flyer = availableFlyers.PopRandom();
        flyer.gameObject.SetActive(true);
        q.cost = (int)(GameManager.Manager.WealthPerTurn * q.costScale);
        flyer.SetQuest(q);
        usedFlyers.Add(flyer);
        counter.UpdateCounter(usedFlyers.Count, true);
        return true;
    }
    
    public bool RemoveQuest(Quest q)
    {
        QuestDisplayManager flyer = usedFlyers.Find(x => x.flyerQuest == q);
        if (!flyer) return false;
        flyer.gameObject.SetActive(false);
        usedFlyers.Remove(flyer);
        counter.UpdateCounter(usedFlyers.Count);
        availableFlyers.Add(flyer);
        return true;
    }

    private void OnDestroy()
    {
        instance = null;
    }
}
