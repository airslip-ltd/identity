using Airslip.Common.Monitoring.Models;
using Airslip.Common.Types;
using FluentAssertions;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Airslip.Identity.Api.Application.UnitTests.IntegrationFacts
{
    public class HealthCheckIntegrationTest : IntegrationTestBase
    {
        [Fact]
        public async Task Check_services_can_load()
        {
            HttpResponseMessage createRequest = await GetUnauthorizedHttpClient()
                .GetAsync(
                    new Uri(BaseUri, HeartbeatEndpoint + "/ping"));

            createRequest.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        
        [Fact]
        public async Task Check_health_check_service_is_called()
        {
            HttpResponseMessage createRequest = await GetUnauthorizedHttpClient()
                .GetAsync(
                    new Uri(BaseUri, HeartbeatEndpoint + "/health"));

            createRequest.StatusCode.Should().Be(HttpStatusCode.OK);
            
            string healthCheckJson = await createRequest.Content.ReadAsStringAsync();

            HealthCheckResponse healthCheckResponse = Json.Deserialize<HealthCheckResponse>(healthCheckJson);

            healthCheckResponse.AllOk.Should().BeTrue(Json.Serialize(healthCheckResponse.HealthChecks));
        }
    }
}