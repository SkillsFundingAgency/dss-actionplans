using System;
using System.Net;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;

namespace NCS.DSS.ActionPlan.PutActionPlanHttpTrigger
{
    public static class PutActionPlanHttpTrigger
    {
        [FunctionName("Put")]
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "Customers/{customerId:guid}/Interactions/{interactionId:guid}/ActionPlans/{actionPlanId:guid}")]HttpRequestMessage req, TraceWriter log, string actionPlanId)
        {
            log.Info("Put Action Plan C# HTTP trigger function processed a request.");

            if (!Guid.TryParse(actionPlanId, out var actionPlanGuid))
            {
                return new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent(JsonConvert.SerializeObject(actionPlanId),
                        System.Text.Encoding.UTF8, "application/json")
                };
            }

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("Replaced Action Plan record with Id of : " + actionPlanGuid)
            };
        }
    }
}