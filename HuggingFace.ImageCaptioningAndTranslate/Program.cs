
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ImageToText;
using OpenAI;
using System.ClientModel;

var builder = WebApplication.CreateBuilder(args);

#pragma warning disable SKEXP0070
builder.Services
    .AddKernel()
    .AddHuggingFaceImageToText(
        model: "Salesforce/blip-image-captioning-base",
        apiKey: "*******"
    )
#pragma warning restore SKEXP0070
    .AddOpenAIChatCompletion(
        modelId: "qwen/qwq-32b:free",
        openAIClient: new OpenAIClient(
            credential: new ApiKeyCredential("*********"),
            options: new OpenAIClientOptions
            {
                Endpoint = new Uri("https://openrouter.ai/api/v1")
            })
        );

var app = builder.Build();

#pragma warning disable SKEXP0001
app.MapPost("/image-captioning", async (IImageToTextService imageToTextService, Kernel kernel, IFormFile file) =>
{
    using var memoryStream = new MemoryStream();
    await file.CopyToAsync(memoryStream);
    var imageContent = new ImageContent(memoryStream.ToArray(), "image/jpeg");
    var textContent = await imageToTextService.GetTextContentAsync(imageContent);

    string promptTemplate = "Bu metni Türkçe'ye çevir : {{$input}}";
    var function = kernel.CreateFunctionFromPrompt(promptTemplate);
    var arguments = new KernelArguments { ["input"] = textContent.Text };
    var result = await function.InvokeAsync(kernel, arguments);

    return result.GetValue<string>();
}).DisableAntiforgery();
#pragma warning restore SKEXP0001

app.Run();