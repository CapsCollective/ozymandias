using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using Requests.Templates;
using UI;
using UnityEngine;
using Utilities;
using static Managers.GameManager;

namespace Requests
{
    public class Requests: MonoBehaviour
    {
        public static Action<Guild> OnRequestCompleted;
        
        [SerializeField] private SerializedDictionary<Guild, RequestDisplay> displays;
        private readonly Dictionary<Guild, Request> _requests = new Dictionary<Guild, Request>();

        private void Awake()
        {
            State.OnNextTurnBegin += () =>
            {
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
            Manager.Stats.RequestsCompleted++;
            Manager.Upgrades.GuildTokens[guild] += _requests[guild].Tokens;
            int purchasable = Manager.Upgrades.TotalPurchasable;
            if (purchasable > 0) Manager.Notifications.Display(
                $"You have {purchasable} upgrades available for purchase", 
                delay: 5f,
                onClick: () => Manager.Book.Open(Book.BookPage.Upgrades)
            );
            Manager.Upgrades.Display();
            _requests[guild] = null;
            displays[guild].Request = null;
            OnRequestCompleted?.Invoke(guild);
        }

        public int TokenCount(Guild guild)
        {
            return _requests[guild].Tokens;
        }

        public bool HasRequest(Guild guild)
        {
            return _requests[guild] != null;
        }
        
        public Dictionary<Guild, RequestDetails> Save()
        {
            return _requests
                .Where(request => request.Value)
                .ToDictionary(request => request.Key, request => new RequestDetails
                {
                    name = request.Value.name, 
                    completed = request.Value.Completed,
                    required = request.Value.Required,
                    tokens = request.Value.Tokens
                });
        }

        public void Load(Dictionary<Guild, RequestDetails> requests)
        {
            foreach (Guild guild in Enum.GetValues(typeof(Guild)))
            {
                _requests.Add(guild, null);
            }
            foreach (KeyValuePair<Guild, RequestDetails> request in requests ?? new Dictionary<Guild, RequestDetails>())
            {
                _requests[request.Key] = Manager.AllRequests.Find(match => request.Value.name == match.name);
                if (_requests[request.Key]  == null)
                {
                    Debug.LogWarning("Requests: Cannot find " + request.Value.name);
                    continue;
                }
                
                _requests[request.Key].Completed = request.Value.completed;
                _requests[request.Key].Required = request.Value.required;
                _requests[request.Key].Tokens = request.Value.tokens;
                _requests[request.Key].Start();
                displays[request.Key].Request = _requests[request.Key];
            }
        }
    }
}
