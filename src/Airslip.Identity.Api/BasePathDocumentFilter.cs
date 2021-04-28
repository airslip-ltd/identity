using Airslip.Common.Types;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace Airslip.Identity.Api
{
    public class BasePathDocumentFilter : IDocumentFilter
    {
        private readonly string _baseUrl;

        public BasePathDocumentFilter(IOptions<PublicApiSettings> publicApiOptions)
        {
            _baseUrl = publicApiOptions.Value.BaseUri;
        }

        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            swaggerDoc.Servers = new List<OpenApiServer> { new() { Url = _baseUrl } };
        }
    }
}