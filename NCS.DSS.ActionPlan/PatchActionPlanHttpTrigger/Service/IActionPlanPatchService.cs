using NCS.DSS.ActionPlan.Models;

namespace NCS.DSS.ActionPlan.PatchActionPlanHttpTrigger.Service
{
    public interface IActionPlanPatchService
    {
        Models.ActionPlan Patch(string actionPlanJson, ActionPlanPatch actionPlanPatch);
    }
}
