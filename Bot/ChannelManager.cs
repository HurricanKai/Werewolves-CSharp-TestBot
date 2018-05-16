using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PluginBot
{
    public class ChannelManager
    {
        public static DiscordChannel StoryTime { get; set; }
        public static DiscordGuild MainGuild { get; set; }


        public static async Task Setup()
        {
            MainGuild = await Bot._client.GetGuildAsync(442678942428561422);

            StoryTime = MainGuild.GetChannel(444573263532916767);
        }
    }
}
