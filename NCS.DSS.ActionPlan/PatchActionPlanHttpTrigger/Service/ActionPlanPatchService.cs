using DFC.JSON.Standard;
using NCS.DSS.ActionPlan.Models;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace NCS.DSS.ActionPlan.PatchActionPlanHttpTrigger.Service
{
    public class ActionPlanPatchService : IActionPlanPatchService
    {
        private readonly IJsonHelper _jsonHelper;

        public ActionPlanPatchService(IJsonHelper jsonHelper)
        {
            _jsonHelper = jsonHelper;
        }
        public string Patch(string actionPlanJson, ActionPlanPatch actionPlanPatch)
        {
            if (string.IsNullOrEmpty(actionPlanJson))
                return null;

            var obj = JObject.Parse(actionPlanJson);

            if (actionPlanPatch.SessionId.HasValue)
            {
                if (obj["SessionId"] == null)
                    _jsonHelper.CreatePropertyOnJObject(obj, "SessionId", actionPlanPatch.SessionId);
                else
                    _jsonHelper.UpdatePropertyValue(obj["SessionId"], actionPlanPatch.SessionId);
            }

            if (actionPlanPatch.DateActionPlanCreated.HasValue)
                _jsonHelper.UpdatePropertyValue(obj["DateActionPlanCreated"], actionPlanPatch.DateActionPlanCreated);

            if (actionPlanPatch.CustomerCharterShownToCustomer.HasValue)
                _jsonHelper.UpdatePropertyValue(obj["CustomerCharterShownToCustomer"], actionPlanPatch.CustomerCharterShownToCustomer);

            if (actionPlanPatch.DateAndTimeCharterShown.HasValue)
                _jsonHelper.UpdatePropertyValue(obj["DateAndTimeCharterShown"], actionPlanPatch.DateAndTimeCharterShown);

            if (actionPlanPatch.DateActionPlanSentToCustomer.HasValue)
                _jsonHelper.UpdatePropertyValue(obj["DateActionPlanSentToCustomer"], actionPlanPatch.DateActionPlanSentToCustomer);

            if (actionPlanPatch.ActionPlanDeliveryMethod.HasValue)
                _jsonHelper.UpdatePropertyValue(obj["ActionPlanDeliveryMethod"], actionPlanPatch.ActionPlanDeliveryMethod);

            if (actionPlanPatch.DateActionPlanAcknowledged.HasValue)
                _jsonHelper.UpdatePropertyValue(obj["DateActionPlanAcknowledged"], actionPlanPatch.DateActionPlanAcknowledged);

            if (!string.IsNullOrEmpty(actionPlanPatch.CurrentSituation))
                _jsonHelper.UpdatePropertyValue(obj["CurrentSituation"], actionPlanPatch.CurrentSituation);

            if (actionPlanPatch.LastModifiedDate.HasValue)
                _jsonHelper.UpdatePropertyValue(obj["LastModifiedDate"], actionPlanPatch.LastModifiedDate);

            if (!string.IsNullOrEmpty(actionPlanPatch.LastModifiedTouchpointId))
                _jsonHelper.UpdatePropertyValue(obj["LastModifiedTouchpointId"], actionPlanPatch.LastModifiedTouchpointId);

            if (!string.IsNullOrEmpty(actionPlanPatch.SubcontractorId))
            {
                if (obj["SubcontractorId"] == null)
                    _jsonHelper.CreatePropertyOnJObject(obj, "SubcontractorId", actionPlanPatch.SubcontractorId);
                else
                    _jsonHelper.UpdatePropertyValue(obj["SubcontractorId"], actionPlanPatch.SubcontractorId);
            }

            if (actionPlanPatch.CustomerSatisfaction.HasValue)
            {
                if (obj["CustomerSatisfaction"] == null)
                    _jsonHelper.CreatePropertyOnJObject(obj, "SubcontractorId", actionPlanPatch.SubcontractorId);
                else
                    _jsonHelper.UpdatePropertyValue(obj["CustomerSatisfaction"], actionPlanPatch.CustomerSatisfaction);
            }

            return obj.ToString();
        }
    }
}
