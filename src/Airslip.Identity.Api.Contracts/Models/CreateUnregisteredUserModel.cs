using Airslip.Common.Repository.Types.Enums;
using Airslip.Common.Repository.Types.Interfaces;

namespace Airslip.Identity.Api.Contracts.Models
{
    public record CreateUnregisteredUserModel(
        string? Email) : IModel
    {
        public string? Id { get; set; }
        public EntityStatus EntityStatus { get; set; }
    }
}