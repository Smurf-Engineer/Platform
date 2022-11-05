terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "=3.29.1"
    }

    azuread = {
      version = "=1.6.0"
    }
  }

  backend "azurerm" {
    storage_account_name = "__terraform-storage-account__"
    container_name       = "__terraform-container__"
    key                  = "__terraform-state-file__"
  }
}

provider "azurerm" {
  features {}
}

provider "azuread" {
  use_microsoft_graph = true
}
