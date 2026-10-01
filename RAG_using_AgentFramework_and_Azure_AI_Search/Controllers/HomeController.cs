using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using RAG_using_AgentFramework_and_Azure_AI_Search.Models;
using System.Text;

namespace RAG_using_AgentFramework_and_Azure_AI_Search.Controllers
{
    public class HomeController : Controller
    {
        private readonly AIAgent _agent;
        private readonly IEmbeddingGenerator<string, Embedding<float>> _embeddingGenerator;
        private readonly SearchClient _searchClient;
        private readonly AppSettings _settings;
        private readonly ILogger<HomeController> _logger;

        public HomeController(AIAgent agent, IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator, SearchIndexClient indexClient, AppSettings settings, ILogger<HomeController> logger)
        {
            _agent = agent;
            _embeddingGenerator = embeddingGenerator;
            _settings = settings;
            _searchClient = indexClient.GetSearchClient(_settings.AzureSearch.Index);
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View(new SearchTerms());
        }


        [HttpPost]
        [Route("ai-search")]
        public async Task<IActionResult> AISearch([FromBody] SearchTerms search)
        {
            var result = new SearchResult();

            if (search.Input == null || search.Input.Trim().Length == 0)
            {
                result.Response = "Please enter some search terms.";

                return BadRequest(result);
            }

            ReadOnlyMemory<float> query = await _embeddingGenerator.GenerateVectorAsync(search.Input);

            var vectorQuery = new VectorizedQuery(query)
            {
                KNearestNeighborsCount = search.TopK,
                Fields = { _settings.AzureSearch.VectorField }
            };

            var options = new SearchOptions
            {
                VectorSearch = new VectorSearchOptions
                {
                    Queries = { vectorQuery }
                },
                Size = search.TopK
            };

            var response = await _searchClient.SearchAsync<AIUsageRecord>(options);

            if (response == null || response.Value.GetResults().Count() == 0)
            {
                result.Response = "No results found.";

                return Ok(result);
            }

            await foreach (var r in response.Value.GetResultsAsync())
            {
                result.Response += r.Document.Snippet ?? string.Empty + "\n\n";
            }

            return Ok(result);
        }


        [HttpPost]
        [Route("agentic-ai-search")]
        public async Task<IActionResult> AgenticAISearch([FromBody] SearchTerms search)
        {
            var searchResult = new SearchResult();

            if (search.Input == null || search.Input.Trim().Length == 0)
            {
                searchResult.Response = "Please enter some search terms.";

                return BadRequest(searchResult);
            }

            _logger.LogInformation("Agentic AI Search Prompt: {prompt}", search.Input);

            AgentSession session = await _agent.CreateSessionAsync();

            AgentResponse agentResponse = await _agent.RunAsync(search.Input, session);

            _logger.LogInformation("Agentic AI Search Assistant Reply: {assistantReply}", agentResponse.Text);

            if (agentResponse.Text == null || agentResponse.Text.Trim().Length == 0)
            {
                searchResult.Response = "No results found.";

                return Ok(searchResult);
            }

            searchResult.Response = search.IncludeCitations
                ? GetSearchText(agentResponse)
                : agentResponse.Text;

            return Ok(searchResult);
        }


        #region Private Methods

        private string GetSearchText(AgentResponse agentResponse)
        {
            var sb = new StringBuilder();

            sb.AppendLine("Search results:");
            sb.AppendLine(agentResponse.Text);
            sb.AppendLine("\n");

            // Each time the agent calls the AzureAISearchTool, the raw chunk it retrieved
            // is captured as a FunctionResultContent on the response messages - use those
            // as the citations for what grounded the answer.
            var citedChunks = agentResponse.Messages
                .SelectMany(m => m.Contents)
                .OfType<FunctionResultContent>()
                .Select(c => c.Result?.ToString())
                .Where(text => !string.IsNullOrWhiteSpace(text))
                .ToList();

            if (citedChunks.Count > 0)
            {
                sb.AppendLine("-----------");
                sb.AppendLine(" Citations:");
                sb.AppendLine("-----------");

                int count = 1;
                foreach (var chunk in citedChunks)
                {
                    sb.AppendLine($"{count}- {chunk}");
                    sb.AppendLine();
                    count += 1;
                }
            }

            return sb.ToString();
        }

        #endregion
    }
}
