using Airslip.Common.Types.Enums;

namespace Airslip.Identity.Api.Contracts.Models
{
    public class ApiKeyValidationModel
    {
        public string? ApiKey { get; set; }
        public string? EntityId { get; set; }
        public AirslipUserType? AirslipUserType { get; set; }
        public string? VerificationToken { get; set; }
    }
}