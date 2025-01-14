using DFC.HTTP.Standard;
using DFC.Swagger.Standard.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using NCS.DSS.ActionPlan.Cosmos.Helper;
using NCS.DSS.ActionPlan.GetActionPlanHttpTrigger.Service;
using NCS.DSS.ActionPlan.Models;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;

namespace NCS.DSS.ActionPlan.GetActionPlanHttpTrigger.Function
{
    public class GetActionPlanHttpTrigger
    {
        private IResourceHelper _resourceHelper;
        private IGetActionPlanHttpTriggerService _actionPlanGetService;
        private IHttpRequestHelper _httpRequestHelper;
        private ILogger<GetActionPlanHttpTrigger> _logger;
        private IConvertToDynamic _dynamicHelper;

        public GetActionPlanHttpTrigger(
            IResourceHelper resourceHelper,
            IGetActionPlanHttpTriggerService actionPlanGetService,
            ILogger<GetActionPlanHttpTrigger> logger,
            IHttpRequestHelper httpRequestHelper,
            IConvertToDynamic dynamicHelper)
        {
            _resourceHelper = resourceHelper;
            _actionPlanGetService = actionPlanGetService;
            _logger = logger;
            _httpRequestHelper = httpRequestHelper;
            _dynamicHelper = dynamicHelper;
        }

        [Function("Get")]
        [ProducesResponseType(typeof(Models.ActionPlan), (int)HttpStatusCode.OK)]
        [Response(HttpStatusCode = (int)HttpStatusCode.OK, Description = "Action Plans found", ShowSchema = true)]
        [Response(HttpStatusCode = (int)HttpStatusCode.NoContent, Description = "Action Plans do not exist", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.BadRequest, Description = "Request was malformed", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Unauthorized, Description = "API key is unknown or invalid", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Forbidden, Description = "Insufficient access", ShowSchema = false)]
        [Display(Name = "Get", Description = "Ability to return all action plans for the given customer.")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Customers/{customerId}/ActionPlans")] HttpRequest req, string customerId)
        {
            _logger.LogInformation("Function {FunctionName} has been invoked", nameof(GetActionPlanHttpTrigger));

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

            _logger.LogInformation("Header validation successful. Associated Touchpoint ID: {TouchpointId}", touchpointId);

            if (!Guid.TryParse(customerId, out var customerGuid))
            {
                _logger.LogWarning("Unable to parse 'customerId' to a GUID. Customer GUID: {CustomerID}", customerId);
                return new BadRequestObjectResult(customerGuid);
            }

            _logger.LogInformation("Attempting to check if customer exists. Customer GUID: {CustomerId}. Correlation GUID: {CorrelationGuid}", customerGuid, correlationGuid);
            var doesCustomerExist = await _resourceHelper.DoesCustomerExist(customerGuid);

            if (!doesCustomerExist)
            {
                _logger.LogWarning("Customer does not exist. Customer GUID: {CustomerGuid}. Correlation GUID: {CorrelationGuid}", customerGuid, correlationGuid);
                return new NoContentResult();
            }
            _logger.LogInformation("Customer exists. Customer GUID: {CustomerGuid}. Correlation GUID: {CorrelationGuid}", customerGuid, correlationGuid);


            _logger.LogInformation("Attempting to get Action Plans for Customer. Customer GUID: {CustomerId}. Correlation GUID: {CorrelationGuid}", customerGuid, correlationGuid);
            var actionPlans = await _actionPlanGetService.GetActionPlansAsync(customerGuid);

            
            if (actionPlans == null)
            {
                _logger.LogInformation("Action Plan(s) does not exist for Customer. Customer GUID: {CustomerGuid}", customerGuid);
                _logger.LogInformation("Function {FunctionName} has finished invoking", nameof(GetActionPlanHttpTrigger));
                return new NoContentResult();
            }

            if (actionPlans.Count == 1)
            {
                _logger.LogInformation("1 Action Plan successfully retrieved. Action Plan GUID: {ActionPlanGuid}", actionPlans.First().ActionPlanId);
                _logger.LogInformation("Function {FunctionName} has finished invoking", nameof(GetActionPlanHttpTrigger));
                return new JsonResult(_dynamicHelper.RenameProperty(actionPlans[0], "id", "ActionPlanId"), 
                    new JsonSerializerOptions()) 
                    { 
                        StatusCode = (int)HttpStatusCode.OK 
                    };
            }

            _logger.LogInformation("{Count} Action Plans successfully retrieved. Customer GUID: {CustomerGuid}", actionPlans.Count, customerGuid);
            _logger.LogInformation("Function {FunctionName} has finished invoking", nameof(GetActionPlanHttpTrigger));

            return new JsonResult(_dynamicHelper.RenameProperty(actionPlans, "id", "ActionPlanId"),
                new JsonSerializerOptions())
                {
                    StatusCode = (int)HttpStatusCode.OK
                };
        }
    }
}