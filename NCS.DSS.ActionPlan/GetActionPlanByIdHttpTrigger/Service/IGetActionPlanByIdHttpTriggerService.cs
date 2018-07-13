using System;
using System.Threading.Tasks;

namespace NCS.DSS.ActionPlan.GetActionPlanByIdHttpTrigger.Service
{
    public interface IGetActionPlanByIdHttpTriggerService
    {
        Task<Models.ActionPlan> GetActionPlanForCustomerAsync(Guid customerId, Guid actionPlanId);
    }
}