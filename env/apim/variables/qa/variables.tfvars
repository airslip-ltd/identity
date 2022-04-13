short_environment = "qa"
location = "UK South"
environment = "QA"
hostname = "qa-auth.airslip.com"

deployment_agent_group_id = "78963579-14c3-4ccc-b445-49f805ddaff2"

apis = [
    {
        api_resource_suffix = "auth",
        api_name = "Identity",
        api_path = "",
        api_description = "Identity API",
        hostname = "airslip-qa-identity-api-app.azurewebsites.net",
        openapi_path = "https://airslip-qa-identity-api-app.azurewebsites.net/swagger/v1/swagger.json",
        from_file = false
    }
]
