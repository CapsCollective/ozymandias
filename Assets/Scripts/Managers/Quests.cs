using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Controllers;
using DG.Tweening;
using Entities;
using UI;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Utilities;
using static Managers.GameManager;

namespace Managers
{
    public class Quests : MonoBehaviour
    {
        [SerializeField] private List<QuestFlyer> availableFlyers = new List<QuestFlyer>();
        [SerializeField] private List<QuestFlyer> usedFlyers = new List<QuestFlyer>();
        [SerializeField] private QuestCounter counter;
        [SerializeField] private float animateInDuration = .5f;
        [SerializeField] private float animateOutDuration = .75f;

        private QuestFlyer _selectedQuest;
        private Canvas _canvas;

        public int Count => usedFlyers.Count;
        
        private void Start()
        {
            OnGameEnd += GameOver;
            
            _canvas = GetComponent<Canvas>();

            foreach (var flyer in availableFlyers)
            {
                flyer.GetComponent<QuestFlyer>().CallbackMethod = OnFlyerClick;
            }
            
            Close();
        }

        private void OnFlyerClick(GameObject flyer)
        {
            if (_selectedQuest) return;
            _selectedQuest = flyer.GetComponent<QuestFlyer>();
            _selectedQuest.DisplaySelected();
        }
        
        public void Open()
        {
            Manager.EnterMenu();
            Jukebox.Instance.PlayScrunch();
            counter.Read();
            
            transform
                .DOLocalMove(Vector3.zero, animateInDuration)
                .OnStart(() => { _canvas.enabled = true; });
            transform.DOLocalRotate(Vector3.zero, animateInDuration);
        }
        
        public void Close()
        {
            Manager.ExitMenu();
            
            transform.DOLocalMove(new Vector3(-300, -1200, 0), animateOutDuration)
            .OnComplete(() => { _canvas.enabled = false; });
        }
        
        public bool Add(Quest q)
        {
            if (availableFlyers.Count == 0 || usedFlyers.Any(x => x.quest == q)) return false;
            q.Assigned = new List<Adventurer>(); // Reset the assigned
            QuestFlyer flyer = availableFlyers.PopRandom();
            flyer.gameObject.SetActive(true);
            q.Cost = (int)(Manager.WealthPerTurn * q.costScale);
            flyer.SetQuest(q);
            if (q.Assigned.Count > 0) flyer.RandomRotateStamps(); // Mark active quests when being added (for loading)
            usedFlyers.Add(flyer);
            counter.UpdateCounter(usedFlyers.Count, true);
            return true;
        }

        public bool Remove(Quest q)
        {
            QuestFlyer flyer = usedFlyers.Find(x => x.quest == q);
            if (!flyer) return false;
            Remove(flyer);
            return true;
        }

        private void Remove(QuestFlyer flyer)
        {
            foreach (var adventurer in flyer.quest.Assigned) adventurer.assignedQuest = null;
            flyer.quest.Assigned = new List<Adventurer>();
            flyer.gameObject.SetActive(false);
            usedFlyers.Remove(flyer);
            counter.UpdateCounter(usedFlyers.Count);
            availableFlyers.Add(flyer);
        }

        public List<QuestDetails> Save()
        {
            return usedFlyers.Select(x => x.quest.Save()).ToList();
        }
        
        public async Task Load(List<QuestDetails> quests)
        {
            foreach (QuestDetails details in quests)
            {
                Quest q = await Addressables.LoadAssetAsync<Quest>(details.name).Task;
                q.Load(details);
                Add(q);
            }
        }
        
        public void GameOver()
        {
            usedFlyers.ToList().ForEach(Remove);
        }
    }
}
