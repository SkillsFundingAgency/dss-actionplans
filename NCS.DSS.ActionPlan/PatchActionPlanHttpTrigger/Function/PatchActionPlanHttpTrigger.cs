using DFC.Common.Standard.Logging;
using DFC.HTTP.Standard;
using DFC.JSON.Standard;
using DFC.Swagger.Standard.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NCS.DSS.ActionPlan.Cosmos.Helper;
using NCS.DSS.ActionPlan.Models;
using NCS.DSS.ActionPlan.PatchActionPlanHttpTrigger.Service;
using NCS.DSS.ActionPlan.Validation;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;

namespace NCS.DSS.ActionPlan.PatchActionPlanHttpTrigger.Function
{
    public class PatchActionPlanHttpTrigger
    {
        private IResourceHelper _resourceHelper;
        private IValidate _validate;
        private IPatchActionPlanHttpTriggerService _actionPlanPatchService;
        private ILoggerHelper _loggerHelper;
        private IHttpRequestHelper _httpRequestHelper;
        private IJsonHelper _jsonHelper;

        public PatchActionPlanHttpTrigger(
             IResourceHelper resourceHelper,
             IValidate validate,
             IPatchActionPlanHttpTriggerService actionPlanPatchService,
             ILoggerHelper loggerHelper,
             IHttpRequestHelper httpRequestHelper,
             IJsonHelper jsonHelper)
        {
            _resourceHelper = resourceHelper;
            _validate = validate;
            _actionPlanPatchService = actionPlanPatchService;
            _loggerHelper = loggerHelper;
            _httpRequestHelper = httpRequestHelper;
            _jsonHelper = jsonHelper;
        }

        [Function("Patch")]
        [ProducesResponseType(typeof(Models.ActionPlan), (int)HttpStatusCode.OK)]
        [Response(HttpStatusCode = (int)HttpStatusCode.OK, Description = "Action Plan Updated", ShowSchema = true)]
        [Response(HttpStatusCode = (int)HttpStatusCode.NoContent, Description = "Action Plan does not exist", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.BadRequest, Description = "Request was malformed", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Unauthorized, Description = "API key is unknown or invalid", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Forbidden, Description = "Insufficient access", ShowSchema = false)]
        [Response(HttpStatusCode = 422, Description = "Action Plan validation error(s)", ShowSchema = false)]
        [Display(Name = "Patch", Description = "Ability to modify/update a customers action plan record. <br>" +
                                               "<br><b>Validation Rules:</b> <br>" +
                                               "<br><b>DateActionPlanCreated:</b> DateActionPlanCreated >= Session.DateAndTimeOfSession <br>" +
                                               "<br><b>DateActionPlanSentToCustomer:</b> DateActionPlanSentToCustomer >= DateActionPlanCreated <br>" +
                                               "<br><b>DateActionPlanAcknowledged:</b> DateActionPlanAcknowledged >= DateActionPlanCreated")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "Customers/{customerId}/Interactions/{interactionId}/ActionPlans/{actionPlanId}")] HttpRequest req, ILogger log, string customerId, string interactionId, string actionPlanId)
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

            var apimUrl = _httpRequestHelper.GetDssApimUrl(req);
            if (string.IsNullOrEmpty(apimUrl))
            {
                var response = new BadRequestObjectResult("");
                log.LogWarning($"Response Status Code: [{response.StatusCode}]. Unable to locate 'apimurl' in request header");
                return response;
            }

            var subcontractorId = _httpRequestHelper.GetDssSubcontractorId(req);
            if (string.IsNullOrEmpty(subcontractorId))
                log.LogInformation($"Unable to locate 'SubcontractorId' in request header");

            log.LogInformation($"Patch Action Plan C# HTTP trigger function  processed a request. By Touchpoint: [{touchpointId}]");

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

            if (!Guid.TryParse(actionPlanId, out var actionPlanGuid))
            {
                var response = new BadRequestObjectResult(actionPlanGuid);
                log.LogWarning($"Response Status Code: [{response.StatusCode}]. Unable to parse 'actionPlanId' to a Guid: [{actionPlanId}]");
                return response;
            }

            ActionPlanPatch actionPlanPatchRequest;

            try
            {
                log.LogInformation($"Attempt to get resource from body of the request");
                actionPlanPatchRequest = await _httpRequestHelper.GetResourceFromRequest<ActionPlanPatch>(req);
            }
            catch (JsonException ex)
            {
                var response = new UnprocessableEntityObjectResult(ex);
                log.LogError($"Response Status Code: [{response.StatusCode}]. Unable to retrieve body from req. ", ex.Message);
                return response;
            }

            if (actionPlanPatchRequest == null)
            {
                var response = new UnprocessableEntityObjectResult(req);
                log.LogWarning($"Response Status Code: [{response.StatusCode}]. Action plan patch request is null");
                return response;
            }

            log.LogInformation($"Attempt to set id's for action plan patch");
            actionPlanPatchRequest.SetIds(touchpointId, subcontractorId);

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
                var response = new ObjectResult(customerGuid)
                {
                    StatusCode = (int)HttpStatusCode.Forbidden
                };
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

            log.LogInformation($"Attempting to get action plan [{actionPlanGuid}] for customer [{customerGuid}]");
            var actionPlanForCustomer = await _actionPlanPatchService.GetActionPlanForCustomerAsync(customerGuid, actionPlanGuid);

            if (actionPlanForCustomer == null)
            {
                var response = new NoContentResult();
                log.LogWarning($"Response Status Code: [{response.StatusCode}]. ActionPlan does not exist [{actionPlanGuid}]");
                return response;
            }

            var patchedActionPlan = _actionPlanPatchService.PatchResource(actionPlanForCustomer, actionPlanPatchRequest);

            if (patchedActionPlan == null)
            {
                var response = new NoContentResult();
                log.LogWarning($"Response Status Code: [{response.StatusCode}]. ActionPlan does not exist [{actionPlanGuid}]");
                return response;
            }

            Models.ActionPlan actionPlanValidationObject;

            try
            {
                actionPlanValidationObject = JsonConvert.DeserializeObject<Models.ActionPlan>(patchedActionPlan);
            }
            catch (JsonException ex)
            {
                log.LogError("Unable to retrieve body from req", ex.Message);
                throw;
            }

            if (actionPlanValidationObject == null)
            {
                var response = new UnprocessableEntityObjectResult(req);
                log.LogWarning($"Response Status Code: [{response.StatusCode}]. Action Plan Validation Object is null");
                return response;
            }

            log.LogInformation($"Attempting to get GetDateAndTimeOfSession for Session [{actionPlanValidationObject.SessionId}]");
            var dateAndTimeOfSession = await _resourceHelper.GetDateAndTimeOfSession(actionPlanValidationObject.SessionId.GetValueOrDefault());

            log.LogInformation($"Attempt to validate resource");
            var errors = _validate.ValidateResource(actionPlanValidationObject, dateAndTimeOfSession);

            if (errors != null && errors.Any())
            {
                var response = new UnprocessableEntityObjectResult(errors);
                log.LogWarning($"Response Status Code: [{response.StatusCode}].  Validation errors: [{errors.FirstOrDefault().ErrorMessage}]");
                return response;
            }

            log.LogInformation($"Attempting to update action plan [{actionPlanGuid}]");
            var updatedActionPlan = await _actionPlanPatchService.UpdateCosmosAsync(patchedActionPlan, actionPlanGuid);

            if (updatedActionPlan != null)
            {
                var response = new OkObjectResult(_jsonHelper.SerializeObjectAndRenameIdProperty(updatedActionPlan, "id", "ActionPlanId"));                
                log.LogInformation($"Response Status Code: [{response.StatusCode}].Patch succeeded, attempting to send to service bus [{actionPlanGuid}]");
                await _actionPlanPatchService.SendToServiceBusQueueAsync(updatedActionPlan, customerGuid, apimUrl);
                return response;
            }
            else
            {
                var response = new BadRequestObjectResult(actionPlanGuid);
                log.LogWarning($"Response Status Code: [{response.StatusCode}]. Failed to patch a resource");
                return response;
            }
            
        }
    }
}