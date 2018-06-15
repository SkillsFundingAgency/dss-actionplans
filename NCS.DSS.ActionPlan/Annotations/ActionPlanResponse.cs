using System;

namespace NCS.DSS.ActionPlan.Annotations
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class ActionPlanResponse : Attribute
    {
        public int HttpStatusCode { get; set; }
        public string Description { get; set; }
        public bool ShowSchema { get; set; }
    }
}
