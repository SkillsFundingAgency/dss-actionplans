﻿using DFC.JSON.Standard;
using NCS.DSS.ActionPlan.Models;
using Newtonsoft.Json.Linq;

namespace NCS.DSS.ActionPlan.PatchActionPlanHttpTrigger.Service
{
    public class ActionPlanPatchService : IActionPlanPatchService
    {
        private IJsonHelper _jsonHelper;

        public ActionPlanPatchService(IJsonHelper jsonHelper)
        {
            _jsonHelper = jsonHelper;
        }
        public string Patch(string actionPlanJson, ActionPlanPatch actionPlanPatch)
        {
            if (string.IsNullOrEmpty(actionPlanJson))
                return null;

            var obj = JObject.Parse(actionPlanJson);

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

            if (actionPlanPatch.PriorityCustomer.HasValue)
                _jsonHelper.UpdatePropertyValue(obj["PriorityCustomer"], actionPlanPatch.PriorityCustomer);

            if (!string.IsNullOrEmpty(actionPlanPatch.CurrentSituation))
                _jsonHelper.UpdatePropertyValue(obj["CurrentSituation"], actionPlanPatch.CurrentSituation);

            if (actionPlanPatch.LastModifiedDate.HasValue)
                _jsonHelper.UpdatePropertyValue(obj["LastModifiedDate"], actionPlanPatch.LastModifiedDate);

            if (!string.IsNullOrEmpty(actionPlanPatch.LastModifiedTouchpointId))
                _jsonHelper.UpdatePropertyValue(obj["LastModifiedTouchpointId"], actionPlanPatch.LastModifiedTouchpointId);

            if (!string.IsNullOrEmpty(actionPlanPatch.SubcontractorId))
                _jsonHelper.UpdatePropertyValue(obj["SubcontractorId"], actionPlanPatch.SubcontractorId);

            return obj.ToString();
        }
    }
}