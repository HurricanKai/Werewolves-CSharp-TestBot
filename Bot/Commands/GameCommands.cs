using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using PluginBot.Attributes;
using PluginBot;
using System.Threading.Tasks;
using System.Linq;

namespace PluginBot.Commands
{
    public class GameCommands : BaseCommandModule
    {
        [Command("State")]
        [RequireGameMaster]
        public async Task StateCMD(CommandContext ctx)
        {
            await ctx.RespondAsync(GameManager.State.ToString());
        }

        [Command("Signup")]
        [RequireGameState(GameState.Signups)]
        [RequireGameMaster]
        public async Task SignupCMD(CommandContext ctx, DiscordMember member, DiscordEmoji avatar)
        {
            if (GameManager.Players.Any(x => x.Id == member.Id))
            {
                await ctx.RespondAsync("You are already Signedup");
                return;
            }
            if (GameManager.Players.Any(x => x.DiscordEmoji == avatar))
            {
                await ctx.RespondAsync("Someone else already has this Emoji");
                return;
            }

            var worked = GameManager.Signup(member, avatar);
            if (!worked)
                await ctx.RespondAsync("Coudnt Signup");
            else
                await ctx.RespondAsync("You are Signuped!");
        }

        [Command("Signup")]
        [RequireGameState(GameState.Signups)]
        public Task SignupCMD(CommandContext ctx, DiscordEmoji avatar)
            => SignupCMD(ctx, ctx.Member, avatar);

        [Command("Setup")]
        [RequireGameMaster]
        public async Task SetMainGuildCMD(CommandContext ctx)
        {
            GameManager.MainGuild = ctx.Guild;

            await ctx.RespondAsync("Great! everything setup!");
        }

        [Command("MoveNextState")]
        [RequireGameMaster]
        public async Task MoveNextState(CommandContext ctx)
        {
            //TODO: Setup-Checks, so last state needs to be setuped first.
            GameManager.MoveNext();
            await GameManager.StateSetup(GameManager.State, ctx);
            await ctx.RespondAsync("New State: " + GameManager.State.ToString());
        }
    }
}