namespace NCS.DSS.ActionPlan.Cosmos.Helper
{
    public interface IResourceHelper
    {
        Task<bool> DoesCustomerExist(Guid customerId);
        bool IsCustomerReadOnly();
        Task<bool> DoesInteractionExistAndBelongToCustomer(Guid interactionGuid, Guid customerGuid);
        Task<bool> DoesSessionExistAndBelongToCustomer(Guid sessionId, Guid interactionId, Guid customerId);
        Task<DateTime?> GetDateAndTimeOfSession(Guid sessionId);
    }
}