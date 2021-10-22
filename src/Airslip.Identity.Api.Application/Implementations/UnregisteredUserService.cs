using Airslip.Common.Repository.Interfaces;
using Airslip.Common.Repository.Models;
using Airslip.Common.Types.Enums;
using Airslip.Identity.Api.Application.Interfaces;
using Airslip.Identity.Api.Contracts.Entities;
using Airslip.Identity.Api.Contracts.Models;
using System.Threading.Tasks;

namespace Airslip.Identity.Api.Application.Implementations
{
    public class UnregisteredUserService : IUnregisteredUserService
    {
        private readonly IUserService _userService;
        private readonly IModelMapper<UserModel> _modelMapper;
        private readonly IUserRepository _userRepository;

        public UnregisteredUserService(
            IUserService userService,
            IModelMapper<UserModel> modelMapper,
            IUserRepository userRepository)
        {
            _userService = userService;
            _modelMapper = modelMapper;
            _userRepository = userRepository;
        }

        public async Task<RepositoryActionResultModel<UserModel>> Create(CreateUnregisteredUserModel createModel)
        {
                UserProfile? user = await _userService.GetProfileByEmail(createModel.Email ?? string.Empty);

                if (user is null)
                {
                    UserModel userModel = _modelMapper.Create(createModel);
                    userModel.AirslipUserType = AirslipUserType.Unregistered;
                    return await _userRepository.Add(userModel);
                }

                RepositoryActionResultModel<UserModel> result = await _userRepository.Get(user.UserId);

                return result;
        }
    }
}