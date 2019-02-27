using System;
using System.Threading.Tasks;
using NCS.DSS.ActionPlan.Models;

namespace NCS.DSS.ActionPlan.PatchActionPlanHttpTrigger.Service
{
    public interface IPatchActionPlanHttpTriggerService
    {
        string PatchResource(string actionPlanJson, ActionPlanPatch actionPlanPatch);
        Task<Models.ActionPlan> UpdateCosmosAsync(string actionPlanJson, Guid actionPlanId);
        Task<string> GetActionPlanForCustomerAsync(Guid customerId, Guid actionPlanId);
        Task SendToServiceBusQueueAsync(Models.ActionPlan actionPlan, Guid customerId, string reqUrl);
    }
}