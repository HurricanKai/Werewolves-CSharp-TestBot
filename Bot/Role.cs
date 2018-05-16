using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PluginBot
{
    public abstract class Role : IRole
    {
        public abstract string Name { get; }
        public List<Player> Players { get; set; }
        public virtual bool WinCheck() { return false; }
        public virtual async Task PreGame() { }
        public virtual async Task PostGame() { }
        public virtual async Task DayStart() { }
        public virtual async Task NightStart() { }
        public virtual async Task GameStart() { }

        public static async Task<Poll> CreatePoll(string title, string description, Predicate<Player> predicate = null)
        {
            var poll = new Poll();
            if (predicate == null)
                predicate = player => true;
            var builder = new DiscordEmbedBuilder();
            builder.WithTitle(title);
            builder.WithDescription(description);
            builder.WithFooter("Presented by HurricanKai ^^");
            int i = 0;
            List<Player> playersToReact = new List<Player>();
            foreach (var player in GameManager.AlivePlayers)
            {
                if (predicate(player))
                {
                    builder.AddField(player.DiscordEmoji.ToString(), player.Member.DisplayName);
                    i++;
                    playersToReact.Add(player);
                    if (i == 20)
                    {
                        var msg = await ChannelManager.StoryTime.SendMessageAsync(embed: builder);
                        i = 0;
                        builder.Title = "";
                        builder.Description = "";
                        builder.ClearFields();
                        foreach (var p in playersToReact)
                            await msg.CreateReactionAsync(p.DiscordEmoji);
                        playersToReact = new List<Player>();
                        poll.AddMessage(msg);
                    }
                }
            }
            if (i != 0)
            {
                var msg = await ChannelManager.StoryTime.SendMessageAsync(embed: builder);
                i = 0;
                builder.Title = "";
                builder.Description = "";
                builder.ClearFields();
                foreach (var p in playersToReact)
                    await msg.CreateReactionAsync(p.DiscordEmoji);
                playersToReact = new List<Player>();
                poll.AddMessage(msg);
            }
            return poll;
        }

        public static void Kill(Player p)
            => GameManager.Kill(p);
    }
}
