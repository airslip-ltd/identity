short_environment = "demo"
location = "UK South"
environment = "Demo"
hostname = "demo-auth.airslip.com"

deployment_agent_group_id = "78963579-14c3-4ccc-b445-49f805ddaff2"

apis = [
    {
        api_resource_suffix = "auth",
        api_name = "Identity",
        api_path = "",
        api_description = "Identity API",
        hostname = "airslip-demo-identity-api-app.azurewebsites.net",
        openapi_path = "https://airslip-demo-identity-api-app.azurewebsites.net/swagger/v1/swagger.json",
        from_file = false
    }
]
