using NCS.DSS.ActionPlan.Cosmos.Provider;

namespace NCS.DSS.ActionPlan.GetActionPlanByIdHttpTrigger.Service
{
    public class GetActionPlanByIdHttpTriggerService : IGetActionPlanByIdHttpTriggerService
    {
        private readonly ICosmosDbProvider _cosmosDbProvider;

        public GetActionPlanByIdHttpTriggerService(ICosmosDbProvider cosmosDbProvider)
        {
            _cosmosDbProvider = cosmosDbProvider;
        }

        public async Task<Models.ActionPlan> GetActionPlanForCustomerAsync(Guid customerId, Guid actionPlanId)
        {
            var actionPlan = await _cosmosDbProvider.GetActionPlanForCustomerAsync(customerId, actionPlanId);

            return actionPlan;
        }
    }
}