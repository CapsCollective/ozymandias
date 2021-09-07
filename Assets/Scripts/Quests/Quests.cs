using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Managers;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Quests
{
    public class Quests : MonoBehaviour
    {
        [SerializeField] private Transform dock, forestPath, mountainPath;
        [SerializeField] private QuestCounter counter;
        
        [SerializeField] private GameObject sectionPrefab;
        public GameObject SectionPrefab => sectionPrefab;

        public readonly List<Quest> quests = new List<Quest>();
        
        public int Count => quests.Count;
        
        // If a location is far enough away from the other quests
        const int MinDistance = 3;

        public bool FarEnoughAway(Vector3 position)
        {
            return quests
                .Where(quest => quest.Structure)
                .All(quest => Vector3.Distance(quest.Structure.transform.position, position) > MinDistance);
        }
        
        private void Awake()
        {
            State.OnGameEnd += OnGameEnd;
        }
        
        public bool Add(Quest q)
        {
            if (quests.Contains(q)) return false;
            quests.Add(q);
            q.Add();
            return true;
        }

        public bool Remove(Quest q)
        {
            if (!quests.Contains(q)) return false;
            quests.Remove(q);
            q.Remove();
            return true;
        }

        public List<QuestDetails> Save()
        {
            return quests.Select(x => x.Save()).ToList();
        }
        
        public async Task Load(List<QuestDetails> quests)
        {
            foreach (QuestDetails details in quests)
            {
                Quest q = await Addressables.LoadAssetAsync<Quest>(details.name).Task;
                this.quests.Add(q);
                q.Load(details);
            }
        }

        private void OnGameEnd()
        {
            for (int i = quests.Count - 1 ; i >= 0 ; i--) Remove(quests[i]);
        }
    }
}
