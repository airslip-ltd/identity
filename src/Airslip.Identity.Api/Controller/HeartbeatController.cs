using Airslip.Common.Monitoring.Interfaces;
using Airslip.Common.Monitoring.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Controller
{
    [AllowAnonymous]
    [ApiController]
    [ApiVersion(ApiConstants.VersionOne)]
    [Route("v{version:apiVersion}/heartbeat/")]
    public class HeartbeatController : ControllerBase
    {
        private readonly IHealthCheckService _healthCheckService;

        public HeartbeatController(IHealthCheckService healthCheckService)
        {
            _healthCheckService = healthCheckService;
        }
        
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Route("ping")]
        public IActionResult Ping()
        {
            return Ok();
        }
        
        [HttpGet]
        [ProducesResponseType(typeof(HealthCheckResponse), StatusCodes.Status200OK)]
        [Route("health")]
        public async Task<IActionResult> Health()
        {
            var heartbeatResponse = await _healthCheckService.CheckServices();
            
            return Ok(heartbeatResponse);
        }
    }
}