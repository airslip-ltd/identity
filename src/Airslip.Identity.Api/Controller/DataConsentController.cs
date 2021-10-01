using Airslip.Common.Auth.Interfaces;
using Airslip.Common.Auth.Models;
using Airslip.Common.Types;
using Airslip.Common.Types.Configuration;
using Airslip.Identity.Api.Application.Interfaces;
using Airslip.Identity.Api.Contracts.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Controller
{
    [ApiController]
    [ApiVersion(ApiConstants.VersionOne)]
    [Route("v{version:apiVersion}/users/data-consent")]
    public class DataConsentController : ApiResponse
    {
        private readonly IDataConsentService _dataConsentService;
        private readonly ILogger _logger;

        public DataConsentController(
            ITokenDecodeService<UserToken> tokenService,
            IOptions<PublicApiSettings> publicApiOptions,
            IDataConsentService dataConsentService, 
            ILogger logger)
            : base(tokenService, publicApiOptions, logger)
        {
            _dataConsentService = dataConsentService;
            _logger = logger;
        }
        
        [HttpPost]
        [ProducesResponseType( StatusCodes.Status204NoContent)]
        [ProducesResponseType( StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Update([FromBody] DataConsentModel model)
        {
            try
            {
                await _dataConsentService.Update(model);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception when updating data consent");
                return BadRequest(ex.Message);
            }
        }
    }
}
