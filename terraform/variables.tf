# ---------------------------------------------------------------------------
# General
# ---------------------------------------------------------------------------

variable "location" {
  description = "Azure region all resources are deployed into. eastus2 has broad GlobalStandard availability for the GPT-5 and text-embedding-3-small models."
  type        = string
  default     = "eastus2"
}

variable "resource_group_name" {
  description = "Name of the resource group that holds all resources in this project."
  type        = string
  default     = "viz-ai-search"
}

variable "tags" {
  description = "Common tags applied to every resource."
  type        = map(string)
  default = {
    project     = "vizsphere-rag"
    environment = "demo"
    managed_by  = "terraform"
  }
}

# ---------------------------------------------------------------------------
# 1) Storage account
# ---------------------------------------------------------------------------

variable "storage_account_name" {
  description = "Globally unique name of the storage account used for blob storage. Lowercase letters/numbers only, 3-24 characters."
  type        = string
  default     = "demovizmsstd"
}

variable "storage_account_replication_type" {
  description = "Replication strategy for the storage account. LRS = locally redundant storage."
  type        = string
  default     = "LRS"
}

variable "storage_container_name" {
  description = "Name of the blob container created in the storage account."
  type        = string
  default     = "data"
}

# ---------------------------------------------------------------------------
# 2) Azure AI Foundry (Cognitive Services account, kind = AIServices) + project
# ---------------------------------------------------------------------------

variable "ai_foundry_name" {
  description = "Name of the Azure AI Foundry resource (Cognitive Services account, kind = AIServices)."
  type        = string
  default     = "demo-viz-ms-ai-foundry"
}

variable "ai_foundry_sku_name" {
  description = "Pricing tier of the Azure AI Foundry resource."
  type        = string
  default     = "S0"
}

variable "ai_foundry_project_name" {
  description = "Name of the Azure AI Foundry project created under the AI Foundry resource."
  type        = string
  default     = "demo-vizsphere"
}

variable "chat_model_name" {
  description = "Azure OpenAI chat model to deploy."
  type        = string
  default     = "gpt-5"
}

variable "chat_model_version" {
  description = "Model version of the chat model deployment. Confirm the currently supported version for your region with `az cognitiveservices account list-models` before applying, as Azure retires older versions over time."
  type        = string
  default     = "2025-08-07"
}

variable "chat_model_deployment_sku_name" {
  description = "Deployment SKU for the chat model. GlobalStandard is the usual pay-as-you-go choice."
  type        = string
  default     = "GlobalStandard"
}

variable "chat_model_capacity" {
  description = "Deployment capacity for the chat model, in thousands of tokens per minute (TPM)."
  type        = number
  default     = 10
}

variable "embedding_model_name" {
  description = "Azure OpenAI embedding model to deploy."
  type        = string
  default     = "text-embedding-3-small"
}

variable "embedding_model_version" {
  description = "Model version of the embedding model deployment."
  type        = string
  default     = "1"
}

variable "embedding_model_deployment_sku_name" {
  description = "Deployment SKU for the embedding model. GlobalStandard is the usual pay-as-you-go choice."
  type        = string
  default     = "GlobalStandard"
}

variable "embedding_model_capacity" {
  description = "Deployment capacity for the embedding model, in thousands of tokens per minute (TPM)."
  type        = number
  default     = 20
}

# ---------------------------------------------------------------------------
# 3) Azure AI Search
# ---------------------------------------------------------------------------

variable "search_service_name" {
  description = "Globally unique name of the Azure AI Search service."
  type        = string
  default     = "demo-viz-ai-search"
}

variable "search_service_location" {
  description = "Azure region for the Search service, independent of var.location. eastus2's free tier has repeatedly come back 400 InsufficientResourcesAvailable, so this defaults to eastus instead - override again if that also runs out of capacity."
  type        = string
  default     = "eastus"
}

variable "search_service_sku" {
  description = "Pricing tier of the Azure AI Search service. 'free' is the shared, no-cost tier meant for development/evaluation (50MB storage, 3 indexes, no SLA)."
  type        = string
  default     = "free"
}
