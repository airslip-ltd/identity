using Airslip.Common.Contracts;
using Airslip.Common.Types.Hateoas;
using JetBrains.Annotations;
using System.Collections.Generic;

namespace Airslip.BankTransactions.Api.Contracts.Responses
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class YapilyUserResponse : LinkResourceBase, ISuccess
    {
        public IEnumerable<YapilyUserAccountResponse> Accounts { get; }

        public YapilyUserResponse(IEnumerable<YapilyUserAccountResponse> userAccounts)
        {
            Accounts = userAccounts;
        }

        public override T AddHateoasLinks<T>(string baseUri, params string[] identifiers)
        {
            Links = new List<Link>
            {
                new($"{baseUri}/v1/admin/user/{identifiers[0]}", "self", "GET")
            };

            return (this as T)!;
        }
    }
}