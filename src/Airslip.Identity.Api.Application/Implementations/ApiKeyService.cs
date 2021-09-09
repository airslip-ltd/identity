using Airslip.Common.Auth.Enums;
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
        private readonly ITokenService<ApiKeyToken, GenerateApiKeyToken> _tokenService;

        public ApiKeyService(IRepository<ApiKey, ApiKeyModel> repository, IModelMapper<ApiKeyModel> modelMapper,
            ITokenService<ApiKeyToken, GenerateApiKeyToken> tokenService)
        {
            _repository = repository;
            _modelMapper = modelMapper;
            _tokenService = tokenService;
        }

        public async Task<RepositoryActionResultModel<ApiKeyModel>> CreateNewApiKey(CreateApiKeyModel createApiKeyModel)
        {
            ApiKeyModel apiKeyModel = _modelMapper.Create(createApiKeyModel);

            // Allocate a new key value, we will use the existing refresh token
            //   logic as the user will never see this value
            apiKeyModel.KeyValue = JwtBearerToken.GenerateRefreshToken();
            
            RepositoryActionResultModel<ApiKeyModel> result = await _repository.Add(apiKeyModel);

            if (result.ResultType == ResultTypeEnum.Success)
            {
                GenerateApiKeyToken generateApiKeyToken = new(apiKeyModel.KeyValue, "", 
                    ApiKeyUsageType.Merchant);

                NewToken newToken = _tokenService.GenerateNewToken(generateApiKeyToken);

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