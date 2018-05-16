using PluginBot;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BaseRoles
{
    public class Werewolf : Role
    {
        public override string Name => "Werewolf";

        public override async Task GameStart()
        {
            string s = "Werewolf:";

            foreach (var v in this.Players)
            {
                s += $"{Environment.NewLine}{v.Member.DisplayName}({v.DiscordEmoji.GetDiscordName()})";
            }

            await ChannelManager.StoryTime.SendMessageAsync(s);
        }

        public override async Task DayStart()
        {
            await ChannelManager.StoryTime.SendMessageAsync("Werewolf - Day Start");
        }

        public override async Task NightStart()
        {
            await ChannelManager.StoryTime.SendMessageAsync("Werewolf - Night Start");
        }
    }
}
