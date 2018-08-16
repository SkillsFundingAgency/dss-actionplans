using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http.Description;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using NCS.DSS.ActionPlan.Annotations;
using NCS.DSS.ActionPlan.Cosmos.Helper;
using NCS.DSS.ActionPlan.Helpers;
using NCS.DSS.ActionPlan.Ioc;
using NCS.DSS.ActionPlan.PatchActionPlanHttpTrigger.Service;
using NCS.DSS.ActionPlan.Validation;
using Newtonsoft.Json;

namespace NCS.DSS.ActionPlan.PatchActionPlanHttpTrigger.Function
{
    public static class PatchActionPlanHttpTrigger
    {
        [FunctionName("Patch")]
        [ResponseType(typeof(Models.ActionPlan))]
        [Response(HttpStatusCode = (int)HttpStatusCode.OK, Description = "Action Plan Updated", ShowSchema = true)]
        [Response(HttpStatusCode = (int)HttpStatusCode.NoContent, Description = "Action Plan does not exist", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.BadRequest, Description = "Request was malformed", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Unauthorized, Description = "API key is unknown or invalid", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Forbidden, Description = "Insufficient access", ShowSchema = false)]
        [Response(HttpStatusCode = 422, Description = "Action Plan validation error(s)", ShowSchema = false)]
        [Display(Name = "Patch", Description = "Ability to modify/update a customers action plan record.")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "Customers/{customerId}/Interactions/{interactionId}/ActionPlans/{actionPlanId}")]HttpRequestMessage req, ILogger log, string customerId, string interactionId, string actionPlanId,
            [Inject]IResourceHelper resourceHelper, 
            [Inject]IHttpRequestMessageHelper httpRequestMessageHelper,
            [Inject]IValidate validate,
            [Inject]IPatchActionPlanHttpTriggerService actionPlanPatchService)
        {
            var touchpointId = httpRequestMessageHelper.GetTouchpointId(req);
            if (string.IsNullOrEmpty(touchpointId))
            {
                log.LogInformation("Unable to locate 'TouchpointId' in request header");
                return HttpResponseMessageHelper.BadRequest();
            }

            log.LogInformation("Patch Action Plan C# HTTP trigger function processed a request. By Touchpoint " + touchpointId);

            if (!Guid.TryParse(customerId, out var customerGuid))
                return HttpResponseMessageHelper.BadRequest(customerGuid);

            if (!Guid.TryParse(interactionId, out var interactionGuid))
                return HttpResponseMessageHelper.BadRequest(interactionGuid);

            if (!Guid.TryParse(actionPlanId, out var actionPlanGuid))
                return HttpResponseMessageHelper.BadRequest(actionPlanGuid);

            Models.ActionPlanPatch actionPlanPatchRequest;

            try
            {
                actionPlanPatchRequest = await httpRequestMessageHelper.GetActionPlanFromRequest<Models.ActionPlanPatch>(req);
            }
            catch (JsonException ex)
            {
                return HttpResponseMessageHelper.UnprocessableEntity(ex);
            }

            if (actionPlanPatchRequest == null)
                return HttpResponseMessageHelper.UnprocessableEntity(req);

            actionPlanPatchRequest.LastModifiedTouchpointId = touchpointId;

            var errors = validate.ValidateResource(actionPlanPatchRequest);

            if (errors != null && errors.Any())
                return HttpResponseMessageHelper.UnprocessableEntity(errors);

            var doesCustomerExist = resourceHelper.DoesCustomerExist(customerGuid);

            if (!doesCustomerExist)
                return HttpResponseMessageHelper.NoContent(customerGuid);

            var doesInteractionExist = resourceHelper.DoesInteractionExist(interactionGuid);

            if (!doesInteractionExist)
                return HttpResponseMessageHelper.NoContent(interactionGuid);
        
            var actionPlan = await actionPlanPatchService.GetActionPlanForCustomerAsync(customerGuid, actionPlanGuid);

            if (actionPlan == null)
                return HttpResponseMessageHelper.NoContent(actionPlanGuid);

            var updatedActionPlan = await actionPlanPatchService.UpdateAsync(actionPlan, actionPlanPatchRequest);

            if (updatedActionPlan != null)
                await actionPlanPatchService.SendToServiceBusQueueAsync(updatedActionPlan, customerGuid,  req.RequestUri.AbsoluteUri);

            return updatedActionPlan == null ?
                HttpResponseMessageHelper.BadRequest(actionPlanGuid) :
                HttpResponseMessageHelper.Ok(JsonHelper.SerializeObject(updatedActionPlan));

        }
    }
}
