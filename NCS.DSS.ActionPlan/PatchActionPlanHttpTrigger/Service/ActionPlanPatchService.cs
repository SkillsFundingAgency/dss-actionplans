using NCS.DSS.ActionPlan.Helpers;
using NCS.DSS.ActionPlan.Models;
using Newtonsoft.Json.Linq;

namespace NCS.DSS.ActionPlan.PatchActionPlanHttpTrigger.Service
{
    public class ActionPlanPatchService : IActionPlanPatchService
    {
        public Models.ActionPlan Patch(string actionPlanJson, ActionPlanPatch actionPlanPatch)
        {
            if (string.IsNullOrEmpty(actionPlanJson))
                return null;

            var obj = JObject.Parse(actionPlanJson);

            if (actionPlanPatch.DateActionPlanCreated.HasValue)
                JsonHelper.UpdatePropertyValue(obj["DateActionPlanCreated"], actionPlanPatch.DateActionPlanCreated);

            if (actionPlanPatch.CustomerCharterShownToCustomer.HasValue)
                JsonHelper.UpdatePropertyValue(obj["CustomerCharterShownToCustomer"], actionPlanPatch.CustomerCharterShownToCustomer);

            if (actionPlanPatch.DateAndTimeCharterShown.HasValue)
                JsonHelper.UpdatePropertyValue(obj["DateAndTimeCharterShown"], actionPlanPatch.DateAndTimeCharterShown);

            if (actionPlanPatch.DateActionPlanSentToCustomer.HasValue)
                JsonHelper.UpdatePropertyValue(obj["DateActionPlanSentToCustomer"], actionPlanPatch.DateActionPlanSentToCustomer);

            if (actionPlanPatch.ActionPlanDeliveryMethod.HasValue)
                JsonHelper.UpdatePropertyValue(obj["ActionPlanDeliveryMethod"], actionPlanPatch.ActionPlanDeliveryMethod);

            if (actionPlanPatch.DateActionPlanAcknowledged.HasValue)
                JsonHelper.UpdatePropertyValue(obj["DateActionPlanAcknowledged"], actionPlanPatch.DateActionPlanAcknowledged);

            if (actionPlanPatch.PriorityCustomer.HasValue)
                JsonHelper.UpdatePropertyValue(obj["PriorityCustomer"], actionPlanPatch.PriorityCustomer);

            if (!string.IsNullOrEmpty(actionPlanPatch.CurrentSituation))
                JsonHelper.UpdatePropertyValue(obj["CurrentSituation"], actionPlanPatch.CurrentSituation);

            if (actionPlanPatch.LastModifiedDate.HasValue)
                JsonHelper.UpdatePropertyValue(obj["LastModifiedDate"], actionPlanPatch.LastModifiedDate);

            if (!string.IsNullOrEmpty(actionPlanPatch.LastModifiedTouchpointId))
                JsonHelper.UpdatePropertyValue(obj["LastModifiedTouchpointId"], actionPlanPatch.LastModifiedTouchpointId);

            return obj.ToObject<Models.ActionPlan>();
        }
    }
}
