# 3) Azure AI Search.
#
# location defaults to the shared var.location but is independently
# overridable via search_service_location, since "free"-tier capacity is
# allocated per region and can run out (400 InsufficientResourcesAvailable)
# independently of whether the other resources deployed fine there.
resource "azurerm_search_service" "this" {
  name                = var.search_service_name
  resource_group_name = azurerm_resource_group.this.name
  location            = var.search_service_location

  sku = var.search_service_sku

  tags = var.tags
}
