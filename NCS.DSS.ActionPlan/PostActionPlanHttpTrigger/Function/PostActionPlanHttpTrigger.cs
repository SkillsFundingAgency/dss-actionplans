using DFC.HTTP.Standard;
using DFC.Swagger.Standard.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using NCS.DSS.ActionPlan.Cosmos.Helper;
using NCS.DSS.ActionPlan.Models;
using NCS.DSS.ActionPlan.PostActionPlanHttpTrigger.Service;
using NCS.DSS.ActionPlan.Validation;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;

namespace NCS.DSS.ActionPlan.PostActionPlanHttpTrigger.Function
{
    public class PostActionPlanHttpTrigger
    {
        private IResourceHelper _resourceHelper;
        private IValidate _validate;
        private IPostActionPlanHttpTriggerService _actionPlanPostService;
        private ILogger<PostActionPlanHttpTrigger> _logger;
        private IHttpRequestHelper _httpRequestHelper;
        private IConvertToDynamic _dynamicHelper;

        public PostActionPlanHttpTrigger(
            IResourceHelper resourceHelper,
            IValidate validate,
            IPostActionPlanHttpTriggerService actionPlanPostService,
            ILogger<PostActionPlanHttpTrigger> logger,
            IHttpRequestHelper httpRequestHelper,
            IConvertToDynamic dynamicHelper)
        {
            _resourceHelper = resourceHelper;
            _validate = validate;
            _actionPlanPostService = actionPlanPostService;
            _logger = logger;
            _httpRequestHelper = httpRequestHelper;
            _dynamicHelper = dynamicHelper;
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
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Customers/{customerId}/Interactions/{interactionId}/ActionPlans")] HttpRequest req, string customerId, string interactionId)
        {
            _logger.LogInformation("Function {FunctionName} has been invoked", nameof(PostActionPlanHttpTrigger));

            var correlationId = _httpRequestHelper.GetDssCorrelationId(req);

            if (!Guid.TryParse(correlationId, out var correlationGuid))
            {
                _logger.LogInformation("Unable to parse 'DssCorrelationId' to a Guid");
                correlationGuid = Guid.NewGuid();
            }

            var touchpointId = _httpRequestHelper.GetDssTouchpointId(req);
            if (string.IsNullOrEmpty(touchpointId))
            {
                _logger.LogInformation("Unable to locate 'TouchpointId' in request header. Correlation GUID: {CorrelationGuid}", correlationGuid);
                return new BadRequestObjectResult(touchpointId);
            }

            var apimUrl = _httpRequestHelper.GetDssApimUrl(req);
            if (string.IsNullOrEmpty(apimUrl))
            {
                _logger.LogInformation("Unable to locate 'apimURL' in request header. Correlation GUID: {CorrelationGuid}", correlationGuid);
                return new BadRequestObjectResult("Unable to locate 'apimUrl' in request header");
            }

            var subcontractorId = _httpRequestHelper.GetDssSubcontractorId(req);
            if (string.IsNullOrEmpty(subcontractorId))
            {
                _logger.LogInformation("Unable to locate 'SubcontractorId' in request header. Correlation GUID: {CorrelationGuid}", correlationGuid);
            }

            _logger.LogTrace("Header validation successful. Associated Touchpoint ID: {TouchpointId}", touchpointId);

            if (!Guid.TryParse(customerId, out var customerGuid))
            {
                _logger.LogInformation("Unable to parse 'customerId' to a GUID. Customer GUID: {CustomerID}", customerId);
                return new BadRequestObjectResult(customerGuid);
            }

            if (!Guid.TryParse(interactionId, out var interactionGuid))
            {
                _logger.LogInformation("Unable to parse 'interactionId' to a GUID. Interaction ID: {InteractionId}", interactionId);
                return new BadRequestObjectResult(interactionGuid);
            }

            Models.ActionPlan actionPlanRequest;
            try
            {                
                _logger.LogTrace("Attempting to get resource from body of the request. Correlation GUID: {CorrelationGuid}", correlationGuid);
                actionPlanRequest = await _httpRequestHelper.GetResourceFromRequest<Models.ActionPlan>(req);
            }
            catch (Exception ex)
            {
                var handledError = _validate.HandleGetResourceFromRequestException(ex);
                if (handledError != null)
                {
                    _logger.LogInformation("Failed to retrieve resource from request. Message: {handledError}", handledError);
                    return new UnprocessableEntityObjectResult(handledError);
                }
                else
                {
                    _logger.LogError(ex, "Unable to read request body. Correlation GUID: {CorrelationGuid}. Exception: {ExceptionMessage}", correlationGuid, ex.Message);
                    return new UnprocessableEntityObjectResult(_dynamicHelper.ExcludeProperty(ex, ["TargetSite", "StackTrace"]));
                }             
            }

            if (actionPlanRequest == null)
            {
                _logger.LogInformation("{ActionPlanRequest} object is NULL. Correlation GUID: {CorrelationGuid}", nameof(actionPlanRequest), correlationGuid);
                return new UnprocessableEntityObjectResult(req);
            }

            _logger.LogTrace("Retrieved resource from request body. Correlation GUID: {CorrelationGuid}", correlationGuid);

            _logger.LogTrace("Attempting to set IDs for Action Plan. Correlation GUID: {CorrelationGuid}", correlationGuid);
            actionPlanRequest.SetIds(customerGuid, interactionGuid, touchpointId, subcontractorId);
            _logger.LogTrace("IDs successfully set for Action Plan. Correlation GUID: {CorrelationGuid}", correlationGuid);

            _logger.LogTrace("Attempting to check if customer exists. Customer GUID: {CustomerId}. Correlation GUID: {CorrelationGuid}", customerGuid, correlationGuid);
            var doesCustomerExist = await _resourceHelper.DoesCustomerExist(customerGuid);

            if (!doesCustomerExist)
            {
                _logger.LogInformation("Customer does not exist. Customer GUID: {CustomerGuid}. Correlation GUID: {CorrelationGuid}", customerGuid, correlationGuid);
                return new NoContentResult();
            }
            _logger.LogTrace("Customer exists. Customer GUID: {CustomerGuid}. Correlation GUID: {CorrelationGuid}", customerGuid, correlationGuid);

            _logger.LogTrace("Attempting to check if customer is read-only. Customer GUID: {CustomerId}. Correlation GUID: {CorrelationGuid}", customerGuid, correlationGuid);
            var isCustomerReadOnly = _resourceHelper.IsCustomerReadOnly();

            if (isCustomerReadOnly)
            {
                var response = new ObjectResult(customerGuid)
                {
                    StatusCode = (int)HttpStatusCode.Forbidden
                };
                _logger.LogInformation("Customer is read-only. Customer GUID: {CustomerId}. Correlation GUID: {CorrelationGuid}", customerGuid, correlationGuid);
                return response;
            }

            _logger.LogTrace("Attempting to get Interaction for Customer. Customer GUID: {CustomerId}. Interaction GUID: {InteractionGuid}. Correlation GUID: {CorrelationGuid}", customerGuid, interactionGuid, correlationGuid);
            var doesInteractionExist = await _resourceHelper.DoesInteractionExistAndBelongToCustomer(interactionGuid, customerGuid);
            if (!doesInteractionExist)
            {
                _logger.LogInformation("Interaction does not exist. Customer GUID: {CustomerId}. Interaction GUID: {InteractionGuid}. Correlation GUID: {CorrelationGuid}", customerGuid, interactionGuid, correlationGuid);
                return new NoContentResult();
            }
            _logger.LogTrace("Interaction exists. Customer GUID: {CustomerId}. Interaction GUID: {InteractionGuid}. Correlation GUID: {CorrelationGuid}", customerGuid, interactionGuid, correlationGuid);

            _logger.LogTrace("Attempting to get Date and Time of Session. SessionID: {SessionID}", actionPlanRequest.SessionId);
            var dateAndTimeOfSession = await _resourceHelper.GetDateAndTimeOfSession(actionPlanRequest.SessionId.GetValueOrDefault());
                        
            _logger.LogTrace("Attempting to validate {ActionPlanRequest} object", nameof(actionPlanRequest));
            var errors = _validate.ValidateResource(actionPlanRequest, dateAndTimeOfSession);
            if (errors != null && errors.Any())
            {
                var er = errors.Select(e => e.ErrorMessage).ToList();
                var response = new UnprocessableEntityObjectResult(errors);
                _logger.LogWarning("Failed to validate {ActionPlanRequest}", nameof(actionPlanRequest));
                return response;
            }
            _logger.LogTrace("Successfully validated {ActionPlanRequest}", nameof(actionPlanRequest));
                        
            _logger.LogTrace("Attempting to POST Action Plan in Cosmos DB. Action Plan GUID: {ActionPlanGuid}", actionPlanRequest.ActionPlanId);
            var actionPlan = await _actionPlanPostService.CreateAsync(actionPlanRequest);

            if (actionPlan != null)
            {
                _logger.LogTrace("Successfully POSTed Action Plan in Cosmos DB. Action Plan GUID: {ActionPlanGuid}", actionPlan.ActionPlanId);

                _logger.LogTrace("Attempting to send message to Service Bus Namespace. Action Plan GUID: {ActionPlanGuid}", actionPlan.ActionPlanId);
                await _actionPlanPostService.SendToServiceBusQueueAsync(actionPlan, apimUrl);
                _logger.LogTrace("Successfully sent message to Service Bus. Action Plan GUID: {ActionPlanGuid}", actionPlan.ActionPlanId);
            }

            if (actionPlan == null)
            {
                _logger.LogInformation("POST request unsuccessful. Action Plan GUID: {ActionPlanGuid}", actionPlanRequest.ActionPlanId);
                return new BadRequestObjectResult(customerGuid);
            }

            _logger.LogTrace("Function {FunctionName} has finished invoking", nameof(PostActionPlanHttpTrigger));
            return new JsonResult(_dynamicHelper.RenameAndExcludeProperty(actionPlan, "id", "ActionPlanId", "CreatedBy"), new JsonSerializerOptions() { }) 
            { 
                StatusCode = (int)HttpStatusCode.Created 
            };
        }
    }
}