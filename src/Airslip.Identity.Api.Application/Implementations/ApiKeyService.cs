using Airslip.Common.Auth.Extensions;
using Airslip.Common.Auth.Implementations;
using Airslip.Common.Auth.Interfaces;
using Airslip.Common.Auth.Models;
using Airslip.Common.Repository.Enums;
using Airslip.Common.Repository.Interfaces;
using Airslip.Common.Repository.Models;
using Airslip.Identity.Api.Application.Interfaces;
using Airslip.Identity.Api.Application.Validators;
using Airslip.Identity.Api.Contracts.Config;
using Airslip.Identity.Api.Contracts.Entities;
using Airslip.Identity.Api.Contracts.Models;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Application.Implementations
{
    public class ApiKeyService : IApiKeyService
    {
        private readonly IApiKeyRepository _repository;
        private readonly IModelMapper<ApiKeyModel> _modelMapper;
        private readonly ITokenGenerationService<GenerateApiKeyToken> _apiKeyTokenService;
        private readonly ITokenDecodeService<UserToken> _userTokenService;
        private readonly ApiKeyValidationSettings _validationSettings;

        public ApiKeyService(IApiKeyRepository repository, IModelMapper<ApiKeyModel> modelMapper,
            ITokenGenerationService<GenerateApiKeyToken> apiKeyTokenService,
            ITokenDecodeService<UserToken> userTokenService, IOptions<ApiKeyValidationSettings> options)
        {
            _repository = repository;
            _modelMapper = modelMapper;
            _apiKeyTokenService = apiKeyTokenService;
            _userTokenService = userTokenService;
            _validationSettings = options.Value;
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

        public async Task<ApiKeyValidationResultModel> ValidateApiKey(ApiKeyValidationModel model)
        {
            var validator = new ApiKeyValidationModelValidator();

            if (model.VerificationToken == null || !model.VerificationToken.Equals(_validationSettings.VerificationToken))
            {
                return new ApiKeyValidationResultModel(false, "Invalid verification token");
            }

            var validationResult = await validator.ValidateAsync(model);

            if (!validationResult.IsValid)
            {
                return new ApiKeyValidationResultModel(false, validationResult.Errors.First().ErrorMessage);
            }
            
            var apiKey = await _repository.GetApiKeyByKeyValue(model.ApiKey!);
            
            if (apiKey == null) return new ApiKeyValidationResultModel(false, 
                "ApiKey not found");
            if (apiKey.EntityId != model.EntityId) return new ApiKeyValidationResultModel(false, 
                "EntityId doesn't match that found on API Key");
            if (apiKey.AirslipUserType != model.AirslipUserType) return new ApiKeyValidationResultModel(false, 
                "AirslipUserType doesn't match that found on API Key");
            if (apiKey.EntityStatus == EntityStatus.Deleted) return new ApiKeyValidationResultModel(false, 
                "ApiKey has expired");

            return new ApiKeyValidationResultModel(true, "");
        }
    }
}