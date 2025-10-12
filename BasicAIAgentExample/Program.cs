
using BasicAIAgentExample.Plugins;
using BasicAIAgentExample.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using OpenAI;
using System.ClientModel;

Logger.ConsoleLog("AI Agent başlatılıyOr..", ConsoleColor.Green);

var builder = Kernel.CreateBuilder();

builder.Services.AddHttpClient();

builder.AddOpenAIChatCompletion(
    modelId: "google/gemini-2.0-flash-exp:free",
    openAIClient: new OpenAIClient(
            credential: new ApiKeyCredential("*******************"),
            options: new OpenAIClientOptions
            {
                Endpoint = new Uri("https://openrouter.ai/api/v1")
            }
        )
    )
#region Plugin'ler aracılığıyla agent'a farklı yetenekler kazandırılıyor.
    .Plugins
        .AddFromType<PostPlugin>()
        .AddFromType<CommentPlugin>();
#endregion



var kernel = builder.Build();

#region Agent'a davranışını tanımlayan bir sistem prompt'u veriliyor.
var agentSystemPrompt = @"
Sen bir yapay zeka asistanısın.
Kullanıcı postlar ve o postlara yapılan yorumlar üzerinde türlü sorular soracaktır.
Bu soruları en iyi şekilde yanıtla.
Yorumları olumlu ya da olumsuz olmasına göre analiz edip değerlendirebilirsin.
Verileri analiz edebilir, aralarında mantıksal ilişki kurabilirsin.
İhtiyaç gördüğün taktirde farklı görevleri yerine getirebilirsin.
İşte kullanıcının girdisi: {{$input}}
Geçmiş konuşmalar: {{$history}}
Yanıtı aşağıda açık ve yardımcı bir şekilde aşağıda ver:
";

var systemFunction = kernel.CreateFunctionFromPrompt(
    promptTemplate: agentSystemPrompt,
    executionSettings: new PromptExecutionSettings
    {
        FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
    });
#endregion

#region Agent çalıştırılıyor.
var chatHistory = new ChatHistory();

Console.WriteLine("AI Agent hazır. Sorunuzu yazın :");
while (true)
{
    Console.Write("> ");
    var userInput = Console.ReadLine();
    chatHistory.AddUserMessage(userInput);

    if (string.IsNullOrEmpty(userInput))
        continue;

    try
    {
        var agentContext = new KernelArguments()
        {
            ["input"] = userInput,
            ["history"] = string.Join("\n", chatHistory.Select(h => $"{h.Role}: {h.Content}"))
            /*ChatHistory nesnesi InvokeAsync metodunda değil IChatCompletionService'de desteklenmektedir.
              Haliyle ChatHistory nesnesini InvokeAsync'de kullanabilmek için ufak bir dönüşüm
              operasyonu gerçekleştiriyoruz.*/
    
    };
        var result = await systemFunction.InvokeAsync(kernel, agentContext);
        chatHistory.AddAssistantMessage(result.GetValue<string>());
        Console.WriteLine("Yanıt : ");
        Console.WriteLine(result.GetValue<string>());
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Hata oluştu : {ex.Message}");
    }
    Console.WriteLine();
}
#endregion