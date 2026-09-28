output "resource_group_name" {
  description = "Name of the resource group all resources were deployed into."
  value       = azurerm_resource_group.this.name
}

# ---------------------------------------------------------------------------
# Storage account
# ---------------------------------------------------------------------------

output "storage_account_name" {
  value = azurerm_storage_account.this.name
}

output "storage_account_primary_blob_endpoint" {
  value = azurerm_storage_account.this.primary_blob_endpoint
}

output "storage_container_name" {
  value = azurerm_storage_container.data.name
}

output "storage_account_primary_access_key" {
  value     = azurerm_storage_account.this.primary_access_key
  sensitive = true
}

# ---------------------------------------------------------------------------
# Azure AI Foundry
# ---------------------------------------------------------------------------

output "ai_foundry_name" {
  value = azurerm_cognitive_account.ai_foundry.name
}

output "ai_foundry_endpoint" {
  value = azurerm_cognitive_account.ai_foundry.endpoint
}

output "ai_foundry_primary_access_key" {
  value     = azurerm_cognitive_account.ai_foundry.primary_access_key
  sensitive = true
}

output "ai_foundry_project_name" {
  value = var.ai_foundry_project_name
}

output "ai_foundry_project_id" {
  value = azapi_resource.ai_foundry_project.id
}

output "chat_model_deployment_name" {
  value = azurerm_cognitive_deployment.chat.name
}

output "embedding_model_deployment_name" {
  value = azurerm_cognitive_deployment.embedding.name
}

# ---------------------------------------------------------------------------
# Azure AI Search
# ---------------------------------------------------------------------------

output "search_service_name" {
  value = azurerm_search_service.this.name
}

output "search_service_endpoint" {
  value = "https://${azurerm_search_service.this.name}.search.windows.net"
}

output "search_service_primary_admin_key" {
  value     = azurerm_search_service.this.primary_key
  sensitive = true
}
