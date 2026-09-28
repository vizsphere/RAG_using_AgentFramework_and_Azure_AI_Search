# Terraform - VizSphere RAG demo infrastructure

Deploys the three Azure resources this app needs, all in one resource group.

## Resources and dependency graph

```
azurerm_resource_group.this ("viz-ai-search")
├── azurerm_storage_account.this        ("demovizmsstd", Standard/LRS)
│   └── azurerm_storage_container.data     ("data", private)
├── azurerm_cognitive_account.ai_foundry ("demo-viz-ms-ai-foundry", kind=AIServices)
│   ├── azapi_resource.ai_foundry_project   ("demo-vizsphere")
│   ├── azurerm_cognitive_deployment.chat      (gpt-5)
│   └── azurerm_cognitive_deployment.embedding (text-embedding-3-small)
└── azurerm_search_service.this          ("demo-viz-ai-search", free tier)
```

The storage account, the AI Foundry account, and the Search service are independent
siblings that only depend on the resource group. The container, project, and
deployments depend on their parent above them via Terraform resource references
(`storage_account_id`, `parent_id`, `cognitive_account_id`). The project and the two
deployments also carry an explicit `depends_on` chain (project → chat → embedding):
Azure's Cognitive Services control plane rejects concurrent writes to the same account,
so without it Terraform's default parallelism causes a `409 RequestConflict`.

`ai_foundry_project` uses the `azapi` provider, not `azurerm`: AzureRM has no native
resource yet for a project under the modern `AIServices` account (only the legacy
hub-based model), so it's created directly against
`Microsoft.CognitiveServices/accounts/projects`.

## Prerequisites

- Terraform >= 1.9 - installed here via `winget install --id Hashicorp.Terraform`.
  Open a **new terminal** after installing so the PATH update applies.
- Azure subscription with GPT-5 access in `eastus2` (or your chosen `location`) -
  availability is regional, check the
  [Azure OpenAI models page](https://learn.microsoft.com/azure/ai-services/openai/concepts/models).
- `chat_model_version` in `variables.tf` should be confirmed against
  `az cognitiveservices account list-models` before applying - Azure retires old
  versions over time.
- Logged in with `az login` (or an equivalent service principal for `azurerm`).

## Usage

```bash
cd terraform
terraform init
terraform plan
terraform apply
terraform apply -destroy   # tear everything down
```

`.terraform.lock.hcl` is committed, pinning `azurerm` v5.7.0 and `azapi` v2.13.0.

> **Known issue on this machine:** `validate`/`plan` fail here with a TLS handshake
> error - Norton 360's local traffic interception, not the `.tf` files. Fix with a
> Norton exclusion for `terraform.exe`/provider plugins, a temporary SSL-inspection
> pause, or run Terraform from WSL/CI instead.

> **Known issue:** `azurerm_search_service` can fail with `400
> InsufficientResourcesAvailable` - the free tier is capacity-constrained per region.
> Override `search_service_location` to a different region and re-apply (default is
> now `eastus` after `eastus2` repeatedly ran out of capacity); the other resources
> are unaffected since they don't share that variable.

> **Known issue:** if a previous `destroy` didn't fully clean up, recreating the AI
> Foundry account can fail with `409 FlagMustBeSetForRestore` - Cognitive Services
> soft-deletes instead of deleting outright. `providers.tf` now sets
> `purge_soft_delete_on_destroy = true` so `destroy` purges automatically going
> forward. To unblock a resource that's *already* stuck soft-deleted:
> `az cognitiveservices account purge --name <name> --resource-group <rg> --location <region>`
> (list them first with `az cognitiveservices account list-deleted -o table`).

## Notes / things to reconsider before using this beyond a demo

- **Search tier is `free`** - shared, no SLA, 50MB/3 indexes. Bump `search_service_sku`
  to `basic`/`standard` when you outgrow it.
- **No RBAC/network restrictions configured** - all three resources use public network
  access and key-based auth, matching how the app reads endpoints/keys today. Tighten
  this (private endpoints, Entra ID auth) before using real data.
