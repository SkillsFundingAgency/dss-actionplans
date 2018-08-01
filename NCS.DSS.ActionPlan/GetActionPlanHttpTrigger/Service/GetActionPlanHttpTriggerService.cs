using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NCS.DSS.ActionPlan.Cosmos.Provider;

namespace NCS.DSS.ActionPlan.GetActionPlanHttpTrigger.Service
{
    public class GetActionPlanHttpTriggerService : IGetActionPlanHttpTriggerService
    {
        public async Task<List<Models.ActionPlan>> GetActionPlansAsync(Guid customerId)
        {
            var documentDbProvider = new DocumentDBProvider();
            var actionPlans = await documentDbProvider.GetActionPlansForCustomerAsync(customerId);

            return actionPlans;
        }
    }
}