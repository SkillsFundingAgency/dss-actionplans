using DFC.HTTP.Standard;
using DFC.Swagger.Standard.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using NCS.DSS.ActionPlan.Cosmos.Helper;
using NCS.DSS.ActionPlan.GetActionPlanHttpTrigger.Service;
using NCS.DSS.ActionPlan.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

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
            var correlationId = _httpRequestHelper.GetDssCorrelationId(req);
            if (string.IsNullOrEmpty(correlationId))
                _logger.LogInformation("Unable to locate 'DssCorrelationId' in request header");

            if (!Guid.TryParse(correlationId, out var correlationGuid))
            {
                _logger.LogInformation("Unable to parse 'DssCorrelationId' to a Guid");
                correlationGuid = Guid.NewGuid();
            }

            _logger.LogInformation($"DssCorrelationId: [{correlationGuid}]");

            var touchpointId = _httpRequestHelper.GetDssTouchpointId(req);
            if (string.IsNullOrEmpty(touchpointId))
            {
                var response = new BadRequestObjectResult("");
                _logger.LogWarning($"Response Status Code: [{response.StatusCode}]. Unable to locate 'TouchpointId' in request header");
                return response;
            }

            _logger.LogInformation($"Get Action Plan C# HTTP trigger function  processed a request. By Touchpoint: [{touchpointId}]");

            if (!Guid.TryParse(customerId, out var customerGuid))
            {
                var response = new BadRequestObjectResult(customerGuid);
                _logger.LogWarning($"Response Status Code: [{response.StatusCode}]. Unable to parse 'customerId' to a Guid: [{customerId}]");
                return response;
            }

            _logger.LogInformation($"Attempting to see if customer exists [{customerGuid}]");
            var doesCustomerExist = await _resourceHelper.DoesCustomerExist(customerGuid);

            if (!doesCustomerExist)
            {
                var response = new NoContentResult();
                _logger.LogWarning($"Response Status Code: [{response.StatusCode}]. Customer does not exist [{customerGuid}]");
                return response;
            }

            _logger.LogInformation($"Attempting to get action plan for customer [{customerGuid}]");
            var actionPlans = await _actionPlanGetService.GetActionPlansAsync(customerGuid);


            JsonResult jsonResponse;
            if (actionPlans == null)
            {
                var response = new NoContentResult();
                _logger.LogWarning($"Response Status Code: [{response.StatusCode}]. Get failed, no action plan found for customer [{customerGuid}]");
                return response;
            }
            else if (actionPlans.Count == 1)
            {
                jsonResponse = new JsonResult(_dynamicHelper.RenameProperty(actionPlans[0], "id", "ActionPlanId"), new JsonSerializerOptions()) { StatusCode = (int)HttpStatusCode.OK };
            }
            else
            {
                jsonResponse = new JsonResult(_dynamicHelper.RenameProperty(actionPlans, "id", "ActionPlanId"), new JsonSerializerOptions()) { StatusCode = (int)HttpStatusCode.OK };
            }
            _logger.LogInformation($"Response Status Code: [{jsonResponse.StatusCode}]. Get returned content");
            return jsonResponse;
        }

    }
}