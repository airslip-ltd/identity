short_environment = "qa"
location = "UK South"
environment = "QA"
admin_group_id = "4a965f57-8ca7-4af3-ab5c-b7384f6ed4c9"
deployment_agent_group_id = "78963579-14c3-4ccc-b445-49f805ddaff2"
apim_hostname = "https://qa-app.airslip.com"
auth_apim_hostname = "https://qa-auth.airslip.com"
hub_url = "https://qa-oauth.airslip.com"
portal_url = "https://qa-secure.airslip.com"

max_size_gb = 4
sku_name = "GP_S_Gen5_2"
min_capacity = 1
auto_pause_delay_in_minutes = 60
additional_ip_addresses = [
    {
        start_ip_address = "88.202.245.245",
        end_ip_address = "88.202.245.245"
    }
]
send_all = "qa-testing@airslip.com"
allowed_domains = "airslip.com"
environment_type = ""