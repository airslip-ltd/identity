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
        private readonly IIdentityContext _context;
        private readonly IModelMapper<UserModel> _modelMapper;
        private readonly IUserRepository _userRepository;

        public UnregisteredUserService(
            IIdentityContext context,
            IModelMapper<UserModel> modelMapper,
            IUserRepository userRepository)
        {
            _context = context;
            _modelMapper = modelMapper;
            _userRepository = userRepository;
        }

        public async Task<RepositoryActionResultModel<UserModel>> Create(CreateUnregisteredUserModel createModel)
        {
                User? user = await _context.GetByEmail(createModel.Email ?? string.Empty);

                if (user is null)
                {
                    UserModel userModel = _modelMapper.Create(createModel);
                    userModel.AirslipUserType = AirslipUserType.Unregistered;
                    return await _userRepository.Add(userModel);
                }

                RepositoryActionResultModel<UserModel> result = await _userRepository.Get(user.Id);

                return result;
        }
    }
}