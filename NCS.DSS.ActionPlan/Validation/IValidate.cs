using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using NCS.DSS.ActionPlan.Models;

namespace NCS.DSS.ActionPlan.Validation
{
    public interface IValidate
    {
        List<ValidationResult> ValidateResource(IActionPlan resource, DateTime dateAndTimeSessionCreated);
    }
}