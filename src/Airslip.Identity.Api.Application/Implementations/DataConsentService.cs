using Airslip.Common.Auth.Interfaces;
using Airslip.Common.Auth.Models;
using Airslip.Common.Contracts;
using Airslip.Common.Repository.Interfaces;
using Airslip.Common.Types.Failures;
using Airslip.Identity.Api.Application.Interfaces;
using Airslip.Identity.Api.Contracts.Entities;
using Airslip.Identity.Api.Contracts.Models;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Application.Implementations
{
    public class DataConsentService : IDataConsentService
    {
        private readonly ITokenDecodeService<UserToken> _userTokenService;
        private readonly IUserService _userService;
        private readonly IModelMapper<DataConsent> _modelMapper;

        public DataConsentService(ITokenDecodeService<UserToken> userTokenService, IUserService userService, IModelMapper<DataConsent> modelMapper)
        {
            _userTokenService = userTokenService;
            _userService = userService;
            _modelMapper = modelMapper;
        }

        public async Task<IResponse> Update(DataConsentModel dataConsentModel)
        {
            UserToken userToken = _userTokenService.GetCurrentToken();
            User? user = await _userService.Get(userToken.UserId);

            if (user is null)
                return new NotFoundResponse(nameof(User), "Unable to find user");

            DataConsent dataConsent = _modelMapper.Create(dataConsentModel);
            
            user.UpdateDataConsent(dataConsent);

            await _userService.Update(user);

            return Success.Instance;
        }
    }
}