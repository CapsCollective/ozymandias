using System;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using Utilities;

namespace GuildFavours
{
    public class Favours: MonoBehaviour
    {
        public Dictionary<Guild, int> GuildTokens { get; private set; }

        public FavourDetails Save()
        {
            return new FavourDetails
            {
                guildTokens = GuildTokens
            };
        }
        
        public void Load(FavourDetails details)
        {
            GuildTokens = details.guildTokens ?? new Dictionary<Guild, int>();
            foreach (Guild guild in Enum.GetValues(typeof(Guild)))
            {
                if (!GuildTokens.ContainsKey(guild)) GuildTokens.Add(guild, 0);
            }
        }
    }
}
