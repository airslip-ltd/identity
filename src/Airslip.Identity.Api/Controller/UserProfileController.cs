﻿using Airslip.Common.Auth.Interfaces;
using Airslip.Common.Auth.Models;
using Airslip.Common.Types.Interfaces;
using Airslip.Common.Types;
using Airslip.Common.Types.Configuration;
using Airslip.Common.Types.Failures;
using Airslip.Identity.Api.Application.UserProfiles;
using Airslip.Identity.Api.Contracts.Requests;
using Airslip.Identity.Api.Contracts.Responses;
using MediatR;
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
    [Route("v{version:apiVersion}/profile")]
    [Produces(Json.MediaType)]
    public class UserProfileController : ApiResponse
    {
        private readonly IMediator _mediator;

        public UserProfileController(
            ITokenDecodeService<UserToken> tokenService,
            ILogger logger,
            IOptions<PublicApiSettings> publicApiOptions,
            IMediator mediator) : base(tokenService, publicApiOptions, logger)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(UserProfileResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetUser()
        {
            GetUserProfileQuery query = new(
                Token.UserId
            );

            IResponse response = await _mediator.Send(query);

            return response is UserProfileResponse accountResponse
                ? Ok(accountResponse.AddHateoasLinks<UserProfileResponse>(_publicApiSettings.Base.BaseUri))
                : BadRequest(response);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserProfileRequest request)
        {
            UpdateUserProfileCommand command = new(
                Token.UserId,
                request.FirstName,
                request.Surname,
                request.Gender,
                request.DateOfBirth,
                request.Postalcode,
                request.FirstLineAddress,
                request.SecondLineAddress,
                request.City,
                request.County,
                request.Country);

            IResponse response = await _mediator.Send(command);

            return response is ISuccess
                ? NoContent()
                : BadRequest(response);
        }
        
        [HttpPost("photo")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdatePhoto([FromForm(Name = "photo")] IFormFile photo)
        {
            UpdateUserProfilePhotoCommand command = new(
                Token.UserId,
                photo);

            IResponse response = await _mediator.Send(command);

            return response is ISuccess
                ? NoContent()
                : BadRequest(response);
        }

        [HttpGet("photo")]
        [ProducesResponseType( typeof(FileStreamResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetPhoto()
        {
            GetUserProfilePhotoQuery query = new(
                Token.UserId);

            IResponse response = await _mediator.Send(query);

            return response switch
            {
                StreamResponse streamResponse => File(streamResponse.Stream, streamResponse.ContentType),
                ISuccess => NoContent(),
                IFail failure => BadRequest(failure),
                _ => throw new InvalidOperationException()
            };
        }
    }
}