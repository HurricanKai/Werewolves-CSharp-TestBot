using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using PluginBot;
using System;
using System.Threading.Tasks;

namespace PluginBot.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class RequireGameStateAttribute : CheckBaseAttribute
    {
        public RequireGameStateAttribute(GameState state)
        {
            this.State = state;
        }

        public GameState State { get; }

        public override async Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
        {
            return GameManager.State == State;
        }
    }
}