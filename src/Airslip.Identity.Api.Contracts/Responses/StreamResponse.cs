using Airslip.Common.Contracts;
using System.IO;

namespace Airslip.BankTransactions.Api.Contracts.Responses
{
    public record StreamResponse : ISuccess
    {
        public Stream Stream { get; }
        public string ContentType { get; }

        public StreamResponse(Stream stream, string? contentType)
        {
            Stream = stream;
            ContentType = contentType ?? string.Empty;
        }
    }
}