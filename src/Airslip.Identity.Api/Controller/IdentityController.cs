using Airslip.Common.Contracts;
using Airslip.Common.Types;
using Airslip.Common.Types.Enums;
using Airslip.Common.Types.Failures;
using Airslip.Identity.Api.Application.Commands;
using Airslip.Identity.Api.Auth;
using Airslip.Identity.Api.Contracts.Requests;
using Airslip.Identity.Api.Contracts.Responses;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Controller
{
    [ApiController]
    [Route("v1/identity/")]
    public class IdentityController : ApiResponse
    {
        private readonly IMediator _mediator;

        public IdentityController(
            Token token,
            IOptions<PublicApiSettings> publicApiOptions,
            IMediator mediator) : base(token, publicApiOptions)
        {
            _mediator = mediator;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType( typeof(AuthenticatedUserResponse), StatusCodes.Status200OK)]
        [ProducesResponseType( typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> IdentityLogin(LoginRequest request)
        {
            GenerateJwtBearerTokenCommand generateJwtBearerTokenCommand = new(
                request.Email,
                request.Password);

            IResponse getUserByEmailResponse = await _mediator.Send(generateJwtBearerTokenCommand);

            return getUserByEmailResponse switch
            {
                AuthenticatedUserResponse response => Ok(response.AddHateoasLinks(
                    BaseUri,
                    response.HasAddedInstitution,
                    Alpha2CountryCodes.GB.ToString())),
                NotFoundResponse response => NotFound(response),
                ErrorResponse response => BadRequest(response),
                IFail r => BadRequest(r),
                _ => throw new InvalidOperationException()
            };
        }
    }
}