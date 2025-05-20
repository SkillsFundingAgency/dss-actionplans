namespace NCS.DSS.ActionPlan.ServiceBus
{
    public interface IActionPlanServiceBusClient
    {
        Task SendPatchMessageAsync(Models.ActionPlan actionPlan, Guid customerId, string reqUrl);
        Task SendPostMessageAsync(Models.ActionPlan actionPlan, string reqUrl);
    }
}