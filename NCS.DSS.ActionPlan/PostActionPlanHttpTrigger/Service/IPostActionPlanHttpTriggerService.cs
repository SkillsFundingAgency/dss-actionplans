﻿using System.Threading.Tasks;

namespace NCS.DSS.ActionPlan.PostActionPlanHttpTrigger.Service
{
    public interface IPostActionPlanHttpTriggerService
    {
        Task<Models.ActionPlan> CreateAsync(Models.ActionPlan actionPlan);
    }
}