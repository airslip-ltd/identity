using Airslip.Common.Auth.AspNetCore.Implementations;
using Airslip.Common.Auth.Interfaces;
using Airslip.Common.Auth.Models;
using Airslip.Common.Repository.Types.Models;
using Airslip.Common.Types;
using Airslip.Common.Types.Configuration;
using Airslip.Common.Types.Failures;
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
        [ProducesResponseType(typeof(EntitySearchResponse<ApiKeyModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse),StatusCodes.Status400BadRequest)]
        [Route("search")]
        public async Task<IActionResult> Search([FromBody] EntitySearchQueryModel query)
        {
            IResponse response = await _apiKeyService.Search(query);

            return HandleResponse<EntitySearchResponse<ApiKeyModel>>(response);
        }
        
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(SuccessfulActionResultModel<ApiKeyModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(FailedActionResultModel<ApiKeyModel>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(NotFoundResponse),StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get([FromRoute] string id)
        {
            IResponse response = await _apiKeyService.Get(id);

            return HandleResponse<SuccessfulActionResultModel<ApiKeyModel>>(response);
        }
        
        [HttpPost]
        [ProducesResponseType(typeof(RepositoryActionResultModel<ApiKeyModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RepositoryActionResultModel<ApiKeyModel>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Add([FromBody] CreateApiKeyModel model)
        {
            IResponse result = await _apiKeyService.CreateNewApiKey(model);
            
            return HandleResponse<SuccessfulActionResultModel<ApiKeyModel>>(result);
        }
        
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(SuccessfulActionResultModel<ApiKeyModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(FailedActionResultModel<ApiKeyModel>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete([FromRoute] string id)
        {
            IResponse response = await _apiKeyService.Delete(id);

            return HandleResponse<SuccessfulActionResultModel<ApiKeyModel>>(response);
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
