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
    [ApiVersion(ApiConstants.VersionOne)]
    // ReSharper disable once RouteTemplates.RouteParameterConstraintNotResolved
    [Route("v{version:apiVersion}/identity")]
    [Produces(ApiConstants.JsonMediaType)]
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
        
        // [HttpPost("register")]
        // [ProducesResponseType(typeof(AuthenticatedUserResponse), StatusCodes.Status200OK)]
        // [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        // [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
        // public async Task<IActionResult> IdentityRegister(LoginRequest request)
        // {
        //     CreateUserCommand createUserCommand = new(
        //         request.Email
        //     );
        //
        //     IResponse createUserResponse = await _mediator.Send(createUserCommand);
        //
        //     switch (createUserResponse)
        //     {
        //         case UserResponse userResponse:
        //
        //             ApplicationUser user = new() {UserName = request.Email, Email = request.Email};
        //
        //             IdentityResult? result = await _userManager.CreateAsync(user, request.Password);
        //
        //             if (!result.Succeeded)
        //                 return result.Errors.First().Code switch
        //                 {
        //                     "DuplicateUserName" => Conflict(new ConflictResponse(nameof(request.Email), request.Email,
        //                         "User already exists")),
        //                     _ => BadRequest(new ErrorResponse(result.Errors.First().Code,
        //                         result.Errors.First().Description))
        //                 };
        //
        //             await _signInManager.SignInAsync(user, true);
        //
        //             await AddClaims(userResponse.UserId);
        //
        //             return Ok(new AuthenticatedUserResponse().AddHateoasLinks(
        //                 BaseUri,
        //                 userResponse.HasAddedInstitution,
        //                 Alpha2CountryCodes.GB.ToString()));
        //
        //         case ConflictResponse r:
        //             return Conflict(r);
        //         case IFail r:
        //             return BadRequest(r);
        //         default:
        //             throw new InvalidOperationException();
        //     }
        // }
    }
}