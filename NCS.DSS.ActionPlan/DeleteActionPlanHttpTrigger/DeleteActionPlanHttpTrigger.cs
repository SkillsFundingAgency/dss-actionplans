using System;
using System.Net;
using System.Net.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;

namespace NCS.DSS.ActionPlan.DeleteActionPlanHttpTrigger
{
    public static class DeleteActionPlanHttpTrigger
    {
        [Disable]
        [FunctionName("Delete")]
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "Customers/{customerId}/Interactions/{interactionId}/ActionPlans/{actionPlanId}")]HttpRequestMessage req, TraceWriter log, string actionPlanId)
        {
            log.Info("Delete Action Plan C# HTTP trigger function processed a request.");

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
                Content = new StringContent("Deleted Action Plan record with Id of : " + actionPlanGuid)
            };
        }
    }
}