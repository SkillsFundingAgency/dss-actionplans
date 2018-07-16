using System;
using System.Net;
using System.Threading.Tasks;
using NCS.DSS.ActionPlan.Cosmos.Provider;
using NCS.DSS.ActionPlan.Models;

namespace NCS.DSS.ActionPlan.PatchActionPlanHttpTrigger.Service
{
    public class PatchActionPlanHttpTriggerService : IPatchActionPlanHttpTriggerService
    {
        public async Task<Models.ActionPlan> UpdateAsync(Models.ActionPlan actionPlan, ActionPlanPatch actionPlanPatch)
        {
            if (actionPlan == null)
                return null;

            if (!actionPlanPatch.LastModifiedDate.HasValue)
                actionPlanPatch.LastModifiedDate = DateTime.Now;

            actionPlan.Patch(actionPlanPatch);

            var documentDbProvider = new DocumentDBProvider();
            var response = await documentDbProvider.UpdateActionPlanAsync(actionPlan);

            var responseStatusCode = response.StatusCode;

            return responseStatusCode == HttpStatusCode.OK ? actionPlan : null;
        }

        public async Task<Models.ActionPlan> GetActionPlanForCustomerAsync(Guid customerId, Guid actionPlanId)
        {
            var documentDbProvider = new DocumentDBProvider();
            var actionPlan = await documentDbProvider.GetActionPlanForCustomerAsync(customerId, actionPlanId);

            return actionPlan;
        }
    }
}