using JetBrains.Annotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace Airslip.Identity.MongoDb.Contracts
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class Account
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        [BsonElement("id")]
        public string Id => CompositeId.Build(UserId, AccountId);

        [BsonElement("institutionConsentToken")]
        public string InstitutionConsentToken { get; init; }

        [BsonElement("accountId")] public string AccountId { get; init; }

        [BsonElement("userId")] public string UserId { get; init; }

        [BsonElement("institution")] public Institution? Institution { get; init; }

        [BsonElement("panNumbers")] public IList<string>? PanNumbers { get; init; }

        [BsonElement("currencyCode")] public string CurrencyCode { get; init; }

        [BsonElement("usageType")] public string UsageType { get; init; }

        [BsonElement("accountType")] public string AccountType { get; init; }

        [BsonElement("sortCode")] public string? SortCode { get; init; }

        [BsonElement("accountNumber")] public string? AccountNumber { get; init; }

        [BsonElement("previousTransactionRetrievalDate")]
        public long? DateOfPreviousTransaction { get; set; }

        [BsonElement("previousApiRequestDateTime")]
        public long? PreviousApiRequestDateTime { get; set; }

        public bool FirstTimeRetrieval => DateOfPreviousTransaction == null;

        public bool CanGetYapilyTransactions => PreviousApiRequestDateTime == null ||
                                                PreviousApiRequestDateTime < DateTimeOffset.UtcNow.AddMinutes(-10)
                                                    .ToUnixTimeMilliseconds();

        public Account(
            string? accountId,
            string institutionConsentToken,
            string userId,
            Institution? institution,
            IList<string>? panNumbers,
            string? sortCode,
            string? accountNumber,
            string? currencyCode,
            string? usageType,
            string? accountType)
        {
            AccountId = accountId ?? string.Empty;
            InstitutionConsentToken = institutionConsentToken;
            UserId = userId;
            Institution = institution;
            PanNumbers = panNumbers;
            SortCode = sortCode;
            AccountNumber = accountNumber;
            CurrencyCode = currencyCode ?? string.Empty;
            UsageType = usageType ?? string.Empty;
            AccountType = accountType ?? string.Empty;
        }


        public void UpdatePreviousTransactionRetrievalDate(long value)
        {
            DateOfPreviousTransaction = value;
        }

        public void UpdatePreviousYapilyApiRequestDateTime()
        {
            PreviousApiRequestDateTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
    }
}