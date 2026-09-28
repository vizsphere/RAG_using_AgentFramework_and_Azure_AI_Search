# 2) Azure AI Foundry.
#
# Modern (non-hub) Foundry is a single Cognitive Services account with
# kind = "AIServices" and project management turned on. Model deployments
# (GPT-5, text-embedding-3-small) live on this account and are shared by
# every project underneath it.
resource "azurerm_cognitive_account" "ai_foundry" {
  name                = var.ai_foundry_name
  resource_group_name = azurerm_resource_group.this.name
  location            = azurerm_resource_group.this.location

  kind     = "AIServices"
  sku_name = var.ai_foundry_sku_name

  custom_subdomain_name         = var.ai_foundry_name
  project_management_enabled    = true
  public_network_access_enabled = true

  identity {
    type = "SystemAssigned"
  }

  tags = var.tags
}

# The Foundry "project" (Microsoft.CognitiveServices/accounts/projects) has no
# native azurerm resource yet, so it's created via the AzAPI provider.
resource "azapi_resource" "ai_foundry_project" {
  type      = "Microsoft.CognitiveServices/accounts/projects@2025-06-01"
  name      = var.ai_foundry_project_name
  parent_id = azurerm_cognitive_account.ai_foundry.id
  location  = azurerm_resource_group.this.location

  identity {
    type = "SystemAssigned"
  }

  body = {
    properties = {
      displayName = var.ai_foundry_project_name
      description = "VizSphere demo project"
    }
  }

  tags = var.tags

  schema_validation_enabled = false
}

# GPT-5 chat model deployment.
#
# Azure's Cognitive Services control plane rejects concurrent writes to the
# same account (a project create racing a deployment create fails with
# "409 RequestConflict: Another operation is in progress"), and these three
# resources have no natural dependency on each other. depends_on forces
# Terraform to create them one at a time instead of in parallel.
resource "azurerm_cognitive_deployment" "chat" {
  name                 = var.chat_model_name
  cognitive_account_id = azurerm_cognitive_account.ai_foundry.id

  model {
    format  = "OpenAI"
    name    = var.chat_model_name
    version = var.chat_model_version
  }

  sku {
    name     = var.chat_model_deployment_sku_name
    capacity = var.chat_model_capacity
  }

  depends_on = [azapi_resource.ai_foundry_project]
}

# text-embedding-3-small embedding model deployment.
resource "azurerm_cognitive_deployment" "embedding" {
  name                 = var.embedding_model_name
  cognitive_account_id = azurerm_cognitive_account.ai_foundry.id

  model {
    format  = "OpenAI"
    name    = var.embedding_model_name
    version = var.embedding_model_version
  }

  sku {
    name     = var.embedding_model_deployment_sku_name
    capacity = var.embedding_model_capacity
  }

  depends_on = [azurerm_cognitive_deployment.chat]
}
