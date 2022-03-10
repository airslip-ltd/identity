using Airslip.Common.Auth.Data;
using Airslip.Common.Auth.Interfaces;
using Airslip.Common.Auth.Models;
using Airslip.Common.Repository.Data;
using Airslip.Common.Repository.Types.Enums;
using Airslip.Common.Repository.Types.Interfaces;
using Airslip.Common.Repository.Types.Models;
using Airslip.Common.Types.Interfaces;
using Airslip.Common.Types.Failures;
using Airslip.Identity.Api.Application.Identity;
using Airslip.Identity.Api.Application.Interfaces;
using Airslip.Identity.Api.Contracts.Entities;
using Airslip.Identity.Api.Contracts.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Application.Implementations
{
    public class UserService : IUserService
    {
        private readonly IRepository<User, UserModel> _repository;
        private readonly IEntitySearch<UserModel> _userSearch;
        private readonly IUserManagerService _userManagerService;
        private readonly IUserLifecycle _userLifecycle;
        private readonly ILogger _logger;
        private readonly UserToken _userToken;

        public UserService(
            ITokenDecodeService<UserToken> tokenDecodeService,
            IRepository<User, UserModel> repository,
            IEntitySearch<UserModel> userSearch, 
            IUserManagerService userManagerService,
            IUserLifecycle userLifecycle,
            ILogger logger)
        {
            _userToken = tokenDecodeService.GetCurrentToken();
            _repository = repository;
            _userSearch = userSearch;
            _userManagerService = userManagerService;
            _userLifecycle = userLifecycle;
            _logger = logger;
        }

        public async Task<IResponse> Update(string id, UserModel model, string? userId = null)
        {
            RepositoryActionResultModel<UserModel> getResult = await _repository.Get(id);

            if (getResult is not SuccessfulActionResultModel<UserModel> getSuccess)
                return getResult;

            UserModel currentModel = getSuccess.CurrentVersion!;
            
            if (currentModel.Id == _userToken.UserId && currentModel.UserRole != model.UserRole)
            {
                return new ErrorResponse(ErrorCodes.ValidationFailed,
                    "You cannot change your own role, please ask a system administrator for help");
            }
            
            RepositoryActionResultModel<UserModel> repositoryActionResult = await _repository.Update(id, model, userId);

            if (repositoryActionResult is not SuccessfulActionResultModel<UserModel> success)
                return repositoryActionResult;
            
            if (success.PreviousVersion!.Email != success.CurrentVersion!.Email)
                await _userManagerService
                    .ChangeEmail(success.PreviousVersion!.Email, success.CurrentVersion!.Email);
                
            if (success.PreviousVersion!.UserRole != success.CurrentVersion!.UserRole)
                await _userLifecycle.SetRole(success.CurrentVersion!.Id ?? throw new ArgumentException(), 
                    success.CurrentVersion.UserRole ?? UserRoles.User);

            return repositoryActionResult;
        }

        public async Task<IResponse> Delete(string id, string? userId = null)
        {
            if (id == _userToken.UserId)
            {
                return new ErrorResponse(ErrorCodes.ValidationFailed,
                    "You cannot delete your own user account, please ask a system administrator for help");
            }
            RepositoryActionResultModel<UserModel> repositoryActionResult = await _repository.Delete(id, userId);
            
            if (repositoryActionResult is SuccessfulActionResultModel<UserModel> success)
            {
                await _userManagerService.Delete(success.PreviousVersion!.Email);
            }

            return repositoryActionResult;
        }

        public async Task<IResponse> Get(string id)
        {
            return await _repository.Get(id);
        }

        public async Task<IResponse> Search(EntitySearchQueryModel query)
        {
            EntitySearchResponse<UserModel> searchResults = await _userSearch
                .GetSearchResults<User>(query,
                    new List<SearchFilterModel>
                    {
                        new(nameof(User.EntityId), _userToken.EntityId),
                        new(nameof(User.AirslipUserType), _userToken.AirslipUserType)
                    });
            
            return searchResults;
        }

        public async Task<IResponse> Create(UserModel model)
        {
            IResponse createUser = await _userLifecycle.Add(new RegisterUserCommand(model.Email, model.FirstName, 
                model.LastName, null, null,
                model.UserRole, model.DisplayName), CancellationToken.None);

            if (createUser is not SuccessfulActionResultModel<UserModel> success)
                return createUser;

            model.Id = success.CurrentVersion?.Id;

            return await Update(model.Id!, model);
        }
    }
}