# Agentic RAG with Microsoft Agent Framework and Azure AI Search

ASP.NET Core MVC application demonstrating Agentic Retrieval-Augmented Generation (RAG) using
[Microsoft Agent Framework](https://github.com/microsoft/agent-framework) and Azure AI Search.
This project showcases both traditional vector search and autonomous agent-driven retrieval
patterns over a survey dataset of how students and professionals use AI tools.

##  Features

- **Dual Search Modes**
  - Direct Vector Search: Traditional semantic search with full control
  - Agentic RAG: An `AIAgent` that calls a search tool on its own and reasons over the results

![Agentic-Azure-AI-Search](https://github.com/user-attachments/assets/77bf8c65-aa68-42b3-8fb5-0916343ff204)

> The screenshot above predates the current dataset/UI copy (it shows the earlier
> speaker-search demo) - the app itself now searches AI-usage survey records.

## Architecture

```
┌─────────────┐
│   Web UI    │
└──────┬──────┘
       │
┌──────▼──────────────────────────┐
│   HomeController                │
│  ┌──────────┐  ┌──────────────┐ │
│  │ AISearch │  │AgenticSearch │ │
│  └──────────┘  └──────────────┘ │
└──────┬──────────────┬───────────┘
       │              │
┌──────▼──────┐  ┌───▼────────────────┐
│  Azure AI   │  │ Microsoft Agent    │
│   Search    │  │ Framework (AIAgent)│
│             │  │  ┌──────────────┐  │
│             │◄─┤  │AzureAISearch │  │
│             │  │  │    Tool      │  │
│             │  │  └──────────────┘  │
└─────────────┘  └────────┬───────────┘
                          │
                  ┌───────▼────────┐
                  │  Azure OpenAI  │
                  │  - gpt-5       │
                  │  - text-embedding-3-small │
                  └────────────────┘
```

## 📋 Prerequisites

- .NET 9.0 SDK or later
- Azure Subscription
- Visual Studio 2022 or VS Code

### Azure Resources Required

See [`terraform/`](terraform/) to provision all of these in one shot (it also documents the
exact resource names, SKUs, and known deployment issues). Manually, you need:

1. **Azure AI Search** (`free` tier works for this demo)
   - A knowledge-source/vectorized index built from the CSV below

2. **Azure AI Foundry** (a Cognitive Services account, `kind = AIServices`)
   - Chat deployment: `gpt-5`
   - Embedding deployment: `text-embedding-3-small`

3. **Azure Storage Account** - holds the source CSV that gets indexed (the running app
   doesn't talk to it directly; it's only read by the Search service's indexer)

4. **Resource Group** (to organize resources)

## 🚀 Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/vizsphere/RAG_using_Azure_AI_Search.git
cd RAG_using_Azure_AI_Search
```

### 2. Configure Azure Resources

Non-secret settings (model names, endpoints, index name) live in
`appsettings.Development.json`. **API keys don't** - they're read from
[.NET User Secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets) so they
never end up in a file git can see:

```bash
cd RAG_using_AgentFramework_and_Azure_AI_Search
dotnet user-secrets init   # only if not already initialized - adds a UserSecretsId to the .csproj
dotnet user-secrets set "AppSettings:AzureOpenAIChatCompletion:ApiKey" "<your-ai-foundry-key>"
dotnet user-secrets set "AppSettings:AzureOpenAITextEmbedding:ApiKey" "<your-ai-foundry-key>"
dotnet user-secrets set "AppSettings:AzureSearch:ApiKey" "<your-search-admin-key>"
```

`appsettings.Development.json` should then look like:

```json
{
  "AppSettings": {
    "AzureSearch": {
      "Endpoint": "https://your-search-service.search.windows.net",
      "Index": "your-index-name",
      "ApiKey": "",
      "TopK": 5,
      "VectorField": "snippet_vector",
      "Size": 10
    },
    "AzureOpenAIChatCompletion": {
      "Model": "gpt-5",
      "Endpoint": "https://your-ai-foundry-resource.cognitiveservices.azure.com/",
      "ApiKey": ""
    },
    "AzureOpenAITextEmbedding": {
      "Model": "text-embedding-3-small",
      "Endpoint": "https://your-ai-foundry-resource.cognitiveservices.azure.com/",
      "ApiKey": ""
    }
  }
}
```

If you'd rather keep everything in one untracked file instead of User Secrets, the app also
works with a plain `appsettings.json` (already gitignored) carrying the same shape with real
`ApiKey` values filled in.

### 3. Install Dependencies

```bash
dotnet restore
```

### 4. Run the Application

```bash
dotnet run
```

Navigate to `http://localhost:5296` in your browser (see `Properties/launchSettings.json`).

## 🔧 Usage

### Basic Vector Search

1. Enter your search query
2. Set the TopK parameter (number of results)
3. Click "Search"

The application performs vector similarity search against Azure AI Search and returns relevant results.

### Simple AI Search queries
    Students using ChatGPT for coding and studying
    Professionals using AI tools for office work and data analysis
    Customer support specialists using AI chatbots

### Agentic RAG queries

	Which AI tool do students use most for coding?
	Find a professional who would recommend their AI tool for office work
	How are professionals using AI for customer support?


## Sample Data source

`AI_Usage_and_Impact_on_Students_and_Professionals.csv` - a synthetic survey dataset of how
students and professionals use AI tools (demographics, tool/purpose, usage hours, and
self-reported productivity/accuracy/satisfaction scores).

**How it's actually indexed today:** the index was created via Azure AI Foundry's
"knowledge source" / import-and-vectorize wizard pointed at the raw CSV file as an
unstructured blob, not as one search document per row. So the real index schema is generic
chunk-and-embed (`uid`, `snippet_parent_id`, `blob_url`, `snippet`, `snippet_vector`) - each
`snippet` is a blob of several raw CSV rows chunked together as plain text, not one clean
record. See `Models/AIUsageRecord.cs` for the exact mapping. There's no per-column filtering
(Age, Country, Profession, etc.) in this setup - only semantic search over those text blobs.

If you want real per-row structured fields instead, you'd need to re-ingest the CSV as
tabular data (one document per row, each column its own field) rather than through the
blob-chunking wizard - `Models/AIUsageRecord.cs` would need to grow back out to match.
