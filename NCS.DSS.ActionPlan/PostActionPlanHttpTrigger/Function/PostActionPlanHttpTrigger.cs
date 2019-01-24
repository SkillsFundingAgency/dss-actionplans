using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using DFC.Common.Standard.Logging;
using DFC.Functions.DI.Standard.Attributes;
using DFC.HTTP.Standard;
using DFC.JSON.Standard;
using DFC.Swagger.Standard.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using NCS.DSS.ActionPlan.Cosmos.Helper;
using NCS.DSS.ActionPlan.PostActionPlanHttpTrigger.Service;
using NCS.DSS.ActionPlan.Validation;
using Newtonsoft.Json;

namespace NCS.DSS.ActionPlan.PostActionPlanHttpTrigger.Function
{
    public static class PostActionPlanHttpTrigger
    {
        [FunctionName("Post")]
        [ProducesResponseType(typeof(Models.ActionPlan), (int)HttpStatusCode.OK)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Created, Description = "Action Plan Created", ShowSchema = true)]
        [Response(HttpStatusCode = (int)HttpStatusCode.NoContent, Description = "Action Plan does not exist", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.BadRequest, Description = "Request was malformed", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Unauthorized, Description = "API key is unknown or invalid", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Forbidden, Description = "Insufficient access", ShowSchema = false)]
        [Response(HttpStatusCode = 422, Description = "Action Plan validation error(s)", ShowSchema = false)]
        [Display(Name = "Post", Description = "Ability to create a new action plan for a customer.")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "Customers/{customerId}/Interactions/{interactionId}/Sessions/{sessionId}/ActionPlans")]HttpRequest req, ILogger log, string customerId, string interactionId, string sessionId,
            [Inject]IResourceHelper resourceHelper,
            [Inject]IValidate validate,
            [Inject]IPostActionPlanHttpTriggerService actionPlanPostService,
            [Inject]ILoggerHelper loggerHelper,
            [Inject]IHttpRequestHelper httpRequestHelper,
            [Inject]IHttpResponseMessageHelper httpResponseMessageHelper,
            [Inject]IJsonHelper jsonHelper)
        {


            return httpResponseMessageHelper.Created("Actionplans resource created successfully");


            //loggerHelper.LogMethodEnter(log);

            //var correlationId = httpRequestHelper.GetDssCorrelationId(req);
            //if (string.IsNullOrEmpty(correlationId))
            //    log.LogInformation("Unable to locate 'DssCorrelationId' in request header");

            //if (!Guid.TryParse(correlationId, out var correlationGuid))
            //{
            //    log.LogInformation("Unable to parse 'DssCorrelationId' to a Guid");
            //    correlationGuid = Guid.NewGuid();
            //}

            //var touchpointId = httpRequestHelper.GetDssTouchpointId(req);
            //if (string.IsNullOrEmpty(touchpointId))
            //{
            //    loggerHelper.LogInformationMessage(log, correlationGuid, "Unable to locate 'TouchpointId' in request header");
            //    return httpResponseMessageHelper.BadRequest();
            //}

            //var ApimURL = httpRequestHelper.GetDssApimUrl(req);
            //if (string.IsNullOrEmpty(ApimURL))
            //{
            //    loggerHelper.LogInformationMessage(log, correlationGuid, "Unable to locate 'apimurl' in request header");
            //    return httpResponseMessageHelper.BadRequest();
            //}

            //var subcontractorId = httpRequestHelper.GetDssSubcontractorId(req);
            //if (string.IsNullOrEmpty(subcontractorId))
            //    loggerHelper.LogInformationMessage(log, correlationGuid, "Unable to locate 'SubcontractorId' in request header");
            
            //loggerHelper.LogInformationMessage(log, correlationGuid,
            //    string.Format("Post Action Plan C# HTTP trigger function  processed a request. By Touchpoint: {0}", touchpointId));

            //if (!Guid.TryParse(customerId, out var customerGuid))
            //{
            //    loggerHelper.LogInformationMessage(log, correlationGuid, string.Format("Unable to parse 'customerId' to a Guid: {0}", customerId));
            //    return httpResponseMessageHelper.BadRequest(customerGuid);
            //}

            //if (!Guid.TryParse(interactionId, out var interactionGuid))
            //{
            //    loggerHelper.LogInformationMessage(log, correlationGuid, string.Format("Unable to parse 'interactionId' to a Guid: {0}", interactionId));
            //    return httpResponseMessageHelper.BadRequest(interactionGuid);
            //}

            //if (!Guid.TryParse(sessionId, out var sessionGuid))
            //{
            //    loggerHelper.LogInformationMessage(log, correlationGuid, string.Format("Unable to parse 'sessionId' to a Guid: {0}", sessionGuid));
            //    return httpResponseMessageHelper.BadRequest(sessionGuid);
            //}

            //Models.ActionPlan actionPlanRequest;

            //try
            //{
            //    loggerHelper.LogInformationMessage(log, correlationGuid, "Attempt to get resource from body of the request");
            //    actionPlanRequest = await httpRequestHelper.GetResourceFromRequest<Models.ActionPlan>(req);
            //}
            //catch (JsonException ex)
            //{
            //    loggerHelper.LogError(log, correlationGuid, "Unable to retrieve body from req", ex);
            //    return httpResponseMessageHelper.UnprocessableEntity(ex);
            //}

            //if (actionPlanRequest == null)
            //{
            //    loggerHelper.LogInformationMessage(log, correlationGuid, "Action plan request is null");
            //    return httpResponseMessageHelper.UnprocessableEntity(req);
            //}

            //loggerHelper.LogInformationMessage(log, correlationGuid, "Attempt to set id's for action plan");
            //actionPlanRequest.SetIds(customerGuid, interactionGuid, sessionGuid, touchpointId, subcontractorId);

            //loggerHelper.LogInformationMessage(log, correlationGuid, "Attempt to validate resource");
            //var errors = validate.ValidateResource(actionPlanRequest);

            //if (errors != null && errors.Any())
            //{
            //    loggerHelper.LogInformationMessage(log, correlationGuid, "validation errors with resource");
            //    return httpResponseMessageHelper.UnprocessableEntity(errors);
            //}

            //loggerHelper.LogInformationMessage(log, correlationGuid, string.Format("Attempting to see if customer exists {0}", customerGuid));
            //var doesCustomerExist = await resourceHelper.DoesCustomerExist(customerGuid);

            //if (!doesCustomerExist)
            //{
            //    loggerHelper.LogInformationMessage(log, correlationGuid, string.Format("Customer does not exist {0}", customerGuid));
            //    return httpResponseMessageHelper.NoContent(customerGuid);
            //}

            //loggerHelper.LogInformationMessage(log, correlationGuid, string.Format("Attempting to see if this is a read only customer {0}", customerGuid));
            //var isCustomerReadOnly = await resourceHelper.IsCustomerReadOnly(customerGuid);

            //if (isCustomerReadOnly)
            //{
            //    loggerHelper.LogInformationMessage(log, correlationGuid, string.Format("Customer is read only {0}", customerGuid));
            //    return httpResponseMessageHelper.Forbidden(customerGuid);
            //}

            //loggerHelper.LogInformationMessage(log, correlationGuid, string.Format("Attempting to get Interaction {0} for customer {1}", interactionGuid, customerGuid));
            //var doesSessionExist = resourceHelper.DoesSessionExistAndBelongToCustomer(sessionGuid, interactionGuid, customerGuid);

            //if (!doesSessionExist)
            //{
            //    loggerHelper.LogInformationMessage(log, correlationGuid, string.Format("Interaction does not exist {0}", interactionGuid));
            //    return httpResponseMessageHelper.NoContent(interactionGuid);
            //}

            //loggerHelper.LogInformationMessage(log, correlationGuid, string.Format("Attempting to get Create Action Plan for customer {0}", customerGuid));
            //var actionPlan = await actionPlanPostService.CreateAsync(actionPlanRequest);

            //if (actionPlan != null)
            //{
            //    loggerHelper.LogInformationMessage(log, correlationGuid, string.Format("attempting to send to service bus {0}", actionPlan.ActionPlanId));
            //    await actionPlanPostService.SendToServiceBusQueueAsync(actionPlan, ApimURL);
            //}

            //loggerHelper.LogMethodExit(log);

            //return actionPlan == null
            //    ? httpResponseMessageHelper.BadRequest(customerGuid)
            //    : httpResponseMessageHelper.Created(jsonHelper.SerializeObjectAndRenameIdProperty(actionPlan, "id", "ActionPlanId"));

        }
    }
}