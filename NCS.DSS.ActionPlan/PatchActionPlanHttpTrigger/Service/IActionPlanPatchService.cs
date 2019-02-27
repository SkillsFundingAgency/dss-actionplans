using NCS.DSS.ActionPlan.Models;

namespace NCS.DSS.ActionPlan.PatchActionPlanHttpTrigger.Service
{
    public interface IActionPlanPatchService
    {
        string Patch(string actionPlanJson, ActionPlanPatch actionPlanPatch);
    }
}
