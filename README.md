# Agentic RAG with Azure AI Search and Microsoft Semantic Kernel

ASP.NET Core MVC application demonstrating Agentic Retrieval-Augmented Generation (RAG) using Microsoft Semantic Kernel and Azure AI Search. This project showcases both traditional vector search and autonomous agent-driven retrieval patterns.

##  Features

- **Dual Search Modes**
  - Direct Vector Search: Traditional semantic search with full control
  - Agentic RAG: Autonomous agents that dynamically retrieve and reason over data


![Agentic-Azure-AI-Search](https://github.com/user-attachments/assets/77bf8c65-aa68-42b3-8fb5-0916343ff204)

  
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
│  Azure AI   │  │ Semantic Kernel    │
│   Search    │  │  ┌──────────────┐  │
│             │  │  │ Azure AI Search │  
│             │◄─┤  │   Plugin     │  │
│             │  │  └──────────────┘  │
└─────────────┘  └────────┬───────────┘
                          │
                  ┌───────▼────────┐
                  │  Azure OpenAI  │
                  │  - GPT-3.5     │
                  │  - Embeddings  │
                  └────────────────┘
```





## 📋 Prerequisites

- .NET 8.0 SDK or later
- Azure Subscription
- Visual Studio 2022 or VS Code

### Azure Resources Required

1. **Azure AI Search**
   - Search service (Basic tier or higher)
   - Search index configured for vector search
   dfwerywwhgbvcz\asa
2. **Azure OpenAI Service**
   - Deployment: `gpt-35-turbo`
   - Deployment: `text-embedding-ada-002`

3. **Azure Storage Account** (for data storage)

4. **Resource Group** (to organize resources)

## 🚀 Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/vizsphere/RAG_using_Azure_AI_Search.git
cd RAG_using_Azure_AI_Search
```

### 2. Configure Azure Resources

Update `appsettings.json` with your Azure credentials:

```json
{
  "AppSettings": {
    "AzureSearch": {
      "Endpoint": "https://your-search-service.search.windows.net",
      "Index": "your-index-name",
      "ApiKey": "your-search-api-key",
      "TopK": 5,
      "VectorField": "text_vector",
      "Size": 10
    },
    "AzureOpenAIChatCompletion": {
      "Model": "gpt-35-turbo",
      "Endpoint": "https://your-openai-service.openai.azure.com/",
      "ApiKey": "your-openai-api-key"
    },
    "AzureOpenAITextEmbedding": {
      "Model": "text-embedding-ada-002",
      "Endpoint": "https://your-openai-service.openai.azure.com/",
      "ApiKey": "your-openai-api-key"
    }
  }
}
```

### 3. Install Dependencies

```bash
dotnet restore
```

### 4. Run the Application

```bash
dotnet run
```

Navigate to `https://localhost:5001` in your browser.


## 🔧 Usage

### Basic Vector Search

1. Enter your search query
2. Set the TopK parameter (number of results)
3. Click "Search"

The application performs vector similarity search against Azure AI Search and returns relevant results.

### Simple AI Search queries
    AI researcher specializing in natural language processing and machine learning
    Find Solution architect and enterprise software designer with expertise
    Find me list of all developers and Solution architect

### Agentic RAG queries

	Find me AI Specialists?
	Find me Data Scientists?
	Give me name and count of Developers?


