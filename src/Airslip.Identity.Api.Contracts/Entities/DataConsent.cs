using Airslip.Common.Repository.Types.Interfaces;
using Airslip.Common.Utilities.Extensions;
using JetBrains.Annotations;

namespace Airslip.Identity.Api.Contracts.Entities
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public record DataConsent : IEntityNoId
    {
        public bool Essential { get; private set; }
        public bool Performance { get;  private set;}
        public bool Personalisation { get;  private set;}
        public long CreatedOn { get; private set; } = DateTimeExtensions.GetTimestamp();
    }
}