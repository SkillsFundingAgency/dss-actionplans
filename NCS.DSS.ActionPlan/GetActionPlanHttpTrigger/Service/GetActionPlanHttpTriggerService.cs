using NCS.DSS.ActionPlan.Cosmos.Provider;

namespace NCS.DSS.ActionPlan.GetActionPlanHttpTrigger.Service
{
    public class GetActionPlanHttpTriggerService : IGetActionPlanHttpTriggerService
    {
        private readonly ICosmosDbProvider _cosmosDbProvider;

        public GetActionPlanHttpTriggerService(ICosmosDbProvider cosmosDbProvider)
        {
            _cosmosDbProvider = cosmosDbProvider;
        }

        public async Task<List<Models.ActionPlan>> GetActionPlansAsync(Guid customerId)
        {
            var actionPlans = await _cosmosDbProvider.GetActionPlansForCustomerAsync(customerId);

            return actionPlans;
        }
    }
}