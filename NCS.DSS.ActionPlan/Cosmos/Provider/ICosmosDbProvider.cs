using Microsoft.Azure.Cosmos;

namespace NCS.DSS.ActionPlan.Cosmos.Provider
{
    public interface ICosmosDbProvider
    {
        Task<bool> DoesCustomerResourceExist(Guid customerId);
        Task<bool> DoesInteractionResourceExistAndBelongToCustomer(Guid interactionId, Guid customerId);
        Task<bool> DoesSessionResourceExistAndBelongToCustomer(Guid sessionId, Guid interactionId, Guid customerId);
        Task<DateTime?> GetDateAndTimeOfSessionFromSessionResource(Guid sessionId);
        Task<string> GetActionPlanForCustomerToUpdateAsync(Guid customerId, Guid actionPlanId);
        Task<Models.ActionPlan> GetActionPlanForCustomerAsync(Guid customerId, Guid actionPlanId);
        Task<List<Models.ActionPlan>> GetActionPlansForCustomerAsync(Guid customerId);
        Task<ItemResponse<Models.ActionPlan>> CreateActionPlanAsync(Models.ActionPlan actionPlan);
        Task<ItemResponse<Models.ActionPlan>> UpdateActionPlanAsync(string actionPlanJson, Guid actionPlanId);
        string GetCustomerJson();
    }
}