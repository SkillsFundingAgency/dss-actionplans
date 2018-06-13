using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http.Description;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace NCS.DSS.ActionPlan.PostActionPlanHttpTrigger
{
    public static class PostActionPlanHttpTrigger
    {
        [FunctionName("Post")]
        [ResponseType(typeof(Models.ActionPlan))]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Customers/{customerId:guid}/Interactions/{interactionId:guid}/ActionPlans")]HttpRequestMessage req, TraceWriter log)
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