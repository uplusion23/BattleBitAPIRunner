using System.Numerics;
using BBRAPIModules;

namespace BattleBitAPIRunner
{
  public class RunnerPlayerDTO
  {
    public ulong SteamID { get; set; }
    public string Name { get; set; }
    public string IPAddress { get; set; }
    public string GameRole { get; set; }
    public string Team { get; set; }
    public string Squad { get; set; }
    public bool IsSquadLeader { get; set; }
    public int PingMs { get; set; }
    public long SessionID { get; set; }
    public bool IsAlive { get; set; }
    public float Health { get; set; }
    public Vector3 Position { get; set; }
    public bool InVehicle { get; set; }
    public bool IsBleeding { get; set; }
  }
}