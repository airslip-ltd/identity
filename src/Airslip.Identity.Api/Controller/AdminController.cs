using Airslip.BankTransactions.Api.Application.Admin;
using Airslip.BankTransactions.Api.Application.Transactions;
using Airslip.BankTransactions.Api.Auth;
using Airslip.BankTransactions.Api.Contracts.Requests;
using Airslip.BankTransactions.Api.Contracts.Responses;
using Airslip.Common.Contracts;
using Airslip.Common.Types;
using Airslip.Common.Types.Failures;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;

namespace Airslip.BankTransactions.Api.Controllers
{
    [ApiController]
    [Route("v1/admin")]
    [ApiExplorerSettings(IgnoreApi = true)]
    // TODO: Add Admin role
    [AllowAnonymous]
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

        [HttpPost("merchants/{merchantName}/logo")]
        [SwaggerResponse(StatusCodes.Status200OK, "A successful response, returning an AccountsResponse.    ",
            typeof(AccountTransactionsResponse))]
        public async Task<IActionResult> UploadCompanyLogo(string merchantName,
            [FromForm(Name = "merchant-logo")] IFormFile merchantLogo)
        {
            AddMerchantLogoCommand command = new(
                Token.CorrelationId,
                merchantName,
                merchantLogo);

            IResponse response = await _mediator.Send(command);

            return response is ISuccess
                ? NoContent()
                : BadRequest(response);
        }

        [HttpPost("institutions/{countryCode}")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "A successful response, returning no content.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Returned if there is an issue with the request.",
            typeof(ErrorResponse))]
        public async Task<IActionResult> CreateInstitutions([FromRoute] string countryCode)
        {
            CreateInstitutionsCommand command = new(countryCode);

            IResponse response = await _mediator.Send(command);

            return response is ISuccess
                ? NoContent()
                : BadRequest(response);
        }

        [HttpGet("user")]
        [SwaggerResponse(StatusCodes.Status204NoContent, "A successful response, returning no content.")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Returned if there is an issue with the request.",
            typeof(ErrorResponse))]
        public async Task<IActionResult> GetYapilyUser()
        {
            GetYapilyUserQuery command = new(Token.UserId);

            IResponse response = await _mediator.Send(command);

            return response is YapilyUserResponse yapilyUserResponse
                ? Ok(yapilyUserResponse.AddHateoasLinks<YapilyUserResponse>(BaseUri, Token.UserId))
                : BadRequest(response);
        }

        [HttpPost("retry-indexing-transactions")]
        [SwaggerResponse(StatusCodes.Status200OK, "A successful response.",
            typeof(AccountResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Returned if there is an issue with the request.",
            typeof(ErrorResponse))]
        public async Task<IActionResult> RetryIndexing([FromBody] AdminRetryTransactionIndexingRequest request)
        {
            RetryTransactionIndexingCommand command = new(
                request.UserId,
                request.AccountId,
                request.CorrelationId,
                request.Metadata);

            IResponse response = await _mediator.Send(command);

            return response is ISuccess
                ? Ok()
                : BadRequest(response);
        }
    }
}