using Airslip.Common.Auth.Data;
using Airslip.Common.Auth.Interfaces;
using Airslip.Common.Auth.Models;
using Airslip.Common.Types;
using Airslip.Common.Types.Configuration;
using Airslip.Common.Types.Failures;
using Airslip.Common.Types.Interfaces;
using Airslip.Identity.Api.Application.Interfaces;
using Airslip.Identity.Api.Contracts.Models;
using Airslip.Common.Auth.AspNetCore.Attributes;
using Airslip.Common.Auth.AspNetCore.Implementations;
using Airslip.Common.Repository.Types.Models;
using Airslip.Identity.Api.Application.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Serilog;
using System.Threading;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Controller
{
    [ApiController]
    [ApiVersion(ApiConstants.VersionOne)]
    [Route("v{version:apiVersion}/user")]
    public class LifecycleController : ApiControllerBase
    {
        private readonly IUserLifecycle _userLifecycle;

        public LifecycleController(
            ITokenDecodeService<UserToken> tokenDecodeService, 
            IOptions<PublicApiSettings> publicApiOptions, 
            IUserLifecycle userLifecycle,
            ILogger logger) : 
            base(tokenDecodeService, publicApiOptions, logger)
        {
            _userLifecycle = userLifecycle;
        }
        
        [HttpPut("")]
        [ProducesResponseType(typeof(SuccessfulActionResultModel<UserModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(FailedActionResultModel<UserModel>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(NotFoundResponse),StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [JwtAuthorize(ApplicationRoles = ApplicationRoles.UserManager)]
        public async Task<IActionResult> Add([FromBody]UserAddModel request)
        {
            RegisterUserCommand registerUserCommand = new(
                request.Email,
                request.FirstName,
                request.LastName,
                null,
                null,
                request.UserRole);
            
            IResponse response = await _userLifecycle.Add(registerUserCommand, CancellationToken.None);

            return HandleResponse<SuccessfulActionResultModel<UserModel>>(response);
        }
        
        [HttpPost("{id}/setrole/{roleName}")]
        [ProducesResponseType(typeof(SuccessfulActionResultModel<UserModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(FailedActionResultModel<UserModel>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(NotFoundResponse),StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [JwtAuthorize(ApplicationRoles = ApplicationRoles.UserManager)]
        public async Task<IActionResult> SetRole([FromRoute] string id, [FromRoute] string roleName)
        {
            IResponse response = await _userLifecycle.SetRole(id, roleName);

            return HandleResponse<SuccessfulActionResultModel<UserModel>>(response);
        }
    }
}