using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Models;
using Microsoft.Extensions.AI;
using RAG_using_AgentFramework_and_Azure_AI_Search.Models;
using System.ComponentModel;

namespace RAG_using_AgentFramework_and_Azure_AI_Search.Tools
{
    public class AzureAISearchTool
    {
        private readonly SearchIndexClient _searchIndexClient;
        private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingGenerator;
        private readonly AppSettings _appSettings;
        private readonly ILogger<AzureAISearchTool> _logger;

        public AzureAISearchTool(SearchIndexClient searchIndexClient, IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator, AppSettings appSettings, ILogger<AzureAISearchTool> logger)
        {
            _searchIndexClient = searchIndexClient;
            _embeddingGenerator = embeddingGenerator;
            _appSettings = appSettings;
            _logger = logger;
        }

        [Description("Search for a speaker")]
        public async Task<string> SearchAsync(string query)
        {
            _logger.LogInformation("AzureAISearchTool SearchAsync called with query: {query}", query);

            var searchClient = _searchIndexClient.GetSearchClient(_appSettings.AzureSearch.Index);

            ReadOnlyMemory<float> embedding = await _embeddingGenerator.GenerateVectorAsync(query);

            var vectorQuery = new VectorizedQuery(embedding)
            {
                KNearestNeighborsCount = _appSettings.AzureSearch.TopK,
                Fields = { _appSettings.AzureSearch.VectorField }
            };

            var options = new SearchOptions
            {
                VectorSearch = new VectorSearchOptions
                {
                    Queries = { vectorQuery }
                },
                Size = _appSettings.AzureSearch.Size
            };

            var response = await searchClient.SearchAsync<Speaker>(options);

            await foreach (var result in response.Value.GetResultsAsync())
            {
                return result.Document.Chunk ?? string.Empty;
            }

            return string.Empty;
        }
    }
}
