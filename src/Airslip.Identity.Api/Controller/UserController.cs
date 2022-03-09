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
using Airslip.Identity.Api.Application.Implementations;
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
    public class UserController : ApiControllerBase
    {
        private readonly IUserService _userService;
        private readonly IUserSearchService _userSearchService;

        public UserController(
            ITokenDecodeService<UserToken> tokenDecodeService, 
            IOptions<PublicApiSettings> publicApiOptions, 
            IUserService userService, 
            IUserSearchService userSearchService,
            ILogger logger) : 
            base(tokenDecodeService, publicApiOptions, logger)
        {
            _userService = userService;
            _userSearchService = userSearchService;
        }
        
        [HttpGet("")]
        [ProducesResponseType(typeof(SuccessfulActionResultModel<UserModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(FailedActionResultModel<UserModel>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(NotFoundResponse),StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get()
        {
            IResponse response = await _userService.Get(Token.UserId);

            return HandleResponse<SuccessfulActionResultModel<UserModel>>(response);
        }
        
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(SuccessfulActionResultModel<UserModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(FailedActionResultModel<UserModel>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(NotFoundResponse),StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [JwtAuthorize(ApplicationRoles = ApplicationRoles.UserManager)]
        public async Task<IActionResult> Get([FromRoute] string id)
        {
            IResponse response = await _userService.Get(id);

            return HandleResponse<SuccessfulActionResultModel<UserModel>>(response);
        }
        
        [HttpGet("all")]
        [ProducesResponseType(typeof(SuccessfulActionResultModel<UserModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(FailedActionResultModel<UserModel>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(NotFoundResponse),StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [JwtAuthorize(ApplicationRoles = ApplicationRoles.UserManager)]
        public async Task<IActionResult> GetAll()
        {
            IResponse response = await _userSearchService.FindUsers();

            return HandleResponse<UserSearchResults>(response);
        }
        
        [HttpPost("{id}")]
        [ProducesResponseType(typeof(SuccessfulActionResultModel<UserModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(FailedActionResultModel<UserModel>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(NotFoundResponse),StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [JwtAuthorize(ApplicationRoles = ApplicationRoles.UserManager)]
        public async Task<IActionResult> Update([FromRoute] string id, [FromBody] UserModel model)
        {
            IResponse response = await _userService.Update(id, model);

            return HandleResponse<SuccessfulActionResultModel<UserModel>>(response);
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
            
            IResponse response = await _userService.Add(registerUserCommand, CancellationToken.None);

            return HandleResponse<SuccessfulActionResultModel<UserModel>>(response);
        }
        
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(SuccessfulActionResultModel<UserModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(FailedActionResultModel<UserModel>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(NotFoundResponse),StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [JwtAuthorize(ApplicationRoles = ApplicationRoles.UserManager)]
        public async Task<IActionResult> Delete([FromRoute] string id)
        {
            IResponse response = await _userService.Delete(id);

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
            IResponse response = await _userService.SetRole(id, roleName);

            return HandleResponse<SuccessfulActionResultModel<UserModel>>(response);
        }
    }
}