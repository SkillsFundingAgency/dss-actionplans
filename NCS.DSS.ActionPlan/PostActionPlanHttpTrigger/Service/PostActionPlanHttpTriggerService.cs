using System.Net;
using System.Threading.Tasks;
using NCS.DSS.ActionPlan.Cosmos.Provider;
using NCS.DSS.ActionPlan.ServiceBus;

namespace NCS.DSS.ActionPlan.PostActionPlanHttpTrigger.Service
{
    public class PostActionPlanHttpTriggerService : IPostActionPlanHttpTriggerService
    {
        public async Task<Models.ActionPlan> CreateAsync(Models.ActionPlan actionPlan)
        {
            if (actionPlan == null)
                return null;

            actionPlan.SetDefaultValues();

            var documentDbProvider = new DocumentDBProvider();

            var response = await documentDbProvider.CreateActionPlanAsync(actionPlan);

            return response.StatusCode == HttpStatusCode.Created ? (dynamic)response.Resource : null;
        }

        public async Task SendToServiceBusQueueAsync(Models.ActionPlan actionPlan, string reqUrl)
        {
            await ServiceBusClient.SendPostMessageAsync(actionPlan, reqUrl);
        }
    }
}