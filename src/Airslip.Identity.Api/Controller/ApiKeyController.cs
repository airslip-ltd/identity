using Airslip.Common.Auth.AspNetCore.Implementations;
using Airslip.Common.Auth.Interfaces;
using Airslip.Common.Auth.Models;
using Airslip.Common.Repository.Types.Models;
using Airslip.Common.Types;
using Airslip.Common.Types.Configuration;
using Airslip.Common.Types.Interfaces;
using Airslip.Identity.Api.Application.Interfaces;
using Airslip.Identity.Api.Contracts.Models;
using Microsoft.AspNetCore.Authorization;
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
    [Route("v{version:apiVersion}/apikey")]
    public class ApiKeyController : ApiControllerBase
    {
        private readonly IApiKeyService _apiKeyService;

        public ApiKeyController(
            ITokenDecodeService<UserToken> tokenService,
            IOptions<PublicApiSettings> publicApiOptions,
            IApiKeyService apiKeyService, ILogger logger)
            : base(tokenService, publicApiOptions, logger)
        {
            _apiKeyService = apiKeyService;
        }
        
        [HttpPost]
        [Route("add")]
        [ProducesResponseType(typeof(RepositoryActionResultModel<ApiKeyModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RepositoryActionResultModel<ApiKeyModel>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Add([FromBody] CreateApiKeyModel model)
        {
            IResponse result;
            
            try
            {
                result = await _apiKeyService.CreateNewApiKey(model);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception when adding Api Key");
                return BadRequest(ex.Message);
            }
            
            return HandleResponse<SuccessfulActionResultModel<ApiKeyModel>>(result);
        }
        
        [HttpGet]
        [Route("expire/{id}")]
        [ProducesResponseType(typeof(RepositoryActionResultModel<ApiKeyModel>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(RepositoryActionResultModel<ApiKeyModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RepositoryActionResultModel<ApiKeyModel>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(string id)
        {
            IResponse result;
            
            try
            {
                result = await _apiKeyService.ExpireApiKey(id);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception when deleting Api Key");
                return BadRequest(ex.Message);
            }

            return HandleResponse<SuccessfulActionResultModel<ApiKeyModel>>(result);
        }
        
        [HttpPost]
        [AllowAnonymous]
        [Route("validate")]
        [ProducesResponseType(typeof(ApiKeyValidationResultModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> Validate(ApiKeyValidationModel model)
        {
            IResponse result;
            
            try
            {
                result = await _apiKeyService.ValidateApiKey(model);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return HandleResponse<ApiKeyValidationResultModel>(result);
        }
    }
}
