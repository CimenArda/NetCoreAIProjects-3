using BasicAIAgentExample.Services;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

using System.ComponentModel;

using System.Text.Json;

namespace BasicAIAgentExample.Plugins
{
    public class CommentPlugin(IHttpClientFactory httpClientFactory)
    {
        [KernelFunction, Description("Tüm yorumları elde eder.")]
        public async Task<object> GetAllCommentsAsync()
        {
            Logger.ConsoleLog("Tüm yorumlar çekiliyor!");

            var httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync("https://jsonplaceholder.typicode.com/comments");
            string jsonData = await response.Content.ReadAsStringAsync();

            var document = JsonDocument.Parse(jsonData);
            var root = document.RootElement;

            var commentDatas = new List<object>();

            foreach (var element in root.EnumerateArray())
            {
                commentDatas.Add(new
                {
                    PostId = element.GetProperty("postId").GetInt32(),
                    Id = element.GetProperty("id").GetInt32(),
                    Name = element.GetProperty("name").GetString(),
                    Email = element.GetProperty("email").GetString(),
                    Body = element.GetProperty("body").GetString()
                });
            }

            Logger.ConsoleLog("Tüm yorumlar çekildi.");
            return commentDatas;
        }
    }
}
