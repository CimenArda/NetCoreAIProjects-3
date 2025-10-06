using ModelContextProtocol.Server;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MCPServer;

[McpServerToolType]
public class UserTool(IHttpClientFactory httpClientFactory)
{
    [McpServerTool, Description("Tüm kullanıcıları, bilgileriyle birlikte getirir.")]
    public async Task<List<object>> GetUsersAsync()
    {
        Log.Information("JSONPlaceholder'dan kullanıcı bilgileri çekiliyor...");

        HttpClient httpClient = httpClientFactory.CreateClient();
        HttpResponseMessage httpResponseMessage = await httpClient.GetAsync("https://jsonplaceholder.typicode.com/users");
        string jsonData = await httpResponseMessage.Content.ReadAsStringAsync();

        JsonDocument document = JsonDocument.Parse(jsonData);
        JsonElement root = document.RootElement;

        List<object> usersData = new();

        foreach (JsonElement element in root.EnumerateArray())
        {
            usersData.Add(new
            {
                Id = element.GetProperty("id").GetInt32(),
                Name = element.GetProperty("name").GetString(),
                Username = element.GetProperty("username").GetString(),
                Email = element.GetProperty("email").GetString()
            });
        }

        Log.Information("JSONPlaceholder'dan kullanıcı bilgileri çekildi...");
        return usersData;
    }
}
