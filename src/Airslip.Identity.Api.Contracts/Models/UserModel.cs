using Airslip.Common.Repository.Types.Entities;
using Airslip.Common.Types.Enums;
using Airslip.Identity.Api.Contracts.Entities;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace Airslip.Identity.Api.Contracts.Models
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public record UserModel : ProfileModel
    {
        public string? UserRole { get; set; }
        public long CreatedDate { get; init; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        public bool BiometricOn { get; private set; }
        public bool AccountVerified { get; private set; }
        [JsonIgnore]
        public BasicAuditInformation? AuditInformation { get; set; }
        [JsonConverter(typeof(StringEnumConverter))] 
        public AirslipUserType AirslipUserType { get; set; } = AirslipUserType.Standard;
        [JsonIgnore]
        public string? EntityId { get; set; }
        [JsonIgnore]
        public ICollection<OpenBankingProvider> OpenBankingProviders { get; private set; } =
            new List<OpenBankingProvider>(1);
        [JsonIgnore]
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>(1);
        public void SetRole(string userRole)
        {
            UserRole = userRole;
        }
        
        public void VerifyAccount()
        {
            AccountVerified = true;
        }
    }
}