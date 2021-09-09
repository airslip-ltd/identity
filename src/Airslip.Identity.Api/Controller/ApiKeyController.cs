using Airslip.Common.Auth.Interfaces;
using Airslip.Common.Auth.Models;
using Airslip.Common.Repository.Enums;
using Airslip.Common.Repository.Models;
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
    [Route("v{version:apiVersion}/apikey")]
    public class ApiKeyController : ApiResponse
    {
        private readonly IApiKeyService _apiKeyService;
        private readonly ILogger _logger;

        public ApiKeyController(
            ITokenService<UserToken, GenerateUserToken> tokenService,
            IOptions<PublicApiSettings> publicApiOptions,
            IApiKeyService apiKeyService, ILogger logger)
            : base(tokenService, publicApiOptions, logger)
        {
            _apiKeyService = apiKeyService;
            _logger = logger;
        }
        
        [HttpPost]
        [Route("add")]
        [ProducesResponseType(typeof(RepositoryActionResultModel<ApiKeyModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RepositoryActionResultModel<ApiKeyModel>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Add([FromBody] CreateApiKeyModel model)
        {
            RepositoryActionResultModel<ApiKeyModel> result;
            
            try
            {
                result = await _apiKeyService.CreateNewApiKey(model);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception when adding Api Key");
                return BadRequest(ex.Message);
            }
            
            return _actionToResult(result);
        }
        
        [HttpGet]
        [Route("expire/{id}")]
        [ProducesResponseType(typeof(RepositoryActionResultModel<ApiKeyModel>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(RepositoryActionResultModel<ApiKeyModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RepositoryActionResultModel<ApiKeyModel>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(string id)
        {
            RepositoryActionResultModel<ApiKeyModel> result;
            
            try
            {
                result = await _apiKeyService.ExpireApiKey(id);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception when deleting Api Key");
                return BadRequest(ex.Message);
            }

            return _actionToResult(result);
        }
        
        private IActionResult _actionToResult(RepositoryActionResultModel<ApiKeyModel> theResult)
        {
            // Dependent on the return type, we will return either BadRequest or OK
            if (theResult.ResultType == ResultTypeEnum.NotFound)
            {
                return NotFound(theResult);
            } 
            if (theResult.ResultType == ResultTypeEnum.FailedValidation || theResult.ResultType == ResultTypeEnum.FailedVerification)
            {
                return BadRequest(theResult);
            }

            return Ok(theResult);
        }
    }
}
