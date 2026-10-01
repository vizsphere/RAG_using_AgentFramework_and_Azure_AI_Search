using Azure.Search.Documents.Indexes;
using System.Text.Json.Serialization;

namespace RAG_using_AgentFramework_and_Azure_AI_Search.Models
{
    // Maps the index actually created for AI_Usage_and_Impact_on_Students_and_Professionals.csv:
    // Azure's "Import and vectorize data" knowledge-source pipeline treated the raw CSV as one
    // unstructured blob and split it into overlapping text chunks, rather than indexing one document
    // per CSV row with its own fields. So there's no Age/Country/Profession/etc. here - each "snippet"
    // is just a blob of several raw CSV rows mashed together as plain text.
    public class AIUsageRecord
    {
        [SimpleField(IsKey = true)]
        [JsonPropertyName("uid")]
        public string Uid { get; set; }

        [SimpleField(IsFilterable = true)]
        [JsonPropertyName("snippet_parent_id")]
        public string SnippetParentId { get; set; }

        [SimpleField(IsFilterable = true)]
        [JsonPropertyName("blob_url")]
        public string BlobUrl { get; set; }

        // The chunked text - a blob of several raw CSV rows, not one clean record.
        [SearchableField]
        [JsonPropertyName("snippet")]
        public string Snippet { get; set; }

        [SimpleField]
        [JsonPropertyName("snippet_vector")]
        public IList<float> SnippetVector { get; set; }
    }
}
