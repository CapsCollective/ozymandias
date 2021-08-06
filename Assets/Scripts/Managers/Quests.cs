using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities;
using UI;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static Managers.GameManager;

namespace Managers
{
    public class Quests : MonoBehaviour
    {
        [SerializeField] private Transform dock, forestPath, mountainPath;
        [SerializeField] private QuestCounter counter;

        [SerializeField] private GameObject buildingPrefab;
        public GameObject BuildingPrefab => buildingPrefab;
        
        [SerializeField] private GameObject sectionPrefab;
        public GameObject SectionPrefab => sectionPrefab;

        private readonly List<Quest> _quests = new List<Quest>();
        
        public int Count => _quests.Count;
        
        private void Awake()
        {
            GameManager.OnGameEnd += OnGameEnd;
        }
        
        public bool Add(Quest q)
        {
            if (_quests.Contains(q)) return false;
            _quests.Add(q);
            q.Add();
            return true;
        }

        public bool Remove(Quest q)
        {
            if (!_quests.Contains(q)) return false;
            _quests.Remove(q);
            q.Remove();
            return true;
        }

        public List<QuestDetails> Save()
        {
            return _quests.Select(x => x.Save()).ToList();
        }
        
        public async Task Load(List<QuestDetails> quests)
        {
            foreach (QuestDetails details in quests)
            {
                Quest q = await Addressables.LoadAssetAsync<Quest>(details.name).Task;
                _quests.Add(q);
                q.Load(details);
            }
        }

        private void OnGameEnd()
        {
            for (int i = _quests.Count - 1 ; i >= 0 ; i--) Remove(_quests[i]);
        }
    }
}
