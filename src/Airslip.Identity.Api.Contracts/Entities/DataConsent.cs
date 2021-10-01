using Airslip.Common.Repository.Interfaces;
using Airslip.Common.Types.Extensions;

namespace Airslip.Identity.Api.Contracts.Entities
{
    public record DataConsent : IEntityNoId
    {
        public bool Essential { get; private set; }
        public bool Performance { get;  private set;}
        public bool Personalisation { get;  private set;}
        public long CreatedOn { get; private set; } = DateTimeExtensions.GetTimestamp();
    }
}