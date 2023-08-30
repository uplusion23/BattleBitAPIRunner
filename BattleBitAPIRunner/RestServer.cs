
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

              app.RunAsync("http://*:8080");
              Console.WriteLine("Started REST server on port 8080");
            });
    }

    private List<RunnerServer> GetRunnerServers()
    {
      return this.servers;
    }

    private void InitializeEndpoints(WebApplication app)
    {
      app.MapGet("/api/servers", GetRunnerServers);
      app.MapFallback(() =>
      {
        return "{ \"error\": 404 }";
      });
    }
  }
}