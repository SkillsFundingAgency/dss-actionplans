using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http.Description;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using NCS.DSS.ActionPlan.Annotations;

namespace NCS.DSS.ActionPlan.PostActionPlanHttpTrigger
{
    public static class PostActionPlanHttpTrigger
    {
        [FunctionName("Post")]
        [ResponseType(typeof(Models.ActionPlan))]
        [ActionPlanResponse(HttpStatusCode = (int)HttpStatusCode.Created, Description = "Action Plan Created", ShowSchema = true)]
        [ActionPlanResponse(HttpStatusCode = (int)HttpStatusCode.BadRequest, Description = "Unable to create Action Plan", ShowSchema = false)]
        [ActionPlanResponse(HttpStatusCode = (int)HttpStatusCode.Forbidden, Description = "Forbidden", ShowSchema = false)]
        [Display(Name = "Post", Description = "Ability to create a new action plan for a customer.")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Customers/{customerId}/Interactions/{interactionId}/ActionPlans")]HttpRequestMessage req, TraceWriter log, string customerId, string interactionId)
        {
            log.Info("Post Action Plan C# HTTP trigger function processed a request.");

            // Get request body
            var actionPlan = await req.Content.ReadAsAsync<Models.ActionPlan>();

            var actionPlanService = new PostActionPlanHttpTriggerService();
            var actionPlanId = actionPlanService.Create(actionPlan);

            return actionPlanId == null
                ? new HttpResponseMessage(HttpStatusCode.BadRequest)
                : new HttpResponseMessage(HttpStatusCode.Created)
                {
                    Content = new StringContent("Created Action Plan record with Id of : " + actionPlanId)
                };
        }
    }
}