using BasicAIAgentExample.Services;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BasicAIAgentExample.Plugins
{
    public class PostPlugin(IHttpClientFactory httpClientFactory)
    {
        [KernelFunction, Description("Tüm postları elde eder.")]
        public async Task<object> GetAllPostsAsync()
        {
            Logger.ConsoleLog("Tüm postlar çekiliyor!");

            var httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync("https://jsonplaceholder.typicode.com/posts");
            string jsonData = await response.Content.ReadAsStringAsync();

            var document = JsonDocument.Parse(jsonData);
            var root = document.RootElement;

            var postDatas = new List<object>();

            foreach (var element in root.EnumerateArray())
            {
                postDatas.Add(new
                {
                    UserId = element.GetProperty("userId").GetInt32(),
                    Id = element.GetProperty("id").GetInt32(),
                    Title = element.GetProperty("title").GetString(),
                    Body = element.GetProperty("body").GetString()
                });
            }

            Logger.ConsoleLog("Tüm postlar çekildi.");
            return postDatas;
        }

        [KernelFunction, Description("Id değeri verilen postu elde eder.")]
        public async Task<object> GetPostsByIdAsync([Description("Elde edilecek postun id değeri.")] int postId)
        {
            Logger.ConsoleLog($"'{postId}' id değerli post çekiliyor!");

            var httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync($"https://jsonplaceholder.typicode.com/posts/{postId}");
            string jsonData = await response.Content.ReadAsStringAsync();

            var document = JsonDocument.Parse(jsonData);
            var root = document.RootElement;

            Logger.ConsoleLog($"'{postId}' id değerli post çekildi!");
            return new
            {
                UserId = root.GetProperty("userId").GetInt32(),
                Id = root.GetProperty("id").GetInt32(),
                Title = root.GetProperty("title").GetString(),
                Body = root.GetProperty("body").GetString()
            };
        }

        [KernelFunction, Description("Id değeri verilen postun tüm yorumlarını elde eder.")]
        public async Task<object> GetCommentsForPostAsync([Description("Yorumları elde edilecek postun id değeri.")] int postId)
        {
            Logger.ConsoleLog($"'{postId}' id değerli postun yorumları çekiliyor!");

            var httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.GetAsync($"https://jsonplaceholder.typicode.com/posts/{postId}/comments");
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

            Logger.ConsoleLog($"'{postId}' id değerli postun yorumları çekildi!");
            return commentDatas;
        }
    }
}
