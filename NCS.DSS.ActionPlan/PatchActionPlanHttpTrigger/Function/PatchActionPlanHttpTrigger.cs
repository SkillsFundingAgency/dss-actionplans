using DFC.Common.Standard.Logging;
using DFC.HTTP.Standard;
using DFC.JSON.Standard;
using DFC.Swagger.Standard.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
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
using System.Net.Http;
using System.Threading.Tasks;

namespace NCS.DSS.ActionPlan.PatchActionPlanHttpTrigger.Function
{
    public class PatchActionPlanHttpTrigger
    {
        private IResourceHelper _resourceHelper;
        private IValidate _validate;
        private IPatchActionPlanHttpTriggerService _actionPlanPatchService;
        private ILoggerHelper _loggerHelper;
        private IHttpRequestHelper _httpRequestHelper;
        private IHttpResponseMessageHelper _httpResponseMessageHelper;
        private IJsonHelper _jsonHelper;

        public PatchActionPlanHttpTrigger(
             IResourceHelper resourceHelper,
             IValidate validate,
             IPatchActionPlanHttpTriggerService actionPlanPatchService,
             ILoggerHelper loggerHelper,
             IHttpRequestHelper httpRequestHelper,
             IHttpResponseMessageHelper httpResponseMessageHelper,
             IJsonHelper jsonHelper)
        {
            _resourceHelper = resourceHelper;
            _validate = validate;
            _actionPlanPatchService = actionPlanPatchService;
            _loggerHelper = loggerHelper;
            _httpRequestHelper = httpRequestHelper;
            _httpResponseMessageHelper = httpResponseMessageHelper;
            _jsonHelper = jsonHelper;
        }

        [FunctionName("Patch")]
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
        public async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "Customers/{customerId}/Interactions/{interactionId}/ActionPlans/{actionPlanId}")] HttpRequest req, ILogger log, string customerId, string interactionId, string actionPlanId)
        {

            _loggerHelper.LogMethodEnter(log);

            var correlationId = _httpRequestHelper.GetDssCorrelationId(req);
            if (string.IsNullOrEmpty(correlationId))
                log.LogInformation("Unable to locate 'DssCorrelationId' in request header");

            if (!Guid.TryParse(correlationId, out var correlationGuid))
            {
                log.LogInformation("Unable to parse 'DssCorrelationId' to a Guid");
                correlationGuid = Guid.NewGuid();
            }

            var touchpointId = _httpRequestHelper.GetDssTouchpointId(req);
            if (string.IsNullOrEmpty(touchpointId))
            {
                _loggerHelper.LogInformationMessage(log, correlationGuid, "Unable to locate 'TouchpointId' in request header");
                return _httpResponseMessageHelper.BadRequest();
            }

            var subcontractorId = _httpRequestHelper.GetDssSubcontractorId(req);
            if (string.IsNullOrEmpty(subcontractorId))
            {
                _loggerHelper.LogInformationMessage(log, correlationGuid, "Unable to locate 'SubcontractorId' in request header");
                return _httpResponseMessageHelper.BadRequest();
            }

            var apimUrl = _httpRequestHelper.GetDssApimUrl(req);
            if (string.IsNullOrEmpty(apimUrl))
            {
                _loggerHelper.LogInformationMessage(log, correlationGuid, "Unable to locate 'apimurl' in request header");
                return _httpResponseMessageHelper.BadRequest();
            }

            var subcontractorId = _httpRequestHelper.GetDssSubcontractorId(req);
            if (string.IsNullOrEmpty(subcontractorId))
                _loggerHelper.LogInformationMessage(log, correlationGuid, "Unable to locate 'SubcontractorId' in request header");

            _loggerHelper.LogInformationMessage(log, correlationGuid,
                string.Format("Patch Action Plan C# HTTP trigger function  processed a request. By Touchpoint: {0}",
                    touchpointId));

            if (!Guid.TryParse(customerId, out var customerGuid))
            {
                _loggerHelper.LogInformationMessage(log, correlationGuid, string.Format("Unable to parse 'customerId' to a Guid: {0}", customerId));
                return _httpResponseMessageHelper.BadRequest(customerGuid);
            }

            if (!Guid.TryParse(interactionId, out var interactionGuid))
            {
                _loggerHelper.LogInformationMessage(log, correlationGuid, string.Format("Unable to parse 'interactionId' to a Guid: {0}", interactionId));
                return _httpResponseMessageHelper.BadRequest(interactionGuid);
            }

            if (!Guid.TryParse(actionPlanId, out var actionPlanGuid))
            {
                _loggerHelper.LogInformationMessage(log, correlationGuid, string.Format("Unable to parse 'actionPlanId' to a Guid: {0}", actionPlanId));
                return _httpResponseMessageHelper.BadRequest(actionPlanGuid);
            }

            ActionPlanPatch actionPlanPatchRequest;

            try
            {
                _loggerHelper.LogInformationMessage(log, correlationGuid, "Attempt to get resource from body of the request");
                actionPlanPatchRequest = await _httpRequestHelper.GetResourceFromRequest<ActionPlanPatch>(req);
            }
            catch (JsonException ex)
            {
                _loggerHelper.LogError(log, correlationGuid, "Unable to retrieve body from req", ex);
                return _httpResponseMessageHelper.UnprocessableEntity(ex);
            }

            if (actionPlanPatchRequest == null)
            {
                _loggerHelper.LogInformationMessage(log, correlationGuid, "Action plan patch request is null");
                return _httpResponseMessageHelper.UnprocessableEntity(req);
            }

            _loggerHelper.LogInformationMessage(log, correlationGuid, "Attempt to set id's for action plan patch");
            actionPlanPatchRequest.SetIds(touchpointId, subcontractorId);

            _loggerHelper.LogInformationMessage(log, correlationGuid, string.Format("Attempting to see if customer exists {0}", customerGuid));
            var doesCustomerExist = await _resourceHelper.DoesCustomerExist(customerGuid);

            if (!doesCustomerExist)
            {
                _loggerHelper.LogInformationMessage(log, correlationGuid, string.Format("Customer does not exist {0}", customerGuid));
                return _httpResponseMessageHelper.NoContent(customerGuid);
            }

            _loggerHelper.LogInformationMessage(log, correlationGuid, string.Format("Attempting to see if this is a read only customer {0}", customerGuid));
            var isCustomerReadOnly = _resourceHelper.IsCustomerReadOnly();

            if (isCustomerReadOnly)
            {
                _loggerHelper.LogInformationMessage(log, correlationGuid, string.Format("Customer is read only {0}", customerGuid));
                return _httpResponseMessageHelper.Forbidden(customerGuid);
            }

            _loggerHelper.LogInformationMessage(log, correlationGuid, string.Format("Attempting to get Interaction {0} for customer {1}", interactionGuid, customerGuid));
            var doesInteractionExist = _resourceHelper.DoesInteractionExistAndBelongToCustomer(interactionGuid, customerGuid);

            if (!doesInteractionExist)
            {
                _loggerHelper.LogInformationMessage(log, correlationGuid, string.Format("Interaction does not exist {0}", interactionGuid));
                return _httpResponseMessageHelper.NoContent(interactionGuid);
            }

            _loggerHelper.LogInformationMessage(log, correlationGuid, string.Format("Attempting to get action plan {0} for customer {1}", actionPlanGuid, customerGuid));
            var actionPlanForCustomer = await _actionPlanPatchService.GetActionPlanForCustomerAsync(customerGuid, actionPlanGuid);

            if (actionPlanForCustomer == null)
            {
                _loggerHelper.LogInformationMessage(log, correlationGuid, string.Format("ActionPlan does not exist {0}", actionPlanGuid));
                return _httpResponseMessageHelper.NoContent(actionPlanGuid);
            }

            var patchedActionPlan = _actionPlanPatchService.PatchResource(actionPlanForCustomer, actionPlanPatchRequest);

            if (patchedActionPlan == null)
            {
                _loggerHelper.LogInformationMessage(log, correlationGuid, string.Format("ActionPlan does not exist {0}", actionPlanGuid));
                return _httpResponseMessageHelper.NoContent(actionPlanGuid);
            }

            Models.ActionPlan actionPlanValidationObject;

            try
            {
                actionPlanValidationObject = JsonConvert.DeserializeObject<Models.ActionPlan>(patchedActionPlan);
            }
            catch (JsonException ex)
            {
                _loggerHelper.LogError(log, correlationGuid, "Unable to retrieve body from req", ex);
                throw;
            }

            if (actionPlanValidationObject == null)
            {
                _loggerHelper.LogInformationMessage(log, correlationGuid, "Action Plan Validation Object is null");
                return _httpResponseMessageHelper.UnprocessableEntity(req);
            }

            _loggerHelper.LogInformationMessage(log, correlationGuid, string.Format("Attempting to get GetDateAndTimeOfSession for Session {0}", actionPlanValidationObject.SessionId));
            var dateAndTimeOfSession = await _resourceHelper.GetDateAndTimeOfSession(actionPlanValidationObject.SessionId.GetValueOrDefault());

            _loggerHelper.LogInformationMessage(log, correlationGuid, "Attempt to validate resource");
            var errors = _validate.ValidateResource(actionPlanValidationObject, dateAndTimeOfSession);

            if (errors != null && errors.Any())
            {
                _loggerHelper.LogInformationMessage(log, correlationGuid, "validation errors with resource");
                return _httpResponseMessageHelper.UnprocessableEntity(errors);
            }

            _loggerHelper.LogInformationMessage(log, correlationGuid, string.Format("Attempting to update action plan {0}", actionPlanGuid));
            var updatedActionPlan = await _actionPlanPatchService.UpdateCosmosAsync(patchedActionPlan, actionPlanGuid);

            if (updatedActionPlan != null)
            {
                _loggerHelper.LogInformationMessage(log, correlationGuid, string.Format("attempting to send to service bus {0}", actionPlanGuid));
                await _actionPlanPatchService.SendToServiceBusQueueAsync(updatedActionPlan, customerGuid, apimUrl);
            }

            _loggerHelper.LogMethodExit(log);

            return updatedActionPlan == null ?
                _httpResponseMessageHelper.BadRequest(actionPlanGuid) :
                _httpResponseMessageHelper.Ok(_jsonHelper.SerializeObjectAndRenameIdProperty(updatedActionPlan, "id", "ActionPlanId"));

        }
    }
}