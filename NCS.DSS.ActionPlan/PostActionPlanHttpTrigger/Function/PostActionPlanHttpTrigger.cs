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
using NCS.DSS.ActionPlan.PostActionPlanHttpTrigger.Service;
using NCS.DSS.ActionPlan.Validation;
using Newtonsoft.Json;

namespace NCS.DSS.ActionPlan.PostActionPlanHttpTrigger.Function
{
    public static class PostActionPlanHttpTrigger
    {
        [FunctionName("Post")]
        [ResponseType(typeof(Models.ActionPlan))]
        [Response(HttpStatusCode = (int)HttpStatusCode.Created, Description = "Action Plan Created", ShowSchema = true)]
        [Response(HttpStatusCode = (int)HttpStatusCode.NoContent, Description = "Action Plan does not exist", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.BadRequest, Description = "Request was malformed", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Unauthorized, Description = "API key is unknown or invalid", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Forbidden, Description = "Insufficient access", ShowSchema = false)]
        [Response(HttpStatusCode = 422, Description = "Action Plan validation error(s)", ShowSchema = false)]
        [Display(Name = "Post", Description = "Ability to create a new action plan for a customer.")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Customers/{customerId}/Interactions/{interactionId}/ActionPlans")]HttpRequestMessage req, ILogger log, string customerId, string interactionId,
            [Inject]IResourceHelper resourceHelper,
            [Inject]IHttpRequestMessageHelper httpRequestMessageHelper,
            [Inject]IValidate validate,
            [Inject]IPostActionPlanHttpTriggerService actionPlanPostService)
        {
            var touchpointId = httpRequestMessageHelper.GetTouchpointId(req);
            if (string.IsNullOrEmpty(touchpointId))
            {
                log.LogInformation("Unable to locate 'TouchpointId' in request header");
                return HttpResponseMessageHelper.BadRequest();
            }

            var ApimURL = httpRequestMessageHelper.GetApimURL(req);
            if (string.IsNullOrEmpty(ApimURL))
            {
                log.LogInformation("Unable to locate 'apimurl' in request header");
                return HttpResponseMessageHelper.BadRequest();
            }

            log.LogInformation("Post Action Plan C# HTTP trigger function processed a request. By Touchpoint " + touchpointId);

            if (!Guid.TryParse(customerId, out var customerGuid))
                return HttpResponseMessageHelper.BadRequest(customerGuid);

            if (!Guid.TryParse(interactionId, out var interactionGuid))
                return HttpResponseMessageHelper.BadRequest(interactionGuid);

            Models.ActionPlan actionPlanRequest;

            try
            {
                actionPlanRequest = await httpRequestMessageHelper.GetActionPlanFromRequest<Models.ActionPlan>(req);
            }
            catch (JsonException ex)
            {
                return HttpResponseMessageHelper.UnprocessableEntity(ex);
            }

            if (actionPlanRequest == null)
                return HttpResponseMessageHelper.UnprocessableEntity(req);

            actionPlanRequest.SetIds(actionPlanRequest, customerGuid, interactionGuid, touchpointId);

            var errors = validate.ValidateResource(actionPlanRequest);

            if (errors != null && errors.Any())
                return HttpResponseMessageHelper.UnprocessableEntity(errors);

            var doesCustomerExist = resourceHelper.DoesCustomerExist(customerGuid);

            if (!doesCustomerExist)
                return HttpResponseMessageHelper.NoContent(customerGuid);

            var isCustomerReadOnly = await resourceHelper.IsCustomerReadOnly(customerGuid);

            if (isCustomerReadOnly)
                return HttpResponseMessageHelper.Forbidden(customerGuid);

            var doesInteractionExist = resourceHelper.DoesInteractionExist(interactionGuid);

            if (!doesInteractionExist)
                return HttpResponseMessageHelper.NoContent(interactionGuid);

            var actionPlan = await actionPlanPostService.CreateAsync(actionPlanRequest);

            if (actionPlan != null)
                await actionPlanPostService.SendToServiceBusQueueAsync(actionPlan, ApimURL);

            return actionPlan == null
                ? HttpResponseMessageHelper.BadRequest(customerGuid)
                : HttpResponseMessageHelper.Created(JsonHelper.SerializeObject(actionPlan));

        }
    }
}