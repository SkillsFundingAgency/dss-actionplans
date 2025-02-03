using DFC.HTTP.Standard;
using DFC.Swagger.Standard.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using NCS.DSS.ActionPlan.Cosmos.Helper;
using NCS.DSS.ActionPlan.GetActionPlanByIdHttpTrigger.Service;
using NCS.DSS.ActionPlan.Models;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;

namespace NCS.DSS.ActionPlan.GetActionPlanByIdHttpTrigger.Function
{
    public class GetActionPlanByIdHttpTrigger
    {
        private IResourceHelper _resourceHelper;
        private IGetActionPlanByIdHttpTriggerService _actionPlanGetService;
        private ILogger<GetActionPlanByIdHttpTrigger> _logger;
        private IHttpRequestHelper _httpRequestHelper;
        private IConvertToDynamic _dynamicHelper;
        public GetActionPlanByIdHttpTrigger(
            IResourceHelper resourceHelper,
            IGetActionPlanByIdHttpTriggerService actionPlanGetService,
            ILogger<GetActionPlanByIdHttpTrigger> logger,
            IHttpRequestHelper httpRequestHelper,
            IConvertToDynamic dynamicHelper)
        {
            _resourceHelper = resourceHelper;
            _actionPlanGetService = actionPlanGetService;
            _logger = logger;
            _httpRequestHelper = httpRequestHelper;
            _dynamicHelper = dynamicHelper;
        }

        [Function("GetById")]
        [ProducesResponseType(typeof(Models.ActionPlan), (int)HttpStatusCode.OK)]
        [Response(HttpStatusCode = (int)HttpStatusCode.OK, Description = "Action Plan found", ShowSchema = true)]
        [Response(HttpStatusCode = (int)HttpStatusCode.NoContent, Description = "Action Plan does not exist", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.BadRequest, Description = "Request was malformed", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Unauthorized, Description = "API key is unknown or invalid", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Forbidden, Description = "Insufficient access", ShowSchema = false)]
        [Display(Name = "Get", Description = "Ability to retrieve an individual action plan for the given customer")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Customers/{customerId}/Interactions/{interactionId}/ActionPlans/{actionPlanId}")] HttpRequest req, string customerId, string interactionId, string actionPlanId)
        {
            _logger.LogInformation("Function {FunctionName} has been invoked", nameof(GetActionPlanByIdHttpTrigger));

            var correlationId = _httpRequestHelper.GetDssCorrelationId(req);
            if (string.IsNullOrEmpty(correlationId))
            {
                _logger.LogInformation("Unable to locate 'DssCorrelationId' in request header");
            }

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

            _logger.LogInformation("Attempting to check if customer exists. Customer GUID: {CustomerId}. Correlation GUID: {CorrelationGuid}", customerGuid, correlationGuid);
            var doesCustomerExist = await _resourceHelper.DoesCustomerExist(customerGuid);

            if (!doesCustomerExist)
            {
                _logger.LogWarning("Customer does not exist. Customer GUID: {CustomerGuid}. Correlation GUID: {CorrelationGuid}", customerGuid, correlationGuid);
                return new NoContentResult();
            }
            _logger.LogInformation("Customer exists. Customer GUID: {CustomerGuid}. Correlation GUID: {CorrelationGuid}", customerGuid, correlationGuid);


            _logger.LogInformation("Attempting to get Interaction for Customer. Customer GUID: {CustomerId}. Interaction GUID: {InteractionGuid}. Correlation GUID: {CorrelationGuid}", customerGuid, interactionGuid, correlationGuid);
            var doesInteractionExist = await _resourceHelper.DoesInteractionExistAndBelongToCustomer(interactionGuid, customerGuid);
            if (!doesInteractionExist)
            {
                _logger.LogWarning("Interaction does not exist. Customer GUID: {CustomerId}. Interaction GUID: {InteractionGuid}. Correlation GUID: {CorrelationGuid}", customerGuid, interactionGuid, correlationGuid);
                return new NoContentResult();
            }
            _logger.LogInformation("Interaction exists. Customer GUID: {CustomerId}. Interaction GUID: {InteractionGuid}. Correlation GUID: {CorrelationGuid}", customerGuid, interactionGuid, correlationGuid);


            _logger.LogInformation("Attempting to get Action Plan. Customer GUID: {CustomerId}. Action Plan GUID: {ActionPlanGuid}. Correlation GUID: {CorrelationGuid}", customerGuid, actionPlanGuid, correlationGuid);
            var actionPlan = await _actionPlanGetService.GetActionPlanForCustomerAsync(customerGuid, actionPlanGuid);
            if (actionPlan == null)
            {
                _logger.LogInformation("Action Plan does not exist for Customer. Customer GUID: {CustomerGuid}", customerGuid);
                _logger.LogInformation("Function {FunctionName} has finished invoking", nameof(GetActionPlanByIdHttpTrigger));
                return new NoContentResult();
            }

            _logger.LogInformation("Action Plan successfully retrieved. Action Plan GUID: {ActionPlanGuid}", actionPlan.ActionPlanId);
            _logger.LogInformation("Function {FunctionName} has finished invoking", nameof(GetActionPlanByIdHttpTrigger));

            return new JsonResult(_dynamicHelper.RenameProperty(actionPlan, "id", "ActionPlanId"),
                new JsonSerializerOptions())
            {
                StatusCode = (int)HttpStatusCode.OK
            };
        }
    }
}