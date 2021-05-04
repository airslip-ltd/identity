using Airslip.Common.Contracts;
using Airslip.Common.Types;
using Airslip.Common.Types.Failures;
using Airslip.Identity.Api.Application.Queries;
using Airslip.Identity.Api.Auth;
using Airslip.Identity.Api.Contracts.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Controller
{
    [ApiController]
    [ApiVersion(ApiConstants.VersionOne)]
    // ReSharper disable once RouteTemplates.RouteParameterConstraintNotResolved
    [Route("v{version:apiVersion}/admin")]
    [ApiExplorerSettings(IgnoreApi = true)]
    // TODO: Add Admin role
    public class AdminController : ApiResponse
    {
        private readonly IMediator _mediator;

        public AdminController(
            IMediator mediator,
            Token token,
            IOptions<PublicApiSettings> publicApiOptions) : base(token, publicApiOptions)
        {
            _mediator = mediator;
        }

        [HttpGet("user")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType( typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetYapilyUser()
        {
            GetYapilyUserQuery command = new(Token.UserId);

            IResponse response = await _mediator.Send(command);

            return response is YapilyUserResponse yapilyUserResponse
                ? Ok(yapilyUserResponse.AddHateoasLinks<YapilyUserResponse>(BaseUri, Token.UserId))
                : BadRequest(response);
        }
    }
}