﻿using Airslip.Common.Types.Interfaces;
using System.IO;

namespace Airslip.Identity.Api.Contracts.Responses
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