using PluginBot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PluginBot
{
    public static class RoleManager
    {
        public const string path = @"\Roles\";
        public static List<IRole> Roles { get; set; }

        static RoleManager()
        {
            Roles = new List<IRole>();
        }

        public static void Load()
        {
            var Base = typeof(IRole);
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            var p = Path.GetDirectoryName(path) + RoleManager.path;
            foreach (var v in Directory.EnumerateFiles(p))
            {
                var assembly = Assembly.LoadFile(v);
                if (assembly != null)
                {
                    foreach(var type in assembly.GetTypes())
                    {
                        if (Base.IsAssignableFrom(type))
                        {
                            Roles.Add((IRole)Activator.CreateInstance(type));
                        }
                    }
                }
            }
        }

        public static void PostGame()
        {
            Roles.ForEach(x => x.PostGame());
        }

        public static void StartGame()
        {
            for (int i = 0; i < Roles.Count; i++)
            {
                Roles[i].Players = (GameManager.Players.Where(x => x.Role.Name == Roles[i].Name)).ToList();
            }
            Roles.ForEach(x => x.GameStart());
        }

        public static void PreGame()
        {
            Roles.ForEach(x => x.PreGame());
        }

        public static void DayStart()
        {
            Roles.ForEach(x => x.DayStart());
            CheckWins();
        }

        public static void NightStart()
        {
            Roles.ForEach(x => x.NightStart());
            CheckWins();
        }

        private static void CheckWins()
        {
            var winners = (List<IRole>)Roles.Where(x => x.WinCheck());
            if (winners.Count() != 0)
            {
                GameManager.MakeWinners(winners);
            }
        }
    }
}