using Airslip.Common.AppIdentifiers;
using Airslip.Common.Types;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Airslip.Identity.Api.Controller
{
    [ApiController]
    [ApiVersionNeutral]
    [Produces(Json.MediaType)]
    [Route(".well-known")]
    public class WellKnownController : ControllerBase
    {
        private readonly IAppleAppIdentificationService _appleAppIdentificationService;

        public WellKnownController(IAppleAppIdentificationService appleAppIdentificationService)
        {
            _appleAppIdentificationService = appleAppIdentificationService;
        }

        [HttpGet("apple-app-site-association")]
        public IActionResult GetAppleSiteAssociation()
        {
            //AppleAppSiteAssociation appleAppSiteAssociation = _appleAppIdentificationService.GetAppSiteAssociation();

            string newAppleImplementationString =
                "{ \"applinks\": { \"details\": [ {\"appIDs\": [ \"CBMZUNS78C.com.airslip.rnairslip\" ], \"components\": [ { \"/\": \"/bank_transactions/v1/consents/*\", \"comment\": \"Matches any URL whose path starts with /bank_transactions/v1/consents/\"  },{ \"/\": \"/v1/identity/password/*\", \"comment\": \"Matches any URL whose path starts with /v1/identity/password/\" }]}]},\"webcredentials\": {\"apps\": [ \"CBMZUNS78C.com.airslip.rnairslip\" ]},\"appclips\": {\"apps\": [\"ABCED12345.com.example.MyApp.Clip\"]}}";

            NewAppleImplementation newAppleImplementation = Json.Deserialize<NewAppleImplementation>(newAppleImplementationString);
            
            return Ok(newAppleImplementation);
        }

        [HttpGet("assetlinks.json")]
        public IActionResult GetAssetLinks()
        {
            IEnumerable<AssetLink> assetLinks = _appleAppIdentificationService.GetAssetLinks();
            
            return Ok(Json.Serialize(assetLinks, Casing.SNAKE_CASE));
        }
    }
    
    public class Component
    {
        [JsonProperty("/")]
        public string? slash { get; set; }
        public string? comment { get; set; }
    }

    public class Detail
    {
        public List<string>? appIDs { get; set; }
        public List<Component>? components { get; set; }
    }

    public class Applinks
    {
        public List<Detail>? details { get; set; }
    }

    public class Webcredentials
    {
        public List<string>? apps { get; set; }
    }

    public class Appclips
    {
        public List<string>? apps { get; set; }
    }

    public class NewAppleImplementation
    {
        public Applinks? Applinks { get; set; }
        public Webcredentials? Webcredentials { get; set; }
        public Appclips? appclips { get; set; }
    }
}