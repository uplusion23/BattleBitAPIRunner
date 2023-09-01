namespace BattleBitAPIRunner
{
  internal class RunnerRestServer
  {
    private readonly ILogger<RunnerRestServer> _logger;
    private readonly IServerService _serverService;

    public RunnerRestServer(ILogger<RunnerRestServer> logger, IServerService serverService)
    {
      _logger = logger;
      _serverService = serverService;
    }

    public async Task InitializeAsync()
    {
      WebApplicationBuilder builder = WebApplication.CreateBuilder(new string[] { });
      builder.Logging.ClearProviders();
      WebApplication app = builder.Build();

      app.UseRouting();
      app.UseDeveloperExceptionPage();
      InitializeEndpoints(app);

      await app.RunAsync("http://*:8080");
      _logger.LogInformation("Started REST server on port 8080");
    }

    private void InitializeEndpoints(WebApplication app)
    {
      app.MapGet("/api/servers", _serverService.GetRunnerServers);
      app.MapGet("/api/servers/{serverId}", _serverService.GetServer);
      app.MapGet("/api/servers/{serverId}/players", _serverService.GetPlayersByServer);
      app.MapGet("/api/servers/{serverId}/modules", _serverService.GetServerModules);
      app.MapGet("/api/servers/{serverId}/modules/{moduleId}", _serverService.GetServerModulesById);
      app.MapGet("/api/modules/", _serverService.GetModules);
      app.MapGet("/api/modules/{moduleId}", _serverService.GetModuleById);
      app.MapFallback(() =>
      {
        return "{ \"error\": 404 }";
      });
    }
  }
}