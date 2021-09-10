using Airslip.Common.Repository.Interfaces;

namespace Airslip.Identity.Api.Contracts.Models
{
    public record CreateQrCodeModel(string Name, string StoreId, string CheckoutId);
}