using Airslip.Common.Auth.Implementations;
using Airslip.Common.Auth.Interfaces;
using Airslip.Common.Auth.Models;
using Airslip.Common.Auth.Schemes;
using Airslip.Common.Contracts;
using Airslip.Common.Repository.Enums;
using Airslip.Common.Repository.Interfaces;
using Airslip.Common.Repository.Models;
using Airslip.Identity.Api.Application.Interfaces;
using Airslip.Identity.Api.Contracts.Entities;
using Airslip.Identity.Api.Contracts.Models;
using QRCoder;
using SharpCompress.Common;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Application.Implementations
{
    public class QrCodeService : IQrCodeService
    {
        private readonly IRepository<QrCode, QrCodeModel> _repository;
        private readonly ITokenService<QrCodeToken, GenerateQrCodeToken> _tokenService;
        private readonly ITokenService<UserToken, GenerateUserToken> _userTokenService;
        private readonly IModelMapper<QrCodeModel> _modelMapper;
        private readonly ITokenValidator<QrCodeToken, GenerateQrCodeToken> _tokenValidator;

        public QrCodeService(
            IRepository<QrCode, QrCodeModel> repository, 
            ITokenService<QrCodeToken, GenerateQrCodeToken> tokenService,
            IModelMapper<QrCodeModel> modelMapper,
            ITokenValidator<QrCodeToken, GenerateQrCodeToken> tokenValidator,
            ITokenService<UserToken, GenerateUserToken> userTokenService)
        {
            _repository = repository;
            _tokenService = tokenService;
            _modelMapper = modelMapper;
            _tokenValidator = tokenValidator;
            _userTokenService = userTokenService;
        }
        
        public async Task<RepositoryActionResultModel<QrCodeModel>> CreateNewQrCode(CreateQrCodeModel createQrCodeModel)
        {
            UserToken userToken = _userTokenService.GetCurrentToken();
            QrCodeModel qrCodeModel = _modelMapper.Create(createQrCodeModel);

            // Allocate a new key value, we will use the existing refresh token
            //   logic as the user will never see this value
            qrCodeModel.EntityId = userToken.EntityId;
            qrCodeModel.AirslipUserType = userToken.AirslipUserType;
            qrCodeModel.KeyValue = JwtBearerToken.GenerateRefreshToken();
            
            RepositoryActionResultModel<QrCodeModel> result = await _repository.Add(qrCodeModel);

            if (result.ResultType == ResultType.Success)
            {
                GenerateQrCodeToken generateQrCodeToken = new(qrCodeModel.StoreId ?? "",
                    qrCodeModel.CheckoutId ?? "",
                    userToken.EntityId, 
                    userToken.AirslipUserType, 
                    qrCodeModel.KeyValue);

                NewToken newToken = _tokenService.GenerateNewToken(generateQrCodeToken);

                result.CurrentVersion!.TokenValue = newToken.TokenValue;                
            }

            return result;
        }

        public async Task<GenerateQrCodeImageModel> GenerateQrCodeImage(string qrCodeToGenerate)
        {
            try
            {
                ClaimsPrincipal? newToken = await _tokenValidator.GetClaimsPrincipalFromToken(qrCodeToGenerate,
                    QrCodeAuthenticationSchemeOptions.QrCodeAuthScheme,
                    QrCodeAuthenticationSchemeOptions.ThisEnvironment);

                if (newToken == null || !newToken.Claims.Any()) 
                    return new GenerateQrCodeImageModel(false, null);
            }
            catch (ArgumentException)
            {
                return new GenerateQrCodeImageModel(false, null);
            }
            
            // Generate the URL payload
            string qrCodeUrl = $"https://connect.airslip.com?{qrCodeToGenerate}";
            PayloadGenerator.Url generator = new(qrCodeUrl);
            string payload = generator.ToString();
            
            // Generate the image
            QRCodeGenerator qrGenerator = new();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);
            
            // Save to a memory stream
            MemoryStream memoryStream = new();
            qrCodeImage.Save(memoryStream, ImageFormat.Jpeg);
            memoryStream.Position = 0;
            
            return new GenerateQrCodeImageModel(true, memoryStream);
        }
    }
}