using Airslip.BankTransactions.MongoDb.Contracts;
using Airslip.Common.Contracts;
using JetBrains.Annotations;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Airslip.BankTransactions.Api.Application.UserProfiles
{
    [UsedImplicitly(ImplicitUseTargetFlags.Itself)]
    public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, IResponse>
    {
        private readonly IUserService _userService;

        public UpdateUserProfileCommandHandler(
            IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IResponse> Handle(UpdateUserProfileCommand command, CancellationToken cancellationToken)
        {
            User user = await _userService.Get(command.UserId);

            user.UpdateUserProfile(
                command.FirstName,
                command.Surname,
                command.Gender,
                command.DateOfBirth?.ToUnixTimeMilliseconds(),
                command.Postalcode,
                command.FirstLineAddress,
                command.SecondLineAddress,
                command.City,
                command.County,
                command.Country);

            await _userService.Update(user);

            return Success.Instance;
        }
    }
}