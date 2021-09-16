using Airslip.Common.Auth.Interfaces;
using Airslip.Common.Auth.Models;
using Airslip.Common.Repository.Enums;
using Airslip.Common.Repository.Models;
using Airslip.Common.Types.Configuration;
using Airslip.Identity.Api.Application.Interfaces;
using Airslip.Identity.Api.Contracts.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Controller
{
    [ApiController]
    [ApiVersion(ApiConstants.VersionOne)]
    [Route("v{version:apiVersion}/qrcode")]
    public class QrCodeController : ApiResponse
    {
        private readonly IQrCodeService _qrCodeService;
        private readonly ILogger _logger;

        public QrCodeController(
            ITokenDecodeService<UserToken> tokenService,
            IOptions<PublicApiSettings> publicApiOptions,
            IQrCodeService qrCodeService, ILogger logger)
            : base(tokenService, publicApiOptions, logger)
        {
            _qrCodeService = qrCodeService;
            _logger = logger;
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
        
        [HttpGet]
        [Route("image")]
        [ProducesResponseType(typeof(RepositoryActionResultModel<QrCodeModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(RepositoryActionResultModel<QrCodeModel>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Image([FromQuery(Name = "qrCode")] string qrCode)
        {
            var generatedImage = await _qrCodeService.GenerateQrCodeImage(qrCode);
            
            if (generatedImage.Success)
            {
                string contentType = "image/jpg";
                string fileName = "qrcode.jpg";
                return File(generatedImage.imageStream, contentType, fileName);
            }

            return BadRequest();
        }
    }
}
