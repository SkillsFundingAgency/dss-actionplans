using System;
using System.Threading.Tasks;
using NCS.DSS.ActionPlan.Cosmos.Provider;

namespace NCS.DSS.ActionPlan.GetActionPlanByIdHttpTrigger.Service
{
    public class GetActionPlanByIdHttpTriggerService : IGetActionPlanByIdHttpTriggerService
    {
        public async Task<Models.ActionPlan> GetActionPlanForCustomerAsync(Guid customerId, Guid actionPlanId)
        {
            var documentDbProvider = new DocumentDBProvider();
            var actionPlan = await documentDbProvider.GetActionPlanForCustomerAsync(customerId, actionPlanId);

            return actionPlan;
        }
    }
}