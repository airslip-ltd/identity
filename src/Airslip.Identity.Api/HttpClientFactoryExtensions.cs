using Airslip.Common.Utilities;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Airslip.Identity.Api
{
    public static class HttpClientFactoryExtensions
    {
        public static void AddDefaults(this HttpClient httpClient, string baseUri, string scheme, string credentials)
        {
            httpClient.BaseAddress = new Uri(baseUri);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                scheme,
                Convert.ToBase64String(Encoding.ASCII.GetBytes(credentials)));

            httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue(Json.MediaType));
        }
    }
}