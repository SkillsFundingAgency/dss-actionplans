using DFC.HTTP.Standard;
using DFC.Swagger.Standard.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using NCS.DSS.ActionPlan.Cosmos.Helper;
using NCS.DSS.ActionPlan.Models;
using NCS.DSS.ActionPlan.PatchActionPlanHttpTrigger.Service;
using NCS.DSS.ActionPlan.Validation;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;

namespace NCS.DSS.ActionPlan.PatchActionPlanHttpTrigger.Function
{
    public class PatchActionPlanHttpTrigger
    {
        private IResourceHelper _resourceHelper;
        private IValidate _validate;
        private IPatchActionPlanHttpTriggerService _actionPlanPatchService;
        private ILogger<PatchActionPlanHttpTrigger> _logger;
        private IHttpRequestHelper _httpRequestHelper;
        private IConvertToDynamic _dynamicHelper;
        public PatchActionPlanHttpTrigger(
             IResourceHelper resourceHelper,
             IValidate validate,
             IPatchActionPlanHttpTriggerService actionPlanPatchService,
             ILogger<PatchActionPlanHttpTrigger> logger,
             IHttpRequestHelper httpRequestHelper,
             IConvertToDynamic dynamicHelper)
        {
            _resourceHelper = resourceHelper;
            _validate = validate;
            _actionPlanPatchService = actionPlanPatchService;
            _logger = logger;
            _httpRequestHelper = httpRequestHelper;
            _dynamicHelper = dynamicHelper;
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
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "Customers/{customerId}/Interactions/{interactionId}/ActionPlans/{actionPlanId}")] HttpRequest req, string customerId, string interactionId, string actionPlanId)
        {
            _logger.LogInformation("Function {FunctionName} has been invoked", nameof(PatchActionPlanHttpTrigger));

            var correlationId = _httpRequestHelper.GetDssCorrelationId(req);
            if (string.IsNullOrEmpty(correlationId))
                _logger.LogInformation("Unable to locate 'DssCorrelationId' in request header");

            if (!Guid.TryParse(correlationId, out var correlationGuid))
            {
                _logger.LogInformation("Unable to parse 'DssCorrelationId' to a Guid");
                correlationGuid = Guid.NewGuid();
            }

            var touchpointId = _httpRequestHelper.GetDssTouchpointId(req);
            if (string.IsNullOrEmpty(touchpointId))
            {
                _logger.LogWarning("Unable to locate 'TouchpointId' in request header. Correlation GUID: {CorrelationGuid}", correlationGuid);
                return new BadRequestObjectResult(touchpointId);
            }

            var apimUrl = _httpRequestHelper.GetDssApimUrl(req);
            if (string.IsNullOrEmpty(apimUrl))
            {
                _logger.LogWarning("Unable to locate 'apimURL' in request header. Correlation GUID: {CorrelationGuid}", correlationGuid);
                return new BadRequestObjectResult("Unable to locate 'apimUrl' in request header");
            }

            var subcontractorId = _httpRequestHelper.GetDssSubcontractorId(req);
            if (string.IsNullOrEmpty(subcontractorId))
            {
                _logger.LogInformation("Unable to locate 'SubcontractorId' in request header. Correlation GUID: {CorrelationGuid}", correlationGuid);
            }

            _logger.LogInformation("Header validation successful. Associated Touchpoint ID: {TouchpointId}", touchpointId);

            if (!Guid.TryParse(customerId, out var customerGuid))
            {
                _logger.LogWarning("Unable to parse 'customerId' to a GUID. Customer GUID: {CustomerID}", customerId);
                return new BadRequestObjectResult(customerGuid);
            }

            if (!Guid.TryParse(interactionId, out var interactionGuid))
            {
                _logger.LogWarning("Unable to parse 'interactionId' to a GUID. Interaction ID: {InteractionId}", interactionId);
                return new BadRequestObjectResult(interactionGuid);
            }

            if (!Guid.TryParse(actionPlanId, out var actionPlanGuid))
            {
                _logger.LogWarning("Unable to parse 'actionPlanId' to a GUID. Action Plan ID: {ActionplanId}", actionPlanId);
                return new BadRequestObjectResult(actionPlanGuid);
            }

            ActionPlanPatch actionPlanPatchRequest;
            try
            {
                _logger.LogInformation("Attempting to get resource from body of the request. Correlation GUID: {CorrelationGuid}", correlationGuid);
                actionPlanPatchRequest = await _httpRequestHelper.GetResourceFromRequest<ActionPlanPatch>(req);
            }
            catch (Exception ex)
            {           
                _logger.LogError(ex, "Unable to read request body. Correlation GUID: {CorrelationGuid}. Exception: {ExceptionMessage}", correlationGuid, ex.Message);
                return new UnprocessableEntityObjectResult(_dynamicHelper.ExcludeProperty(ex, ["TargetSite"]));
            }

            if (actionPlanPatchRequest == null)
            {
                _logger.LogWarning("{ActionPlanPatchRequest} object is NULL. Correlation GUID: {CorrelationGuid}", nameof(actionPlanPatchRequest), correlationGuid);
                return new UnprocessableEntityObjectResult(req);
            }

            _logger.LogInformation("Retrieved resource from request body. Correlation GUID: {CorrelationGuid}", correlationGuid);
                        
            _logger.LogInformation("Attempting to set IDs for Action Plan PATCH. Correlation GUID: {CorrelationGuid}", correlationGuid);
            actionPlanPatchRequest.SetIds(touchpointId, subcontractorId);
            _logger.LogInformation("IDs successfully set for Action Plan PATCH. Correlation GUID: {CorrelationGuid}", correlationGuid);

            _logger.LogInformation("Attempting to check if customer exists. Customer GUID: {CustomerId}. Correlation GUID: {CorrelationGuid}", customerGuid, correlationGuid);
            var doesCustomerExist = await _resourceHelper.DoesCustomerExist(customerGuid);

            if (!doesCustomerExist)
            {
                _logger.LogWarning("Customer does not exist. Customer GUID: {CustomerGuid}. Correlation GUID: {CorrelationGuid}", customerGuid, correlationGuid);
                return new NoContentResult();
            }
            _logger.LogInformation("Customer exists. Customer GUID: {CustomerGuid}. Correlation GUID: {CorrelationGuid}", customerGuid, correlationGuid);

            _logger.LogInformation("Attempting to check if customer is read-only. Customer GUID: {CustomerId}. Correlation GUID: {CorrelationGuid}", customerGuid, correlationGuid);
            var isCustomerReadOnly = _resourceHelper.IsCustomerReadOnly();

            if (isCustomerReadOnly)
            {
                var response = new ObjectResult(customerGuid)
                {
                    StatusCode = (int)HttpStatusCode.Forbidden
                };                
                _logger.LogWarning("Customer is read-only. Customer GUID: {CustomerId}. Correlation GUID: {CorrelationGuid}", customerGuid, correlationGuid);
                return response;
            }
                        
            _logger.LogInformation("Attempting to get Interaction for Customer. Customer GUID: {CustomerId}. Interaction GUID: {InteractionGuid}. Correlation GUID: {CorrelationGuid}", customerGuid, interactionGuid, correlationGuid);
            var doesInteractionExist = await _resourceHelper.DoesInteractionExistAndBelongToCustomer(interactionGuid, customerGuid);
            if (!doesInteractionExist)
            {
                _logger.LogWarning("Interaction does not exist. Customer GUID: {CustomerId}. Interaction GUID: {InteractionGuid}. Correlation GUID: {CorrelationGuid}", customerGuid, interactionGuid, correlationGuid);
                return new NoContentResult();
            }
            _logger.LogInformation("Interaction exists. Customer GUID: {CustomerId}. Interaction GUID: {InteractionGuid}. Correlation GUID: {CorrelationGuid}", customerGuid, interactionGuid, correlationGuid);


            _logger.LogInformation("Attempting to get Action Plan for Customer. Customer GUID: {CustomerId}. Action Plan GUID: {ActionPlanGuid}. Correlation GUID: {CorrelationGuid}", customerGuid, actionPlanGuid, correlationGuid);
            var actionPlanForCustomer = await _actionPlanPatchService.GetActionPlanForCustomerAsync(customerGuid, actionPlanGuid);
            if (actionPlanForCustomer == null)
            {                                
                _logger.LogWarning("Action Plan does not exist. Customer GUID: {CustomerId}. Action Plan GUID: {ActionPlanGuid}. Correlation GUID: {CorrelationGuid}", customerGuid, actionPlanGuid, correlationGuid);
                return new NoContentResult();
            }

            _logger.LogInformation("Attempting to PATCH Action Plan resource.");
            var patchedActionPlan = _actionPlanPatchService.PatchResource(actionPlanForCustomer, actionPlanPatchRequest);
            if (patchedActionPlan == null)
            {
                _logger.LogWarning("Failed to PATCH Action Plan resource.");
                return new NoContentResult();
            }

            Models.ActionPlan actionPlanValidationObject;

            _logger.LogInformation("Attempting to deserialize the PATCH Action Plan resource.");
            try
            {
                actionPlanValidationObject = JsonSerializer.Deserialize<Models.ActionPlan>(patchedActionPlan);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failure deserializing the PATCH Action Plan resource. Correlation GUID: {CorrelationGuid}. Exception: {ExceptionMessage}", correlationGuid, ex.Message);
                throw;
            }

            if (actionPlanValidationObject == null)
            {   
                _logger.LogWarning("Action Plan validation object is NULL. Correlation GUID: {CorrelationGuid}", correlationGuid);
                return new UnprocessableEntityObjectResult(req);
            }

            _logger.LogInformation("Attempting to get Date and Time of Session. SessionID: {SessionID}", actionPlanValidationObject.SessionId);            
            var dateAndTimeOfSession = await _resourceHelper.GetDateAndTimeOfSession(actionPlanValidationObject.SessionId.GetValueOrDefault());

            _logger.LogInformation("Attempting to validate {ActionPlanValidationObject} object", nameof(actionPlanValidationObject));
            var errors = _validate.ValidateResource(actionPlanValidationObject, dateAndTimeOfSession);
            if (errors != null && errors.Any())
            {
                var er = errors.Select(e => e.ErrorMessage).ToList();
                var response = new UnprocessableEntityObjectResult(errors);
                _logger.LogWarning("Failed to validate {ActionPlanValidationObject}", nameof(actionPlanValidationObject));
                return response;
            }
            _logger.LogInformation("Successfully validated {ActionPlanValidationObject}", nameof(actionPlanValidationObject));
                        
            _logger.LogInformation("Attempting to PATCH Action Plan in Cosmos DB. Action Plan GUID: {ActionPlanGuid}", actionPlanGuid);
            var updatedActionPlan = await _actionPlanPatchService.UpdateCosmosAsync(patchedActionPlan, actionPlanGuid);

            if (updatedActionPlan != null)
            {
                _logger.LogInformation("Successfully PATCH an Action Plan in Cosmos DB. Action Plan GUID: {ActionPlanGuid}", actionPlanGuid);
                
                _logger.LogInformation("Attempting to send message to Service Bus Namespace. Action Plan GUID: {ActionPlanGuid}", actionPlanGuid);
                await _actionPlanPatchService.SendToServiceBusQueueAsync(updatedActionPlan, customerGuid, apimUrl);
                _logger.LogInformation("Successfully sent message to Service Bus. Action Plan GUID: {ActionPlanGuid}", actionPlanGuid);
            }

            if (updatedActionPlan == null)
            {
                _logger.LogWarning("PATCH request unsuccessful. Action Plan GUID: {ActionPlanGuid}", actionPlanGuid);
                _logger.LogInformation("Function {FunctionName} has finished invoking", nameof(PatchActionPlanHttpTrigger));
                return new BadRequestObjectResult(actionPlanGuid);
            }

            _logger.LogInformation("Function {FunctionName} has finished invoking", nameof(PatchActionPlanHttpTrigger));
            return new JsonResult(_dynamicHelper.ExcludeProperty(updatedActionPlan, "CreatedBy"), new JsonSerializerOptions()) 
            { 
                StatusCode = (int)HttpStatusCode.OK 
            };
        }
    }
}