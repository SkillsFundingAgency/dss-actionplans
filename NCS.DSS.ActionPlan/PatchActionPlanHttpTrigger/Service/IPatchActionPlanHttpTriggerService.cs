using System;
using System.Threading.Tasks;

namespace NCS.DSS.ActionPlan.PatchActionPlanHttpTrigger.Service
{
    public interface IPatchActionPlanHttpTriggerService
    {
        Task<Models.ActionPlan> UpdateAsync(Models.ActionPlan actionPlan, Models.ActionPlanPatch actionPlanPatch);
        Task<Models.ActionPlan> GetActionPlanForCustomerAsync(Guid customerId, Guid actionPlanId);
    }
}