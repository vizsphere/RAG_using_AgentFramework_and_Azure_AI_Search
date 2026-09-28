using Azure;
using Azure.AI.OpenAI;
using Azure.Search.Documents.Indexes;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.OpenAI;
using Microsoft.Extensions.AI;
using RAG_using_AgentFramework_and_Azure_AI_Search.Models;
using RAG_using_AgentFramework_and_Azure_AI_Search.Tools;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddLogging(b => b.AddConsole().SetMinimumLevel(LogLevel.Trace));

// Load AppSettings from configuration
var appSettings = new AppSettings();
builder.Configuration.GetSection("AppSettings").Bind(appSettings);
builder.Services.AddSingleton(appSettings);

// Chat client used to power the agent
var chatClient = new AzureOpenAIClient(
        new Uri(appSettings.AzureOpenAIChatCompletion.Endpoint),
        new AzureKeyCredential(appSettings.AzureOpenAIChatCompletion.ApiKey))
    .GetChatClient(appSettings.AzureOpenAIChatCompletion.Model)
    .AsIChatClient();


// Text embedding generator, used for both the plain vector search and the search tool
builder.Services.AddSingleton<IEmbeddingGenerator<string, Embedding<float>>>(
    new AzureOpenAIClient(
            new Uri(appSettings.AzureOpenAITextEmbedding.Endpoint),
            new AzureKeyCredential(appSettings.AzureOpenAITextEmbedding.ApiKey))
        .GetEmbeddingClient(appSettings.AzureOpenAITextEmbedding.Model)
        .AsIEmbeddingGenerator());

// Search Index Client
builder.Services.AddSingleton(sp =>
    new SearchIndexClient(new Uri(appSettings.AzureSearch.Endpoint), new AzureKeyCredential(appSettings.AzureSearch.ApiKey))
);

// Search tool, exposed to the agent as a function tool
builder.Services.AddSingleton<AzureAISearchTool>();

// Agent
builder.Services.AddSingleton<AIAgent>(sp =>
{
    var searchTool = sp.GetRequiredService<AzureAISearchTool>();

    var instructions = "You are a knowledgeable agent specialised in retrieving data using Azure AI Search."
                      + "For user."
                      + "1) Search required information in Azure AI Search using the SearchAsync tool"
                      + "2) Use the search results to provide the user with the required information";

    return chatClient.AsAIAgent(
        name: "AzureAISearchAgent",
        instructions: instructions,
        tools: [AIFunctionFactory.Create(searchTool.SearchAsync)]);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
