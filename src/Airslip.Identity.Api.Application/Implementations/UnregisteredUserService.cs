using Airslip.Common.Repository.Types.Interfaces;
using Airslip.Common.Repository.Types.Models;
using Airslip.Common.Types;
using Airslip.Common.Types.Enums;
using Airslip.Common.Types.Failures;
using Airslip.Common.Types.Interfaces;
using Airslip.Identity.Api.Application.Interfaces;
using Airslip.Identity.Api.Contracts.Models;
using Airslip.Identity.Api.Contracts.Responses;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using User = Airslip.Identity.Api.Contracts.Entities.User;

namespace Airslip.Identity.Api.Application.Implementations
{
    public class UnregisteredUserService : IUnregisteredUserService
    {
        private readonly IIdentityContext _context;
        private readonly IModelMapper<UserModel> _modelMapper;
        private readonly IRepository<User, UserModel> _userRepository;
        private readonly IUserManagerService _userManagerService;

        public UnregisteredUserService(
            IIdentityContext context,
            IModelMapper<UserModel> modelMapper,
            IRepository<User, UserModel> userRepository, 
            IUserManagerService userManagerService)
        {
            _context = context;
            _modelMapper = modelMapper;
            _userRepository = userRepository;
            _userManagerService = userManagerService;
        }

        public async Task<IResponse> Create(CreateUnregisteredUserModel createModel)
        {
            // create validator for CreateUnregisteredUserModel
            User? user = await _context.GetByEmail(createModel.Email ?? string.Empty);
            RepositoryActionResultModel<UserModel> result;
            if (user is null)
            {
                UserModel userModel = _modelMapper.Create(createModel);
                userModel.AirslipUserType = AirslipUserType.Unregistered;
                result = await _userRepository.Add(userModel);
            }
            else
            {
                result = await _userRepository.Get(user.Id);
            }

            string email = result.CurrentVersion!.Email;
            IdentityResult identityResult = await _userManagerService.Create(email);

            if (identityResult.Errors.Any())
                return new InvalidResource(
                    identityResult.Errors.First().Code,
                    identityResult.Errors.First().Description);

            string? confirmationToken = await _userManagerService.GenerateEmailConfirmationTokenAsync(email);

            if (confirmationToken is null)
                return new InvalidResource("EMAIL_CONFIRMATION_GENERATION_LINK_ERROR", "Error generating token");

            return new CreateUnregisteredUserResponse
            {
                Email = email,
                ConfirmationToken = confirmationToken
            };
        }

        public async Task<IResponse> ConfirmEmail(string email, string token)
        {
            IdentityResult? identityResult = await _userManagerService.ConfirmEmailAsync(email, token);
            User? user = await _context.GetByEmail(email);

            if (identityResult is null || user is null)
                return new InvalidResource("EMAIL_CONFIRMATION_ERROR", "Unable to find user");

            if (identityResult.Errors.Any())
                return new InvalidResource(
                    identityResult.Errors.First().Code,
                    identityResult.Errors.First().Description);

            RepositoryActionResultModel<UserModel> result = await _userRepository.Get(user.Id);

            result.CurrentVersion!.VerifyAccount();

            await _userRepository.Update(user.Id, result.CurrentVersion!);

            return Success.Instance;
        }
    }
}