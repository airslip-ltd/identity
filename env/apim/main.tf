terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "2.90.0"
    }
  }

  backend "azurerm" {
  }
}

provider "azurerm" {
  features {}
}

locals {
  tags = {
    Environment = var.environment
  }
  app_id                    = "identity-api"
  short_app_id              = "iapi"
  short_environment         = var.short_environment
  location                  = var.location
  deployment_agent_group_id = var.deployment_agent_group_id

  resource_group_name = "airslip-${var.short_environment}-identity-api-resources"

  hostname = var.hostname

  apis                = var.apis
  revision            = replace(var.release_name, ".", "")
  api_publisher_name  = var.api_publisher_name
  api_publisher_email = var.api_publisher_email
  api_sku_name        = var.api_sku_name
  api_policy          = "./variables/${var.short_environment}/api_policy.xml"
}

data "azurerm_client_config" "current" {}

data "azurerm_resource_group" "ingredient_bowl" {
  name = local.resource_group_name
}

module "apim" {
  source = "./tf_modules/Airslip.Terraform.Modules/recipes/apim_multiple_apis"

  resource_group = {
    use_existing            = true,
    resource_group_name     = local.resource_group_name,
    resource_group_location = data.azurerm_resource_group.ingredient_bowl.location
  }

  app_configuration = {
    app_id              = local.app_id,
    app_id_short        = local.short_app_id,
    short_environment   = local.short_environment,
    location            = local.location,
    tags                = local.tags,
    api_publisher_name  = local.api_publisher_name,
    api_publisher_email = local.api_publisher_email,
    api_sku_name        = local.api_sku_name,
    api_custom_domain   = local.hostname,
    tenant_id           = data.azurerm_client_config.current.tenant_id,
    deployer_id         = local.deployment_agent_group_id,
    revision            = local.revision
    policy              = local.api_policy
  }

  apis = local.apis
}

