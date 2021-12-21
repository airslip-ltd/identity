using Airslip.Common.Types.Interfaces;
using Airslip.Identity.Api.Contracts.Models;
using System.Collections.Generic;

namespace Airslip.Identity.Api.Application.Implementations
{
    public record UserSearchResults(IEnumerable<UserModel> Users) : ISuccess;
}