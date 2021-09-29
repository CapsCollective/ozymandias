using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Managers;
using Requests.Templates;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Utilities;
using static Managers.GameManager;

namespace Requests
{
    public class Requests: MonoBehaviour
    {
        [SerializeField] private SerializedDictionary<Guild, RequestDisplay> displays;
        private readonly Dictionary<Guild, Request> _requests = new Dictionary<Guild, Request>();

        private void Awake()
        {
            State.OnEnterState += () =>
            {
                if (!Manager.State.NextTurn) return;
                foreach (KeyValuePair<Guild, Request> request in _requests)
                {
                    if (request.Value == null)
                        Manager.EventQueue.AddRequest(request.Key);
                    else if (request.Value.IsCompleted)
                        Manager.EventQueue.Add(request.Value.completedEvent, true);
                }
            };
        }

        public void Add(Request request)
        {
            _requests[request.guild] = request;
            displays[request.guild].Request = request;
            request.Init();
            request.Start();
        }

        public void Remove(Guild guild)
        {
            _requests[guild].Complete();
            Manager.Upgrades.GuildTokens[guild]++;
            _requests[guild] = null;
            displays[guild].Request = null;
        }
        
        public Dictionary<Guild, RequestDetails> Save()
        {
            return _requests
                .Where(request => request.Value)
                .ToDictionary(request => request.Key, request => new RequestDetails
                {
                    name = request.Value.name, 
                    completed = request.Value.Completed,
                    required = request.Value.Required
                });
        }

        public async Task Load(Dictionary<Guild, RequestDetails> requests)
        {
            foreach (Guild guild in Enum.GetValues(typeof(Guild)))
            {
                _requests.Add(guild, null);
            }
            foreach (KeyValuePair<Guild, RequestDetails> request in requests)
            {
                _requests[request.Key] = await Addressables.LoadAssetAsync<Request>(request.Value.name).Task;
                _requests[request.Key].Completed = request.Value.completed;
                _requests[request.Key].Required = request.Value.required;
                _requests[request.Key].Start();
                displays[request.Key].Request = _requests[request.Key];
            }
        }
    }
}
