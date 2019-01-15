using System;
using System.Threading.Tasks;
using NCS.DSS.ActionPlan.Models;

namespace NCS.DSS.ActionPlan.PatchActionPlanHttpTrigger.Service
{
    public interface IPatchActionPlanHttpTriggerService
    {
        Task<Models.ActionPlan> UpdateAsync(string actionPlanJson, ActionPlanPatch actionPlanPatch, Guid customerId);
        Task<string> GetActionPlanForCustomerAsync(Guid customerId, Guid actionPlanId);
        Task SendToServiceBusQueueAsync(Models.ActionPlan actionPlan, Guid customerId, string reqUrl);
    }
}