using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using BBRAPIModules;

namespace BattleBitAPIRunner
{
  internal class RunnerRestServer
  {
    private List<RunnerServer> servers = new();

    public RunnerRestServer(List<RunnerServer> servers)
    {
      this.servers = servers;
    }

    public void Initialize()
    {
      Task.Run(() =>
            {
              WebApplicationBuilder builder = WebApplication.CreateBuilder(new string[] { });
              builder.Logging.ClearProviders();
              WebApplication app = builder.Build();

              InitializeEndpoints(app);
              app.UseDeveloperExceptionPage();

              app.RunAsync("http://*:8080");
              Console.WriteLine("Started REST server on port 8080");
            });
    }

    private IResult GetRunnerServers()
    {
      List<RunnerServerDTO> serverDTOs = this.servers.Select(GetServerDTO).ToList();
      return Results.Ok(serverDTOs);
    }

    private IResult GetServer(ulong serverId)
    {
      RunnerServer? foundServer = FindServerByHash(serverId);
      return (foundServer != null)
          ? Results.Ok(GetServerDTO(foundServer))
          : ServerNotFoundResponse(serverId);
    }

    private IResult GetServerModules(ulong serverId)
    {
      RunnerServer? foundServer = FindServerByHash(serverId);
      return (foundServer != null)
          ? Results.Ok(foundServer.GetModules())
          : ServerNotFoundResponse(serverId);
    }

    private IResult GetServerModulesById(ulong serverId, string id)
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

    private IResult GetServerModulesByName(ulong serverId, string name)
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

    private IResult GetModules()
    {
      return Results.Ok(Module.Modules);
    }

    private IResult GetModuleById(string moduleId)
    {
      Module? foundModule = this.FindModuleById(moduleId.ToString());

      if (foundModule == null)
      {
        return ModuleNotFoundResponse($"[id:{moduleId}]");
      }

      return Results.Ok(foundModule);
    }

    private void InitializeEndpoints(WebApplication app)
    {
      app.UseRouting();

      app.MapGet("/api/servers", GetRunnerServers);
      app.MapGet("/api/servers/{serverId}", GetServer);
      app.MapGet("/api/servers/{serverId}/modules", GetServerModules);
      // app.MapGet("/api/servers/{serverId}/modules/{name:alpha}", GetServerModulesByName);
      app.MapGet("/api/servers/{serverId}/modules/{moduleId}", GetServerModulesById);
      app.MapGet("/api/modules/", GetModules);
      app.MapGet("/api/modules/{moduleId}", GetModuleById);
      app.MapFallback(() =>
      {
        return "{ \"error\": 404 }";
      });
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

    // These would be inside a service class but I'm new to C#

    private RunnerServer? FindServerByHash(ulong serverId)
    {
      return this.servers.Find(ser => ser.ServerHash == serverId);
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
  }

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