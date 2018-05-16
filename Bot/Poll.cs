using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.Entities;

namespace PluginBot
{
    public class Poll
    {
        List<DiscordMessage> messages = new List<DiscordMessage>();
        public void AddMessage(DiscordMessage msg)
        {
            messages.Add(msg);
        }

        public async Task<Player> GetWinner()
        {
            int mostCount = int.MinValue;
            DiscordEmoji min = null;
            foreach (var v in messages)
            {
                foreach (var v2 in v.Reactions)
                {
                    await v.DeleteOwnReactionAsync(v2.Emoji);
                }
                foreach (var v2 in v.Reactions)
                {
                    if (v2.Count > mostCount)
                    {
                        mostCount = v2.Count;
                        min = v2.Emoji;
                    }
                }
            }
            return GameManager.Players.Where(x => x.DiscordEmoji.GetDiscordName() == min.GetDiscordName()).Single();
        }
    }
}