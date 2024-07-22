using DFC.Common.Standard.Logging;
using DFC.HTTP.Standard;
using DFC.JSON.Standard;
using DFC.Swagger.Standard.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NCS.DSS.ActionPlan.Cosmos.Helper;
using NCS.DSS.ActionPlan.PostActionPlanHttpTrigger.Service;
using NCS.DSS.ActionPlan.Validation;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;

namespace NCS.DSS.ActionPlan.PostActionPlanHttpTrigger.Function
{
    public class PostActionPlanHttpTrigger
    {
        private IResourceHelper _resourceHelper;
        private IValidate _validate;
        private IPostActionPlanHttpTriggerService _actionPlanPostService;
        private ILoggerHelper _loggerHelper;
        private IHttpRequestHelper _httpRequestHelper;
        private IJsonHelper _jsonHelper;

        public PostActionPlanHttpTrigger(
            IResourceHelper resourceHelper,
            IValidate validate,
            IPostActionPlanHttpTriggerService actionPlanPostService,
            ILoggerHelper loggerHelper,
            IHttpRequestHelper httpRequestHelper,
            IJsonHelper jsonHelper)
        {
            _resourceHelper = resourceHelper ;
            _validate = validate;
            _actionPlanPostService = actionPlanPostService;
            _loggerHelper = loggerHelper;
            _httpRequestHelper = httpRequestHelper;
            _jsonHelper = jsonHelper;
        }


        [Function("Post")]
        [ProducesResponseType(typeof(Models.ActionPlan), (int)HttpStatusCode.OK)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Created, Description = "Action Plan Created", ShowSchema = true)]
        [Response(HttpStatusCode = (int)HttpStatusCode.NoContent, Description = "Action Plan does not exist", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.BadRequest, Description = "Request was malformed", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Unauthorized, Description = "API key is unknown or invalid", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Forbidden, Description = "Insufficient access", ShowSchema = false)]
        [Response(HttpStatusCode = 422, Description = "Action Plan validation error(s)", ShowSchema = false)]
        [Display(Name = "Post", Description = "Ability to create a new action plan for a customer. <br>" +
                                              "<br><b>Validation Rules:</b> <br>" +
                                              "<br><b>DateActionPlanCreated:</b> DateActionPlanCreated >= Session.DateAndTimeOfSession <br>" +
                                              "<br><b>DateActionPlanSentToCustomer:</b> DateActionPlanSentToCustomer >= DateActionPlanCreated <br>" +
                                              "<br><b>DateActionPlanAcknowledged:</b> DateActionPlanAcknowledged >= DateActionPlanCreated")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Customers/{customerId}/Interactions/{interactionId}/ActionPlans")]HttpRequest req, ILogger log, string customerId, string interactionId)
        {

            var correlationId = _httpRequestHelper.GetDssCorrelationId(req);
            if (string.IsNullOrEmpty(correlationId))
                log.LogInformation("Unable to locate 'DssCorrelationId' in request header");

            if (!Guid.TryParse(correlationId, out var correlationGuid))
            {
                log.LogInformation("Unable to parse 'DssCorrelationId' to a Guid");
                correlationGuid = Guid.NewGuid();
            }

            log.LogInformation($"DssCorrelationId: [{correlationGuid}]");


            var touchpointId = _httpRequestHelper.GetDssTouchpointId(req);
            if (string.IsNullOrEmpty(touchpointId))
            {
                var response = new BadRequestObjectResult("");
                log.LogWarning($"Response Status Code: [{response.StatusCode}]. Unable to locate 'TouchpointId' in request header");
                return response;
            }

            var ApimURL = _httpRequestHelper.GetDssApimUrl(req);
            if (string.IsNullOrEmpty(ApimURL))
            {
                var response = new BadRequestObjectResult("");
                log.LogWarning($"Response Status Code: [{response.StatusCode}]. Unable to locate 'apimurl' in request header");
                return response;
            }

            var subcontractorId = _httpRequestHelper.GetDssSubcontractorId(req);
            if (string.IsNullOrEmpty(subcontractorId))
                log.LogInformation($"Unable to locate 'SubcontractorId' in request header");

            log.LogInformation($"Post Action Plan C# HTTP trigger function  processed a request. By Touchpoint: [{touchpointId}]");

            if (!Guid.TryParse(customerId, out var customerGuid))
            {
                var response = new BadRequestObjectResult(customerGuid);
                log.LogWarning($"Response Status Code: [{response.StatusCode}]. Unable to parse 'customerId' to a Guid: [{customerId}]");
                return response;
            }

            if (!Guid.TryParse(interactionId, out var interactionGuid))
            {
                var response = new BadRequestObjectResult(interactionGuid);
                log.LogWarning($"Response Status Code: [{response.StatusCode}]. Unable to parse 'interactionId' to a Guid: [{interactionId}]");
                return response;
            }

            Models.ActionPlan actionPlanRequest;

            try
            {
                log.LogInformation($"Attempt to get resource from body of the request");
                actionPlanRequest = await _httpRequestHelper.GetResourceFromRequest<Models.ActionPlan>(req);
            }
            catch (JsonException ex)
            {
                var response = new UnprocessableEntityObjectResult(ex);
                log.LogError($"Response Status Code: [{response.StatusCode}]. Unable to retrieve body from req", ex);
                return response;
            }

            if (actionPlanRequest == null)
            {
                var response = new UnprocessableEntityObjectResult(req);
                log.LogWarning($"Response Status Code: [{response.StatusCode}]. Action plan request is null");
                return response;
            }

            log.LogInformation($"Attempt to set id's for action plan");
            actionPlanRequest.SetIds(customerGuid, interactionGuid, touchpointId, subcontractorId);

            log.LogInformation($"Attempting to see if customer exists [{customerGuid}]");
            var doesCustomerExist = await _resourceHelper.DoesCustomerExist(customerGuid);

            if (!doesCustomerExist)
            {
                var response = new NoContentResult();
                log.LogWarning($"Response Status Code: [{response.StatusCode}]. Customer does not exist [{customerGuid}]");
                return response;
            }

            log.LogInformation($"Attempting to see if this is a read only customer [{customerGuid}]");
            var isCustomerReadOnly = _resourceHelper.IsCustomerReadOnly();

            if (isCustomerReadOnly)
            {
                var response = new ObjectResult(customerGuid) { StatusCode = (int)HttpStatusCode.Forbidden};
                log.LogWarning($"Response Status Code: [{response.StatusCode}]. Customer is read only [{customerGuid}]");
                return response;
            }

            log.LogInformation($"Attempting to get Interaction [{interactionGuid}] for customer [{customerGuid}]");
            var doesInteractionExist = _resourceHelper.DoesInteractionExistAndBelongToCustomer(interactionGuid, customerGuid);

            if (!doesInteractionExist)
            {
                var response = new NoContentResult();
                log.LogWarning($"Response Status Code: [{response.StatusCode}]. Interaction does not exist [{interactionGuid}]");
                return response;
            }

            log.LogInformation($"Attempting to get GetDateAndTimeOfSession for Session [{actionPlanRequest.SessionId}]");
            var dateAndTimeOfSession = await _resourceHelper.GetDateAndTimeOfSession(actionPlanRequest.SessionId.GetValueOrDefault());

            log.LogInformation($"Attempt to validate Action Plan resource");
            var errors = _validate.ValidateResource(actionPlanRequest, dateAndTimeOfSession);

            if (errors != null && errors.Any())
            {
                var response = new UnprocessableEntityObjectResult(errors);
                log.LogWarning($"Response Status Code: [{response.StatusCode}]. Validation errors: [{errors.FirstOrDefault().ErrorMessage}]");
                return response;
            }

            log.LogInformation($"Attempting to get Create Action Plan for customer [{customerGuid}]");
            var actionPlan = await _actionPlanPostService.CreateAsync(actionPlanRequest);

            if (actionPlan != null)
            {
                var response = new ObjectResult(_jsonHelper.SerializeObjectAndRenameIdProperty(actionPlan, "id", "ActionPlanId")) { StatusCode = (int) HttpStatusCode.Created};
                log.LogInformation($"Response Status Code: [{response.StatusCode}]. attempting to send to service bus [{actionPlan.ActionPlanId}]");
                await _actionPlanPostService.SendToServiceBusQueueAsync(actionPlan, ApimURL);
                return response;
            }
            else
            {
                var response = new BadRequestObjectResult(customerGuid);
                log.LogWarning($"Response Status Code: [{response.StatusCode}]. Failed to create the Action Plan");
                return response;
            }
            
        }
    }
}