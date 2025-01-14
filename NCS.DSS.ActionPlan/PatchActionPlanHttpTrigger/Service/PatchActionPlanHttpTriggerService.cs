using Microsoft.Extensions.Logging;
using NCS.DSS.ActionPlan.Cosmos.Provider;
using NCS.DSS.ActionPlan.Models;
using NCS.DSS.ActionPlan.ServiceBus;
using System.Net;

namespace NCS.DSS.ActionPlan.PatchActionPlanHttpTrigger.Service
{
    public class PatchActionPlanHttpTriggerService : IPatchActionPlanHttpTriggerService
    {
        private readonly IActionPlanPatchService _actionPlanPatchService;
        private readonly ICosmosDbProvider _cosmosDbProvider;
        private readonly IActionPlanServiceBusClient _actionPlanServiceBusClient;
        private readonly ILogger<PatchActionPlanHttpTriggerService> _logger;

        public PatchActionPlanHttpTriggerService(
            IActionPlanPatchService actionPlanPatchService, 
            ICosmosDbProvider cosmosDbProvider,
            IActionPlanServiceBusClient actionPlanServiceBusClient, 
            ILogger<PatchActionPlanHttpTriggerService> logger)
        {
            _actionPlanPatchService = actionPlanPatchService;
            _cosmosDbProvider = cosmosDbProvider;
            _actionPlanServiceBusClient = actionPlanServiceBusClient;
            _logger = logger;
        }

        public string PatchResource(string actionPlanJson, ActionPlanPatch actionPlanPatch)
        {
            _logger.LogInformation("Started patching action plan");
            if (string.IsNullOrEmpty(actionPlanJson))
            {
                _logger.LogInformation("Can't patch action plan because input action plan json is null");
                return null;
            }

            if (actionPlanPatch == null)
            {
                _logger.LogInformation("Can't patch action plan because input actionPlanPatch object is null");
                return null;
            }

            _logger.LogInformation("Setting default values for action plan PATCH object.");
            actionPlanPatch.SetDefaultValues();
            _logger.LogInformation("Default values for action plan PATCH object are successfully set.");

            var updatedActionPlan = _actionPlanPatchService.Patch(actionPlanJson, actionPlanPatch);

            _logger.LogInformation("Completed patching action plan");

            return updatedActionPlan;
        }

        public async Task<Models.ActionPlan> UpdateCosmosAsync(string actionPlanJson, Guid actionPlanId)
        {
            if (string.IsNullOrEmpty(actionPlanJson))
            {
                _logger.LogInformation("The actionPlanJson object provided is either null or empty.");
                return null;
            }
            _logger.LogInformation("Started updating action plan in Cosmos DB with ID: {ActionPlanId}", actionPlanId);

            var response = await _cosmosDbProvider.UpdateActionPlanAsync(actionPlanJson, actionPlanId);

            if (response?.StatusCode == HttpStatusCode.OK)
            {
                _logger.LogInformation("Completed updating action plan in Cosmos DB with ID: {ActionPlanId}", actionPlanId);
                return response.Resource;
            }

            _logger.LogError("Failed to update action plan in Cosmos DB with ID: {ActionPlanId}.", actionPlanId);
            return null;
        }

        public async Task<string> GetActionPlanForCustomerAsync(Guid customerId, Guid actionPlanId)
        {
            var actionPlan = await _cosmosDbProvider.GetActionPlanForCustomerToUpdateAsync(customerId, actionPlanId);

            return actionPlan;
        }

        public async Task SendToServiceBusQueueAsync(Models.ActionPlan actionPlan, Guid customerId, string reqUrl)
        {
            try
            {
                _logger.LogInformation("Sending action plan with ID: {ActionPlanId} to Service Bus for customer ID: {CustomerId}.", actionPlan.ActionPlanId, customerId);

                await _actionPlanServiceBusClient.SendPatchMessageAsync(actionPlan, customerId, reqUrl);

                _logger.LogInformation("Successfully sent action plan with ID: {ActionPlanId} to Service Bus for customer ID: {CustomerId}.", actionPlan.ActionPlanId, customerId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while sending action plan with ID: {ActionPlanId} to Service Bus for customer ID: {CustomerId}.", actionPlan.ActionPlanId, customerId);
                throw;
            }
        }
    }
}