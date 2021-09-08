using System.Collections.Generic;

namespace Airslip.Common.Repository.Models
{
    public class ValidationResultModel
    {
        public bool IsValid { get; private set; } = true; // Default to true

        public List<ValidationResultMessageModel> Results { get; } = new();

        public void AddMessage(string fieldName, string message)
        {
            Results
                .Add(new ValidationResultMessageModel(fieldName, message));

            // Assume error at this stage
            IsValid = false;
        }
    }
}