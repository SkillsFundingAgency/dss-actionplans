using System;
using System.ComponentModel.DataAnnotations;

namespace NCS.DSS.ActionPlan.Models
{
    public class ActionPlan
    {
        public Guid ActionPlanId { get; set; }

        [Required]
        public Guid InteractionId { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime DateActionPlanCreated { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime DateActionPlanSentToCustomer { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime DateActionPlanAcknowledged { get; set; }

        [Required]
        public int PriorityCustomerId { get; set; }

        public string CurrentSituation { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime LastModifiedDate { get; set; }

        public Guid LastModifiedTouchpointId { get; set; }
    }
}