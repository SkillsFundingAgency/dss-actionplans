using System;
using System.Net;
using System.Threading.Tasks;
using NCS.DSS.ActionPlan.Cosmos.Provider;

namespace NCS.DSS.ActionPlan.PostActionPlanHttpTrigger.Service
{
    public class PostActionPlanHttpTriggerService : IPostActionPlanHttpTriggerService
    {
        public async Task<Models.ActionPlan> CreateAsync(Models.ActionPlan actionPlan)
        {
            if (actionPlan == null)
                return null;

            var actionPlanId = Guid.NewGuid();
            actionPlan.ActionPlanId = actionPlanId;

            var documentDbProvider = new DocumentDBProvider();

            var response = await documentDbProvider.CreateActionPlanAsync(actionPlan);

            return response.StatusCode == HttpStatusCode.Created ? (dynamic)response.Resource : null;
        }
    }
}