using Airslip.Common.Auth.AspNetCore.Implementations;
using Airslip.Common.Auth.Interfaces;
using Airslip.Common.Auth.Models;
using Airslip.Common.Repository.Types.Models;
using Airslip.Common.Types;
using Airslip.Common.Types.Configuration;
using Airslip.Identity.Api.Application.Interfaces;
using Airslip.Identity.Api.Contracts.Models;
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
    [Route("v{version:apiVersion}/qrcode")]
    public class QrCodeController : ApiControllerBase
    {
        private readonly IQrCodeService _qrCodeService;

        public QrCodeController(
            ITokenDecodeService<UserToken> tokenService,
            IOptions<PublicApiSettings> publicApiOptions,
            IQrCodeService qrCodeService, ILogger logger)
            : base(tokenService, publicApiOptions, logger)
        {
            _qrCodeService = qrCodeService;
        }
        
        [HttpPost]
        [Route("add")]
        [ProducesResponseType(typeof(RepositoryActionResultModel<QrCodeModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RepositoryActionResultModel<QrCodeModel>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Add([FromBody] CreateQrCodeModel model)
        {
            RepositoryActionResultModel<QrCodeModel> result;
            
            try
            {
                result = await _qrCodeService.CreateNewQrCode(model);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception when adding Api Key");
                return BadRequest(ex.Message);
            }
            
            return RepositoryActionToResult(result);
        }
    }
}
