using DSharpPlus.Entities;

namespace PluginBot
{
    public class Player
    {
        public ulong Id { get; set; }
        public DiscordMember Member => ChannelManager.MainGuild.GetMemberAsync(Id).Result;
        private string _DiscordEmoji { get; set; }
        public DiscordEmoji DiscordEmoji
        {
            get
                => DiscordEmoji.FromName(Bot._client, _DiscordEmoji);
            set
                => _DiscordEmoji = value.GetDiscordName();
        }
        public IRole Role { get; set; }
        public bool Alive { get; set; }

        public Player()
        {
            Alive = true;
        }
    }
}