namespace NCS.DSS.ActionPlan.GetActionPlanHttpTrigger.Service
{
    public interface IGetActionPlanHttpTriggerService
    {
        Task<List<Models.ActionPlan>> GetActionPlansAsync(Guid customerId);
    }
}