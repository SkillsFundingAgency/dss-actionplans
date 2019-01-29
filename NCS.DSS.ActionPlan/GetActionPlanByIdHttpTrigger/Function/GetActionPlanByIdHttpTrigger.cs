using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using DFC.Common.Standard.Logging;
using DFC.Functions.DI.Standard.Attributes;
using Microsoft.AspNetCore.Mvc;
using NCS.DSS.ActionPlan.Cosmos.Helper;
using NCS.DSS.ActionPlan.GetActionPlanByIdHttpTrigger.Service;
using Microsoft.AspNetCore.Http;
using DFC.HTTP.Standard;
using DFC.JSON.Standard;
using DFC.Swagger.Standard.Annotations;
using Microsoft.Extensions.Logging;

namespace NCS.DSS.ActionPlan.GetActionPlanByIdHttpTrigger.Function
{
    public static class GetActionPlanByIdHttpTrigger
    {
        [FunctionName("GetById")]
        [ProducesResponseType(typeof(Models.ActionPlan), (int) HttpStatusCode.OK)]
        [Response(HttpStatusCode = (int)HttpStatusCode.OK, Description = "Action Plan found", ShowSchema = true)]
        [Response(HttpStatusCode = (int)HttpStatusCode.NoContent, Description = "Action Plan does not exist", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.BadRequest, Description = "Request was malformed", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Unauthorized, Description = "API key is unknown or invalid", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Forbidden, Description = "Insufficient access", ShowSchema = false)]
        [Display(Name = "Get", Description = "Ability to retrieve an individual action plan for the given customer")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Customers/{customerId}/Interactions/{interactionId}/Sessions/{sessionId}/ActionPlans/{actionPlanId}")]HttpRequest req, ILogger log, string customerId, string interactionId, string sessionId, string actionPlanId,
            [Inject]IResourceHelper resourceHelper,
            [Inject]IGetActionPlanByIdHttpTriggerService actionPlanGetService,
            [Inject]ILoggerHelper loggerHelper,
            [Inject]IHttpRequestHelper httpRequestHelper,
            [Inject]IHttpResponseMessageHelper httpResponseMessageHelper,
            [Inject]IJsonHelper jsonHelper)
        {


            Models.ActionPlan testActionPlan = new Models.ActionPlan
            {
                ActionPlanId = Guid.Parse("d5529a13-fca1-4775-b456-b5ee12d02fcd"),
                CustomerId = Guid.Parse("518b8b41-ff04-4668-9bf1-62800399b90c"),
                InteractionId = Guid.Parse("2730af9c-fc34-4c2b-a905-c4b584b0f379"),
                SessionId = Guid.Parse("f01f7631-1765-4c18-9885-afa244de372a"),
                SubcontractorId = "01234567899876543210",
                DateActionPlanCreated = DateTime.Parse("01/05/2018"),
                CustomerCharterShownToCustomer = true,
                DateAndTimeCharterShown = DateTime.Parse("04/04/2018"),
                DateActionPlanSentToCustomer = DateTime.Parse("07/07/2018"),
                ActionPlanDeliveryMethod = ReferenceData.ActionPlanDeliveryMethod.Email,
                PriorityCustomer = ReferenceData.PriorityCustomer.AdultsWhoHaveBeenUnemployedForMoreThan12Months,
                CurrentSituation = "Sample Currentsituation Text",
                LastModifiedDate = DateTime.Parse("05/01/2019"),
                LastModifiedTouchpointId = "000000010"
            };

            return httpResponseMessageHelper.Ok(jsonHelper.SerializeObjectAndRenameIdProperty(testActionPlan, "id", "ActionPlanId"));


            //loggerHelper.LogMethodEnter(log);

            //var correlationId = httpRequestHelper.GetDssCorrelationId(req);
            //if (string.IsNullOrEmpty(correlationId))
            //{
            //    log.LogInformation("Unable to locate 'DssCorrelationId' in request header");
            //}

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

            //loggerHelper.LogInformationMessage(log, correlationGuid, 
            //    string.Format("Get Action Plan By Id C# HTTP trigger function  processed a request. By Touchpoint: {0}", 
            //        touchpointId));

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

            //if (!Guid.TryParse(actionPlanId, out var actionPlanGuid))
            //{
            //    loggerHelper.LogInformationMessage(log, correlationGuid, string.Format("Unable to parse 'actionPlanId' to a Guid: {0}", actionPlanId));
            //    return httpResponseMessageHelper.BadRequest(actionPlanGuid);
            //}

            //loggerHelper.LogInformationMessage(log, correlationGuid, string.Format("Attempting to see if customer exists {0}", customerGuid));
            //var doesCustomerExist = await resourceHelper.DoesCustomerExist(customerGuid);

            //if (!doesCustomerExist)
            //{
            //    loggerHelper.LogInformationMessage(log, correlationGuid, string.Format("Customer does not exist {0}",customerGuid));
            //    return httpResponseMessageHelper.NoContent(customerGuid);
            //}

            //loggerHelper.LogInformationMessage(log, correlationGuid, string.Format("Attempting to get Interaction {0} for customer {1}", interactionGuid, customerGuid));
            //var doesSessionExist = resourceHelper.DoesSessionExistAndBelongToCustomer(sessionGuid, interactionGuid, customerGuid);

            //if (!doesSessionExist)
            //{
            //    loggerHelper.LogInformationMessage(log, correlationGuid, string.Format("Interaction does not exist {0}", interactionGuid));
            //    return httpResponseMessageHelper.NoContent(interactionGuid);
            //}

            //loggerHelper.LogInformationMessage(log, correlationGuid, string.Format("Attempting to get action plan {0} for customer {1}", actionPlanGuid, customerGuid));
            //var actionPlan = await actionPlanGetService.GetActionPlanForCustomerAsync(customerGuid, actionPlanGuid);

            //loggerHelper.LogMethodExit(log);

            //return actionPlan == null ?
            //    httpResponseMessageHelper.NoContent(actionPlanGuid) :
            //    httpResponseMessageHelper.Ok(jsonHelper.SerializeObjectAndRenameIdProperty(actionPlan, "id", "ActionPlanId"));

        }
    }
}