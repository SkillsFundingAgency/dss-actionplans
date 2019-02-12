using System;
using System.Threading.Tasks;
using NCS.DSS.ActionPlan.Cosmos.Provider;

namespace NCS.DSS.ActionPlan.GetActionPlanByIdHttpTrigger.Service
{
    public class GetActionPlanByIdHttpTriggerService : IGetActionPlanByIdHttpTriggerService
    {
        private readonly IDocumentDBProvider _documentDbProvider;

        public GetActionPlanByIdHttpTriggerService(IDocumentDBProvider documentDbProvider)
        {
            _documentDbProvider = documentDbProvider;
        }

        public async Task<Models.ActionPlan> GetActionPlanForCustomerAsync(Guid customerId, Guid actionPlanId)
        {
            var actionPlan = await _documentDbProvider.GetActionPlanForCustomerAsync(customerId, actionPlanId);

            return actionPlan;
        }
    }
}