using System;
using System.Collections.Generic;
using NCS.DSS.ActionPlan.ReferenceData;

namespace NCS.DSS.ActionPlan.Models
{
    public interface IActionPlan
    {
        DateTime? DateActionPlanCreated { get; set; }
        bool? CustomerCharterShownToCustomer { get; set; }
        DateTime? DateAndTimeCharterShown { get; set; }
        DateTime? DateActionPlanSentToCustomer { get; set; }
        ActionPlanDeliveryMethod? ActionPlanDeliveryMethod { get; set; }
        DateTime? DateActionPlanAcknowledged { get; set; }
        List<PriorityCustomer> PriorityCustomer { get; set; }
        string CurrentSituation { get; set; }
        DateTime? LastModifiedDate { get; set; }
        string LastModifiedTouchpointId { get; set; }

        void SetDefaultValues();
    }
}