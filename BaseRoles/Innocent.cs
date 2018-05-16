using PluginBot;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BaseRoles
{
    public class Innocent : Role
    {
        public override string Name => "Innocent";
        private Poll poll { get; set; }
        public override async Task GameStart()
        {
            string s = "Innocent:";

            foreach (var v in this.Players)
            {
                s += $"{Environment.NewLine}{v.Member.DisplayName}({v.DiscordEmoji.GetDiscordName()})";
            }

            await ChannelManager.StoryTime.SendMessageAsync(s);
        }

        public override async Task DayStart()
        {
            var rip = await poll.GetWinner();
            Kill(rip);
            await ChannelManager.StoryTime.SendMessageAsync($"{rip.Member.DisplayName} is Fucking rip rip");
            await ChannelManager.StoryTime.SendMessageAsync("Innocent - Day Start");
            poll = await CreatePoll("Lynch", "Innocent Lynch, no Good Description");
        }

        public override async Task NightStart()
        {
            await ChannelManager.StoryTime.SendMessageAsync("Innocent - Night Start");
        }
    }
}
