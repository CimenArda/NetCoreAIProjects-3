using MCPServer.SemanticKernel.Extensions;
using MCPServer.SemanticKernel.Plugins;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.SemanticKernel;
using Serilog;


Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Verbose()
    .WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "MCP.Server.Log"))
    .WriteTo.Debug()
    .WriteTo.Console()
    .CreateLogger();

Log.Information("Server başlatılıyor...");

var builder = Host.CreateApplicationBuilder(args);

builder.Services
.AddKernel()
.Plugins.AddFromType<ToDoPlugin>();

builder.Services.AddHttpClient();

builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly()
    .WithTools(builder.Services.BuildServiceProvider().GetService<Kernel>().Plugins);

await builder.Build().RunAsync();