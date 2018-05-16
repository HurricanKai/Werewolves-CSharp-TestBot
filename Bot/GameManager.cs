using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using PluginBot;

namespace PluginBot
{
    public class GameManager
    {
        public static GameState State { get; set; }
        public static List<Player> Players { get; set; }
        public static List<Player> AlivePlayers { get { return Players.Where(x => x.Alive).ToList(); } }
        public static List<Player> DeadPlayers { get { return Players.Where(x => !x.Alive).ToList(); } }
        public static DiscordGuild MainGuild { get; set; }
        public static GameConfig Config { get; private set; }
        private static Dictionary<IRole, int> Roles { get; set; }

        static GameManager()
        {
            Init();
        }

        private static void Init()
        {
            Players = new List<Player>();
            State = GameState.Idle;
            Roles = new Dictionary<IRole, int>();
            Config = new GameConfig();
        }

        public static bool Signup(DiscordMember member, DiscordEmoji avatar)
        {
            try
            {
                Players.Add(new Player()
                {
                    DiscordEmoji = avatar,
                    Id = member.Id,
                });
                return true;
            }
            catch
            { return false; }
        }

        public static void MoveNext()
        {
            switch (State)
            {
                case GameState.Idle:
                    State = GameState.Signups;
                    break;
                case GameState.Signups:
                    State = GameState.PreGame;
                    RoleManager.PreGame();
                    break;
                case GameState.PreGame:
                    State = GameState.InGame;
                    RoleManager.StartGame();
                    TimeManager.OnDayStart += (sender, e) => { RoleManager.DayStart(); };
                    TimeManager.OnNightStart += (sender, e) => { RoleManager.NightStart(); };
                    TimeManager.Start();
                    break;
                case GameState.InGame:
                    State = GameState.PostGame;
                    TimeManager.Stop();
                    TimeManager.OnDayStart -= (sender, e) => { RoleManager.DayStart(); };
                    TimeManager.OnNightStart -= (sender, e) => { RoleManager.NightStart(); };
                    RoleManager.PostGame();
                    break;
                case GameState.PostGame:
                    State = GameState.Idle;
                    break;
            }
        }

        public static void Kill(Player p)
        {
            for (int i = 0; i < Players.Count; i++)
            {
                if (Players[i].Id == p.Id)
                    Players[i].Alive = false;
            }
        }

        public static async Task StateSetup(GameState state, CommandContext ctx)
        {
            switch (state)
            {
                case GameState.Idle:
                    //Initial Setup
                    Init();
                    break;
                case GameState.Signups:
                    //Max Players
                    await ctx.RespondAsync("Please enter the Maximum amount of Players:");
                    int value = 0;
                    while (value == 0)
                        await Bot._interactivity.WaitForMessageAsync(x => Int32.TryParse(x.Content, out value));
                    GameManager.Config.MaxPlayers = value;
                    break;
                case GameState.PreGame:
                    bool Failed = true;
                    while (Failed)
                    {
                        Failed = false;
                        //Role Selection
                        await ctx.RespondAsync("Please enter all Roles you want, in format: RoleName,Count|RoleName,Count ...");
                        var msg1 = await Bot._interactivity.WaitForMessageAsync(x => true);
                        var Roles = msg1.Message.Content.Split('|');
                        foreach (var role in Roles)
                        {
                            var components = role.Split(',');
                            var RoleName = components[0];
                            var Worked = Int32.TryParse(components[1], out int RoleCount);
                            if (!Worked)
                            {
                                Failed = true;
                                break;
                            }

                            var RoleToAdd = RoleManager.Roles.First(x => x.Name == RoleName);
                            if (GameManager.Roles.TryGetValue(RoleToAdd, out int OldCount))
                            {
                                GameManager.Roles.Remove(RoleToAdd);
                                GameManager.Roles.Add(RoleToAdd, OldCount + RoleCount);
                            }
                            else
                                GameManager.Roles.Add(RoleToAdd, RoleCount);
                        }
                    }

                    List<Player> TODO = GameManager.Players;
                    List<Player> Done = new List<Player>();
                    Random random = new Random((int)DateTime.Now.ToBinary());
                    foreach (var v in GameManager.Roles)
                    {
                        for (int i = 0; i < v.Value; i++)
                        {
                            if (TODO.Count == 0)
                            {
                                await ctx.RespondAsync("Im sorry, but this is not possible?");
                                GameManager.State = GameState.Idle;
                                await StateSetup(GameState.Idle, ctx);
                                return;
                            }

                            var t = TODO[random.Next(TODO.Count)];
                            TODO.Remove(t);
                            t.Role = v.Key;
                            Done.Add(t);
                        }
                    }
                    GameManager.Players = Done;
                    break;
                case GameState.InGame:
                    //I dont think anything?
                    await ctx.RespondAsync("eyyyy, you managed to start the game");
                    break;
                case GameState.PostGame:
                    //Store stuff
                    break;
            }
            await ctx.RespondAsync("new state: " + GameManager.State);
        }

        public static void MakeWinners(List<IRole> winners)
        {
            if (State != GameState.InGame)
                throw new NotSupportedException("Winning outside of InGame, nice try ~Kai (If this happens, please *Instantly* Call me)");
            MoveNext();
            //TODO: Winner Logic!
        }
    }
}