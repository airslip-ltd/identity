using System.IO;

namespace Airslip.Identity.Api.Contracts.Models
{
    public record GenerateQrCodeImageModel(bool Success, MemoryStream? imageStream);
}