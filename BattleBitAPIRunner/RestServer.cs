using System.Linq;
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
      return Results.Ok(this.servers);
    }

    private IResult GetServer(ulong serverId)
    {
      RunnerServer? foundServer = FindServerByHash(serverId);
      return (foundServer != null)
          ? Results.Ok(foundServer)
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

      BattleBitModule module = FindModuleById(foundServer, id);
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

      BattleBitModule module = FindModuleByName(foundServer, name);
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

    private void InitializeEndpoints(WebApplication app)
    {
      app.UseRouting();

      app.MapGet("/api/servers", GetRunnerServers);
      app.MapGet("/api/servers/{serverId}", GetServer);
      app.MapGet("/api/servers/{serverId}/modules", GetServerModules);
      // app.MapGet("/api/servers/{serverId}/modules/{name:alpha}", GetServerModulesByName);
      app.MapGet("/api/servers/{serverId}/modules/{moduleId}", GetServerModulesById);
      app.MapGet("/api/modules/", GetModules);
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

    private BattleBitModule FindModuleById(RunnerServer server, string moduleId)
    {
      return server.GetModules().First(module => (module.Name != null) && module.Id.Contains(moduleId));
    }

    private BattleBitModule FindModuleByName(RunnerServer server, string moduleName)
    {
      return server.GetModules().First(module => (module.Name != null) && module.Name.Contains(moduleName));
    }
  }
}