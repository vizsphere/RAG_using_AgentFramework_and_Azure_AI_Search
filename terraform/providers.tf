terraform {
  required_version = ">= 1.9.0"

  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = ">= 4.0, < 6.0"
    }
    # The AI Foundry "project" resource (Microsoft.CognitiveServices/accounts/projects) has no
    # native azurerm resource yet, so it's created with the AzAPI provider instead.
    azapi = {
      source  = "Azure/azapi"
      version = ">= 1.14, < 3.0"
    }
  }
}

provider "azurerm" {
  features {
    # Cognitive Services accounts (kind = AIServices/OpenAI) soft-delete instead of
    # deleting outright. Without this, a `terraform destroy` leaves a soft-deleted
    # stub behind that blocks recreating a resource with the same name (409
    # FlagMustBeSetForRestore) until it's purged - explicitly true here (it's also
    # the provider default) so `destroy` always purges instead.
    cognitive_account {
      purge_soft_delete_on_destroy = true
    }
  }
}

provider "azapi" {}
