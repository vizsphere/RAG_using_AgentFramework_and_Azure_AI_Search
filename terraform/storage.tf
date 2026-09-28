# 1) Storage account for blob storage - locally redundant (LRS).
resource "azurerm_storage_account" "this" {
  name                = var.storage_account_name
  resource_group_name = azurerm_resource_group.this.name
  location            = azurerm_resource_group.this.location

  account_kind             = "StorageV2"
  account_tier             = "Standard"
  account_replication_type = var.storage_account_replication_type

  tags = var.tags
}

resource "azurerm_storage_container" "data" {
  name                  = var.storage_container_name
  storage_account_id    = azurerm_storage_account.this.id
  container_access_type = "private"
}
