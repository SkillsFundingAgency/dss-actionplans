using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using DFC.Common.Standard.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NCS.DSS.ActionPlan.Cosmos.Helper;
using NCS.DSS.ActionPlan.GetActionPlanHttpTrigger.Service;
using DFC.Swagger.Standard.Annotations;
using DFC.HTTP.Standard;
using DFC.JSON.Standard;
using Microsoft.AspNetCore.Http;
using DFC.Functions.DI.Standard.Attributes;

namespace NCS.DSS.ActionPlan.GetActionPlanHttpTrigger.Function
{
    public static class GetActionPlanHttpTrigger
    {
        [FunctionName("Get")]
        [ProducesResponseType(typeof(Models.ActionPlan), (int)HttpStatusCode.OK)]
        [Response(HttpStatusCode = (int)HttpStatusCode.OK, Description = "Action Plans found", ShowSchema = true)]
        [Response(HttpStatusCode = (int)HttpStatusCode.NoContent, Description = "Action Plans do not exist", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.BadRequest, Description = "Request was malformed", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Unauthorized, Description = "API key is unknown or invalid", ShowSchema = false)]
        [Response(HttpStatusCode = (int)HttpStatusCode.Forbidden, Description = "Insufficient access", ShowSchema = false)]
        [Display(Name = "Get", Description = "Ability to return all action plans for the given customer.")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Customers/{customerId}/ActionPlans")]HttpRequest req, ILogger log, string customerId,
            [Inject]IResourceHelper resourceHelper,
            [Inject]IGetActionPlanHttpTriggerService actionPlanGetService,
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

            //loggerHelper.LogInformationMessage(log, correlationGuid,
            //    string.Format("Get Action Plan C# HTTP trigger function  processed a request. By Touchpoint: {0}",
            //        touchpointId));

            //if (!Guid.TryParse(customerId, out var customerGuid))
            //{
            //    loggerHelper.LogInformationMessage(log, correlationGuid, string.Format("Unable to parse 'customerId' to a Guid: {0}", customerId));
            //    return httpResponseMessageHelper.BadRequest(customerGuid);
            //}

            //loggerHelper.LogInformationMessage(log, correlationGuid, string.Format("Attempting to see if customer exists {0}", customerGuid));
            //var doesCustomerExist = await resourceHelper.DoesCustomerExist(customerGuid);

            //if (!doesCustomerExist)
            //{
            //    loggerHelper.LogInformationMessage(log, correlationGuid, string.Format("Customer does not exist {0}", customerGuid));
            //    return httpResponseMessageHelper.NoContent(customerGuid);
            //}

            //loggerHelper.LogInformationMessage(log, correlationGuid, string.Format("Attempting to get action plan for customer {0}", customerGuid));
            //var actionPlans = await actionPlanGetService.GetActionPlansAsync(customerGuid);

            //loggerHelper.LogMethodExit(log);

            //return actionPlans == null ?
            //    httpResponseMessageHelper.NoContent(customerGuid) :
            //    httpResponseMessageHelper.Ok(jsonHelper.SerializeObjectsAndRenameIdProperty(actionPlans, "id", "ActionPlanId"));

        }
    }
}