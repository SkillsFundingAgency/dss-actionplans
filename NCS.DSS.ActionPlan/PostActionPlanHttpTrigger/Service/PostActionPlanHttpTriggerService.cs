using Microsoft.Extensions.Logging;
using NCS.DSS.ActionPlan.Cosmos.Provider;
using NCS.DSS.ActionPlan.ServiceBus;
using System.Net;

namespace NCS.DSS.ActionPlan.PostActionPlanHttpTrigger.Service
{
    public class PostActionPlanHttpTriggerService : IPostActionPlanHttpTriggerService
    {
        private readonly ICosmosDbProvider _cosmosDbProvider;
        private readonly IActionPlanServiceBusClient _actionPlanServiceBusClient;
        private readonly ILogger<PostActionPlanHttpTriggerService> _logger;

        public PostActionPlanHttpTriggerService(
            ICosmosDbProvider cosmosDbProvider, 
            IActionPlanServiceBusClient actionPlanServiceBusClient,
            ILogger<PostActionPlanHttpTriggerService> logger)
        {
            _cosmosDbProvider = cosmosDbProvider;
            _actionPlanServiceBusClient = actionPlanServiceBusClient;
            _logger = logger;
        }

        public async Task<Models.ActionPlan> CreateAsync(Models.ActionPlan actionPlan)
        {
            if (actionPlan == null)
            {
                _logger.LogInformation("The actionPlan object provided is null.");
                return null;
            }

            actionPlan.SetDefaultValues();

            var response = await _cosmosDbProvider.CreateActionPlanAsync(actionPlan);

            if (response?.StatusCode == HttpStatusCode.Created)
            {
                _logger.LogInformation("Completed creating action plan in Cosmos DB with ID: {ActionPlanId}", actionPlan.ActionPlanId);
                return response.Resource;
            }

            _logger.LogError("Failed to creating action plan in Cosmos DB with ID: {ActionPlanId}.", actionPlan.ActionPlanId);
            return null;
        }

        public async Task SendToServiceBusQueueAsync(Models.ActionPlan actionPlan, string reqUrl)
        {
            try
            {
                _logger.LogInformation("Sending action plan with ID: {ActionPlanId} to Service Bus.", actionPlan.ActionPlanId);
                await _actionPlanServiceBusClient.SendPostMessageAsync(actionPlan, reqUrl);
                _logger.LogInformation("Successfully sent action plan with ID: {ActionPlanId} to Service Bus.", actionPlan.ActionPlanId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while sending action plan with ID: {ActionPlanId} to Service Bus.", actionPlan.ActionPlanId);
                throw;
            }
        }
    }
}