using Airslip.Common.Types.Failures;
using System.Collections.Generic;

namespace Airslip.Identity.Api.Contracts
{
    public class IncorrectPasswordResponse : ErrorResponse
    {
        public IncorrectPasswordResponse(string validator)
            : base("INCORRECT_PASSWORD", validator, new Dictionary<string, object>
            {
                {
                    nameof(validator), validator
                }
            })
        {
        }
    }
}