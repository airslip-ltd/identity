using Airslip.Common.Auth.Interfaces;
using Airslip.Common.Auth.Models;
using Airslip.Common.Repository.Types.Models;
using Airslip.Common.Types.Enums;
using Airslip.Common.Types.Interfaces;
using Airslip.Identity.Api.Application.Interfaces;
using Airslip.Identity.Api.Contracts.Entities;
using Airslip.Identity.Api.Contracts.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Application.Implementations
{
    public class UserSearchService : IUserSearchService
    {
        // private readonly IEntitySearch<User, UserModel> _entitySearch;
        private readonly UserToken _userToken;

        public UserSearchService(
            //IEntitySearch<User, UserModel> entitySearch, 
            ITokenDecodeService<UserToken> tokenDecodeService)
        {
            //_entitySearch = entitySearch;
            _userToken = tokenDecodeService.GetCurrentToken();
        }
        
        public Task<IResponse> FindUsers()
        {
            throw new NotImplementedException();
            // if (_userToken.AirslipUserType == AirslipUserType.Standard)
            //     throw new InvalidOperationException();
            //
            // List<SearchFilterModel> searchParams = new()
            // {
            //     new SearchFilterModel("EntityId", _userToken.EntityId),
            //     new SearchFilterModel("AirslipUserType", _userToken.AirslipUserType.ToString())
            // };
            // List<UserModel> searchResults = await _entitySearch.GetSearchResults(searchParams);
            //
            // return new UserSearchResults(searchResults);
        }
    }
}