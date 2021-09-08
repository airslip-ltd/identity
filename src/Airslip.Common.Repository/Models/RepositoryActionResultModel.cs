using Airslip.Common.Repository.Enums;
using Airslip.Common.Repository.Interfaces;

namespace Airslip.Common.Repository.Models
{
    public record RepositoryActionResultModel<TModel>
    (ResultTypeEnum ResultType, TModel? CurrentVersion = null,
        TModel? PreviousVersion = null,
        ValidationResultModel? ValidationResult = null) where TModel : class, IModel;
}