# Azure AI Search Notes

---

## 1. Introduction to Azure AI Search and Its Role

### https://notegpt.io/share/9636ba830

Azure AI Search is a fully managed search-as-a-service that provides a dedicated search layer for applications, enabling efficient information retrieval from large and diverse datasets like documents, PDFs, databases, websites, and Azure Storage.

- Abstracts the complexity of building and maintaining custom search engines.
- Supports filtering, sorting, ranking, relevance, and high performance even at scale.
- Critical as a retrieval layer in AI applications (chatbots, copilots, retrieval-augmented generation) to supply accurate, trusted data prior to AI response generation.

**Core Concepts Introduced:**

- **Search index:** Central repository where searchable content is stored, not the original data files.
- **Documents:** Individual units of searchable data, stored as JSON documents inside the index.
- **Index schema:** A critical structure defining fields, data types, and their behaviors (searchable, filterable, sortable, retrievable).
- **Key field:** Unique identifier for each document, mandatory for index creation, allows efficient document updating and deletion.

---

## 2. Keyword Search: Traditional Exact-Match Search

Keyword search is the classical search model where queries match exact words or phrases in text fields.

- Operates on fields marked searchable in the index schema (e.g., titles, descriptions).
- Indexing involves text analysis to enable fast searches without scanning entire documents.

**Key supporting features improving keyword search capability:**

- **Filtering:** Strict condition application (e.g., ratings > 4, specific categories).
- **Sorting:** Explicit result ordering (by rating, date).
- **Faceted search:** Provides grouped counts per field values for UI filters.
- **Lucene syntax support:** Enables advanced queries including wildcards, fuzzy matching, field-specific searches.
- **Autocomplete and suggestions:** Enhances user input experience by offering search term suggestions.

**Limitations:**

- Requires users to know exact or close-to-exact words.
- Lacks understanding of query meaning or intent beyond literal text matching.

---

## 3. Vector Search: Semantic Meaning-Based Search

Vector search addresses keyword search's limitations by enabling meaning-based retrieval.

- Converts texts into numerical embeddings (vectors) representing semantic meaning using machine learning models.
- Embeddings allow comparison of content based on meaning similarity, not just text overlap.
- Azure AI Search integrates vector fields within the same index, enabling hybrid search combining keyword and vector queries.
- Two embedding approaches:
  - User-generated embeddings pushed to the index.
  - Azure AI Search auto-generates embeddings via configured vectorizers linked to embedding models (e.g., Azure OpenAI's embedding models).

**Key Characteristics:**

- Similarity is calculated via vector distance metrics.
- Supports natural language queries and approximate matching for conceptual similarity.
- Complements but does not replace keyword search; often combined for hybrid search to balance precision and semantic recall.

---

## 4. Semantic Ranking: Advanced Relevance and Explanation Layer

Semantic ranking is a post-retrieval enhancement layer, improving ordering and explanation of search results obtained from keyword/vector search.

- Employs deep learning models similar to Bing's search models.
- Goes beyond word matching and similarity scores to understand user intent and content meaning.
- Performs L2 ranking: re-ranks the top candidate results (L1 ranking is initial keyword/vector retrieval).

**Provides:**

- **Semantic captions:** Highlight the most relevant passage in results.
- **Semantic answers:** Extracted direct answers to natural language questions (not generated text, but pulled from indexed content).
- **Re-ranker score:** Reflects semantic relevance differing from keyword or vector scores.

**Usage considerations:**

- Works only on text fields.
- Requires explicit enabling.
- Supports limited languages.
- Incurs extra costs beyond free query quotas; use when semantic quality adds value (e.g., exploratory queries or question answering).

---

## 5. Agentic Retrieval: AI-Driven Multi-Step Reasoning Search

Agentic retrieval fundamentally transforms search from a single-shot query into a reasoning-driven, multi-query process controlled by a large language model (LLM).

- LLM acts as an AI agent/planner, breaking down complex user questions into multiple subqueries.
- Executes and aggregates search results using various search types (keyword, vector, semantic ranking, hybrid).
- Synthesizes a well-structured, grounded final response, rather than just returning a list of documents.
- Underpins sophisticated AI applications like chatbots, copilots, and research assistants.
- Strongly related to retrieval-augmented generation (RAG) but expands it with iterative and reasoned retrieval.

**When to use:**

- For complex natural language queries involving multiple conditions.
- When responses must synthesize info from multiple documents.
- When conversational context maintenance is needed.

**Not suited for:**

- Simple lookups.
- High-frequency, low-complexity queries.
- Scenarios requiring very low latency or merely listing results.

---

## 6. Multimodal Search: Combining Text and Image Content Understanding

Multimodal search extends Azure AI Search to incorporate both text and rich image content (charts, graphs, diagrams, scanned docs).

- Uses AI services to perform image verbalization, creating meaningful textual descriptions of images—not just OCR text extraction.
- Embedding models convert both text and image descriptions into vectors within a shared semantic space.
- Enables queries to retrieve content based on semantic similarity even if the answer is embedded in complex visuals.
- Integrates seamlessly with existing search types and semantic ranking for enhanced retrieval quality across modalities.

**Important notes:**

- Requires multiple AI services orchestrated together.
- Only available in select Azure regions currently.
- Vital for processing real-world documents that contain both textual and visual information.

---

## 7. Practical Demonstrations and Feature Implementation Notes

- Index creation and schema design showcased via Azure Portal and code.
- Import data from Azure Blob Storage (JSON hotel data with fields like hotel ID, name, categories, ratings).
- Demonstrations cover keyword search with filters, sorting, facets, Lucene syntax, autocomplete.
- Vector search built on text embeddings generated by deployed Azure OpenAI models; involves creating vector fields and uploading documents with embeddings.
- Semantic ranking configured with semantic fields; examples show improved query relevance and contextual results.
- Agentic retrieval demonstrates multi-step reasoning—query breakdown, multiple Azure AI Search queries combined with GPT-based synthesis.
- Multimodal search demo uses a complex PDF with textual and graphical data, showing extraction of nuanced answers from chart analysis.
- Access to Azure Foundry, managed identities, and appropriate role assignments are prerequisites for system connectivity and security.

---

## 8. Cost Overview for Azure AI Search

| Feature | Estimated Monthly Cost for 15 GB Data* |
|---|---|
| Basic Keyword / Vector Search | Approx. $95–$150 |
| Semantic Ranking | Additional cost per query, free quota limited |
| Image Verbalization | Included in multimodal search pricing (region dependent) |
| Agentic Retrieval | Higher costs due to multi-query & LLM usage |

*Costs vary by tier (Basic, Standard, Storage Optimized), region, data volume, and query usage.

---

## 9. Summary and Key Takeaways

- Azure AI Search is a feature-rich, layered search platform designed for modern AI-powered applications.
- Its core components—indexes, schema design, and document units—enable flexible, fast retrieval.
- Keyword search is suitable for precise, text-based queries with structured filters and sorting.
- Vector search brings semantic, meaning-based retrieval using text embeddings.
- Semantic ranking improves result relevance and explanatory power using deep learning models.
- Agentic retrieval empowers LLM-driven multi-stage reasoning for complex queries, enhancing chatbot and copilot experiences.
- Multimodal search integrates text and images for comprehensive content understanding.
- Selecting and combining these capabilities appropriately allows building tailored, powerful search solutions addressing diverse real-world information needs.
- Understanding these distinctions equips developers and architects to design effective AI search experiences, balancing precision, recall, relevance, interpretability, and multimodal knowledge extraction.

---

*End of Azure AI Search Notes*
