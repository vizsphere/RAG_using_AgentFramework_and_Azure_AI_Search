namespace RAG_using_AgentFramework_and_Azure_AI_Search.Models
{
    public class AppSettings  
    {
        public AzureOpenAIChatCompletion AzureOpenAIChatCompletion { get; set; }

        public AzureOpenAITextEmbedding AzureOpenAITextEmbedding { get; set; }

        public AzureSearch AzureSearch { get; set; }
    }

    public class AzureSearch
    {
        public string Endpoint { get; set; }

        public string ApiKey { get; set; }

        public string Index { get; set; }

        public string VectorField { get; set; }

        public int TopK { get; set; } 

        public int Size { get; set; }
    }

    public class AzureOpenAIChatCompletion
    {
        public string Model { get; set; }

        public string Endpoint { get; set; }

        public string ApiKey { get; set; }

    }

    public class AzureOpenAITextEmbedding
    {
        public string Model { get; set; }

        public string Endpoint { get; set; }

        public string ApiKey { get; set; }
    }


}
