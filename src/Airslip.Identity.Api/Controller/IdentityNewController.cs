using Airslip.Common.Auth.AspNetCore.Interfaces;
using Airslip.Common.Auth.Interfaces;
using Airslip.Common.Auth.Models;
using Airslip.Common.Types.Interfaces;
using Airslip.Common.Types;
using Airslip.Common.Types.Configuration;
using Airslip.Common.Types.Failures;
using Airslip.Common.Utilities;
using Airslip.Identity.Api.Application.Interfaces;
using Airslip.Identity.Api.Contracts.Models;
using Airslip.Identity.Api.Contracts.Responses;
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
    // ReSharper disable once RouteTemplates.RouteParameterConstraintNotResolved
    [Route("v{version:apiVersion}/identity")]
    [Produces(Json.MediaType)]
    public class IdentityNewController : ApiControllerBase
    {
        private readonly IUnregisteredUserService _unregisteredUserService;
        private readonly IApiRequestAuthService _apiAuthService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IdentityNewController(
            ITokenDecodeService<UserToken> tokenService,
            ILogger logger,
            IOptions<PublicApiSettings> publicApiOptions,
            IUnregisteredUserService unregisteredUserService,
            IApiRequestAuthService apiAuthService,
            IHttpContextAccessor httpContextAccessor) : base(tokenService, publicApiOptions, logger)
        {
            _unregisteredUserService = unregisteredUserService;
            _apiAuthService = apiAuthService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("unregistered")]
        [ProducesResponseType( StatusCodes.Status204NoContent)]
        [ProducesResponseType( StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateRegisteredUser([FromBody] CreateUnregisteredUserModel model)
        {
            KeyAuthenticationResult authResult = await _apiAuthService.Handle(_httpContextAccessor.HttpContext!.Request);

            if (authResult.AuthResult == AuthResult.Fail)
                return BadRequest(authResult.Message);
            
            try
            {
                IResponse result = await _unregisteredUserService.Create(model);

                switch (result)
                {
                    case CreateUnregisteredUserResponse r:
                        r.GenerateLink(BaseUri, "identity/confirm-email");
                        return Ok(r);
                    case NotFoundResponse r:
                        return NotFound(r);
                    case IFail r:
                        return BadRequest(r);
                    default:
                        throw new InvalidOperationException();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception when creating unregistered user");
                return BadRequest(ex.Message);
            }
        }
        
        [Route("confirm-email")]
        [ProducesResponseType( StatusCodes.Status204NoContent)]
        [ProducesResponseType( StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string email, string token)
        {
            try
            {
                IResponse result = await _unregisteredUserService.ConfirmEmail(email, token);

                return CommonResponseHandler<Success>(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception when creating unregistered user");
                return BadRequest(ex.Message);
            }
        }
    }
}