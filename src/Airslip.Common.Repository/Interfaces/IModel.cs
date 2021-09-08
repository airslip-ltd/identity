using Airslip.Common.Repository.Enums;

namespace Airslip.Common.Repository.Interfaces
{
    /// <summary>
    /// A simple interface defining the common properties you can expect on an Api facing model
    /// </summary>
    public interface IModel
    {
        string? Id { get; set; }
        EntityStatusEnum EntityStatus { get; set; }
    }
}