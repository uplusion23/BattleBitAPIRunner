using BBRAPIModules;

namespace BattleBitAPIRunner
{
  public class RunnerServerDTO
  {
    public ulong ServerHash { get; set; }
    public bool IsConnected { get; set; }

    public bool HasActiveConnectionSession { get; set; }

    public string GameIP { get; set; }

    public int GamePort { get; set; }

    public bool IsPasswordProtected { get; set; }

    public string ServerName { get; set; }

    public string Gamemode { get; set; }

    public string Map { get; set; }

    public string MapSize { get; set; }

    public List<BattleBitModule> Modules { get; set; }

    public bool IsNight { get; set; }

    public int CurrentPlayerCount { get; set; }

    public int InQueuePlayerCount { get; set; }

    public int MaxPlayerCount { get; set; }

    public string LoadingScreenText { get; set; }
  }
}