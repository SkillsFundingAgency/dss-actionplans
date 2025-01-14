using Microsoft.Extensions.Logging;
using NCS.DSS.ActionPlan.Cosmos.Provider;

namespace NCS.DSS.ActionPlan.GetActionPlanByIdHttpTrigger.Service
{
    public class GetActionPlanByIdHttpTriggerService : IGetActionPlanByIdHttpTriggerService
    {
        private readonly ICosmosDbProvider _cosmosDbProvider;
        private readonly ILogger<GetActionPlanByIdHttpTriggerService> _logger;

        public GetActionPlanByIdHttpTriggerService(ICosmosDbProvider cosmosDbProvider, ILogger<GetActionPlanByIdHttpTriggerService> logger)
        {
            _cosmosDbProvider = cosmosDbProvider;
            _logger = logger;
        }

        public async Task<Models.ActionPlan> GetActionPlanForCustomerAsync(Guid customerId, Guid actionPlanId)
        {
            _logger.LogInformation("Attempting to get Action Plan for Customer. Customer ID: {CustomerId}.", customerId);
            var actionPlan = await _cosmosDbProvider.GetActionPlanForCustomerAsync(customerId, actionPlanId);

            if (actionPlan == null)
            {
                _logger.LogInformation("Action Plan does not exist for Customer. Action Plan GUID: {ActionPlanId} Customer GUID: {CustomerId}", actionPlanId, customerId);
                return null;
            }

            _logger.LogInformation("Action Plan successfully retrieved. Action Plan GUID: {ActionPlanId} Customer GUID: {CustomerId}", actionPlan.ActionPlanId, customerId);
            return actionPlan;
        }
    }
}