using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NCS.DSS.ActionPlan.Models;
using NCS.DSS.ActionPlan.ReferenceData;

namespace NCS.DSS.ActionPlan.Validation
{
    public class Validate : IValidate
    {
        public List<ValidationResult> ValidateResource(IActionPlan resource)
        {
            var context = new ValidationContext(resource, null, null);
            var results = new List<ValidationResult>();

            Validator.TryValidateObject(resource, context, results, true);
            ValidateActionPlanRules(resource, results);

            return results;
        }

        private void ValidateActionPlanRules(IActionPlan actionPlanResource, List<ValidationResult> results)
        {
            if (actionPlanResource == null)
                return;

            if (actionPlanResource.DateActionPlanCreated.HasValue && actionPlanResource.DateActionPlanCreated.Value > DateTime.UtcNow)
                results.Add(new ValidationResult("Date ActionPlan Created must be less the current date/time", new[] { "DateActionPlanCreated" }));

            if (actionPlanResource.DateAndTimeCharterShown.HasValue && actionPlanResource.DateAndTimeCharterShown.Value > DateTime.UtcNow)
                results.Add(new ValidationResult("Date And Time Charter Shown must be less the current date/time", new[] { "DateAndTimeCharterShown" }));

            if (actionPlanResource.DateActionPlanSentToCustomer.HasValue && actionPlanResource.DateActionPlanSentToCustomer.Value > DateTime.UtcNow)
                results.Add(new ValidationResult("Date ActionPlan Sent To Customer must be less the current date/time", new[] { "DateActionPlanSentToCustomer" }));

            if (actionPlanResource.DateActionPlanAcknowledged.HasValue && actionPlanResource.DateActionPlanAcknowledged.Value > DateTime.UtcNow)
                results.Add(new ValidationResult("Date ActionPlan Acknowledged must be less the current date/time", new[] { "DateActionPlanAcknowledged" }));

            if (actionPlanResource.LastModifiedDate.HasValue && actionPlanResource.LastModifiedDate.Value > DateTime.UtcNow)
                results.Add(new ValidationResult("Last Modified Date must be less the current date/time", new[] { "LastModifiedDate" }));

            if (actionPlanResource.PriorityCustomer.HasValue && !Enum.IsDefined(typeof(PriorityCustomer), actionPlanResource.PriorityCustomer.Value))
                results.Add(new ValidationResult("Please supply a valid Priority Customer", new[] { "PriorityCustomer" }));
        }

    }
}
