using Airslip.Common.Repository.Enums;
using System;

namespace Airslip.Common.Repository.Interfaces
{
    /// <summary>
    /// A simple interface defining the common data properties for basic auditing of changes to an entity object
    /// </summary>
    public interface IEntity : IEntityWithId
    {
        string? CreatedByUserId { get; set; }
        
        DateTime DateCreated { get; set; }
        
        string? UpdatedByUserId { get; set; }
        
        DateTime? DateUpdated { get; set; }
        
        string? DeletedByUserId { get; set; }
        
        DateTime? DateDeleted { get; set; }
        
        EntityStatusEnum EntityStatus { get; set; }
    }
}