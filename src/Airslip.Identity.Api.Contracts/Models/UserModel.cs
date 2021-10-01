using Airslip.Common.Repository.Entities;
using Airslip.Common.Repository.Enums;
using Airslip.Common.Repository.Interfaces;
using Airslip.Common.Types.Enums;
using Airslip.Identity.Api.Contracts.Entities;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;

namespace Airslip.Identity.Api.Contracts.Models
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public record UserModel : IModel
    {
        public string? Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public BasicAuditInformation? AuditInformation { get; set; }

        public EntityStatus EntityStatus { get; set; } = EntityStatus.Active;

        public AirslipUserType AirslipUserType { get; set; } = AirslipUserType.Standard;
        public string? EntityId { get; set; }

        public ICollection<OpenBankingProvider> OpenBankingProviders { get; private set; } =
            new List<OpenBankingProvider>(1);

        public long CreatedDate { get; init; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        public bool BiometricOn { get; private set; }

        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>(1);
        public DataConsent DataConsent { get; set; } = new ();

    }
}