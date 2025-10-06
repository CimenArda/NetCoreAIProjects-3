using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol.Transport;
using ModelContextProtocol.Protocol.Types;

namespace MCPClientt;

public class GoogleGeminiAIMCPClient(string MCPServerName, string MCPServerPath)
{
    private IMcpClient _mcpClient;
    public async Task<IMcpClient> GetClientAsync()
    {
        if (_mcpClient == null)
            _mcpClient = await McpClientFactory.CreateAsync(
                     clientTransport: new StdioClientTransport(new()
                     {
                         Name = MCPServerName,
                         Command = MCPServerPath
                     }),
                     clientOptions: new McpClientOptions()
                     {
                         ClientInfo = new Implementation()
                         {
                             Name = "Google.Gemini.AI.MCP.Client",
                             Version = "1.0.0"
                         }
                     }
                 );

        return _mcpClient;
    }

    public async Task<IList<McpClientTool>> GetToolListAsync()
    {
        _mcpClient = await GetClientAsync();
        IList<McpClientTool> tools = await _mcpClient.ListToolsAsync();
        return tools;
    }

    public async Task<object> CallToolAsync(string toolName, Dictionary<string, object?> input)
    {
        _mcpClient = await GetClientAsync();
        CallToolResponse response = await _mcpClient.CallToolAsync(
                   toolName,
                   input
                   );
        return response.Content.First(c => c.Type == "text").Text;
    }
}
