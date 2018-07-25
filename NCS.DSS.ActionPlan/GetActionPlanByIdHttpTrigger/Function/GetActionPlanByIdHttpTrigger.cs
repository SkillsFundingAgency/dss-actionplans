using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http.Description;
using Microsoft.Extensions.Logging;
using NCS.DSS.ActionPlan.Annotations;
using NCS.DSS.ActionPlan.Cosmos.Helper;
using NCS.DSS.ActionPlan.GetActionPlanByIdHttpTrigger.Service;
using NCS.DSS.ActionPlan.Helpers;
using NCS.DSS.ActionPlan.Ioc;

namespace NCS.DSS.ActionPlan.GetActionPlanByIdHttpTrigger.Function
{
    public static class GetActionPlanByIdHttpTrigger
    {
        [FunctionName("GetById")]
        [ResponseType(typeof(Models.ActionPlan))]
        [Response(HttpStatusCode = (int)HttpStatusCode.OK, Description = "Action Plan found", ShowSchema = true)]
        [Response(HttpStatusCode = (int)HttpStatusCode.NoContent, Description = "Action Plan does not exist", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.BadRequest, Description = "Request was malformed", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Unauthorized, Description = "API key is unknown or invalid", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Forbidden, Description = "Insufficient access", ShowSchema = false)]
        [Display(Name = "Get", Description = "Ability to retrieve an individual action plan for the given customer")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Customers/{customerId}/Interactions/{interactionId}/ActionPlans/{actionPlanId}")]HttpRequestMessage req, ILogger log, string customerId, string interactionId, string actionPlanId,
            [Inject]IResourceHelper resourceHelper,
            [Inject]IHttpRequestMessageHelper httpRequestMessageHelper,
            [Inject]IGetActionPlanByIdHttpTriggerService actionPlanGetService)
        {
            var touchpointId = httpRequestMessageHelper.GetTouchpointId(req);
            if (touchpointId == null)
            {
                log.LogInformation("Unable to locate 'APIM-TouchpointId' in request header");
                return HttpResponseMessageHelper.BadRequest();
            }

            log.LogInformation("Get Action Plan By Id C# HTTP trigger function  processed a request. By Touchpoint " + touchpointId);

            if (!Guid.TryParse(customerId, out var customerGuid))
                return HttpResponseMessageHelper.BadRequest(customerGuid);

            if (!Guid.TryParse(interactionId, out var interactionGuid))
                return HttpResponseMessageHelper.BadRequest(interactionGuid);

            if (!Guid.TryParse(actionPlanId, out var actionPlanGuid))
                return HttpResponseMessageHelper.BadRequest(actionPlanGuid);

            var doesCustomerExist = resourceHelper.DoesCustomerExist(customerGuid);

            if (!doesCustomerExist)
                return HttpResponseMessageHelper.NoContent(customerGuid);

            var doesInteractionExist = resourceHelper.DoesInteractionExist(interactionGuid);

            if (!doesInteractionExist)
                return HttpResponseMessageHelper.NoContent(interactionGuid);

            var actionPlan = await actionPlanGetService.GetActionPlanForCustomerAsync(customerGuid, actionPlanGuid);

            return actionPlan == null ?
                HttpResponseMessageHelper.NoContent(actionPlanGuid) :
                HttpResponseMessageHelper.Ok(actionPlan);

        }
    }
}