short_environment = "dev"
location = "UK South"
environment = "Development"

hostname = "dev-auth2.airslip.com"
ui_hostname = "https://dev-secure.airslip.com"
send_all = "dev-testing@airslip.com"
allowed_domains = "airslip.com"

admin_group_id = "4a965f57-8ca7-4af3-ab5c-b7384f6ed4c9"
deployment_agent_group_id = "78963579-14c3-4ccc-b445-49f805ddaff2"

apis = [
    {
        api_resource_suffix = "oauth",
        api_name = "Identity",
        api_path = "",
        api_description = "Identity API",
        hostname = "airslip-dev-identity-api-app.azurewebsites.net",
        openapi_path = "https://airslip-dev-identity-api-app.azurewebsites.net/swagger/v1/swagger.json",
        from_file = false,
        policy = "./variables/dev/api_policy.xml"
    }
]

