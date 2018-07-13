using System;

namespace NCS.DSS.ActionPlan.PostActionPlanHttpTrigger.Service
{
    public class PostActionPlanHttpTriggerService : IPostActionPlanHttpTriggerService
    {
        public Guid? Create(Models.ActionPlan actionPlan)
        {
            if (actionPlan == null)
                return null;

            return Guid.NewGuid();
;        }
    }
}