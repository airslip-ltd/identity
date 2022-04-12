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
  admin_group_id            = var.admin_group_id
  app_tier                  = var.web_tier
  app_size                  = var.web_size
  hostname                  = var.hostname
  uri                       = "https://${var.hostname}"
  ui_hostname               = var.ui_hostname
  database_name             = "identity-db"
  send_all                  = var.send_all
  allowed_domains           = var.allowed_domains
  log_level                 = var.log_level
}

module "ingredient_bowl" {
  source = "./tf_modules/Airslip.Terraform.Modules/modules/core/resource_group"

  tags              = local.tags
  app_id            = local.app_id
  short_environment = local.short_environment
  location          = local.location
}

module "cosmos_db" {
  source = "./tf_modules/Airslip.Terraform.Modules/recipes/mongodb_with_databases"

  db_configuration = {
    app_id            = local.app_id,
    short_environment = local.short_environment,
    location          = local.location,
    tags              = local.tags
  }

  resource_group = {
    use_existing            = true,
    resource_group_name     = module.ingredient_bowl.name,
    resource_group_location = module.ingredient_bowl.location
  }

  databases = [
    {
      database_name = local.database_name
    }
  ]
}

module "api_management" {
  source = "./tf_modules/Airslip.Terraform.Modules/recipes/api_only"

  app_configuration = {
    app_id            = local.app_id,
    short_environment = local.short_environment,
    location          = local.location,
    tags              = local.tags,
    app_tier          = local.app_tier,
    app_size          = local.app_size,
    health_check_path = "",
    apim_hostname     = local.uri
  }

  resource_group = {
    use_existing            = true,
    resource_group_name     = module.ingredient_bowl.name,
    resource_group_location = module.ingredient_bowl.location
  }

  app_settings = {
    "EnvironmentSettings:EnvironmentName" = var.environment,
    "Serilog:MinimumLevel:Default" : local.log_level,
    "MongoDbSettings:ConnectionString" : module.cosmos_db.connection_string,
    "MongoDbSettings:DatabaseName" : local.database_name,

    "PublicApiSettings:BaseUri" : local.uri,
    "PublicApiSettings:Base:BaseUri" : local.uri,

    "PublicApiSettings:Settings:UI:BaseUri" : local.ui_hostname,

    "EmailConfigurationSettings:SendAll" : var.send_all,
    "EmailConfigurationSettings:AllowedDomains" : var.allowed_domains
  }
}
