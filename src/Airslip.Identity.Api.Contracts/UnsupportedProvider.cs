using Airslip.Common.Types.Failures;
using System.Collections.Generic;

namespace Airslip.Identity.Api.Contracts
{
    public class UnsupportedProvider : ErrorResponse
    {
        public UnsupportedProvider(string supportedProviders)
            : base("UNSUPPORTED_PROVIDER",
                $"Must be one of {supportedProviders}.",
                new Dictionary<string, object>
                {
                    { nameof(supportedProviders), supportedProviders }
                })
        {
        }
    }
}