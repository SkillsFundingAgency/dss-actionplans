using System;
using System.ComponentModel.DataAnnotations;
using NCS.DSS.ActionPlan.ReferenceData;

namespace NCS.DSS.ActionPlan.Models
{
    public class ActionPlan
    {
        [Display(Description = "Unique identifier of the action plan record.")]
        public Guid ActionPlanId { get; set; }

        [Required]
        [Display(Description = "Unique identifier to the related interaction resource.")]
        public Guid InteractionId { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Description = "Date and time action plan was created.")]
        public DateTime DateActionPlanCreated { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Description = "Date and time the action plan was sent (or made available) to the customer.")]
        public DateTime DateActionPlanSentToCustomer { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Description = "Date and time the customer acknowledged receipt of the action plan.")]
        public DateTime DateActionPlanAcknowledged { get; set; }

        [Required]
        [Display(Description = "Priority Customer reference data.")]
        public PriorityCustomer PriorityCustomer { get; set; }

        [Display(Description = "Summary of a customer current situation and how it affects their career.")]
        public string CurrentSituation { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Description = "Date and time of the last modification to the record.")]
        public DateTime LastModifiedDate { get; set; }

        [Display(Description = "Identifier of the touchpoint who made the last change to the record")]
        public Guid LastModifiedTouchpointId { get; set; }
    }
}