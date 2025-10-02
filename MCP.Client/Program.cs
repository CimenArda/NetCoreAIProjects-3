using ModelContextProtocol;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol.Transport;

Console.WriteLine("MCP sunucusuna bağlantı sağlanıyor...");

IMcpClient client = await McpClientFactory.CreateAsync(serverConfig: new McpServerConfig()
{
    Id = "MCPServer",
    Name = "MCP Server",
    TransportType = TransportTypes.StdIo,
    TransportOptions = new()
    {
        ["command"] = "NetYapayZekaUygulamaVeNotlar\\NetCoreAIProjects-3\\MCP.Server\\bin\\Debug\\net8.0\\MCP.Server.exe"

    }
});

Console.WriteLine("MCP sunucusuna bağlantı sağlandı...");

var tools = await client.ListToolsAsync();
foreach (var tool in tools.Select((tool, index) => new { Tool = tool, Index = index }))
{
    Console.WriteLine($"\tTool {tool.Index + 1} : {tool.Tool.Name} ({tool.Tool.Description})");
}

var result = await client.CallToolAsync(
    "echo",
    new Dictionary<string, object?>() { ["message"] = "Selaaaamm,Nasılsın??" });

Console.WriteLine($"Result : {result.Content.First(c => c.Type == "text").Text}");

Console.Read();
await client.DisposeAsync();