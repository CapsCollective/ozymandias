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
            _canvas = GetComponent<Canvas>();

            foreach (QuestFlyer flyer in availableFlyers)
            {
                flyer.GetComponent<QuestFlyer>().CallbackMethod = OnFlyerClick;
            }
        }
        
        private void Update()
        {
            if (!Input.GetMouseButtonDown(0) || !_selectedQuest) return;
            if (_selectedQuest.mouseOver) return;
            _selectedQuest.ResetDisplay();
            _selectedQuest = null;
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
            
            /*if (PlayerPrefs.GetInt("tutorial_video_quests", 0) > 0) return;
            PlayerPrefs.SetInt("tutorial_video_quests", 1);
            TutorialPlayerController.Instance.PlayClip(2);*/
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
            QuestFlyer flyer = availableFlyers.PopRandom();
            flyer.gameObject.SetActive(true);
            q.cost = (int)(GameManager.Manager.WealthPerTurn * q.costScale);
            flyer.SetQuest(q);
            if (q.assigned.Count > 0) flyer.RandomRotateStamps(); // Mark active quests when being added (for loading)
            usedFlyers.Add(flyer);
            counter.UpdateCounter(usedFlyers.Count, true);
            return true;
        }

        public bool Remove(Quest q)
        {
            QuestFlyer flyer = usedFlyers.Find(x => x.quest == q);
            if (!flyer) return false;
            flyer.gameObject.SetActive(false);
            usedFlyers.Remove(flyer);
            counter.UpdateCounter(usedFlyers.Count);
            availableFlyers.Add(flyer);
            return true;
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
        
    }
}
