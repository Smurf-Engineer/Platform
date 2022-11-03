resource "azurerm_log_analytics_workspace" "log_analytics_workspace" {
  name                       = "log-analytics-workspace"
  resource_group_name        = azurerm_resource_group.monitor_resource_group.name
  location                   = var.global_resource_location
  sku                        = "PerGB2018"
  retention_in_days          = "30"
  internet_ingestion_enabled = "true"
  internet_query_enabled     = "true"

  lifecycle {
    prevent_destroy = true
  }

  tags = local.tags
}
