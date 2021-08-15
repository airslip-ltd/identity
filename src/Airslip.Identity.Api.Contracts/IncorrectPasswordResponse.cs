using Airslip.Common.Types.Failures;

namespace Airslip.Identity.Api.Contracts
{
    public class IncorrectPasswordResponse : ErrorResponse
    {
        public IncorrectPasswordResponse()
            : base("INCORRECT_PASSWORD", "You have entered an incorrect password.")
        {
        }
    }
}