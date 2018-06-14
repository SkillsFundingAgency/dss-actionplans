using System.ComponentModel.DataAnnotations;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http.Description;

namespace NCS.DSS.ActionPlan.GetActionPlanHttpTrigger
{
    public static class GetActionPlanHttpTrigger
    {
        [FunctionName("Get")]
        [ResponseType(typeof(Models.ActionPlan))]
        [Display(Name = "Get", Description = "Ability to return all action plans for the given customer.")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Customers/{customerId}/Interactions/{interactionId}/ActionPlans")]HttpRequestMessage req, TraceWriter log, string customerId, string interactionId)
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