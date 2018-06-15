using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http;
using System.Web.Http.Description;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using NCS.DSS.ActionPlan.Annotations;
using Newtonsoft.Json;

namespace NCS.DSS.ActionPlan.PatchActionPlanHttpTrigger
{
    public static class PatchActionPlanHttpTrigger
    {
        [FunctionName("Patch")]
        [ResponseType(typeof(Models.ActionPlan))]
        [ActionPlanResponse(HttpStatusCode = (int)HttpStatusCode.OK, Description = "Action Plan Updated", ShowSchema = true)]
        [ActionPlanResponse(HttpStatusCode = (int)HttpStatusCode.NotFound, Description = "Supplied Action Plan Id does not exist", ShowSchema = false)]
        [Display(Name = "Patch", Description = "Ability to modify/update a customers action plan record.")]
        public static HttpResponseMessage Run([HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "Customers/{customerId}/Interactions/{interactionId}/ActionPlans/{actionPlanId}")]HttpRequestMessage req, TraceWriter log, string customerId, string interactionId, string actionPlanId)
        {
            log.Info("Patch Action Plan C# HTTP trigger function processed a request.");

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
                Content = new StringContent("Updated Action Plan record with Id of : " + actionPlanGuid)
            };
        }
    }
}
