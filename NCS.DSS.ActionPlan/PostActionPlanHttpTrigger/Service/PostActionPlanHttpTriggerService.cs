using System.Net;
using System.Threading.Tasks;
using NCS.DSS.ActionPlan.Cosmos.Provider;
using NCS.DSS.ActionPlan.ServiceBus;

namespace NCS.DSS.ActionPlan.PostActionPlanHttpTrigger.Service
{
    public class PostActionPlanHttpTriggerService : IPostActionPlanHttpTriggerService
    {
        private readonly IDocumentDBProvider _documentDbProvider;

        public PostActionPlanHttpTriggerService(IDocumentDBProvider documentDbProvider)
        {
            _documentDbProvider = documentDbProvider;
        }

        public async Task<Models.ActionPlan> CreateAsync(Models.ActionPlan actionPlan)
        {
            if (actionPlan == null)
                return null;

            actionPlan.SetDefaultValues();

            var response = await _documentDbProvider.CreateActionPlanAsync(actionPlan);

            return response.StatusCode == HttpStatusCode.Created ? (dynamic)response.Resource : null;
        }

        public async Task SendToServiceBusQueueAsync(Models.ActionPlan actionPlan, string reqUrl)
        {
            await ServiceBusClient.SendPostMessageAsync(actionPlan, reqUrl);
        }
    }
}