using System;
using System.ComponentModel.DataAnnotations;
using NCS.DSS.ActionPlan.Annotations;
using NCS.DSS.ActionPlan.ReferenceData;

namespace NCS.DSS.ActionPlan.Models
{
    public class ActionPlan
    {
        [Display(Description = "Unique identifier of the action plan record.")]
        [Example(Description = "b8592ff8-af97-49ad-9fb2-e5c3c717fd85")]
        [Newtonsoft.Json.JsonProperty(PropertyName = "id")]
        public Guid? ActionPlanId { get; set; }

        [Required]
        [Display(Description = "Unique identifier of a customer.")]
        [Example(Description = "2730af9c-fc34-4c2b-a905-c4b584b0f379")]
        public Guid? CustomerId { get; set; }

        [Required]
        [Display(Description = "Unique identifier to the related interaction resource.")]
        [Example(Description = "2730af9c-fc34-4c2b-a905-c4b584b0f379")]
        public Guid? InteractionId { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [Display(Description = "Date and time action plan was created.")]
        [Example(Description = "2018-06-20T21:45:00")]
        public DateTime? DateActionPlanCreated { get; set; }

        [Required]
        [Display(Description = "Customer has seen the customer charter.")]
        [Example(Description = "true")]
        public bool? CustomerCharterShownToCustomer { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Description = "Date and time the customer was shown the customer charter.")]
        [Example(Description = "2018-06-20T21:45:00")]
        public DateTime? DateAndTimeCharterShown { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Description = "Date and time the action plan was sent (or made available) to the customer.")]
        [Example(Description = "2018-06-21T13:32:00")]
        public DateTime? DateActionPlanSentToCustomer { get; set; }

        [Display(Description = "Action Plan Delivery Method reference data.")]
        [Example(Description = "1")]
        public ActionPlanDeliveryMethod ActionPlanDeliveryMethod { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Description = "Date and time the customer acknowledged receipt of the action plan.")]
        [Example(Description = "2018-06-22T07:55:00")]
        public DateTime? DateActionPlanAcknowledged { get; set; }

        [Required]
        [Display(Description = "Priority Customer reference data.")]
        [Example(Description = "1")]
        public PriorityCustomer PriorityCustomer { get; set; }

        [StringLength(4000)]
        [Display(Description = "Summary of a customer current situation and how it affects their career.")]
        [Example(Description = "this is some text")]
        public string CurrentSituation { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Description = "Date and time of the last modification to the record.")]
        [Example(Description = "2018-06-20T13:45:00")]
        public DateTime? LastModifiedDate { get; set; }

        [Display(Description = "Identifier of the touchpoint who made the last change to the record")]
        [Example(Description = "d1307d77-af23-4cb4-b600-a60e04f8c3df")]
        public Guid? LastModifiedTouchpointId { get; set; }

        public void Patch(ActionPlanPatch actionPlanPatch)
        {
            if (actionPlanPatch == null)
                return;

            if(actionPlanPatch.DateActionPlanCreated.HasValue)
                DateActionPlanCreated = actionPlanPatch.DateActionPlanCreated;

            if (actionPlanPatch.CustomerCharterShownToCustomer.HasValue)
                CustomerCharterShownToCustomer = actionPlanPatch.CustomerCharterShownToCustomer;

            if (actionPlanPatch.DateAndTimeCharterShown.HasValue)
                DateAndTimeCharterShown = actionPlanPatch.DateAndTimeCharterShown;

            if (actionPlanPatch.DateActionPlanSentToCustomer.HasValue)
                DateActionPlanSentToCustomer = actionPlanPatch.DateActionPlanSentToCustomer;
            
            if (actionPlanPatch.ActionPlanDeliveryMethod.HasValue)
                ActionPlanDeliveryMethod = actionPlanPatch.ActionPlanDeliveryMethod.Value;

            if (actionPlanPatch.DateActionPlanAcknowledged.HasValue)
                DateActionPlanAcknowledged = actionPlanPatch.DateActionPlanAcknowledged;

            if (actionPlanPatch.PriorityCustomer.HasValue)
                PriorityCustomer = actionPlanPatch.PriorityCustomer.Value;

            if (!string.IsNullOrEmpty(actionPlanPatch.CurrentSituation))
                CurrentSituation = actionPlanPatch.CurrentSituation;

            if (actionPlanPatch.LastModifiedDate.HasValue)
                LastModifiedDate = actionPlanPatch.LastModifiedDate;

            if (actionPlanPatch.LastModifiedTouchpointId.HasValue)
                LastModifiedTouchpointId = actionPlanPatch.LastModifiedTouchpointId;

        }
    }
}