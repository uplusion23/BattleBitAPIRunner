using System.Text.Json;
using BattleBitAPI;
using BBRAPIModules;

namespace BattleBitAPIRunner
{
  public class ServerService : IServerService
  {
    private readonly List<RunnerServer> _servers;
    private readonly ILogger<ServerService> _logger;

    public ServerService(List<RunnerServer> servers, ILogger<ServerService> logger)
    {
      _servers = servers;
      _logger = logger;
    }

    public IResult GetRunnerServers()
    {
      List<RunnerServerDTO> serverDTOs = this._servers.Select(GetServerDTO).ToList();
      return Results.Ok(serverDTOs);
    }

    public IResult GetServer(ulong serverId)
    {
      RunnerServer? foundServer = FindServerByHash(serverId);
      return (foundServer != null)
          ? Results.Ok(GetServerDTO(foundServer))
          : ServerNotFoundResponse(serverId);
    }

    public IResult GetServerModules(ulong serverId)
    {
      RunnerServer? foundServer = FindServerByHash(serverId);
      return (foundServer != null)
          ? Results.Ok(foundServer.GetModules())
          : ServerNotFoundResponse(serverId);
    }

    public IResult GetServerModulesById(ulong serverId, string id)
    {
      RunnerServer? foundServer = FindServerByHash(serverId);

      if (foundServer == null)
      {
        return ServerNotFoundResponse(serverId);
      }

      BattleBitModule? module = FindModuleById(foundServer, id);
      if (module == null)
      {
        return ModuleNotFoundResponse($"[id:${id}]");
      }
      return Results.Ok(module);
    }

    public IResult GetServerModulesByName(ulong serverId, string name)
    {
      RunnerServer? foundServer = FindServerByHash(serverId);

      if (foundServer == null)
      {
        return ServerNotFoundResponse(serverId);
      }

      BattleBitModule? module = FindModuleByName(foundServer, name);
      if (module == null)
      {
        return ModuleNotFoundResponse($"[name:${name}]");
      }
      return Results.Ok(module);
    }

    public IResult GetModules()
    {
      return Results.Ok(Module.Modules);
    }

    public IResult GetModuleById(string moduleId)
    {
      Module? foundModule = this.FindModuleById(moduleId.ToString());

      if (foundModule == null)
      {
        return ModuleNotFoundResponse($"[id:{moduleId}]");
      }

      return Results.Ok(foundModule);
    }

    public IResult GetModulesByServer(RunnerServer server)
    {
      List<Module> modules = new();
      // Module.Modules.All(module => module)
      return Results.NoContent();
    }

    public IResult GetPlayersByServer(ulong serverId)
    {
      return Results.Ok(FindPlayersByServerId(serverId));
    }

    private RunnerServer? FindServerByHash(ulong serverId)
    {
      return this._servers.Find(ser => ser.ServerHash == serverId);
    }

    private Module? FindModuleById(string moduleId)
    {
      // Refactor Program.cs to globally list BattleBitModules, so that way the getmodulebyid can be more specific and include server, isloaded, etc.
      return Module.Modules.FirstOrDefault(module => module.Id == moduleId);
    }

    private BattleBitModule? FindModuleById(RunnerServer server, string moduleId)
    {
      return server.GetModules().FirstOrDefault(module => (module.Name != null) && module.Id.Contains(moduleId));
    }

    private BattleBitModule? FindModuleByName(RunnerServer server, string moduleName)
    {
      return server.GetModules().FirstOrDefault(module => (module.Name != null) && module.Name.Contains(moduleName));
    }

    private List<RunnerPlayerDTO> FindPlayersByServerId(ulong serverId)
    {
      RunnerServer? server = FindServerByHash(serverId);
      if (server == null)
      {
        return new List<RunnerPlayerDTO>();
      }
      List<RunnerPlayer> players = server.AllPlayers.ToList();
      return players.Select(player => GetPlayerDTO(player)).ToList();
    }

    private RunnerServerDTO GetServerDTO(RunnerServer server)
    {
      Console.WriteLine(JsonSerializer.Serialize(server.GetModules()).ToString());
      return new RunnerServerDTO
      {
        ServerHash = server.ServerHash,
        IsConnected = server.IsConnected,
        GameIP = server.GameIP.ToString(),
        GamePort = server.GamePort,
        IsPasswordProtected = server.IsPasswordProtected,
        ServerName = server.ServerName,
        Gamemode = server.Gamemode,
        Map = server.Map,
        MapSize = server.MapSize.ToString().Replace("_", ""),
        Modules = server.GetModules(),
        IsNight = server.DayNight.ToString() == "Night",
        CurrentPlayerCount = server.CurrentPlayerCount,
        InQueuePlayerCount = server.InQueuePlayerCount,
        MaxPlayerCount = server.MaxPlayerCount,
        LoadingScreenText = server.LoadingScreenText
      };
    }

    private RunnerPlayerDTO GetPlayerDTO(RunnerPlayer player)
    {
      return new RunnerPlayerDTO
      {
        SteamID = player.SteamID,
        Name = player.Name,
        IPAddress = player.IP.ToString(),
        GameRole = player.Role.ToString(),
        Team = player.Team.ToString(),
        Squad = player.SquadName.ToString(),
        IsSquadLeader = player.IsSquadLeader,
        PingMs = player.PingMs,
        SessionID = player.CurrentSessionID,
        IsAlive = player.IsAlive,
        Health = player.HP,
        Position = player.Position,
        InVehicle = player.InVehicle,
        IsBleeding = player.IsBleeding
      };
    }

    private IResult ServerNotFoundResponse(ulong serverId)
    {
      return Results.Json(new
      {
        message = $"Server by hash '{serverId}' not found."
      }, null, null, 404);
    }

    private IResult ModuleNotFoundResponse(string moduleName)
    {
      return Results.Json(new
      {
        message = $"Module '{moduleName}' not found."
      }, null, null, 404);
    }
  }
}