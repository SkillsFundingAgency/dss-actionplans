using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;

namespace NCS.DSS.ActionPlan.GetActionPlanByIdHttpTrigger
{
    public static class GetActionPlanByIdHttpTrigger
    {
        [FunctionName("GetById")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Customers/{customerId:guid}/Interactions/{interactionId:guid}/ActionPlans/{actionPlanId:guid}")]HttpRequestMessage req, TraceWriter log, string actionPlanId)
        {
            log.Info("Get Action Plan By Id C# HTTP trigger function  processed a request.");

            if (!Guid.TryParse(actionPlanId, out var actionPlanGuid))
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(actionPlanId),
                        System.Text.Encoding.UTF8, "application/json")
                };
            }

            var actionPlanService = new GetActionPlanByIdHttpTriggerService();
            var actionPlan = await actionPlanService.GetActionPlan(actionPlanGuid);

            if (actionPlan == null)
                return new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(
                        "Unable to find Action Plan record with Id of : " + actionPlanGuid)
                };

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(actionPlan),
                    System.Text.Encoding.UTF8, "application/json")
            };
        }
    }
}