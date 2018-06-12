using System;

namespace NCS.DSS.ActionPlan.PostActionPlanHttpTrigger
{
    public class PostActionPlanHttpTriggerService
    {
        public Guid? Create(Models.ActionPlan actionPlan)
        {
            if (actionPlan == null)
                return null;

            return Guid.NewGuid();
;        }
    }
}