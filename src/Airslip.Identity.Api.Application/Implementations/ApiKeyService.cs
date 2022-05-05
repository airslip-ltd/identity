using Airslip.Common.Auth.Data;
using Airslip.Common.Auth.Implementations;
using Airslip.Common.Auth.Interfaces;
using Airslip.Common.Auth.Models;
using Airslip.Common.Repository.Types.Enums;
using Airslip.Common.Repository.Types.Interfaces;
using Airslip.Common.Repository.Types.Models;
using Airslip.Common.Types.Configuration;
using Airslip.Common.Types.Interfaces;
using Airslip.Identity.Api.Application.Interfaces;
using Airslip.Identity.Api.Application.Validators;
using Airslip.Identity.Api.Contracts.Entities;
using Airslip.Identity.Api.Contracts.Models;
using FluentValidation.Results;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Application.Implementations
{
    public class ApiKeyService : IApiKeyService
    {
        private readonly IApiKeyRepository _repository;
        private readonly IModelMapper<ApiKeyModel> _modelMapper;
        private readonly IUserLifecycle _userLifecycle;
        private readonly ITokenGenerationService<GenerateApiKeyToken> _apiKeyTokenService;
        private readonly UserToken _userToken;
        private readonly IEntitySearch<ApiKeyModel> _apiKeySearch;
        private readonly ApiKeyValidationSettings _validationSettings;

        public ApiKeyService(IApiKeyRepository repository, 
            IModelMapper<ApiKeyModel> modelMapper,
            IUserLifecycle userLifecycle,
            ITokenGenerationService<GenerateApiKeyToken> apiKeyTokenService,
            ITokenDecodeService<UserToken> userTokenService,
            IEntitySearch<ApiKeyModel> apiKeySearch, 
            IOptions<ApiKeyValidationSettings> options)
        {
            _repository = repository;
            _modelMapper = modelMapper;
            _userLifecycle = userLifecycle;
            _apiKeyTokenService = apiKeyTokenService;
            _userToken = userTokenService.GetCurrentToken();
            _apiKeySearch = apiKeySearch;
            _validationSettings = options.Value;
        }

        public async Task<IResponse> CreateNewApiKey(CreateApiKeyModel createApiKeyModel)
        {
            ApiKeyModel apiKeyModel = _modelMapper.Create(createApiKeyModel);

            string userId = await _userLifecycle.AddApiUser(createApiKeyModel.Name);
            
            // Allocate a new key value, we will use the existing refresh token
            //   logic as the user will never see this value
            apiKeyModel.KeyValue = JwtBearerToken.GenerateRefreshToken();
            apiKeyModel.ApiKeyUserId = userId;
            
            RepositoryActionResultModel<ApiKeyModel> result = await _repository.Add(apiKeyModel);

            if (result is not SuccessfulActionResultModel<ApiKeyModel> success || success.CurrentVersion == null) 
                return result;
            
            GenerateApiKeyToken generateApiKeyToken = new(success.CurrentVersion.EntityId!, 
                apiKeyModel.KeyValue, 
                success.CurrentVersion.AirslipUserType,
                success.CurrentVersion.ApiKeyUserId!)
            {
                UserRole = UserRoles.User
            };

            NewToken newToken = _apiKeyTokenService.GenerateNewToken(generateApiKeyToken);

            success.CurrentVersion!.TokenValue = newToken.TokenValue;

            return success;
        }

        public async Task<IResponse> Search(EntitySearchQueryModel query)
        {
            EntitySearchResponse<ApiKeyModel> searchResults = await _apiKeySearch
                .GetSearchResults<ApiKey>(query,
                    new List<SearchFilterModel>
                    {
                        new(nameof(ApiKey.EntityStatus), EntityStatus.Active.ToString()),
                        new(nameof(ApiKey.EntityId), _userToken.EntityId),
                        new(nameof(ApiKey.AirslipUserType), _userToken.AirslipUserType.ToString())
                    });
            
            return searchResults;
        }
        
        public async Task<IResponse> Delete(string id)
        {
            return await _repository.Delete(id);
        }

        public async Task<IResponse> ValidateApiKey(ApiKeyValidationModel model)
        {
            ApiKeyValidationModelValidator validator = new ApiKeyValidationModelValidator();

            if (model.VerificationToken == null || !model.VerificationToken.Equals(_validationSettings.VerificationToken))
            {
                return new ApiKeyValidationResultModel(false, "Invalid verification token");
            }

            ValidationResult? validationResult = await validator.ValidateAsync(model);

            if (!validationResult.IsValid)
            {
                return new ApiKeyValidationResultModel(false, validationResult.Errors.First().ErrorMessage);
            }
            
            ApiKey? apiKey = await _repository.GetApiKeyByKeyValue(model.ApiKey!);
            
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

        public async Task<IResponse> Get(string id)
        {
            return await _repository.Get(id);
        }
    }
}