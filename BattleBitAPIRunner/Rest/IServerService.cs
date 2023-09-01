

using BBRAPIModules;

namespace BattleBitAPIRunner
{
  public interface IServerService
  {
    IResult GetRunnerServers();
    IResult GetServer(ulong serverId);
    IResult GetServerModules(ulong serverId);
    IResult GetServerModulesById(ulong serverId, string id);
    IResult GetServerModulesByName(ulong serverId, string name);
    IResult GetModules();
    IResult GetModuleById(string moduleId);
    IResult GetModulesByServer(RunnerServer server);
    IResult GetPlayersByServer(ulong serverId);
  }
}