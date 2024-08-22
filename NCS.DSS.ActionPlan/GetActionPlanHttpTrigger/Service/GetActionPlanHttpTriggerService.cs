using NCS.DSS.ActionPlan.Cosmos.Provider;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NCS.DSS.ActionPlan.GetActionPlanHttpTrigger.Service
{
    public class GetActionPlanHttpTriggerService : IGetActionPlanHttpTriggerService
    {
        private readonly IDocumentDBProvider _documentDbProvider;

        public GetActionPlanHttpTriggerService(IDocumentDBProvider documentDbProvider)
        {
            _documentDbProvider = documentDbProvider;
        }

        public async Task<List<Models.ActionPlan>> GetActionPlansAsync(Guid customerId)
        {
            var actionPlans = await _documentDbProvider.GetActionPlansForCustomerAsync(customerId);

            return actionPlans;
        }
    }
}