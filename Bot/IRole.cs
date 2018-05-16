using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PluginBot
{
    public interface IRole
    {
        string Name { get; }
        List<Player> Players { get; set; }
        bool WinCheck();
        Task PreGame();
        Task PostGame();
        Task DayStart();
        Task NightStart();
        Task GameStart();
    }
}
