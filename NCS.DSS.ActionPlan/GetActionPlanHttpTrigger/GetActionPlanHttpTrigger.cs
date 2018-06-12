using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;

namespace NCS.DSS.ActionPlan.GetActionPlanHttpTrigger
{
    public static class GetActionPlanHttpTrigger
    {
        [FunctionName("Get")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Customers/{customerId:guid}/Interactions/{interactionId:guid}/ActionPlans")]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("Get Action Plan C# HTTP trigger function processed a request.");

            var actionPlanService = new GetActionPlanHttpTriggerService();
            var actionPlans = await actionPlanService.GetActionPlans();

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(actionPlans),
                    System.Text.Encoding.UTF8, "application/json")
            };
        }
    }
}