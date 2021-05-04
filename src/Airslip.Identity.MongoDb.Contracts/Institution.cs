using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace Airslip.BankTransactions.MongoDb.Contracts
{
    public class Institution
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        [BsonElement("id")]
        public string Id { get; }

        [BsonElement("name")] public string Name { get; }

        [BsonElement("logo")] public string Logo { get; }

        [BsonElement("icon")] public string Icon { get; }

        [BsonElement("countries")] public List<Country> Countries { get; }

        public Institution(string id, string name, string logo, string icon, List<Country> countries)
        {
            Id = id;
            Name = name;
            Logo = logo;
            Icon = icon;
            Countries = countries;
        }
    }

    public class Country
    {
        [BsonElement("name")] public string Name { get; }

        [BsonElement("countryCode")] public string CountryCode { get; }

        public Country(string name, string countryCode)
        {
            Name = name;
            CountryCode = countryCode;
        }
    }
}