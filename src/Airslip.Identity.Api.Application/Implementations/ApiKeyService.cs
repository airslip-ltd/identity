using Airslip.Common.Auth.Implementations;
using Airslip.Common.Auth.Interfaces;
using Airslip.Common.Auth.Models;
using Airslip.Common.Repository.Enums;
using Airslip.Common.Repository.Interfaces;
using Airslip.Common.Repository.Models;
using Airslip.Identity.Api.Application.Interfaces;
using Airslip.Identity.Api.Contracts.Entities;
using Airslip.Identity.Api.Contracts.Models;
using System;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Application.Implementations
{
    public class ApiKeyService : IApiKeyService
    {
        private readonly IRepository<ApiKey, ApiKeyModel> _repository;
        private readonly IModelMapper<ApiKeyModel> _modelMapper;
        private readonly ITokenGenerationService<GenerateApiKeyToken> _apiKeyTokenService;
        private readonly ITokenDecodeService<UserToken> _userTokenService;

        public ApiKeyService(IRepository<ApiKey, ApiKeyModel> repository, IModelMapper<ApiKeyModel> modelMapper,
            ITokenGenerationService<GenerateApiKeyToken> apiKeyTokenService,
            ITokenDecodeService<UserToken> userTokenService)
        {
            _repository = repository;
            _modelMapper = modelMapper;
            _apiKeyTokenService = apiKeyTokenService;
            _userTokenService = userTokenService;
        }

        public async Task<RepositoryActionResultModel<ApiKeyModel>> CreateNewApiKey(CreateApiKeyModel createApiKeyModel)
        {
            UserToken userToken = _userTokenService.GetCurrentToken();
            ApiKeyModel apiKeyModel = _modelMapper.Create(createApiKeyModel);

            // Allocate a new key value, we will use the existing refresh token
            //   logic as the user will never see this value
            apiKeyModel.KeyValue = JwtBearerToken.GenerateRefreshToken();
            apiKeyModel.EntityId = userToken.EntityId;
            apiKeyModel.AirslipUserType = userToken.AirslipUserType;
            
            RepositoryActionResultModel<ApiKeyModel> result = await _repository.Add(apiKeyModel);

            if (result.ResultType == ResultType.Success)
            {
                GenerateApiKeyToken generateApiKeyToken = new(userToken.EntityId, 
                    apiKeyModel.KeyValue, 
                    userToken.AirslipUserType);

                NewToken newToken = _apiKeyTokenService.GenerateNewToken(generateApiKeyToken);

                result.CurrentVersion!.TokenValue = newToken.TokenValue;                
            }

            return result;
        }

        public Task<RepositoryActionResultModel<ApiKeyModel>> ExpireApiKey(string id)
        {
            // TODO: Add deletion logic
            throw new NotImplementedException();
        }
    }
}