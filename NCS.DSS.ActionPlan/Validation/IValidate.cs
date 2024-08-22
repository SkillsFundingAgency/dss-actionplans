using NCS.DSS.ActionPlan.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NCS.DSS.ActionPlan.Validation
{
    public interface IValidate
    {
        List<ValidationResult> ValidateResource(IActionPlan resource, DateTime? dateAndTimeSessionCreated);
    }
}