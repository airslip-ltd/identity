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
  certificate_name          = var.certificate_name
  certificate_path          = var.certificate_path
  certificate_password      = var.certificate_password
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

data "azurerm_client_config" "current" {}

module "frontdoor" {
  source = "./tf_modules/Airslip.Terraform.Modules/recipes/app_service_front_door"

  app_configuration = {
    app_id               = local.app_id,
    hostname             = local.hostname,
    backend_hostname     = module.api_management.app_service_hostname,
    app_id_short         = local.short_app_id,
    short_environment    = local.short_environment,
    location             = module.ingredient_bowl.location,
    tags                 = local.tags,
    certificate_name     = local.certificate_name,
    certificate_path     = local.certificate_path,
    certificate_password = local.certificate_password,
    tenant_id            = data.azurerm_client_config.current.tenant_id,
    admin_group_id       = local.admin_group_id,
    deployer_id          = local.deployment_agent_group_id
  }

  resource_group = {
    use_existing            = true,
    resource_group_name     = module.ingredient_bowl.name,
    resource_group_location = module.ingredient_bowl.location
  }
}
