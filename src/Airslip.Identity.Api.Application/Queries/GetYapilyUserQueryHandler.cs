using Airslip.BankTransactions.Api.Contracts.Responses;
using Airslip.BankTransactions.Common;
using Airslip.BankTransactions.MongoDb.Contracts;
using Airslip.Common.Contracts;
using JetBrains.Annotations;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Airslip.BankTransactions.Api.Application.Admin
{
    [UsedImplicitly(ImplicitUseTargetFlags.Itself)]
    public class GetYapilyUserQueryHandler : IRequestHandler<GetYapilyUserQuery, IResponse>
    {
        private readonly IAccountService _accountService;

        public GetYapilyUserQueryHandler(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public async Task<IResponse> Handle(GetYapilyUserQuery query, CancellationToken cancellationToken)
        {
            List<Account> accounts = await _accountService.GetAccountsForUser(query.UserId);

            IEnumerable<YapilyUserAccountResponse> yapilyUserAccounts = accounts
                .Select(account => new YapilyUserAccountResponse(
                    account.Id.Substring(account.Id.IndexOf("|", StringComparison.Ordinal) + 1),
                    query.UserId,
                    account.Institution!.Id,
                    account.InstitutionConsentToken,
                    account.Institution!.Id.ToAccountNickname(account.UsageType, account.AccountType)));

            return new YapilyUserResponse(yapilyUserAccounts);
        }
    }
}