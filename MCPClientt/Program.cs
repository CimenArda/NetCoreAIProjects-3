#define Example1
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel;
using ModelContextProtocol.Client;
using OpenAI;
using System.ClientModel;
using MCPClientt;
using Microsoft.SemanticKernel.Connectors.OpenAI;

var builder = WebApplication.CreateBuilder(args);

GoogleGeminiAIMCPClient mcpClient = new("MCP.Server", Path.Combine("-------", "MCP.Server", "bin", "Debug", "net8.0", "MCP.Server.exe"));

GoogleGeminiAIMCPClient mcpClient2 = new("MCPServer.SemanticKernel", Path.Combine("---------", "MCPServer.SemanticKernel", "bin", "Debug", "net8.0", "MCPServer.SemanticKernel.exe"));

IList<McpClientTool> tools = await mcpClient.GetToolListAsync();

foreach (var tool in await mcpClient2.GetToolListAsync())
    tools.Add(tool);

builder.Services
    .AddKernel()
    .AddOpenAIChatCompletion(
        modelId: "google/gemini-2.0-flash-exp:free",
        openAIClient: new OpenAIClient(
                credential: new ApiKeyCredential("*******************"),
                options: new OpenAIClientOptions
                {
                    Endpoint = new Uri("https://openrouter.ai/api/v1"),
                }
            )
    )
    .Plugins.AddFromFunctions("UserTool", tools.Select(_tool => _tool.AsKernelFunction()));

var app = builder.Build();

#if Example1
app.MapGet("/", async (IChatCompletionService chatCompletionService, Kernel kernel) =>
{
    ChatMessageContent content = await chatCompletionService.GetChatMessageContentAsync(
        prompt: "JSONPlaceholder API’sinden kullanıcıların isimlerini örnek bir liste halinde göster.”",
        executionSettings: new OpenAIPromptExecutionSettings()
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(options: new() { }),
        },
        kernel: kernel);

    return content.Content.ToString();
});
#else
app.MapGet("/", async (IChatCompletionService chatCompletionService, Kernel kernel) =>
{
    ChatMessageContent content = await chatCompletionService.GetChatMessageContentAsync(
        prompt: "Tüm kullanıcıları getir. Kaç adet olduklarını, kullanıcı adı ve e-posta bilgilerini ‘[username | email]’ formatında yazmanı istiyorum. Ayrıca sıralamayı kullanıcı adına göre alfabetik olarak tersine yapmanı istiyorum. Ardından ‘----------’ çizgisiyle sayfayı ayır ve şu dediklerimi yap: Bir şirket senaryosundaymış gibi davran ve bu kullanıcılardan ikisini bu şirkette müdür olarak seç. Diğerlerine de kendine göre belirlediğin senaryolarda farklı görevler atayarak bu yöneticilerin altında paylaştır. Yani, bir hikâye çizecekmiş gibi yap.",
        executionSettings: new OpenAIPromptExecutionSettings()
        {
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(options: new() { }),
        },
        kernel: kernel);

    return content.Content.ToString();
});
#endif


app.Run();
