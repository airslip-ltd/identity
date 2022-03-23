using Airslip.Common.Auth.Implementations;
using Airslip.Common.Auth.Interfaces;
using Airslip.Common.Auth.Models;
using Airslip.Common.Repository.Types.Enums;
using Airslip.Common.Repository.Types.Interfaces;
using Airslip.Common.Repository.Types.Models;
using Airslip.Identity.Api.Application.Interfaces;
using Airslip.Identity.Api.Contracts.Entities;
using Airslip.Identity.Api.Contracts.Models;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Application.Implementations
{
    public class QrCodeService : IQrCodeService
    {
        private readonly IRepository<QrCode, QrCodeModel> _repository;
        private readonly ITokenGenerationService<GenerateQrCodeToken> _tokenService;
        private readonly ITokenDecodeService<UserToken> _userTokenService;
        private readonly IModelMapper<QrCodeModel> _modelMapper;
        
        public QrCodeService(
            IRepository<QrCode, QrCodeModel> repository, 
            ITokenGenerationService<GenerateQrCodeToken> tokenService,
            IModelMapper<QrCodeModel> modelMapper,
            ITokenDecodeService<UserToken> userTokenService)
        {
            _repository = repository;
            _tokenService = tokenService;
            _modelMapper = modelMapper;
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

            if (result.ResultType != ResultType.Success) return result;
            
            GenerateQrCodeToken generateQrCodeToken = new(userToken.EntityId,
                qrCodeModel.StoreId ?? "",
                qrCodeModel.CheckoutId ?? "", 
                qrCodeModel.KeyValue,
                userToken.AirslipUserType);

            NewToken newToken = _tokenService.GenerateNewToken(generateQrCodeToken);

            result.CurrentVersion!.TokenValue = newToken.TokenValue;

            return result;
        }
    }
}