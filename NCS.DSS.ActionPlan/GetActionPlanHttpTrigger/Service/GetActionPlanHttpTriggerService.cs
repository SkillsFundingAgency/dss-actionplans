using Microsoft.Extensions.Logging;
using NCS.DSS.ActionPlan.Cosmos.Provider;

namespace NCS.DSS.ActionPlan.GetActionPlanHttpTrigger.Service
{
    public class GetActionPlanHttpTriggerService : IGetActionPlanHttpTriggerService
    {
        private readonly ICosmosDbProvider _cosmosDbProvider;
        private readonly ILogger<GetActionPlanHttpTriggerService> _logger;

        public GetActionPlanHttpTriggerService(ICosmosDbProvider cosmosDbProvider, ILogger<GetActionPlanHttpTriggerService> logger)
        {
            _cosmosDbProvider = cosmosDbProvider;
            _logger = logger;
        }

        public async Task<List<Models.ActionPlan>> GetActionPlansAsync(Guid customerId)
        {
            _logger.LogInformation("Attempting to get Action Plans for Customer. Customer ID: {CustomerId}.", customerId);
            var actionPlans = await _cosmosDbProvider.GetActionPlansForCustomerAsync(customerId);

            if (actionPlans == null)
            {
                _logger.LogInformation("No Action Plan exist for Customer. Customer GUID: {CustomerId}", customerId);
                return null;
            }

            _logger.LogInformation("{Count} Action Plan(s) successfully retrieved. Customer GUID: {CustomerId}", actionPlans.Count, customerId);
            return actionPlans;
        }
    }
}