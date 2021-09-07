using System.Collections.Generic;

namespace Airslip.Identity.Api.Contracts
{
    public static class OpenBankingProviders
    {
        public static readonly IEnumerable<string> Names = new List<string>() { "Yapily" };
    }
}