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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Serilog;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Controller
{
    [ApiController]
    [ApiVersion(ApiConstants.VersionOne)]
    [Route("v{version:apiVersion}/user")]
    public class UserController : ApiControllerBase
    {
        private readonly IUserService _userService;

        public UserController(
            ITokenDecodeService<UserToken> tokenDecodeService, 
            IOptions<PublicApiSettings> publicApiOptions, 
            IUserService userService, 
            ILogger logger) : 
            base(tokenDecodeService, publicApiOptions, logger)
        {
            _userService = userService;
        }
        
        [HttpGet("")]
        [ProducesResponseType(typeof(SuccessfulActionResultModel<UserModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(FailedActionResultModel<UserModel>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(NotFoundResponse),StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetMyDetails()
        {
            IResponse response = await _userService.Get(Token.UserId);

            return HandleResponse<SuccessfulActionResultModel<UserModel>>(response);
        }
        
        [HttpPut("")]
        [ProducesResponseType(typeof(SuccessfulActionResultModel<ProfileModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(FailedActionResultModel<ProfileModel>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(NotFoundResponse),StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateMyDetails([FromBody] ProfileModel model)
        {
            IResponse response = await _userService.UpdateMyDetails(model);

            return HandleResponse<SuccessfulActionResultModel<ProfileModel>>(response);
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
        
        [HttpPost]
        [ProducesResponseType(typeof(EntitySearchResponse<UserModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse),StatusCodes.Status400BadRequest)]
        [JwtAuthorize(ApplicationRoles = ApplicationRoles.UserManager)]
        [Route("search")]
        public async Task<IActionResult> Search([FromBody] EntitySearchQueryModel query)
        {
            IResponse response = await _userService.Search(query);

            return HandleResponse<EntitySearchResponse<UserModel>>(response);
        }
        
        [HttpPut("{id}")]
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
        
        [HttpPost()]
        [ProducesResponseType(typeof(SuccessfulActionResultModel<UserModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(FailedActionResultModel<UserModel>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(NotFoundResponse),StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        [JwtAuthorize(ApplicationRoles = ApplicationRoles.UserManager)]
        public async Task<IActionResult> Create([FromBody] UserModel model)
        {
            IResponse response = await _userService.Create(model);

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
    }
}