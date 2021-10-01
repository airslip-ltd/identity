using Airslip.Common.Repository.Enums;
using Airslip.Common.Repository.Interfaces;

namespace Airslip.Identity.Api.Contracts.Models
{
    public record CreateUnregisteredUserModel(
        string? Email,
        string? DeviceId) : IModel
    {
        public string? Id { get; set; }
        public EntityStatus EntityStatus { get; set; }
    }
}