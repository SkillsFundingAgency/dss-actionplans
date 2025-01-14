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
                return null;

            actionPlan.SetDefaultValues();

            var response = await _cosmosDbProvider.CreateActionPlanAsync(actionPlan);

            return response.StatusCode == HttpStatusCode.Created ? (dynamic)response.Resource : null;
        }

        public async Task SendToServiceBusQueueAsync(Models.ActionPlan actionPlan, string reqUrl)
        {
            await _actionPlanServiceBusClient.SendPostMessageAsync(actionPlan, reqUrl);
        }
    }
}