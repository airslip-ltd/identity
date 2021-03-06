variable "short_environment" {
  description = "The prefix used for all resources"
}

variable "location" {
  description = "The Azure location where all resources should be created"
}
variable "environment" {
  description = "The environment name being deployed to"
}

variable "api_publisher_name" {
  description = "The publisher name used for the API"
  default = "Airslip Ltd"
}

variable "api_publisher_email" {
  description = "The publisher email used for the API"
  default = "support@airslip.com"
}
variable "api_sku_name" {
  description = "The sku used for the API Management Service"
  default = "Consumption_0"
}
variable "admin_group_id" {}
variable "deployment_agent_group_id" {}
variable "web_tier" {
  description = "The tier used for the app service plan"
  default = "PremiumV2"
}
variable "web_size" {
  description = "The size used for the app service plan"
  default = "P1v2"
}
variable "hostname" {}
variable "ui_hostname" {}
variable "send_all" {}
variable "allowed_domains" {}
variable "log_level" {
  default = "Warning"
}
